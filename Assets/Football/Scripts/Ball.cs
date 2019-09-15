using System;
using UnityEngine;

namespace TableFootball
{
    public class Ball : MonoBehaviour
    {
        public Vector3 Velocity => rb.velocity;

        Rigidbody rb;
        Vector3 defPos;
        int idleCount;

        const int idleTimeout = 1; // seconds
        const float kickForce = 0.25f;
        const float velocityThreshold = 0.1f;
        const float centerRadius = 0.2f;
        // Bouncing off walls is controlled by the OnCollision handler below,
        // not via bounce properties in physics materials.
        const float wallBounce = 0.9f;

        // Measured values:
        const float minY = 0.065f;
        const float yRange = 0.11f;
        float fieldWidth = 0.762f;
        float fieldLength = 1.4224f;
        // Ball resets when leaving these bounds (which shouldn't happen).
        Bounds bounds = new Bounds(Vector3.zero, new Vector3(0.8f, 0.4f, 1.5f));

        /// <summary>
        /// Returns the ball's normalized position (-1/+1) in 2D space.
        /// </summary>
        public Vector2 GetNormalizedPosition2D()
        {
            return new Vector2(
               transform.localPosition.x / fieldWidth,
               transform.localPosition.z / fieldLength
            );
        }

        /// <summary>
        /// Returns the ball's normalized position (-1/+1) in 3D space.
        /// </summary>
        public Vector3 GetNormalizedPosition3D()
        {
            return new Vector3(
               transform.localPosition.x / fieldWidth,
               ((transform.localPosition.y - minY) / yRange) * 2f - 1f,
               transform.localPosition.z / fieldLength
            );
        }

        /// <summary>
        /// Returns the ball's normalized velocity (-1/+1) in 2D space.
        /// </summary>
        public Vector2 GetNormalizedVelocity2D()
        {
            return new Vector2(
                Util.Sigmoid(rb.velocity.x),
                Util.Sigmoid(rb.velocity.z)
            );
        }

        /// <summary>
        /// Returns the ball's normalized velocity (-1/+1) in 3D space.
        /// </summary>
        public Vector2 GetNormalizedVelocity3D()
        {
            return Util.Sigmoid(rb.velocity);
        }

        public void Initialize()
        {
            rb = GetComponent<Rigidbody>();
            defPos = transform.localPosition;
            float r = transform.localScale.x;
            fieldWidth = (fieldWidth - r) * 0.5f;
            fieldLength = (fieldLength - r) * 0.5f;
        }

        public void ReSet()
        {
            DispatchResetEvent();
            SetToCenter();
        }

        void SetToCenter()
        {
            transform.localPosition = defPos;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            CancelInvoke();
            Invoke("AutoKick", 0.5f);
        }

        void AutoKick()
        {
            DispatchAutoKickEvent();

            Vector2 p = UnityEngine.Random.insideUnitCircle;
            // Kick outward from center.
            Vector3 dir = new Vector3(p.x, 0, p.y);

            if (transform.localPosition.magnitude > centerRadius)
            {
                // Kick towards random place inside [centerRadius].
                p *= centerRadius;
                dir = new Vector3(p.x, 0, p.y) - transform.localPosition;
            }

            rb.AddForce(dir.normalized * kickForce, ForceMode.Impulse);

            idleCount = 0;
            CancelInvoke();
            InvokeRepeating("CheckVelocity", 1, 1);
        }

        void CheckVelocity()
        {
            if (rb.velocity.magnitude < velocityThreshold)
            {
                if (++idleCount == idleTimeout)
                {
                    AutoKick();
                }
            }
            else
            {
                idleCount = 0;
            }
        }

        void Update()
        {
            if (!bounds.Contains(transform.localPosition))
            {
                Debug.LogWarning("Ball out of bounds " + transform.localPosition);
                SetToCenter();
            }
        }

        #region Events

        [HideInInspector]
        public event EventHandler<BallEvent> ResetEventHandler;
        [HideInInspector]
        public event EventHandler<BallEvent> AutoKickEventHandler;
        [HideInInspector]
        public event EventHandler<BallEvent> GoalEventHandler;
        [HideInInspector]
        public event EventHandler<BallEvent> PlayerContactEventHandler;
        [HideInInspector]
        public event EventHandler<BallEvent> TableContactEventHandler; // walls only

        void DispatchResetEvent()
        {
            if (ResetEventHandler != null)
            {
                ResetEventHandler(this, new BallEvent());
            }
        }

        void DispatchAutoKickEvent()
        {
            if (AutoKickEventHandler != null)
            {
                AutoKickEventHandler(this, new BallEvent());
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(Tags.GOAL))
            {
                DispatchGoalEvent(new BallEvent(collider.gameObject));
                SetToCenter();
            }
        }

        void DispatchGoalEvent(BallEvent e)
        {
            if (GoalEventHandler != null)
            {
                GoalEventHandler(this, e);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            OnCollision(new BallEvent(collision, BallEvent.CollisionState.Enter));
        }

        void OnCollisionExit(Collision collision)
        {
            OnCollision(new BallEvent(collision, BallEvent.CollisionState.Exit));
        }

        void OnCollision(BallEvent e)
        {
            if (e.Object.CompareTag(Tags.PLAYER))
            {
                DispatchPlayerContactEvent(e);
            }
            else if (e.Object.CompareTag(Tags.TABLE))
            {
                DispatchTableContactEvent(e);

                if (e.State == BallEvent.CollisionState.Enter)
                {
                    Vector3 p = e.Object.transform.localPosition;
                    Vector3 normal = Mathf.Abs(p.z) > 0.5f
                        ? Vector3.back * Mathf.Sign(p.z)
                        : Vector3.left * Mathf.Sign(p.x);
                    rb.velocity = Vector3.Reflect(-e.Collision.relativeVelocity, normal) * wallBounce;
                }
            }
        }

        void DispatchPlayerContactEvent(BallEvent e)
        {
            if (PlayerContactEventHandler != null)
            {
                PlayerContactEventHandler(this, e);
            }
        }

        void DispatchTableContactEvent(BallEvent e)
        {
            if (TableContactEventHandler != null)
            {
                TableContactEventHandler(this, e);
            }
        }

        #endregion
    }
}