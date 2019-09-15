using UnityEngine;

namespace TableFootball
{
    public class StatsUI : MonoBehaviour
    {
        [SerializeField]
        FootballAgent agentA;
        [SerializeField]
        FootballAgent agentB;

        GUIStyle style;

        Bar bg, progress;
        Bar possA, rateA, shotA, spinA;
        Bar possB, rateB, shotB, spinB;

        void Start()
        {
            style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white * 0.9f;

            bg = new Bar(Bar.Type.Background, 0, 0, 320, 172);
            progress = new Bar(Bar.Type.Progress, 10, 10, 300, 14);

            possA = new Bar(Bar.Type.Percent, 10, 62);
            rateA = new Bar(Bar.Type.Percent, 10, 92);
            shotA = new Bar(Bar.Type.AutoMinMax, 10, 122);
            spinA = new Bar(Bar.Type.AutoInvMin, 10, 152);
            
            possB = new Bar(Bar.Type.Percent, 165, 62);
            rateB = new Bar(Bar.Type.Percent, 165, 92);
            shotB = new Bar(Bar.Type.AutoMinMax, 165, 122);
            spinB = new Bar(Bar.Type.AutoInvMin, 165, 152);
        }

        void OnGUI()
        {
            bg.Draw();
            progress.Draw(agentA.Progress);

            style.fontSize = 12;
            GUI.Label(new Rect(11, 10, 110, 200), "GAME #" + agentA.GameCount, style);

            style.fontSize = 14;
            GUI.Label(new Rect(10, 28, 110, 20), TeamGoals(agentA.Stats), style);
            possA.Draw(agentA.Stats.GetBallPossession());
            rateA.Draw(agentA.Stats.GetOverallScoreRate());
            shotA.Draw(agentA.Stats.GetReward(AgentStats.SHOT_REWARD));
            spinA.Draw(agentA.Stats.GetReward(AgentStats.SPIN_PENALTY));
            GUI.Label(new Rect(165, 28, 110, 20), TeamGoals(agentB.Stats), style);
            possB.Draw(agentB.Stats.GetBallPossession());
            rateB.Draw(agentB.Stats.GetOverallScoreRate());
            shotB.Draw(agentB.Stats.GetReward(AgentStats.SHOT_REWARD));
            spinB.Draw(agentB.Stats.GetReward(AgentStats.SPIN_PENALTY));

            style.fontSize = 11;
            GUI.Label(new Rect(10, 48, 110, 20), "Ball Possession", style);
            GUI.Label(new Rect(10, 78, 110, 20), "Overall Score Rate", style);
            GUI.Label(new Rect(10, 108, 110, 20), "Shot Reward", style);
            GUI.Label(new Rect(10, 138, 110, 20), "Spin Penalty", style);
        }

        string TeamGoals(AgentStats stats)
        {
            return string.Format("{0} {1}", stats.Name.ToUpper(), stats.GoalsScored);
        }
    }

    public class Bar
    {
        static GUIStyle style = CreateStyle();
        static Texture2D txBg1 = CreateTexture(1, 1, 0.8f, 0.1f);
        static Texture2D txBg2 = CreateTexture(0, 0, 0, 0.7f);
        static Texture2D txMrk = CreateTexture(1, 1, 1, 0.5f);
        static Texture2D txDef = CreateTexture(0, 0.3f, 0.4f);
        static Texture2D txPos = CreateTexture(0, 0.7f, 0);
        static Texture2D txNeg = CreateTexture(0.5f, 0, 0);

        public enum Type
        {
            Background,
            Progress,
            Percent,
            AutoMinMax,
            AutoInvMin
        }

        Type type;
        Rect rect;
        float min;
        float max;

        public Bar(Type type, int x, int y, int width = 145, int height = 10)
        {
            this.type = type;
            rect = new Rect(x, y, width, height);
        }

        public void Draw(float value = 0)
        {
            if (type == Type.Background)
            {
                GUI.DrawTexture(rect, txBg1);
            }
            else
            {
                min = Mathf.Min(min, value);
                max = Mathf.Max(max, value);

                GUI.DrawTexture(rect, txBg2);
                Rect r = rect;

                if (type == Type.AutoInvMin)
                {
                    r.width *= Mathf.InverseLerp(0, -min, -value);
                    GUI.DrawTexture(r, txNeg);
                    GUI.Label(rect, Round(min), style);
                }
                else if (type == Type.AutoMinMax)
                {
                    int zero = GetPos(0);
                    GUI.DrawTexture(new Rect(zero, r.y, 1, r.height), txMrk);

                    if (value < 0)
                    {
                        r.xMin = GetPos(value);
                        r.xMax = zero;
                        GUI.DrawTexture(r, txNeg);
                    }
                    else
                    {
                        r.xMin = zero;
                        r.xMax = GetPos(value);
                        GUI.DrawTexture(r, txPos);
                    }

                    style.alignment = TextAnchor.MiddleLeft;
                    GUI.Label(rect, Round(min), style);
                    style.alignment = TextAnchor.MiddleRight;
                    GUI.Label(rect, Round(max), style);
                }
                else
                {
                    r.width *= value;
                    GUI.DrawTexture(r, txDef);

                    if (type == Type.Percent)
                    {
                        r.width += Mathf.Lerp(0, 13, 1 - value * 5f); // dyn. text offset
                        GUI.Label(r, Percent(value), style);
                    }
                }
            }
        }

        int GetPos(float value)
        {
            return Mathf.RoundToInt(Mathf.InverseLerp(min, max, value) * rect.width + rect.xMin);
        }

        static string Round(float value)
        {
            return (Mathf.Round(value * 1000) / 1000f).ToString();
        }

        static string Percent(float value)
        {
            return Mathf.Round(value * 100).ToString() + "%";
        }

        static Texture2D CreateTexture(float r = 0, float g = 0, float b = 0, float a = 1)
        {
            Texture2D t = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            t.SetPixel(0, 0, new Color(r, g, b, a));
            t.Apply();
            return t;
        }

        static GUIStyle CreateStyle()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 9;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleRight;
            style.normal.textColor = Color.white * 0.9f;
            return style;
        }
    }
}