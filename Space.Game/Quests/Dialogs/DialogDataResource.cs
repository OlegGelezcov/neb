/*
using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Dialogs {
    public class DialogDataResource {

        private readonly ConcurrentDictionary<Race, DialogDataCollection> m_Dialogs = new ConcurrentDictionary<Race, DialogDataCollection>();

        public void Load(string basePath) {
            m_Dialogs.Clear();

            DialogDataCollection humanDialogs = new DialogDataCollection();
            humanDialogs.Load(Path.Combine(basePath, "Data/dialogs_h.xml"));
            m_Dialogs.TryAdd(Race.Humans, humanDialogs);

            DialogDataCollection cripDialogs = new DialogDataCollection();
            cripDialogs.Load(Path.Combine(basePath, "Data/dialogs_c.xml"));
            m_Dialogs.TryAdd(Race.Criptizoids, cripDialogs);

            DialogDataCollection borgDialogs = new DialogDataCollection();
            borgDialogs.Load(Path.Combine(basePath, "Data/dialogs_b.xml"));
        }

        public DialogDataCollection GetDialogs(Race race) {
            DialogDataCollection result = null;
            if(m_Dialogs.TryGetValue(race, out result)) {
                return result;
            }
            return null;
        }

        public DialogData GetDialog(Race race, string id ) {
            var dialogs = GetDialogs(race);
            if(dialogs != null ) {
                return dialogs.GetDialogData(id);
            }
            return null;
        }
    }
}*/

