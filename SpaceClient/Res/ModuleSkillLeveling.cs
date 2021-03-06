﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class ModuleSkillLeveling {
        public ShipModelSlotType slotType { get; private set; }
        public List<LevelSkillPair> levelSkillList { get; private set; }

        public ModuleSkillLeveling(XElement element) {
            slotType = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), element.GetString("slot_type"));
            levelSkillList = new List<LevelSkillPair>();
            var dmp = element.Elements("level").Select(e => {
                levelSkillList.Add(new LevelSkillPair(e));
                return e;
            }).ToList();
        }

        public int GetSkill(int level) {
            foreach(var lSkill in levelSkillList) {
                if(lSkill.level == level) {
                    return lSkill.skill;
                }
            }
            return -1;
        }
    }
}
