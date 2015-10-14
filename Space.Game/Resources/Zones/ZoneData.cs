using Common;
using System.Collections.Generic;
using System.Collections;
using ServerClientCommon;
using System.Collections.Concurrent;
using Nebula.Server.Components;
using GameMath;

namespace Space.Game.Resources.Zones
{
    /// <summary>
    /// Info about world zone
    /// </summary>
    public class ZoneData : IInfoSource
    {
        /// <summary>
        /// Zone id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Zone name 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Zone level
        /// </summary>
        public int Level { get; set; }

        public Race InitiallyOwnedRace { get; set; }

        /// <summary>
        /// List of asteroids in zone
        /// </summary>
        public List<ZoneAsteroidInfo> Asteroids { get; set; }

        public List<ActivatorData> Activators { get; set; }

        /// <summary>
        /// Events for world zone
        /// </summary>
        public List<WorldEventData> Events { get; set; }

        public Dictionary<string, ZoneNpcInfo> Npcs { get; set; }

        public Hashtable Inputs { get; set; }

        public List<string> NpcGroups { get; set; }

        public List<ZonePlanetInfo> Planets { get; set; }

        public ZoneType ZoneType { get; set; }

        public ConcurrentDictionary<string, NebulaObjectData> nebulaObjects { get; set; } = new ConcurrentDictionary<string, NebulaObjectData>();

        public WorldType worldType { get; set; } = WorldType.neutral;

        public Vector3 humanSP;
        public Vector3 criptizidSP;
        public Vector3 borguzandSP;


        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Id, this.Id);
            info.Add((int)SPC.DisplayName, this.Name);
            info.Add((int)SPC.Level, this.Level);
            info.Add((int)SPC.InitialOwnedRace, this.InitiallyOwnedRace.toByte());
            info.Add((int)SPC.ZoneType, this.ZoneType.toByte());
            return info;
        }

    }
}
