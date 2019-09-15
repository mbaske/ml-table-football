using UnityEngine;
using System.Collections.Generic;

namespace TableFootball
{
    public class DumbHeuristic : MonoBehaviour
    {
        [SerializeField]
        Team team;
        [SerializeField]
        Transform ball;

        float[] actions = new float[8];

        void Start()
        {
            team.Initialize();
        }

        void FixedUpdate()
        {
            float x = ball.position.x;
            float z = ball.localPosition.z;
            for (int i = 0; i < 4; i++)
            {
                PlayerPosition pp = team.Positions[i];
                float noise = Mathf.Sin(Time.time * 10f + i) * 0.15f;
                float dx = GetDeltaX(pp.Players, x) * team.Sign * 5f + noise;
                float dz = Mathf.Abs(z - pp.transform.localPosition.z * team.Sign);
                actions[i * 2] = dx * Mathf.Max(0, 0.5f - dz);
                actions[i * 2 + 1] = Mathf.Max(0, 0.05f - dz * 0.1f);
            }

            team.StepUpdate(actions);
        }

        float GetDeltaX(List<Transform> players, float x)
        {
            float delta = x - players[0].position.x;
            for (int i = 1; i < players.Count; i++)
            {
                float d = x - players[i].position.x;
                if (Mathf.Abs(d) < Mathf.Abs(delta))
                {
                    delta = d;
                }
            }
            return delta;
        }
    }
}