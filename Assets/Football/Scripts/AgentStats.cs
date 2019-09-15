using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TableFootball
{
    /// <summary>
    /// Contains stats and reward info for a single agent.
    /// </summary>
    public class AgentStats
    {
        // Reward types.
        public static int SHOT_REWARD = 0;
        public static int SPIN_PENALTY = 1;

        public string Name { get; private set; }
        // The number of scored goals in the current game.
        // The game's duration depends on the agent's & academy's max step property.
        public int GoalsScored { get; private set; }
        // Ball possession starts when an agent player kicks the ball and ends either
        // when an opponent player touches it, when a goal is scored, or when the ball's 
        // velocity drops below some threshold value.
        public bool HasBall => ballTimer.IsRunning;

        Timer gameTimer;
        Timer ballTimer;
        
        int totalGoalsCount;
        int totalGoalsScored;

        Queue<float>[] rewardQueues;
        const int queueSize = 5;
        const int numRewardTypes = 2;

        public AgentStats(string name)
        {
            Name = name;

            gameTimer = new Timer();
            ballTimer = new Timer();

            rewardQueues = new Queue<float>[numRewardTypes];
            for (int i = 0; i < numRewardTypes; i++)
            {
                rewardQueues[i] = new Queue<float>();
            }
        }

        /// <summary>
        /// Returns the average reward over the past [queueSize] steps.
        /// <param name="type">Reward type.</param>
        /// </summary>
        public float GetReward(int type)
        {
            return rewardQueues[type].Count > 0 ? rewardQueues[type].Average() : 0;
        }

        /// <summary>
        /// Returns the ball possession ratio (0 - 1) for the current game.
        /// </summary>
        public float GetBallPossession()
        {
            return ballTimer.EllapsedTotal / Mathf.Max(1f, gameTimer.EllapsedTotal);
        }

        /// <summary>
        /// Returns the overall score rate (0 - 1) since applictaion start.
        /// </summary>
        public float GetOverallScoreRate()
        {
            return totalGoalsScored / Mathf.Max(1f, totalGoalsCount);
        }

        public void Reset()
        {
            GoalsScored = 0;

            gameTimer.Reset();
            ballTimer.Reset();

            for (int i = 0; i < numRewardTypes; i++)
            {
                rewardQueues[i].Clear();
            }
        }

        public void AddReward(int type, float value)
        {
            rewardQueues[type].Enqueue(value);
            if (rewardQueues[type].Count > queueSize)
            {
                rewardQueues[type].Dequeue();
            }
        }

        public void OnGoal(bool hasScored)
        {
            gameTimer.StopInterval();

            if (HasBall)
            {
                ballTimer.StopInterval();
            }

            if (hasScored)
            {
                GoalsScored++;
                totalGoalsScored++;
            }

            totalGoalsCount++;
        }

        public void OnPlayerContact(bool isAgentTeam)
        {
            if (!gameTimer.IsRunning)
            {
                gameTimer.StartInterval();
            }

            if (HasBall)
            {
                ballTimer.StopInterval();
            }

            if (isAgentTeam)
            {
                ballTimer.StartInterval();
            }
        }

        public void OnAutoKick()
        {
            if (gameTimer.IsRunning)
            {
                // TBD Maybe keep gameTimer running?
                // Ball possession ratios won't add up to 100% though.
                gameTimer.StopInterval();
            }

            if (HasBall)
            {
                ballTimer.StopInterval();
            }
        }
    }
}
