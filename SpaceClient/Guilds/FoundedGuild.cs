// FoundedGuild.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, September 22, 2015 4:47:42 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Guilds {
    /// <summary>
    /// Founded guild from search guilds request
    /// </summary>
    public class FoundedGuild : IInfoParser {

        public string id { get; private set; }
        public int rating { get; private set; }
        public Race race { get; private set; }
        public string description { get; private set; }
        public string name { get; private set; }
        public int count { get; private set; }
        public bool opened { get; private set; }

        public FoundedGuild(Hashtable info) {
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            id              =           info.GetValue<string>((int)SPC.Id, string.Empty);
            rating          =           info.GetValue<int>((int)SPC.GuildRating, 0);
            race            =           (Race)(byte)info.GetValue<int>((int)SPC.Race, (int)(byte)Race.Humans);
            description     =           info.GetValue<string>((int)SPC.Description, string.Empty);
            name            =           info.GetValue<string>((int)SPC.Name, string.Empty);
            count           =           info.GetValue<int>((int)SPC.Count, 0);
            opened          =           info.GetValue<bool>((int)SPC.Opened, true);
        }

        public bool IsAdministrator(string characterID) {
            return (id == characterID);
        }
    }
}
