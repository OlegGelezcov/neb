/*
using Common;
using Nebula.Quests;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components.Quests {
    public abstract class QuestVariable : IInfo {
        private string m_Name;
        private string m_Type;

        public QuestVariable(string name, string type ) {
            m_Name = name;
            m_Type = type;
        }

        public QuestVariable() {
            m_Name = string.Empty;
            m_Type = string.Empty;
        }

        public virtual void ParseInfo(Hashtable hash) {
            m_Name = hash.GetValue<string>((int)SPC.Name, string.Empty);
            m_Type = hash.GetValue<string>((int)SPC.Type, string.Empty);
        }

        public virtual Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Name, name },
                { (int)SPC.Type, type }
            };
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public string type {
            get {
                return m_Type;
            }
        }

        public static QuestVariable Parse(Hashtable hash) {
            string type = hash.GetValue<string>((int)SPC.Type, string.Empty);
            switch(type) {
                case QuestVariableTypeName.INT:
                    return new IntegerQuestVariable(hash);
                case QuestVariableTypeName.FLOAT:
                    return new FloatQuestVariable(hash);
                case QuestVariableTypeName.BOOL:
                    return new BooleanQuestVariable(hash);
                default:
                    return null;
            }
        }

        public static QuestVariable CreateFromData(QuestVariableData data ) {
            switch(data.type ) {
                case QuestVariableTypeName.INT: {
                        int val = (data as IntegerQuestVariableData).value;
                        return new IntegerQuestVariable(data.name, data.type, val);
                    }
                case QuestVariableTypeName.FLOAT: {
                        float val = (data as FloatQuestVariableData).value;
                        return new FloatQuestVariable(data.name, data.type, val);
                    }
                case QuestVariableTypeName.BOOL: {
                        bool val = (data as BoolQuestVariableData).value;
                        return new BooleanQuestVariable(data.name, data.type, val);
                    }
                default:
                    return null;
            }
        }
    }
}
*/