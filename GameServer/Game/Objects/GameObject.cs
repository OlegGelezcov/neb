using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Game;
using Nebula.Game.Components;
using Nebula.Server.Components;
using Photon.SocketServer;
using Space.Game.Events;
using Space.Server;
using Space.Server.Events;
using Space.Server.Messages;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Space.Game {
    public class GameObject : Item, IDatabaseObject
    {
        public InterestArea interestArea { get; private set; }
        protected EventedObject mEvent;
        protected RaceableObject mRace;
        protected BotObject mBot;
        protected MmoMessageComponent mMessage;

        private static ILogger log = LogManager.GetCurrentClassLogger();


        protected internal override ItemSnapshot GetItemSnapshot()
        {
            return new MmoItemSnapshot(this, transform.position.ToVector(), this.CurrentWorldRegion, properties.propertiesRevision, transform.rotation.ToArray(), 
                transform.position.ToArray());
        }

        protected override ItemPositionMessage GetPositionUpdateMessage(Vector position, Region region)
        {
            return new MmoItemPositionUpdate(this, transform.position.ToVector(), region, transform.position.ToArray());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (interestArea != null) {
                    this.interestArea.Dispose();
                    interestArea = null;
                }

                this.Fiber.Dispose();

                world.RemoveObject(Type, Id);
            }

            base.Dispose(disposing);
        }


        public GameObject(Vector position, Hashtable inProperties, string id, byte type, IWorld world, Dictionary<byte, object> inTags, float size, int subZone, Type[] components):
            base(position, inProperties, id, type, world, null, true, inTags, size, subZone, components) {

        }


        //public GameObject(string id, MmoWorld world, Vector position, byte itemType, Race race, byte subType, string name, IEvent worldEvent = null, object worldEventTag = null)
        //    : base(position, new Hashtable(), id, itemType, world, null, true, new Dictionary<byte, object>())
        //{
        //    mEvent = AddComponent<EventedObject>();
        //    mRace = AddComponent<RaceableObject>();
        //    mRace.SetRace(race);
        //    mBot = AddComponent<BotObject>();
        //    mBot.SetSubType(subType);
        //    mEvent.SetEvent(worldEvent);
        //    mEvent.SetEventTag(worldEventTag);
        //    mMessage = AddComponent<MmoMessageComponent>();
        //    this.name = name;
       
        //}

        public bool AddToWorld() {
            try {

                if(world.AddObject(this)) {
                    StartFiber();
                    this.interestArea = new InterestArea(0, (IWorld)world);
                    this.interestArea.AttachToItem(this);
                    this.interestArea.ViewDistanceEnter = new Vector { X = 0, Y = 0, Z = 0 };
                    this.interestArea.ViewDistanceExit = new Vector { X = 0, Y = 0, Z = 0 };
                    transform.SetPosition(new Vector { X = transform.position.X, Y = transform.position.Y, Z = transform.position.Z });
                    UpdateInterestManagement();
                    //log.InfoFormat("GameObject {0} added to world successfully", name);
                    return true;
                } else {
                    log.Error("Error of adding to world");
                    Destroy();
                    return false;
                }
            } catch(Exception exception) {
                log.Error(exception);
                log.Error(exception.StackTrace);
            }
            return false;
        }

        public bool AddToWorld(float[] pos, float[] rot) {
            try {

                if (world.AddObject(this)) {
                    StartFiber();
                    this.interestArea = new InterestArea(0, (IWorld)world);
                    this.interestArea.AttachToItem(this);
                    this.interestArea.ViewDistanceEnter = new Vector { X = 0, Y = 0, Z = 0 };
                    this.interestArea.ViewDistanceExit = new Vector { X = 0, Y = 0, Z = 0 };
                    transform.SetPosition(new Vector { X = pos[0], Y = pos[1], Z = pos[2] });
                    transform.SetRotation(rot);
                    UpdateInterestManagement();
                    //log.InfoFormat("GameObject {0} added to world successfully", name);
                    return true;
                } else {
                    log.Error("Error of adding to world");
                    Destroy();
                    return false;
                }
            } catch (Exception exception) {
                log.Error(exception);
                log.Error(exception.StackTrace);
            }
            return false;
        }

        protected override void OnDestroy()
        {
            //log.InfoFormat("destroy object of type {0}", (ItemType)Type);
            var eventInstance = new ItemDestroyed { ItemId = this.Id, ItemType = this.Type };
            var eventData = new EventData((byte)EventCode.ItemDestroyed, eventInstance);
            var message = new ItemEventMessage(this, eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
            this.EventChannel.Publish(message);
            base.OnDestroy();
            this.Dispose(true);
        }

        public Hashtable GetDatabaseSave() {
            NebulaObjectComponentData data = new NebulaObjectComponentData((ItemType)Type, tags, badge, size, subZone);
            return data.AsHash();
        }



        //public static void DestroyStandardNpcObjectsWithTag(MmoWorld world, string tag)
        //{
        //    //var bots = FindGameObjectsWithTag(world, tag); 
        //    //foreach(var b in bots.Values)
        //    //{
        //    //    var npc = (b as EventConnectedCombatNpcObject);
        //    //    if (npc != null)
        //    //    {
        //    //        var eventedObject = npc.GetComponent<EventedObject>();
        //    //        if(eventedObject == null ) {
        //    //            continue;
        //    //        }
        //    //        if(eventedObject.eventTag == null ) {
        //    //            continue;
        //    //        }

        //    //        if(eventedObject.eventTag.ToString() == tag) {
        //    //            if(npc.Fiber != null ) {
        //    //                npc.Fiber.Enqueue(() => npc.ForceDestroy());
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}

        //public static void DestroyGameObjectsWithTag(MmoWorld world, string tag)
        //{
        //    var objs = FindGameObjectsWithTag(world, tag);
        //    foreach(var pObj in objs)
        //    {
        //        if(pObj.Value.Fiber != null )
        //        {
        //            pObj.Value.Fiber.Enqueue(() => pObj.Value.ForceDestroy());
        //        }
        //    }
        //}

        //public static Dictionary<string, GameObject> FindGameObjectsWithTag(MmoWorld world, string tag)
        //{
        //    Dictionary<string, GameObject> result = new Dictionary<string, GameObject>();

        //    foreach(object objType in Enum.GetValues(typeof(ItemType)))
        //    {
        //        var validItems = world.ItemCache.GetItems((byte)(ItemType)objType, (it) =>
        //            {
        //                if (it is GameObject)
        //                {
        //                    var eventedComponent = it.GetComponent<EventedObject>();
        //                    if (eventedComponent != null) {

        //                        object checkedTag = eventedComponent.eventTag;

        //                        if (checkedTag != null) {
        //                            if (checkedTag is string) {
        //                                return (checkedTag.ToString() == tag);
        //                            } else if (checkedTag is Hashtable) {
        //                                if ((checkedTag as Hashtable).ContainsKey("tag")) {
        //                                    return (((checkedTag as Hashtable)["tag"]).ToString() == tag);
        //                                }
        //                            }
        //                        }
        //                    }

        //                }
        //                return false;
        //            });
        //        foreach(var pIt in validItems)
        //        {
        //            if (result.ContainsKey(pIt.Key) == false)
        //                result.Add(pIt.Key, pIt.Value as GameObject);
        //        }
        //    }
        //    return result;
        //}
    }
}
