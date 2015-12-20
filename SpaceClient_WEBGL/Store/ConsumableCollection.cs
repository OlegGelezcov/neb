﻿using Common;
using ServerClientCommon;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Nebula.Client.Store {
    public class ConsumableCollection : IInfoParser {

        public readonly Dictionary<string, ConsumableItem> items = new Dictionary<string, ConsumableItem>();

        public ConsumableCollection() {

        }

        public ConsumableCollection(Hashtable info) {
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            items.Clear();
            foreach (System.Collections.DictionaryEntry entry in info) {
                string id = (string)entry.Key;
                Hashtable cInfo = entry.Value as Hashtable;
                if (info != null) {
                    items.Add(id, new ConsumableItem(cInfo));
                }
            }
        }


    }
}