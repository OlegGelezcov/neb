// ClientShipCombatStats.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Wednesday, November 19, 2014 10:54:29 AM
// Copyright (c) 2014 KomarGames. All rights reserved.
//
namespace Nebula.Client {
    using Common;
    using ServerClientCommon;
    using System.Collections;

    public class ClientShipCombatStats : IInfoParser
    {
        public float maxHealth { get; private set; }
        public float resist { get; private set; }
        public float energy { get; private set; }
        public float damage { get; private set; }
        public float critChance { get; private set; }
        public float critDamage { get; private set; }
        public float speed { get; private set; }

        public void ParseInfo(Hashtable info)
        {
            maxHealth = info.GetValue<float>((int)SPC.MaxHealth, 0f);
            resist = info.GetValue<float>((int)SPC.Resist, 0f);
            energy = info.GetValue<float>((int)SPC.Energy, 0f);
            damage = info.GetValue<float>((int)SPC.Damage, 0f);
            critChance = info.GetValue<float>((int)SPC.CritChance, 0f);
            critDamage = info.GetValue<float>((int)SPC.CritDamage, 0f);
            speed = info.GetValue<float>((int)SPC.Speed, 0f);
        }


    }
}
