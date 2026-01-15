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
        [SerializeField, Range(1f, 3f)] float m_PowerMultiplier = 1.2f;

        [Tooltip("Koliko ignorirati gravitaciju? 0 = parabola, 1 = laser dosl")]
        [SerializeField, Range(0f, 1f)] float m_TrajectoryFlatness = 0.2f;

        [SerializeField, Range(0f, 1f)] float m_DirectionalSmoothing = 0.5f;
        [SerializeField, Range(0f, 1f)] public float m_ReleaseThreshold = 0.25f;

        [Header("Calibration Settings")]
        [SerializeField] bool m_UseCalibration = false; //isključeno
        [SerializeField] int m_RequiredThrows = 5;
        [Tooltip("m/s koji zelite dobiti")]
        [SerializeField] float m_TargetIdealSpeed = 12f;

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

            Debug.Log($"[EnhancedThrowable] RAW: {baseVelocity.magnitude:F2} m/s");

            if (m_IsCalibrating)
            {
                RecordCalibrationData(baseVelocity);
            }

            Vector3 enhancedVelocity = CalculateEnhancedVelocity(baseVelocity, baseAngularVelocity);
            m_Rb.linearVelocity = enhancedVelocity;

            m_InFlight = true;

            if (m_ThrowingStyle == ThrowingStyle.Spear)
            {
                m_Rb.angularVelocity = baseAngularVelocity * 0.1f;
            }

            Debug.Log($"[EnhancedThrowable] BAČENO: Flatness {m_TrajectoryFlatness * 100}% | Speed {enhancedVelocity.magnitude:F2}");
        }

        private void RecordCalibrationData(Vector3 velocity)
        {
            float speed = velocity.magnitude;

            if (speed < 1.0f)
            {
                Debug.LogWarning("[EnhancedThrowable] Ispalo");
                return;
            }

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
            float avgSpeed = m_RecordedThrows.Average(t => t.RawSpeed);

            m_ReleaseThreshold = Mathf.Clamp(avgStrength * 0.5f, 0.05f, 0.8f);
            m_DirectionalSmoothing = Mathf.Clamp(1.0f - avgAccuracy, 0.1f, 0.6f);

            float neededMultiplier = m_TargetIdealSpeed / Mathf.Max(avgSpeed, 1f);
            m_PowerMultiplier = Mathf.Clamp(neededMultiplier, 1.0f, 1.8f);

            Debug.Log($"[EnhancedThrowable] KALIBRIRANO Mult: {m_PowerMultiplier:F2} | Thresh: {m_ReleaseThreshold:F2}");
        }

        private Vector3 CalculateEnhancedVelocity(Vector3 velocity, Vector3 angularVelocity)
        {
            Vector3 finalVelocity = velocity;

            switch (m_ThrowingStyle)
            {
                case ThrowingStyle.Spear:
                    float forwardSpeed = Vector3.Dot(velocity, transform.forward);
                    if (forwardSpeed > 0)
                    {
                        Vector3 directionalForce = transform.forward * forwardSpeed;
                        finalVelocity = Vector3.Lerp(velocity, directionalForce, m_DirectionalSmoothing);
                    }
                    finalVelocity *= m_PowerMultiplier;
                    break;

                case ThrowingStyle.Baseball:
                    Vector3 aimDirection = transform.forward;
                    float rawSpeed = velocity.magnitude;

                    if (Vector3.Dot(velocity.normalized, aimDirection) > 0)
                    {
                        Vector3 flattenedDirection = Vector3.Lerp(velocity.normalized, aimDirection, m_DirectionalSmoothing).normalized;
                        finalVelocity = flattenedDirection * rawSpeed;
                    }

                    float flickBonus = m_ApplyFlickBonus ? Mathf.Clamp(angularVelocity.magnitude * 0.1f, 0f, 3f) : 0f;

                    if (m_TrajectoryFlatness > 0.4f && finalVelocity.y < 0)
                    {
                        finalVelocity.y *= 0.5f;
                    }

                    float finalSpeed = (rawSpeed * m_PowerMultiplier) + flickBonus;
                    finalVelocity = finalVelocity.normalized * finalSpeed;
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
                ApplyFlightPhysics();
            }
        }

        private void ApplyFlightPhysics()
        {
            if (m_ThrowingStyle == ThrowingStyle.Spear && m_StabilizeFlight)
            {
                if (m_Rb.linearVelocity.magnitude > 1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(m_Rb.linearVelocity);
                    m_Rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * m_AlignmentSpeed));
                }
            }

            if (m_TrajectoryFlatness > 0.05f && m_Rb.linearVelocity.magnitude > 2.0f)
            {
                Vector3 counterGravity = -Physics.gravity * m_TrajectoryFlatness;

                if (m_ThrowingStyle == ThrowingStyle.Underhand) counterGravity *= 0.2f;

                m_Rb.AddForce(counterGravity, ForceMode.Acceleration);
            }
            else if (m_Rb.linearVelocity.magnitude < 1.0f)
            {
                m_InFlight = false;
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

        private void OnCollisionEnter(Collision collision)
        {
            m_InFlight = false;
            Debug.Log($"[EnhancedThrowable] Brzina udara: {collision.relativeVelocity.magnitude:F2}");
        }
    }

    }