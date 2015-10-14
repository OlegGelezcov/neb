using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Space.Game.Ship
{
    public class SetInfoStorage
    {
        private List<SetInfo> moduleSets;

        public SetInfoStorage(List<SetInfo> sets) {
            moduleSets = sets;
        }

        

        public SetInfo Set(string id) {
            return moduleSets.Where((s) => s.Id == id).FirstOrDefault();
        }

        public bool HasSet(string id) {
            var set = moduleSets.Where((s) => s.Id == id).FirstOrDefault();
            return (set != null) ? true : false;
        }

        public string[] Ids {
            get {
                return moduleSets.Select((m) => m.Id).ToArray();
            }
        }
    }
}
