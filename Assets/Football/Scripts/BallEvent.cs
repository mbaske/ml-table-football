using UnityEngine;

namespace TableFootball
{
    public class BallEvent
    {
        public enum CollisionState
        {
            Enter,
            Stay,
            Exit
        }

        public Collision Collision { get; private set; }
        public CollisionState State { get; private set; }
        public GameObject Object { get; private set; }

        public GameObject Team { get; private set; }
        public GameObject Position { get; private set; }
        public GameObject Player { get; private set; }

        public BallEvent(Collision collision, CollisionState state)
        {
            State = state;
            Collision = collision;
            Object = collision.collider.gameObject;

            if (Object.CompareTag(Tags.PLAYER))
            {
                Player = Object;
                Position = Player.transform.parent.gameObject;
                Team = Position.transform.parent.gameObject;
            }
        }

        public BallEvent(GameObject obj)
        {
            Object = obj;

            if (Object.CompareTag(Tags.GOAL))
            {
                Team = Object.transform.parent.gameObject;
            }
        }

        public BallEvent()
        {
        }
    }
}