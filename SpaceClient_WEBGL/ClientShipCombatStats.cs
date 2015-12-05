// ClientShipCombatStats.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Wednesday, November 19, 2014 10:54:29 AM
// Copyright (c) 2014 KomarGames. All rights reserved.
//
namespace Nebula.Client {
    using Common;
    using ServerClientCommon;
    using ExitGames.Client.Photon;
    using Utils;

    public class ClientShipCombatStats : IInfoParser {
        public float maxHealth { get; private set; }
        public float resist { get; private set; }
        public float energy { get; private set; }
        public float damage { get; private set; }
        public float critChance { get; private set; }
        public float critDamage { get; private set; }
        public float speed { get; private set; }

        public void ParseInfo(Hashtable info) {
            maxHealth = info.GetValueFloat((int)SPC.MaxHealth);
            resist = info.GetValueFloat((int)SPC.Resist);
            energy = info.GetValueFloat((int)SPC.Energy);
            damage = info.GetValueFloat((int)SPC.Damage);
            critChance = info.GetValueFloat((int)SPC.CritChance);
            critDamage = info.GetValueFloat((int)SPC.CritDamage);
            speed = info.GetValueFloat((int)SPC.Speed);
        }


    }
}
