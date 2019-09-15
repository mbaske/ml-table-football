using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TableFootball
{
    public class TrailUI : MonoBehaviour
    {
        [SerializeField]
        Ball ball;
        [SerializeField]
        Team teamRed;
        [SerializeField]
        Color[] colors;

        bool draw;
        int crntTeamIndex = -1;
        Queue<Vector3> contacts;
        List<LineRenderer> lines;

        void Start()
        {
            contacts = new Queue<Vector3>();
            lines = new List<LineRenderer>();

            ball.ResetEventHandler += OnReset;
            ball.AutoKickEventHandler += OnAutoKick;
            ball.PlayerContactEventHandler += OnPlayerContact;
            ball.TableContactEventHandler += OnTableContact;
            ball.GoalEventHandler += OnGoal;
        }

        void Update()
        {
            if (draw)
            {
                Vector3[] c = contacts.ToArray();
                int n = c.Length - 1;

                Color col = colors[crntTeamIndex];
                col.a = 0;
                float a = 1 / (float)c.Length;

                LineRenderer line;
                for (int i = 0; i < n; i++)
                {
                    line = GetLine(i);
                    line.SetPosition(0, c[i]);
                    line.SetPosition(1, c[i + 1]);
                    line.startColor = col;
                    col.a += a;
                    line.endColor = col;
                }

                line = GetLine(n);
                line.SetPosition(0, c[n]);
                line.SetPosition(1, ball.transform.position);
                line.startColor = col;
                col.a += a;
                line.endColor = col;

                DisableLines(c.Length);
            }
        }

        LineRenderer GetLine(int index)
        {
            if (index > lines.Count - 1)
            {
                lines.Add(CreateLineRenderer());
            }
            transform.GetChild(index).gameObject.SetActive(true);
            return lines[index];
        }

        void DisableLines(int startIndex)
        {
            for (int i = startIndex; i < lines.Count; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        void ReSet()
        {
            DisableLines(0);
            contacts.Clear();
            crntTeamIndex = -1;
            draw = false;
        }

        void OnReset(object sender, BallEvent e)
        {
            ReSet();
        }

        void OnAutoKick(object sender, BallEvent e)
        {
            ReSet();
        }

        void OnPlayerContact(object sender, BallEvent e)
        {
            if (e.State == BallEvent.CollisionState.Enter)
            {
                int index = e.Team == teamRed.gameObject ? 0 : 1;
                if (index != crntTeamIndex)
                {
                    contacts.Clear();
                    crntTeamIndex = index;
                    draw = true;
                }
                contacts.Enqueue(e.Object.transform.position);
            }
        }

        void OnTableContact(object sender, BallEvent e)
        {
            if (e.State == BallEvent.CollisionState.Enter)
            {
                contacts.Enqueue(ball.transform.position);
            }
        }

        void OnGoal(object sender, BallEvent e)
        {
             // Highlight.
            foreach (LineRenderer line in lines)
            {
                Color col = colors[2];
                col.a = line.startColor.a;
                line.startColor = col;
                col.a = line.endColor.a;
                line.endColor = col;
            }
            draw = false;
        }

        void OnApplicationQuit()
        {
            ball.ResetEventHandler -= OnReset;
            ball.AutoKickEventHandler -= OnAutoKick;
            ball.PlayerContactEventHandler -= OnPlayerContact;
            ball.TableContactEventHandler -= OnTableContact;
            ball.GoalEventHandler -= OnGoal;
        }

        private LineRenderer CreateLineRenderer()
        {
            LineRenderer line = new GameObject().AddComponent<LineRenderer>();
            line.transform.parent = transform;
            line.transform.name = "Line";
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.widthMultiplier = 0.01f;
            line.positionCount = 2;
            line.alignment = LineAlignment.View;
            line.receiveShadows = false;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            line.gameObject.layer = gameObject.layer;
            return line;
        }
    }
}
