### Adversarial Self-Play Table Football Environment for Unity ML-Agents - [Video](https://www.youtube.com/watch?v=_H11-7eXIko)

This is a table football environment for [Unity Machine Learning Agents](https://github.com/Unity-Technologies/ml-agents), using adversarial self-play.

Agents can manipulate the player rods by exerting forces and torques. I wasn't able to achieve realistic looking rolling ball physics, which might be due to a scale issue. Instead, I went for a sliding puck like behaviour: The ball's material is frictionless, the rigidbody's drag slows it down. The ball receives a random kick if its velocity drops below a threshold value defined in [Ball.cs](https://github.com/mbaske/ml-table-football/blob/master/Assets/Football/Scripts/Ball.cs). Bouncing off the table walls is handled through code.

Agents receive rewards for scoring goals and penalties for conceding them. In all my training attempts thus far, agents eventually ended up spinning the player rods continuously. An optional per-step spin penalty can be added to counteract this. Furthermore, shots can be rewarded proportional to the vector dot product of the ball's velocity and the direction towards the goal (relative to the ball's position). If enabled, this reward is set for each step while an agent controls the ball. Ball possession starts when an agent player first kicks the ball and ends either when an opponent player touches it, when a goal is scored, or when the ball's velocity drops under the threshold as described above. The minimap UI visualizes ball possession as color trails.

Dependencies:
[Unity Machine Learning Agents 0.14.1](https://github.com/Unity-Technologies/ml-agents/releases/tag/0.14.1)  
The `FootballAgent` class needs to access `agentParameters.maxStep` from the base `Agent` class for providing game progress to the stats UI. Please change `AgentParameters` struct and `agentParameters` field from `internal` to `protected` in Agent class line 124ff.

