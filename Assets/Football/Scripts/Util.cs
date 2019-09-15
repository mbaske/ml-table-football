using UnityEngine;

namespace TableFootball
{
    public static class Util
    {
        public static float Sigmoid(float value)
        {
            // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiJ4LygxK2Ficyh4KSkiLCJjb2xvciI6IiMwMDAwMDAifSx7InR5cGUiOjEwMDAsIndpbmRvdyI6WyItMTIiLCIxMiIsIi0xLjIiLCIxLjIiXX1d
            return value / (1f + Mathf.Abs(value));

            // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiIyLygxK2V4cCgtMip4KSktMSIsImNvbG9yIjoiIzAwMDAwMCJ9LHsidHlwZSI6MTAwMCwid2luZG93IjpbIi01IiwiNSIsIi0xLjIiLCIxLjIiXX1d
            // return 2f / (1f + Mathf.Exp(-2f * value)) - 1f;
        }

        public static Vector3 Sigmoid(Vector3 v3)
        {
            v3.x = Sigmoid(v3.x);
            v3.y = Sigmoid(v3.y);
            v3.z = Sigmoid(v3.z);
            return v3;
        }

        /// <summary>
        /// Returns normalized decimal places, e.g. 0.12345 -> [0.1, 0.2, 0.345]
        /// This is supposed to make an agent more sensitive to small changes in observed values.
        /// <param name="value">Value to split.</param>
        /// <param name="n">Number of decimal places.</param>
        /// </summary>
        public static float[] SplitDecimalPlaces(float value, int n = 3)
        {
            float[] result = new float[n];
            float sign = Mathf.Sign(value);
            value = Mathf.Abs(value);
            for (int i = 1; i <= n; i++)
            {
                float d = value * PowInt(10, i) % 10;
                d = i == n ? d : Mathf.Floor(d);
                result[i - 1] = d * sign / 10;
            }
            return result;
        }

        public static float PowInt(float value, int exp)
        {
            float result = 1f;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                {
                    result *= value;
                }
                exp >>= 1;
                value *= value;
            }
            return result;
        }
    }
}