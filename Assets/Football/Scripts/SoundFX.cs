using UnityEngine;

namespace TableFootball
{
    public class SoundFX : MonoBehaviour
    {
        [SerializeField]
        Ball ball;
        [SerializeField]
        Team teamRed;

        [Space]
        [Header("Audio Clips")]
        [SerializeField]
        AudioClip reset;
        [SerializeField]
        AudioClip bounce;
        [SerializeField]
        AudioClip kickA;
        [SerializeField]
        AudioClip kickB;
        [SerializeField]
        AudioClip goalA;
        [SerializeField]
        AudioClip goalB;

        AudioSource source;

        void Start()
        {
            source = GetComponent<AudioSource>();

            ball.ResetEventHandler += OnReset;
            ball.PlayerContactEventHandler += OnPlayerContact;
            ball.TableContactEventHandler += OnTableContact;
            ball.GoalEventHandler += OnGoal;
        }

        void OnReset(object sender, BallEvent e)
        {
            source.PlayOneShot(reset);
        }

        void OnPlayerContact(object sender, BallEvent e)
        {
            if (e.State == BallEvent.CollisionState.Exit)
            {
                if (e.Team == teamRed.gameObject)
                {
                    source.PlayOneShot(kickA);
                }
                else
                {
                    source.PlayOneShot(kickB);
                }
            }
        }

        void OnTableContact(object sender, BallEvent e)
        {
            if (e.State == BallEvent.CollisionState.Exit)
            {
                source.PlayOneShot(bounce, 0.5f);
            }
        }

        void OnGoal(object sender, BallEvent e)
        {
            if (e.Team == teamRed.gameObject)
            {
                source.PlayOneShot(goalA, 0.75f);
            }
            else
            {
                source.PlayOneShot(goalB, 0.75f);
            }
        }

        void OnApplicationQuit()
        {
            ball.ResetEventHandler -= OnReset;
            ball.PlayerContactEventHandler -= OnPlayerContact;
            ball.TableContactEventHandler -= OnTableContact;
            ball.GoalEventHandler -= OnGoal;
        }
    }
}
