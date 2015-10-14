// WeaponDataRes.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Sunday, November 23, 2014 8:00:35 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//

namespace Space.Game.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Xml.Linq;
    using Common;
    using GameMath;

    public class WeaponDataRes
    {
        public Dictionary<string, WeaponData> Weapons { get; private set; }

        public void Load(string basePath )
        {
            string fullPath = Path.Combine(basePath, "Data/Drop/weapons.xml");
            XDocument document = XDocument.Load(fullPath);
            this.Weapons = document.Element("weapons").Elements("weapon").Select(e =>
                {
                    return new WeaponData
                    {
                        Id = e.Attribute("id").Value,
                        Workshop = (Workshop)Enum.Parse(typeof(Workshop), e.Attribute("workshop").Value)
                    };
                }).ToDictionary(d => d.Id, d => d);
        }

        public WeaponData Weapon(string id )
        {
            if (this.Weapons.ContainsKey(id))
                return this.Weapons[id];
            return null;
        }

        public WeaponData RandomWeapon(Workshop workshop )
        {
            var list = this.Weapons.Where(kv => kv.Value.Workshop == workshop).Select(kv => kv.Value).ToList();
            if (list.Count == 0)
                return null;
            return list[Rand.Int(list.Count - 1)];
        }
    }
}
