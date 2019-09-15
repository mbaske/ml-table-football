using UnityEngine;
using System.Collections.Generic;

namespace TableFootball
{
    public class Team : MonoBehaviour
    {
        public List<PlayerPosition> Positions;
        public GameObject Goal;

        // One of the team gameobjects is y-rotated by 180 degrees.
        // Flip sign to calculate identical observations for each team.
        public float Sign { get; private set; }

        [SerializeField]
        GameObject highlightGoal;
        [SerializeField]
        GameObject highlightReset;
        int flashCount;

        public void Initialize()
        {
            Sign = transform.localEulerAngles.y < 90 ? 1f : -1f;

            foreach (PlayerPosition pp in Positions)
            {
                pp.Initialize(Sign);
            }
        }

        public void ReSet()
        {
            foreach (PlayerPosition pp in Positions)
            {
                pp.ReSet();
            }

            HighlightReset();
        }

        public void StepUpdate(float[] actions)
        {
            int i = 0;
            foreach (PlayerPosition pp in Positions)
            {
                pp.StepUpdate(actions[i++], actions[i++]);
            }
        }

        public List<float> GetNormalizedObs()
        {
            List<float> obs = new List<float>();
            foreach (PlayerPosition pp in Positions)
            {
                obs.Add(pp.GetNormalizedVelocity());
                obs.Add(pp.GetNormalizedAngularVelocity());
                obs.Add(pp.GetNormalizedPosition());
                obs.Add(pp.GetNormalizedAngle());
            }
            return obs;
        }

        // FX / Highlights 

        public void HighlightGoal()
        {
            KillHighlights();
            InvokeRepeating("ToggleHighlightGoal", 0, 0.05f);
        }

        public void HighlightReset()
        {
            KillHighlights();
            InvokeRepeating("ToggleHighlightReset", 0, 0.05f);
        }

        void ToggleHighlightGoal()
        {
            highlightGoal.SetActive(!highlightGoal.activeSelf);
            flashCount++;
            if (flashCount == 8)
            {
                KillHighlights();
            }
        }

        void ToggleHighlightReset()
        {
            highlightReset.SetActive(!highlightReset.activeSelf);
            flashCount++;
            if (flashCount == 4)
            {
                KillHighlights();
            }
        }

        void KillHighlights()
        {
            CancelInvoke();
            flashCount = 0;
            highlightGoal.SetActive(false);
            highlightReset.SetActive(false);
        }
    }
}