using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class QuestItemUsedNearPointCondition : QuestCondition {
        private string m_World;
        private string m_ItemId;
        private float m_Radius;
        private Vector3 m_Point;

        public QuestItemUsedNearPointCondition(string itemId, string worldId, float radius, Vector3 point)
            : base(QuestConditionName.QUEST_ITEM_USED_NEAR_POINT) {
            m_ItemId = itemId;
            m_World = worldId;
            m_Radius = radius;
            m_Point = point;
        }

        private string itemId {
            get {
                return m_ItemId;
            }
        }

        private string worldId {
            get {
                return m_World;
            }
        }

        private float radius {
            get {
                return m_Radius;
            }
        }

        private Vector3 point {
            get {
                return m_Point;
            }
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            if(data != null ) {
                string sid = data as string;
                if(!string.IsNullOrEmpty(sid)) {
                    if(sid == itemId) {
                        if(target.isWorld(worldId)) {
                            if (target.NearPoint(point, radius)) {
                                LibLogger.Log(string.Format("QuestItemUsedNearPointCondition()->True, World={0}, Point={1}, Radius={2}", worldId, point, radius));
                                return true;
                            }
                        } else {
                            LibLogger.Log("invalid world at condition");
                        }
                    } else {
                        LibLogger.Log("invalid item id in condition");
                    }
                }
            }
            LibLogger.Log(string.Format("QuestItemUsedNearPointCondition()->False, World={0}, Point={1}, Radius={2}", worldId, point, radius));
            return false;
        }
    }
}
