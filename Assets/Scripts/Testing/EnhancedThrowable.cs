using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Samples
{
    public enum ThrowingStyle
    {
        Spear,
        Baseball,
        Underhand
    }

    [AddComponentMenu("XR/Enhanced Throwable")]
    public class EnhancedThrowable : XRGrabInteractable, IXRSelectFilter
    {
        [Header("Throwing Profile")]
        [SerializeField] ThrowingStyle m_ThrowingStyle = ThrowingStyle.Baseball;
        [SerializeField, Range(1f, 5f)] float m_PowerMultiplier = 1.5f;
        [SerializeField, Range(0f, 1f)] float m_DirectionalSmoothing = 0.5f;
        [SerializeField, Range(0f, 1f)] float m_ReleaseThreshold = 0.1f;

        [Header("Calibration")]
        [SerializeField] bool m_UseCalibration = true;
        [SerializeField] int m_RequiredThrows = 5;

        private List<CalibrationResult> m_RecordedThrows = new List<CalibrationResult>();
        private bool m_IsCalibrating = false;
        private float m_LastStrengthValue;

        [Header("Spear Settings")]
        [SerializeField] bool m_StabilizeFlight = true;
        [SerializeField] float m_AlignmentSpeed = 5f;

        [Header("Baseball/Underhand Settings")]
        [SerializeField] bool m_ApplyFlickBonus = true;
        [SerializeField] float m_VerticalBoost = 1.2f;

        private Rigidbody m_Rb;
        private bool m_InFlight = false;

        public bool canProcess => isActiveAndEnabled;

        private struct CalibrationResult
        {
            public float ReleaseStrength;
            public float RawSpeed;
            public float AimAccuracy;
        }

        protected override void Awake()
        {
            base.Awake();
            m_Rb = GetComponent<Rigidbody>();
            selectFilters.Add(this);

            if (m_UseCalibration) StartCalibration();
        }

        public void StartCalibration()
        {
            m_IsCalibrating = true;
            m_RecordedThrows.Clear();
            m_ReleaseThreshold = 0.1f;
        }

        protected override void Detach()
        {
            base.Detach();

            Vector3 baseVelocity = m_Rb.linearVelocity;
            Vector3 baseAngularVelocity = m_Rb.angularVelocity;

            if (m_IsCalibrating)
            {
                RecordCalibrationData(baseVelocity);
            }

            Vector3 enhancedVelocity = CalculateEnhancedVelocity(baseVelocity, baseAngularVelocity);
            m_Rb.linearVelocity = enhancedVelocity;

            if (m_ThrowingStyle == ThrowingStyle.Spear)
            {
                m_InFlight = true;
                m_Rb.angularVelocity = baseAngularVelocity * 0.1f;
            }
        }

        private void RecordCalibrationData(Vector3 velocity)
        {
            float speed = velocity.magnitude;
            if (speed < 0.5f) return;

            CalibrationResult result = new CalibrationResult
            {
                ReleaseStrength = m_LastStrengthValue,
                RawSpeed = speed,
                AimAccuracy = Vector3.Dot(velocity.normalized, transform.forward)
            };

            m_RecordedThrows.Add(result);

            if (m_RecordedThrows.Count >= m_RequiredThrows)
            {
                FinishCalibration();
            }
        }

        private void FinishCalibration()
        {
            m_IsCalibrating = false;

            float avgStrength = m_RecordedThrows.Average(t => t.ReleaseStrength);
            float avgAccuracy = m_RecordedThrows.Average(t => t.AimAccuracy);
            float maxSpeed = m_RecordedThrows.Max(t => t.RawSpeed);

            m_ReleaseThreshold = Mathf.Clamp(avgStrength, 0.05f, 0.5f);

            m_DirectionalSmoothing = Mathf.Clamp(1.0f - avgAccuracy, 0.2f, 0.8f);

            if (maxSpeed < 5f) m_PowerMultiplier = 2.5f;
            else if (maxSpeed < 10f) m_PowerMultiplier = 1.8f;
            else m_PowerMultiplier = 1.2f;
        }

        private Vector3 CalculateEnhancedVelocity(Vector3 velocity, Vector3 angularVelocity)
        {
            Vector3 finalVelocity = velocity;

            switch (m_ThrowingStyle)
            {
                case ThrowingStyle.Spear:
                    float forwardSpeed = Vector3.Dot(velocity, transform.forward);
                    Vector3 directionalForce = transform.forward * forwardSpeed;
                    finalVelocity = Vector3.Lerp(velocity, directionalForce, m_DirectionalSmoothing);
                    finalVelocity *= m_PowerMultiplier;
                    break;

                case ThrowingStyle.Baseball:
                    Vector3 aimDirection = transform.forward;
                    float rawSpeed = velocity.magnitude;
                    Vector3 flattenedDirection = Vector3.Lerp(velocity.normalized, aimDirection, m_DirectionalSmoothing).normalized;
                    float flickBonus = Mathf.Clamp(angularVelocity.magnitude * 0.15f, 0f, 10f);
                    float finalSpeed = (rawSpeed * m_PowerMultiplier) + flickBonus;
                    finalVelocity = flattenedDirection * finalSpeed;
                    break;

                case ThrowingStyle.Underhand:
                    finalVelocity.y *= m_VerticalBoost;
                    finalVelocity.x *= m_PowerMultiplier;
                    finalVelocity.z *= m_PowerMultiplier;
                    break;
            }

            return finalVelocity;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected)
            {
                foreach (var interactor in interactorsSelecting)
                {
                    if (interactor is IXRInteractionStrengthInteractor strengthInteractor)
                    {
                        m_LastStrengthValue = strengthInteractor.GetInteractionStrength(this);
                    }
                }
            }

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed && m_InFlight)
            {
                if (m_ThrowingStyle == ThrowingStyle.Spear && m_StabilizeFlight)
                {
                    UpdateSpearFlight();
                }
            }
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            if (interactor is IXRInteractionStrengthInteractor strengthInteractor)
            {
                float currentStrength = strengthInteractor.GetInteractionStrength(interactable);
                if (interactorsSelecting.Contains(interactor))
                {
                    return currentStrength > m_ReleaseThreshold;
                }
            }
            return true;
        }

        private void UpdateSpearFlight()
        {
            if (m_Rb.linearVelocity.magnitude > 1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(m_Rb.linearVelocity);
                m_Rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * m_AlignmentSpeed));
            }
            else
            {
                m_InFlight = false;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            m_InFlight = false;
        }
    }
}