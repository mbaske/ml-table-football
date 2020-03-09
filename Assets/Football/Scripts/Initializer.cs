using UnityEngine;

namespace TableFootball
{
    public class Initializer : MonoBehaviour
    {
        Ball[] balls;

        public void Awake()
        {
            balls = FindObjectsOfType<Ball>();
            foreach (Ball ball in balls)
            {
                ball.Initialize();
            }

            Team[] teams = FindObjectsOfType<Team>();
            foreach (Team team in teams)
            {
                team.Initialize();
            }
        }
    }
}