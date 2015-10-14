

//namespace Space.Game.Resources
//{
//    using Common;
//    using System;
//    using System.Collections;
//    using System.Collections.Generic;
//    using System.IO;
//    using System.Linq;
//    using System.Xml.Linq;

//    public class NpcTypeDataRes : IResourceLoader
//    {
//        private bool loaded = false;

//        private Dictionary<string, NpcTypeData> npcTypes;


//        public bool Load(string basePath)
//        {
//            try
//            {
//                string fullPath = Path.Combine(basePath, "Data/npc_types.xml");
//                XDocument document = XDocument.Load(fullPath);
//                npcTypes = document.Element("npc_types").Elements("npc_type").Select(e =>
//                {
//                    string typeName = e.GetString("type");
//                    //List<AttackTargetType> attackTargets = e.GetAttackTargetList("attack_targets");
//                    IdleStateType idleType = e.GetIdleType("idle_type");
//                    Hashtable settings = new Hashtable();
//                    var dumpList = e.Elements("input").Select(ie =>
//                    {
//                        settings.Add(ie.GetString("key"), CommonUtils.ParseValue(ie.GetString("value"), ie.GetString("type")));
//                        return ie;
//                    }).ToList();
//                    return new NpcTypeData
//                    {
//                        //AttackTargetTypes = attackTargets,
//                        IdleType = idleType,
//                        Settings = settings,
//                        TypeName = typeName
//                    };
//                }).ToDictionary(td => td.TypeName, td => td);
//                this.loaded = true;
//            }
//            catch(Exception ex)
//            {
//                this.loaded = false;
//            }
//            return this.loaded;
//        }

//        public bool Loaded
//        {
//            get { return this.loaded; }
//        }

//        public bool TryGetNpcType(string typeName, out NpcTypeData data)
//        {
//            data = null;
//            if(this.loaded == false)
//            {
//                return false;
//            }
//            if(this.npcTypes == null )
//            {
//                return false;
//            }
//            return this.npcTypes.TryGetValue(typeName, out data);
//        }
//    }
//}
