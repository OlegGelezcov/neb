using Common;
using GameMath;
using System;
using System.Collections.Generic;

namespace nebula_console_test {
    public class RemapWeightsTest {


        public void CollectStatistics(float[] remaps ) {
            Dictionary<float, Dictionary<ObjectColor, int>> m_Stats = new Dictionary<float, Dictionary<ObjectColor, int>>();
            for(int i = 0; i < remaps.Length; i++ ) {
                Dictionary<ObjectColor, int> m_ColorStats = new Dictionary<ObjectColor, int>() {
                    { ObjectColor.white, 0 },
                    { ObjectColor.blue, 0 },
                    { ObjectColor.yellow, 0 },
                    { ObjectColor.green, 0 },
                    { ObjectColor.orange, 0}
                };
                m_Stats[remaps[i]] = m_ColorStats;
                for(int j = 0; j < 1000; j++ ) {
                    m_ColorStats[Test(remaps[i])]++;
                }
            }

            Console.WriteLine("OUTPUT:");
            foreach(var kvp in m_Stats ) {
                Console.WriteLine("Remap: {0}", kvp.Key);
                Console.WriteLine("white: {0}, blue: {1}, yellow: {2}, green: {3}, orange: {4}",
                    kvp.Value[ObjectColor.white], kvp.Value[ObjectColor.blue], kvp.Value[ObjectColor.yellow], kvp.Value[ObjectColor.green], kvp.Value[ObjectColor.orange]);
                Console.WriteLine("-------------------------");
            }
            Console.WriteLine("END OUTPUT");
        }
        public ObjectColor Test(float remapWeight) {
            float[] probs = { 0.005f, 0.01f, 0.02f, 0.05f, 1.0f };
            float accum = 0f;
            float[] weights = new float[probs.Length];
            weights[0] = probs[0];
            accum += weights[0];
            weights[1] = probs[1];
            accum += weights[1];
            weights[2] = probs[2];
            accum += weights[2];
            weights[3] = probs[3];
            accum += weights[3];
            weights[4] = 1.0f - accum;

            Console.WriteLine("source probs: ");
            PrintProbs(weights);

            remapWeight = Mathf.Clamp01(remapWeight);
            float val = weights[weights.Length - 1] * remapWeight;
            weights[weights.Length - 1] -= val;
            float unitVal = val / (weights.Length - 1);
            Console.WriteLine("Unit added %: {0}", 100 * unitVal);
            for(int i = 0; i < weights.Length - 1; i++ ) {
                weights[i] += unitVal;
            }

            Console.WriteLine("Remapped probs:");
            PrintProbs(weights);

            int index = Rand.RandomIndex(weights);
            ObjectColor resultColor = ObjectColor.white;
            switch(index) {
                case 0:
                    resultColor = ObjectColor.orange;
                    break;
                case 1:
                    resultColor = ObjectColor.green;
                    break;
                case 2:
                    resultColor = ObjectColor.yellow;
                    break;
                case 3:
                    resultColor = ObjectColor.blue;
                    break;
                default:
                    resultColor = ObjectColor.white;
                    break;
            }
            Console.WriteLine("Final color: {0}", resultColor);
            return resultColor;
        }

        private void PrintProbs(float[] probs) {
            for(int i = 0; i < probs.Length; i++ ) {
                Console.Write(string.Format("{0}: {1}%, ", i, System.Math.Round(100 * probs[i])));
            }
            Console.WriteLine();
        }
    }
}
