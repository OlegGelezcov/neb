// SubZoneWorldConnection.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 8:43:09 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class SubZoneWorldConnection {
        public int subZone { get; private set; }
        public string world { get; private set; }

#if UP
        public SubZoneWorldConnection(UPXElement element) {
            subZone = element.GetInt("id");
            world = element.GetString("world");
        }
#else
        public SubZoneWorldConnection(XElement element) {
            subZone = element.GetInt("id");
            world = element.GetString("world");
        }
#endif
    }
}
