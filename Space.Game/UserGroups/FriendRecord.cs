// FriendRecord.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, February 15, 2015 1:37:26 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Space.Game.UserGroups {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Common;
    using System.Collections;

    public class FriendRecord : IInfo {
        private string gameRefId;
        private string name;

        public FriendRecord(string gameRefId, string name) {
            this.gameRefId = gameRefId;
            this.name = name;
        }

        public FriendRecord(Hashtable info) {
            this.ParseInfo(info);
        }

        public string Name() {
            return this.name;
        }

        public string GameRefId() {
            return this.gameRefId;
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                {GenericEventProps.game_ref_id, this.GameRefId() },
                {GenericEventProps.name, this.Name() }
            };
        }

        public void ParseInfo(Hashtable info) {
            this.gameRefId = info.GetValueOrDefault<string>(GenericEventProps.game_ref_id);
            this.name = info.GetValueOrDefault<string>(GenericEventProps.name);
        }
    }
}
