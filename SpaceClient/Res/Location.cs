using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class Location {
   
        public string id { get; private set; }
        public string name { get; private set; }
        public int level { get; private set; }
        public Race startRace { get; private set; }
        public WorldType type { get; private set; }
        public float[] humansSpawnPoint { get; private set; }
        public float[] borguzandsSpawnPoint { get; private set; }
        public float[] criptizidsSpawnPoint { get; private set; }

        public static Location Parse(XElement e) {
            string id = e.GetString("id");
            string name = e.GetString("name");
            int level = e.GetInt("level");
            Race race = (Race)(byte)(int)e.GetInt("owned_race");
            WorldType type = (WorldType)Enum.Parse(typeof(WorldType), e.GetString("world_type"));
            float[] hSP = new float[] { 300, 10, 120 };
            float[] bSP = new float[] { -145, 10, 157 };
            float[] cSP = new float[] { -34, 10, -260 };

            if(e.HasAttribute("h_sp")) {
                hSP = e.GetString("h_sp").ToFloatArray3();
            }
            if(e.HasAttribute("b_sp")) {
                bSP = e.GetString("b_sp").ToFloatArray3();
            }
            if(e.HasAttribute("c_sp")) {
                cSP = e.GetString("c_sp").ToFloatArray3();
            }
            return new Location {
                id = id,
                level = level,
                name = name,
                startRace = race,
                type = type,
                humansSpawnPoint = hSP,
                borguzandsSpawnPoint = bSP,
                criptizidsSpawnPoint = cSP
            };
        }
    }
}
