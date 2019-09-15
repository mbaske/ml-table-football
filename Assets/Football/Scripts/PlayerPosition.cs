using UnityEngine;
using System.Collections.Generic;

namespace TableFootball
{
    /// <summary>
    /// PlayerPosition corresponds to a player rod for Attack, Midfield, Defense or Keeper.
    /// </summary>
    public class PlayerPosition : MonoBehaviour
    {
        // The individual player figures on a single rod.
        public List<Transform> Players;

        const float moveForce = 1f;
        const float turnForce = 25f;

        const float maxAngularVelocity = 25f;
        // Measured maximum velocity.
        const float maxVelocity = 3.4f;

        Rigidbody rb;
        ConfigurableJoint joint;

        float sign;
        float limit;
        Vector3 defPos;
        Quaternion defRot;

        public void Initialize(float sign)
        {
            this.sign = sign;

            defPos = transform.localPosition;
            defRot = transform.localRotation;

            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = Vector3.zero;
            rb.maxAngularVelocity = maxAngularVelocity;

            joint = GetComponent<ConfigurableJoint>();
            limit = joint.linearLimit.limit;
        }

        /// <summary>
        /// Moves and turns the player rod.
        /// <param name="move">Normalized move amount (-1/+1).</param>
        /// <param name="turn">Normalized turn amount (-1/+1).</param>
        /// </summary>
        public void StepUpdate(float move, float turn)
        {
            rb.AddRelativeForce(Vector3.right * move * moveForce, ForceMode.VelocityChange);
            rb.AddRelativeTorque(Vector3.right * turn * turnForce, ForceMode.VelocityChange);
        }

        public void ReSet()
        {
            transform.localPosition = defPos;
            transform.localRotation = defRot;
            joint.targetPosition = Vector3.zero;
            joint.targetRotation = Quaternion.identity;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Returns the normalized rod angle (-1/+1).
        /// </summary>
        public float GetNormalizedAngle()
        {
            return Vector3.SignedAngle(transform.up, Vector3.up, Vector3.right) * sign / 180f;
        }

        /// <summary>
        /// Returns the normalized rod position (-1/+1).
        /// </summary>
        public float GetNormalizedPosition()
        {
            return transform.localPosition.x / limit;
        }

        /// <summary>
        /// Returns the normalized rod velocity (-1/+1).
        /// </summary>
        public float GetNormalizedVelocity()
        {
            return rb.velocity.x * sign / maxVelocity;
        }

        /// <summary>
        /// Returns the normalized angular rod velocity (-1/+1).
        /// </summary>
        public float GetNormalizedAngularVelocity()
        {
            return rb.angularVelocity.x * sign / maxAngularVelocity;
        }
    }
}