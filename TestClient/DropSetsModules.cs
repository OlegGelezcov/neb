using Common;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory.Objects;
using Space.Game.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient {
    public class DropSetsModules {

        public void Test() {
            for(int i = 0; i < 10; i++ ) {
                TestCase();
            }
        }

        public void TestCase() {
            Dictionary<ShipModelSlotType, int> countDict = new Dictionary<ShipModelSlotType, int>() {
                {  ShipModelSlotType.CB, 0},
                { ShipModelSlotType.CM, 0 },
                { ShipModelSlotType.DF , 0},
                { ShipModelSlotType.DM, 0 },
                { ShipModelSlotType.ES, 0 }
            };
            Res res = new Res(@"C:\development\Nebula\TestClient\bin\Debug");
            res.Load();

            DropManager dropManager = DropManager.Get(res);
            int totalModules = 0;

            while(!Check(countDict)) {
                SchemeDropper schemeDropper = new SchemeDropper(Workshop.RedEye, 5, res);
                var scheme = schemeDropper.Drop() as SchemeObject;
                var module = scheme.Transform(dropManager) as ShipModule;
                totalModules++;
                if(module.Color == ObjectColor.green) {
                    Console.WriteLine("Green module {0} = {1}", module.SlotType, module.Set);
                    countDict[module.SlotType]++;
                }
            }

            Console.WriteLine("============================");
            Console.WriteLine("Total modules = {0}", totalModules);
            foreach(var pair in countDict ) {
                Console.WriteLine("{0}={1}", pair.Key, pair.Value);
            }
            Console.WriteLine("===============================");
        }

        private bool Check(Dictionary<ShipModelSlotType, int> dict ) {
            bool result = true;
            foreach(var p in dict) {
                if(p.Value == 0) {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}
