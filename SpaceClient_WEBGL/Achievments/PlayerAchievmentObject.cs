using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System.Collections.Generic;

namespace Nebula.Client.Achievments {
    public class PlayerAchievmentObject : IInfoParser {
        private readonly Dictionary<string, int> m_Variables = new Dictionary<string, int>();
        public int points { get; private set; }
        public List<string> foundedLoreRecords { get; private set; }

        public void ParseInfo(Hashtable inputHash) {

            if (inputHash != null) {
                points = inputHash.GetValueInt((int)SPC.Points);
                var hash = inputHash.GetValueHash((int)SPC.Variables);
                m_Variables.Clear();
                if (hash != null) {
                    foreach (System.Collections.DictionaryEntry entry in hash) {
                        string varName = entry.Key.ToString();
                        int varCount = (int)entry.Value;
                        m_Variables.Add(varName, varCount);
                    }
                }

                string[] recordArr = inputHash.GetValueStringArray((int)SPC.LoreRecords);
                foundedLoreRecords = new List<string>();
                if(recordArr != null ) {
                    foundedLoreRecords.AddRange(recordArr);
                }
            }
        }

        public int GetValue(string varName) {
            if(m_Variables.ContainsKey(varName)) {
                return m_Variables[varName];
            }
            return 0;
        }
    }
}
