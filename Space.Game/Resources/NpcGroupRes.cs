using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using GameMath;
using Common;

namespace Space.Game.Resources
{
    public class NpcGroupRes : IResourceLoader
    {
        private Dictionary<string, NpcGroupData> npcGroups;
        private bool loaded = false;

        public bool Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/npc_groups.xml");
            XDocument document = XDocument.Load(fullPath);
            this.npcGroups = document.Element("npc_groups").Elements("npc_group").Select(e =>
                {
                    string id = e.Attribute("id").Value;
                    Vector3 center = e.Attribute("center").Value.ToFloatArray3().ToVector3();
                    MinMax bounds = e.Attribute("bounds").Value.ToMinMax();
                    int maxCount = e.Attribute("max_count").ToInt();
                    string npcTypeName = e.Attribute("npc_type").Value;
                    int level = e.Attribute("level").ToInt();
                    float spawnInterval = e.Attribute("spawn_interval").ToFloat();
                    Difficulty difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), e.Attribute("difficulty").Value);
                    Race race = (Race)Enum.Parse(typeof(Race), e.Attribute("race").Value);
                    string npcName = e.Attribute("npc_name").Value;

                    return new NpcGroupData(id, center, bounds, maxCount, npcTypeName, level, spawnInterval, difficulty, race, npcName);
                }).ToDictionary(g => g.Id, g => g);
            this.loaded = true;
            return this.loaded;
        }

        public bool Loaded
        {
            get { return this.loaded; }
        }

        public NpcGroupData GroupData(string id)
        {
            NpcGroupData result = null;
            this.npcGroups.TryGetValue(id, out result);
            return result;
        }
    }
}
