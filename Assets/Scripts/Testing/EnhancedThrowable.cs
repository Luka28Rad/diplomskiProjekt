using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
    public class EnhancedThrowable : XRGrabInteractable
    {
        [Header("Enhanced Throwing Settings")]
        [Tooltip("The duration (in seconds) to track the controller's velocity before release. A shorter duration captures more of the final flick of the wrist.")]
        [Range(0.05f, 0.5f)]
        public float throwHistoryDuration = 0.2f;

        [Tooltip("A multiplier for the final linear velocity of the throw. Increases the overall force of the throw.")]
        public float velocityMultiplier = 1.5f;

        [Tooltip("A multiplier for the final angular velocity of the throw. Affects the spin of the object upon release.")]
        public float angularVelocityMultiplier = 1.0f;

        [Tooltip("A factor to control how much the controller's angular velocity contributes to the throw's linear velocity. Higher values are better for dart-like or overhand throws with a strong wrist flick.")]
        [Range(0.0f, 1.0f)]
        public float angularVelocityInfluence = 0.75f;

        [Tooltip("An offset from the throwable object's center of mass to calculate the tangential velocity from. Can be used to fine-tune the throwing arc.")]
        public Vector3 centerOfMassOffset = Vector3.zero;

        private readonly Queue<Pose> poseHistory = new Queue<Pose>();
        private IXRSelectInteractor selectingInteractor = null;
        private Rigidbody rb;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);
            selectingInteractor = args.interactorObject;
            poseHistory.Clear();
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);
            if (args.interactorObject == selectingInteractor)
            {
                selectingInteractor = null;
            }
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (isSelected && selectingInteractor != null && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                poseHistory.Enqueue(new Pose(selectingInteractor.transform.position, selectingInteractor.transform.rotation));

                while (poseHistory.Count > 0 && (Time.time - poseHistory.Peek().time) > throwHistoryDuration)
                {
                    poseHistory.Dequeue();
                }
            }
        }

        protected override void Detach()
        {
            if (throwOnDetach && selectingInteractor != null)
            {
                if (rb.isKinematic)
                {
                    Debug.LogWarning("Cannot throw a kinematic Rigidbody. Disable 'Throw On Detach' or 'Is Kinematic' to fix this.", this);
                    base.Detach();
                    return;
                }

                var (linearVelocity, angularVelocity) = CalculateThrowVelocities();

                rb.linearVelocity = linearVelocity * velocityMultiplier;
                rb.angularVelocity = angularVelocity * angularVelocityMultiplier;
            }
            else
            {
                base.Detach();
            }
        }

        private (Vector3, Vector3) CalculateThrowVelocities()
        {
            if (poseHistory.Count < 2)
            {
                return (Vector3.zero, Vector3.zero);
            }

            var poses = new List<Pose>(poseHistory);
            var averageLinearVelocity = Vector3.zero;
            var averageAngularVelocity = Vector3.zero;

            for (int i = 0; i < poses.Count - 1; i++)
            {
                var currentPose = poses[i];
                var nextPose = poses[i + 1];
                var deltaTime = nextPose.time - currentPose.time;

                if (deltaTime > 0)
                {
                    averageLinearVelocity += (nextPose.position - currentPose.position) / deltaTime;

                    var deltaRotation = nextPose.rotation * Quaternion.Inverse(currentPose.rotation);
                    deltaRotation.ToAngleAxis(out var angle, out var axis);
                    var angularVelocity = (axis * angle * Mathf.Deg2Rad) / deltaTime;
                    averageAngularVelocity += angularVelocity;
                }
            }

            averageLinearVelocity /= (poses.Count - 1);
            averageAngularVelocity /= (poses.Count - 1);

            var tangentialVelocity = Vector3.Cross(averageAngularVelocity, rb.worldCenterOfMass + centerOfMassOffset - selectingInteractor.transform.position);

            var finalLinearVelocity = Vector3.Lerp(averageLinearVelocity, averageLinearVelocity + tangentialVelocity, angularVelocityInfluence);

            return (finalLinearVelocity, averageAngularVelocity);
        }

        private struct Pose
        {
            public Vector3 position;
            public Quaternion rotation;
            public float time;

            public Pose(Vector3 position, Quaternion rotation)
            {
                this.position = position;
                this.rotation = rotation;
                this.time = Time.time;
            }
        }
    }
}