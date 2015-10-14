using Nebula.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestBalance {
    public static class TestBalance {

        public static void Speed() {
            float f = BalanceFormulas.ComputeSPEED(1, 0.3f, 1.008f, 1, 3, 0.5f, 1.008f);
            Console.WriteLine((f * 5));
        }

        public static void Distance() {
            float f = BalanceFormulas.ComputeWeaponOPTIMALDISTANCE(1, 10, 1.02f, 1, 5, 10, 1.02f);
            Console.WriteLine("distance = {0}", f);
        }
    }
}
