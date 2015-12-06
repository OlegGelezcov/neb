// FoundedGuild.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, September 22, 2015 4:47:42 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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
            id = info.GetValueString((int)SPC.Id);
            rating = info.GetValueInt((int)SPC.GuildRating);
            race = (Race)(byte)info.GetValueInt((int)SPC.Race, (int)(byte)Race.Humans);
            description = info.GetValueString((int)SPC.Description);
            name = info.GetValueString((int)SPC.Name);
            count = info.GetValueInt((int)SPC.Count);
            opened = info.GetValueBool((int)SPC.Opened, true);
        }

        public bool IsAdministrator(string characterID) {
            return (id == characterID);
        }
    }
}
