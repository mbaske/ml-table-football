using MLAgents;
using UnityEngine;

namespace TableFootball
{
    public class FootballAgent : Agent
    {
        public int ID { get; private set; }
        public AgentStats Stats { get; private set; }
        public int GameCount { get; private set; }
        // NOTE: Change AgentParameters struct and agentParameters field
        // from internal to protected in Agent class line 124ff.
        public float Progress => GetStepCount() / (float)agentParameters.maxStep;

        [SerializeField]
        Team agentTeam;
        [SerializeField]
        Team opponentTeam;
        [SerializeField]
        Ball ball;
        [Tooltip("Observe ball position & velocity on XZ plane only")]
        [SerializeField]
        bool use2DBallObs;

        [Tooltip("Reward for scoring a goal")]
        [SerializeField]
        [Range(0f, 1f)]
        float goalScoredReward = 1f;

        [Tooltip("Penalty for conceding a goal")]
        [SerializeField]
        [Range(0f, 1f)]
        float goalConcededPenalty = 1f;

        [Tooltip("Per step multiplier for rewarding shots, 0 -> disabled")]
        [SerializeField]
        [Range(0f, 0.25f)]
        float shotRewardMultiplier = 0.1f;
        bool useShotReward;

        [Tooltip("Maximum per step penalty for spinning player rods, 0 -> disabled")]
        [SerializeField]
        [Range(0f, 0.1f)]
        float maxSpinPenalty = 0.01f;
        bool useSpinPenalty;

        public override void InitializeAgent()
        {
            ID = gameObject.GetInstanceID();
            Stats = new AgentStats(agentTeam.transform.name);

            ball.AutoKickEventHandler += OnAutoKick;
            ball.PlayerContactEventHandler += OnPlayerContact;
            ball.GoalEventHandler += OnGoal;

            useShotReward = shotRewardMultiplier > 0;
            useSpinPenalty = maxSpinPenalty > 0;
        }

        public override void AgentReset()
        {
            GameCount++;
            agentTeam.ReSet();
            Stats.Reset();
            ball.ReSet();
        }

        // N = 8
        public override void AgentAction(float[] vectorAction)
        {
            agentTeam.StepUpdate(vectorAction);
        }

        // N = 58 - 2D obs
        // N = 60 - 3D obs
        public override void CollectObservations()
        {
            if (use2DBallObs)
            {
                AddVectorObs(ball.GetNormalizedVelocity2D() * agentTeam.Sign);
                Vector2 np = ball.GetNormalizedPosition2D() * agentTeam.Sign;
                AddVectorObs(Util.SplitDecimalPlaces(np.x, 4));
                AddVectorObs(Util.SplitDecimalPlaces(np.y, 4));
            }
            else
            {
                Vector3 nv = ball.GetNormalizedVelocity3D();
                AddVectorObs(nv.x * agentTeam.Sign);
                AddVectorObs(nv.y);
                AddVectorObs(nv.z * agentTeam.Sign);
                Vector3 np = ball.GetNormalizedPosition3D();
                AddVectorObs(Util.SplitDecimalPlaces(np.x * agentTeam.Sign, 4));
                AddVectorObs(np.y);
                AddVectorObs(Util.SplitDecimalPlaces(np.z * agentTeam.Sign, 4));
            }

            float spinSum = 0;
            foreach (PlayerPosition pp in agentTeam.Positions)
            {
                AddVectorObs(pp.GetNormalizedVelocity());
                float spin = pp.GetNormalizedAngularVelocity();
                spinSum += Mathf.Abs(spin);
                AddVectorObs(spin);
                AddVectorObs(Util.SplitDecimalPlaces(pp.GetNormalizedPosition(), 3));
                AddVectorObs(Util.SplitDecimalPlaces(pp.GetNormalizedAngle(), 3));
            }

            AddVectorObs(opponentTeam.GetNormalizedObs());

            if (useShotReward)
            {
                AddShotReward();
            }
            if (useSpinPenalty)
            {
                AddSpinPenalty(spinSum);
            }
        }

        void AddShotReward()
        {
            if (Stats.HasBall)
            {
                Vector3 bp = ball.transform.position;
                Vector3 delta = opponentTeam.Goal.transform.position - bp;
                float reward = shotRewardMultiplier * Vector3.Dot(delta.normalized, ball.Velocity);
                Stats.AddReward(AgentStats.SHOT_REWARD, reward);
                AddReward(reward);
                // Debug.DrawRay(bp, delta, Color.cyan);
                // Debug.DrawRay(bp, ball.Velocity, Color.magenta);
            }
            else
            {
                Stats.AddReward(AgentStats.SHOT_REWARD, 0);
            }
        }

        void AddSpinPenalty(float spinSum)
        {
            float penalty = maxSpinPenalty * spinSum * 0.25f; // 4 player rods
            Stats.AddReward(AgentStats.SPIN_PENALTY, -penalty);
            AddReward(-penalty);
        }

        void OnAutoKick(object sender, BallEvent e)
        {
            Stats.OnAutoKick();
        }

        void OnPlayerContact(object sender, BallEvent e)
        {
            if (e.State == BallEvent.CollisionState.Exit)
            {
                bool isAgentTeam = e.Team == agentTeam.gameObject;
                Stats.OnPlayerContact(isAgentTeam);
            }
        }

        void OnGoal(object sender, BallEvent e)
        {
            bool hasScored = e.Object == opponentTeam.Goal;
            Stats.OnGoal(hasScored);
            AddReward(hasScored ? goalScoredReward : -goalConcededPenalty);

            if (hasScored)
            {
                agentTeam.HighlightGoal();
            }
        }

        void OnApplicationQuit()
        {
            ball.AutoKickEventHandler -= OnAutoKick;
            ball.PlayerContactEventHandler -= OnPlayerContact;
            ball.GoalEventHandler -= OnGoal;
        }
    }
}