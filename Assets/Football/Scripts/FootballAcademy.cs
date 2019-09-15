using MLAgents;

namespace TableFootball
{
    public class FootballAcademy : Academy
    {
        Ball[] balls;

        public override void InitializeAcademy()
        {
            balls = FindObjectsOfType<Ball>();
            foreach (Ball ball in balls)
            {
                ball.Initialize();
            }
        }

        public override void AcademyReset()
        {
            foreach (Ball ball in balls)
            {
                ball.ReSet();
            }
        }

        public override void AcademyStep()
        {
        }
    }
}