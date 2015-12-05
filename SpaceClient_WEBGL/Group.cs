﻿using Common;
using System.Collections.Generic;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class Group : IInfoParser {

        public string groupID { get; private set; }
        public Dictionary<string, GroupMember> members { get; private set; }
        public bool opened { get; private set; }

        public Group() {
            groupID = string.Empty;
            members = new Dictionary<string, GroupMember>();
        }

        public void Clear() {
            groupID = string.Empty;
            members.Clear();
            opened = false;
        }

        public void ParseInfo(Hashtable info) {
            if (info == null || info.Count == 0) {
                groupID = string.Empty;
                members.Clear();
                opened = false;
            } else {
                members.Clear();
                groupID = info.GetValueString((int)SPC.Group);
                opened = info.GetValueBool((int)SPC.Opened);

                Hashtable memberHash = info.Value<Hashtable>((int)SPC.Members);

                foreach (System.Collections.DictionaryEntry pair in memberHash) {
                    GroupMember m = new GroupMember();
                    m.ParseInfo(pair.Value as Hashtable);
                    members.Add(pair.Key.ToString(), m);
                }
            }
        }

        public bool GameRefIsMember(string gameRefID) {
            foreach (var member in members) {
                if (member.Value.gameRefID == gameRefID) {
                    return true;
                }
            }
            return false;
        }

        public bool has {
            get {
                return (!string.IsNullOrEmpty(groupID));
            }
        }

        public int count {
            get {
                return members.Count;
            }
        }

        public GroupMember leader {
            get {
                foreach (var mem in members) {
                    if (mem.Value.isLeader) {
                        return mem.Value;
                    }
                }
                return null;
            }
        }
    }
}
