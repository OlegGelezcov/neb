using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestVariableDataFactory {

        private QuestVariableData Create(XElement element) {
            string type = element.GetString("type");
            string name = element.GetString("name");
            switch (type) {
                case QuestVariableTypeName.INT: {
                        int value = element.GetInt("value");
                        return new IntegerQuestVariableData(type, name, value);
                    }
                case QuestVariableTypeName.FLOAT: {
                        float value = element.GetFloat("value");
                        return new FloatQuestVariableData(type, name, value);
                    }
                case QuestVariableTypeName.BOOL: {
                        bool value = element.GetBool("value");
                        return new BoolQuestVariableData(type, name, value);
                    }
                default:
                    return null;
            }
        }

        public List<QuestVariableData> CreateVariables(XElement parent) {
            List<QuestVariableData> result = new List<QuestVariableData>();
            if (parent != null) {
                var dumpList = parent.Elements("variable").Select(variableElement => {
                    var data = Create(variableElement);
                    if (data != null) {
                        result.Add(data);
                    }
                    return data;
                }).ToList();
            }
            return result;
        }
    }
}
