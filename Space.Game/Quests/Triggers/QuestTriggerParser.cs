using Common;
using GameMath;
using Nebula.Quests.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests.Triggers {
    public class QuestTriggerParser {

        private readonly PostActionParser m_PostActionParser = new PostActionParser();

        private QuestTrigger ParseTrigger(XElement element) {
            QuestTriggerType type = (QuestTriggerType)Enum.Parse(typeof(QuestTriggerType), element.GetString("type"));
            EventType eventType = (EventType)Enum.Parse(typeof(EventType), element.GetString("event"));

            QuestTrigger trigger = null;

            List<PostAction> actions = null;
            var paelem = element.Element("post_actions");
            if(paelem != null ) {
                actions = m_PostActionParser.ParseList(paelem);
            }

            switch(type) {
                case QuestTriggerType.quest_item_used_near_point: {
                        string itemId = element.GetString("id");
                        string worldId = element.GetString("world");
                        Vector3 point = element.GetFloatArray("point").ToVector3();
                        float radius = element.GetFloat("radius");
                        trigger = new QuestItemUsedNearPointTrigger(itemId, worldId, point, radius, actions);
                    }
                    break;
            }

            return trigger;
        }

        public List<QuestTrigger> ParseList(XElement parent) {
            if(parent == null ) {
                return new List<QuestTrigger>();
            }
            List<QuestTrigger> trs = new List<QuestTrigger>();
            var dump = parent.Elements("trigger").Select(te => {
                var trigger = ParseTrigger(te);
                if (trigger != null) {
                    trs.Add(trigger);
                }
                return trigger;
            }).ToList();
            return trs;
        }
    }
}
