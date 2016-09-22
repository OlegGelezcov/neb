using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Concurrent;
using ServerClientCommon;
using Nebula.Quests;
using Space.Game;
using Nebula.Game.Events;

namespace Nebula.Game.Components.Quests {
    public class ServerQuest : IInfo, IQuest {

        private string m_Id;
        private readonly ConcurrentDictionary<string, QuestVariable> m_Variables = new ConcurrentDictionary<string, QuestVariable>();
        private ServerQuestState m_State;
        private QuestData m_Data;

        public ServerQuest(Hashtable hash) {
            ParseInfo(hash);
            m_Data = null;
        }

        public ServerQuest(QuestData questData ) {
            m_Id = questData.id;
            foreach(var variableData in questData.variables ) {
                var variable = QuestVariable.CreateFromData(variableData);
                if(variable != null ) {
                    m_Variables.TryAdd(variable.name, variable);
                }
            }
            m_State = ServerQuestState.accepted;
            m_Data = questData;
        }

        public Hashtable GetInfo() {
            Hashtable varHash = new Hashtable();
            foreach(var kvp in m_Variables ) {
                varHash.Add(kvp.Key, kvp.Value.GetInfo());
            }
            return new Hashtable {
                { (int)SPC.Id,  id },
                { (int)SPC.Variables, varHash },
                { (int)SPC.State, (int)state }
            };
        }

        public void ParseInfo(Hashtable hash) {
            m_Id = hash.GetValue<string>((int)SPC.Id, string.Empty);
            m_State = (ServerQuestState)hash.GetValue<int>((int)SPC.State, (int)ServerQuestState.accepted);

            Hashtable varHash = hash.GetValue<Hashtable>((int)SPC.Variables, new Hashtable());
            m_Variables.Clear();
            foreach(DictionaryEntry kvp in varHash ) {
                string id = (string)kvp.Key;
                Hashtable variableHash = kvp.Value as Hashtable;

                if((!string.IsNullOrEmpty(id)) && (variableHash != null)) {
                    QuestVariable parsedVariable = QuestVariable.Parse(variableHash);
                    if(parsedVariable != null ) {
                        m_Variables.TryAdd(id, parsedVariable);
                    }
                }
            }
        }

        public bool ExecuteTriggers(IQuestConditionTarget target, BaseEvent evt) {
            var data = GetData(target.GetRace(), target.GetResource());
            bool executed = false;
            if(data != null ) {
                if(data.triggers != null ) {
                    foreach(var trigger in data.triggers ) {
                        executed = (executed || trigger.Execute(target, evt));
                    }
                }
            }
            return executed;
        }

        public bool SetInteger(string name, int val ) {
            QuestVariable target = null;
            if(m_Variables.TryGetValue(name, out target )) {
                if(target is IntegerQuestVariable ) {
                    (target as IntegerQuestVariable).SetValue(val);
                    return true;
                }
            }
            return false;
        }

        public bool IncreaseInteger(string name, int inc, out int newVal ) {
            newVal = 0;

            QuestVariable target = null;
            if(m_Variables.TryGetValue(name, out target)) {
                if(target is IntegerQuestVariable) {
                    int val = (target as IntegerQuestVariable).value;
                    newVal = val + inc;
                    (target as IntegerQuestVariable).SetValue(newVal);
                    return true;
                }
            }
            return false;
        }

        public bool SetFloat(string name, float val) {
            QuestVariable target = null;
            if(m_Variables.TryGetValue(name, out target)) {
                if(target is FloatQuestVariable ) {
                    (target as FloatQuestVariable).SetValue(val);
                    return true;
                }
            }
            return false;
        }

        public bool SetBool(string name, bool val) {
            QuestVariable target = null;
            if(m_Variables.TryGetValue(name, out target)) {
                if(target is BooleanQuestVariable) {
                    (target as BooleanQuestVariable).SetValue(val);
                    return true;
                }
            }
            return false;
        }

        public bool TryGetInteger(string name, out int value ) {
            value = 0;
            QuestVariable target = null;
            if(m_Variables.TryGetValue(name, out target )) {
                if(target is IntegerQuestVariable ) {
                    value = (target as IntegerQuestVariable).value;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetFloat(string name, out float value ) {
            value = 0.0f;
            QuestVariable target = null;
            if(m_Variables.TryGetValue(name, out target)) {
                if(target is FloatQuestVariable) {
                    value = (target as FloatQuestVariable).value;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetBool(string name, out bool value ) {
            value = false;
            QuestVariable target = null;
            if(m_Variables.TryGetValue(name, out target)) {
                if(target is BooleanQuestVariable) {
                    value = (target as BooleanQuestVariable).value;
                    return true;
                }
            }
            return false;
        }

        public List<QuestCondition> FilterCompleteConditions(string name, Race race, IRes resource) {
            var data = GetData(race, resource);
            if(data != null ) {
                return data.FilterCompleteConditions(name);
            }
            return new List<QuestCondition>();
        }

        public string id {
            get {
                return m_Id;
            }
        }
        public ServerQuestState state {
            get {
                return m_State;
            }
        }

        public QuestData GetData(Race race, IRes res ) {
            if(m_Data == null ) {
                m_Data = res.quests.GetQuest(race, id);
            }
            return m_Data;
        }

        public void SetReady() {
            m_State = ServerQuestState.ready;
        }
    }
}
