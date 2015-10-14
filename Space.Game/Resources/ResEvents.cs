//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Xml.Linq;
//using Space.Game.Events;
//using Common;
//using GameMath;
//using System.Collections;

//namespace Space.Game.Resources
//{
//    public class ResEvents
//    {
//        private Dictionary<string, EventInfo> events;

//        public Dictionary<string, EventInfo> Events
//        {
//            get
//            {
//                return this.events;
//            }
//        }

//        public EventInfo Event(string evtId )
//        {
//            if (events.ContainsKey(evtId))
//                return events[evtId];
//            throw new Exception("not founded event with id: {0}".f(evtId));
//        }

//        public void Load(string basePath)
//        {
//            string directoryPath = Path.Combine(basePath, "Data/Events");
//            string[] files = GetEventsFiles(directoryPath);
//            events = new Dictionary<string, EventInfo>();

//            foreach(string file in files)
//            {
//                Console.WriteLine("LOAD FILE: " + file);
//                LoadFile(file);
//            }
//        }

//        private void LoadFile(string file)
//        {
//            XDocument document = XDocument.Load(file);

//            if(document.Element("events") != null )
//            {
//                var dump = document.Element("events").Elements("event").Select(e =>
//                    {
//                        var evt = LoadEvent(e);
//                        events.Add(evt.Id, evt);
//                        return e;
//                    }).ToList();
//            }
//        }

//        private EventInfo LoadEvent(XElement element)
//        {
//            string evtId = element.Attribute("id").Value;
//            string name = element.Attribute("name").Value;
//            string description = element.Attribute("description").Value;
//            float cooldown = float.Parse(element.Attribute("cooldown").Value, System.Globalization.CultureInfo.InvariantCulture);
//            int rewardExp = int.Parse(element.Attribute("reward_exp").Value);
//            int rewardCoins = int.Parse(element.Attribute("reward_coins").Value);
//            float[] positionArr = (float[])CommonUtils.ParseValue(element.Attribute("position").Value, "vector");
//            Vector3 position = new Vector3(positionArr);
//            Dictionary<int, EventStage> stages = new Dictionary<int, EventStage>();

//            if(element.Element("stages") != null )
//            {
//                var dump = element.Element("stages").Elements("stage").Select(e =>
//                    {
//                        var s = LoadEventStage(e);
//                        stages.Add(s.StageId, s);
//                        return e;
//                    }).ToList();
//            }

//            Hashtable inputs = new Hashtable();
//            if(element.Element("inputs") != null )
//            {
//                var dumpKeys = element.Element("inputs").Elements("input").Select(e =>
//                    {
//                        string key = e.Attribute("key").Value;
//                        string valStr = e.Attribute("value").Value;
//                        string valType = e.Attribute("type").Value;
//                        object value = CommonUtils.ParseValue(valStr, valType);
//                        inputs.Add(key, value);
//                        return key;
//                    }).ToList();
//            }

//            return new EventInfo(evtId, name, description, cooldown, rewardExp, rewardCoins, position, stages, inputs);
//        }

//        private EventStage LoadEventStage(XElement element)
//        {
//            int stageId = int.Parse(element.Attribute("id").Value);
//            string startText = element.Attribute("start_text").Value;
//            string taskText = element.Attribute("task_text").Value;
//            bool isFinal = bool.Parse(element.Attribute("is_final").Value);
//            bool isSuccess = bool.Parse(element.Attribute("is_success").Value);
//            int timeout = int.Parse(element.Attribute("timeout").Value);
//            List<EventStageTransition> transitions = new List<EventStageTransition>();

//            if(element.Element("transitions") != null )
//            {
//                var dump = element.Element("transitions").Elements("transition").Select(e =>
//                {
//                    var kv = LoadTransition(e);
//                    transitions.Add(kv);
//                    return e;
//                }).ToList();
//            }

//            return new EventStage(stageId, startText, taskText, isFinal, isSuccess, timeout, transitions);
//        }

//        private EventStageTransition LoadTransition(XElement element)
//        {
//            int to = int.Parse(element.Attribute("to").Value);
//            List<EventCondition> conditions = new List<EventCondition>();
//            if(element.Element("conditions") != null)
//            {
//                var dump = element.Element("conditions").Elements("condition").Select(e =>
//                {
//                    conditions.Add(LoadCondition(e));
//                    return e;
//                }).ToList();
//            }

//            string transitionText = string.Empty;
//            foreach(var attr in element.Attributes())
//            {
//                if(attr.Name == "text")
//                {
//                    transitionText = attr.Value;
//                    break;
//                }
//            }
//            return new EventStageTransition(to, transitionText, conditions);
//        }

//        private EventCondition LoadCondition(XElement element)
//        {
//            string type = element.Attribute("type").Value;
//            switch(type)
//            {
//                case "GEQ":
//                    {
//                        string varName = element.Attribute("var_name").Value;
//                        string varType = element.Attribute("var_type").Value;
//                        object checkValue = ParseVariable(varType, element.Attribute("value").Value);
//                        switch(varType)
//                        {
//                            case "int":
//                                return new GreaterOrEqualsVariableEventCondition<int>(varName, (int)checkValue);
//                            case "float":
//                                return new GreaterOrEqualsVariableEventCondition<float>(varName, (float)checkValue);
//                            default:
//                                throw new Exception("unsupported type: {0} for GreaterOrEqualsVariableEventCondition".f(varType));
//                        }
//                    }
//                case "EQ":
//                    {
//                        string varName = element.Attribute("var_name").Value;
//                        string varType = element.Attribute("var_type").Value;
//                        object checkValue = ParseVariable(varType, element.Attribute("value").Value);
//                        switch(varType)
//                        {
//                            case "int":
//                                return new EqualsVariableEventCondition<int>(varName, (int)checkValue);
//                            case "float":
//                                return new EqualsVariableEventCondition<float>(varName, (float)checkValue);
//                            case "bool":
//                                return new EqualsVariableEventCondition<bool>(varName, (bool)checkValue);
//                            default:
//                                throw new Exception("unsupported type: {0} for EqualsVariableEventCondition".f(varType));
//                        }
//                    }
//                default:
//                    throw new Exception("unsupported condition type: {0}".f(type));
//            }
//        }

//        private object ParseVariable(string varType, string varValueStr)
//        {
//            return CommonUtils.ParseValue(varValueStr, varType);
//            /*switch(varType)
//            {
//                case "int":
//                    return int.Parse(varValueStr);
//                case "float":
//                    return float.Parse(varValueStr, System.Globalization.CultureInfo.InvariantCulture);
//                case "bool":
//                    return bool.Parse(varValueStr);
//                default:
//                    return new Exception("unknown var_type for stage condition");
//            }*/
//        }

//        private string[] GetEventsFiles(string directoryPath)
//        {
//            DirectoryInfo d = new DirectoryInfo(directoryPath);
//            FileInfo[] files = d.GetFiles("*.xml");
//            return files.Select(f => f.FullName).ToArray();
//        }


//    }
//}
