﻿using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.DropList {
    public class DropItem {
        private int m_MinCount;
        private int m_MaxCount;
        private float m_Prob;
        private InventoryObjectType m_Type;


        public DropItem(int minCount, int maxCount, float prob, InventoryObjectType type) {
            m_MinCount = minCount;
            m_MaxCount = maxCount;
            m_Prob = prob;
            m_Type = type;
        }

        public int minCount {
            get {
                return m_MinCount;
            }
        }

        public int maxCount {
            get {
                return m_MaxCount;
            }
        }

        public float prob {
            get {
                return m_Prob;
            }
        }

        private float RemapParameter(int groupCount, int level) {
            return 0.05f * groupCount + 0.003f * level;
        }

        public bool Roll(out int count, int groupCount, int playerLevel) {
            if (Rand.Float01() < (prob + RemapParameter(groupCount, playerLevel)) ) {
                count = Rand.Int(minCount, maxCount);
                return true;
            } else {
                count = 0;
                return false;
            }
        }

        public InventoryObjectType type {
            get {
                return m_Type;
            }
        }
    }
}
