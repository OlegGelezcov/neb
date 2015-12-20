// ClientPlayerSkills.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Monday, November 10, 2014 2:40:56 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//
namespace Nebula.Client {
    using Common;
    using System;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;
    using global::Common;

    public class ClientPlayerSkills : IInfo {
        private Dictionary<int, ClientPlayerSkill> skills;

        private void InitSkills() {
            this.skills = new Dictionary<int, ClientPlayerSkill>
            {
                {0, ClientPlayerSkill.Empty },
                {1, ClientPlayerSkill.Empty },
                {2, ClientPlayerSkill.Empty },
                {3, ClientPlayerSkill.Empty },
                {4, ClientPlayerSkill.Empty },
                {5, ClientPlayerSkill.Empty }
            };
        }

        public ClientPlayerSkills() {
            this.InitSkills();
        }

        public ClientPlayerSkills(Hashtable info) {
            this.InitSkills();
            this.ParseInfo(info);
        }


        public Hashtable GetInfo() {
            throw new NotImplementedException();
        }

        public void ParseInfo(Hashtable info) {
            foreach (System.Collections.DictionaryEntry entry in info) {
                int index = (int)entry.Key;
                Hashtable skillInfo = entry.Value as Hashtable;
                if (skillInfo != null) {
                    if (this.skills.ContainsKey(index)) {
                        this.skills[index] = new ClientPlayerSkill(skillInfo);
                    } else {
                        this.skills.Add(index, new ClientPlayerSkill(skillInfo));
                    }
                }
            }
        }

        public ClientPlayerSkill Skill(int index) {
            if (this.skills.ContainsKey(index))
                return this.skills[index];
            return ClientPlayerSkill.Empty;
        }

        public Dictionary<int, ClientPlayerSkill> Skills {
            get {
                return this.skills;
            }
        }
    }
}
