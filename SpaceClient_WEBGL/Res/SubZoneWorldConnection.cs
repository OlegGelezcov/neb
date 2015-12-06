// SubZoneWorldConnection.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 8:43:09 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class SubZoneWorldConnection {
        public int subZone { get; private set; }
        public string world { get; private set; }

        public SubZoneWorldConnection(XElement element) {
            subZone = element.GetInt("id");
            world = element.GetString("world");
        }
    }
}
