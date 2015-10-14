// SubZoneWorldCollection.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 8:47:25 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class SubZoneWorldCollection {

        private Dictionary<int, SubZoneWorldConnection> mConnections = new Dictionary<int, SubZoneWorldConnection>();

        public void Load(string xmlText ) {
            mConnections.Clear();
            XDocument document = XDocument.Parse(xmlText);
            mConnections = document.Element("subzones").Elements("subzone").Select(element => {
                return new SubZoneWorldConnection(element);
            }).ToDictionary(s => s.subZone, s => s);
        }

        public SubZoneWorldConnection GetConnection(int subZone) {
            if(mConnections.ContainsKey(subZone)) {
                return mConnections[subZone];
            }
            return null;
        }
    }
}
