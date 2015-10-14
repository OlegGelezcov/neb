using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Common;
using Space.Game;
using System.Collections;
using GameMath;
using System.Globalization;

namespace Space.Game.Resources
{
    public class SkillRes
    {
        public Dictionary<int, SkillData> Skills { get; private set; }

        public void Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/Skills/skills.xml");
            XDocument document = XDocument.Load(fullPath);
            this.Skills = document.Element("skills").Elements("skill").Select(e =>
            {
                Hashtable inputs = new Hashtable();
                XElement inputsElement = e.Element("inputs");
                if(inputsElement != null )
                {
                    var dumpList = inputsElement.Elements("input").Select(ie =>
                    {
                        string key = ie.Attribute("key").Value;
                        object value = CommonUtils.ParseValue(ie.Attribute("value").Value, ie.Attribute("type").Value);
                        inputs.Add(key, value);
                        return key;
                    }).ToList();
                }

                return new SkillData
                {
                    Id = int.Parse(e.Attribute("id").Value, NumberStyles.HexNumber),
                    Cooldown = e.GetFloat("cooldown"),
                    Durability = e.GetFloat("durability"),
                    RequiredEnergy = e.GetFloat("energy"),
                    Type = e.GetEnum<SkillType>("type"),
                    Inputs = inputs
                };
            }).ToDictionary(s => s.Id, s => s);
        }

        public SkillData Skill(int id)
        {
            if (this.Skills.ContainsKey(id))
                return this.Skills[id];
            else
                return SkillData.Empty;
        }

        public SkillData RandomSkill()
        {
            int index = Rand.Int(this.Skills.Count - 1);
            int key = this.Skills.Keys.ToArray()[index];
            return this.Skills[key];
        }
    }
}
