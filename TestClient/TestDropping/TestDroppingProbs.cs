using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestDropping {
    class TestDroppingProbs {
        
        public void TestModifyWeights() {
            float[] weights = new float[] { 0.1f, 0.15f, 0.15f, 0.2f, 0.4f };
            float prob = 0.5f;
            float distribValue = weights[4] * prob;
            weights[4] -= distribValue;
            float val = distribValue / (weights.Length - 1);
            for(int i = 0; i < weights.Length - 1; i++) {
                weights[i] += val;
            }

            float accum = 0f;
            for(int i = 0; i < weights.Length; i++ ) {
                Console.Write("{0}={1}, ", i, weights[i]);
                accum += weights[i];
            }
            Console.WriteLine();
            Console.WriteLine("SUM: " + accum);
        }
    }
}
