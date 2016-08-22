using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class ClientDialogManager : IInfoParser {
        private readonly List<string> m_CompletedDialogs = new List<string>();

        public void ParseInfo(Hashtable hash) {
            m_CompletedDialogs.Clear();
            string[] compldlgs = hash.GetValueStringArray((int)SPC.CompletedDialogs);
            foreach (string cd in compldlgs) {
                m_CompletedDialogs.Add(cd);
            }
        }

        public bool IsCompleted(string dlgId ) {
            return m_CompletedDialogs.Contains(dlgId);
        }

        public void Clear() {
            m_CompletedDialogs.Clear();
        }

        public bool CompleteDialog(string dialogId ) {
            if(!IsCompleted(dialogId)) {
                m_CompletedDialogs.Add(dialogId);
                return true;
            }
            return false;
        }

        public List<string> completedDialogs {
            get {
                return m_CompletedDialogs;
            }
        }
    }
}
