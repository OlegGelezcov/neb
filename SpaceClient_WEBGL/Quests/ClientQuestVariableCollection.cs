using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public class ClientQuestVariableCollection : Dictionary<string, ClientQuestVariable> {

        public void Load(Hashtable hash) {
            Clear();
            foreach(System.Collections.DictionaryEntry kvp in hash ) {
                string name = (string)kvp.Key;
                Hashtable varHash = kvp.Value as Hashtable;
                if(varHash != null ) {
                    ClientQuestVariable variable = null;
                    string type = varHash.GetValueString((int)SPC.Type);
                    switch(type) {
                        case "int":
                            variable = new IntegerClientQuestVariable(varHash);
                            break;
                        case "float":
                            variable = new FloatClientQuestVariable(varHash);
                            break;
                        case "bool":
                            variable = new BoolenClientQuestVariable(varHash);
                            break;
                    }
                    if(variable != null ) {
                        Add(name, variable);
                    }
                }
            }
        }
    }
}
