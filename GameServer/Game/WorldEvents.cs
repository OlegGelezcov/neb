//namespace Space.Game
//{
//    using Common;
//    using ExitGames.Concurrency.Fibers;
//    using Space.Game.Events;
//    using GameMath;
//    using Space.Server;
//    using System.Collections.Generic;
//    using System.Collections;
//    using System.Linq;
//    using Space.Game.Resources;
//using ExitGames.Logging;
//    using System.IO;
//using System;

//    public class WorldEvents : IDisposable
//    {
//        private readonly List<WorldEvent> events;
//        private readonly IFiber fiber;
//        private readonly MmoWorld world;
//        private object lockObject = new object();

//        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
//        private IDisposable updateDisposable = null;
//        private bool disposed = false;


//        public WorldEvents(MmoWorld world, IRes resource) 
//        {

//            this.world = world;
//            this.events = new List<WorldEvent>();
//            this.fiber = new PoolFiber();

            
//            foreach(var eventData in world.Zone.Events )
//            {
//                EventInfo eventInfo = resource.Events.Event(eventData.Id);
//                if(eventInfo == null )
//                {
//                    CL.Out(LogFilter.EVENTS, "Not found event info for: {0}".f(eventData.Id));
//                    continue;
//                }
//                switch(eventData.Id)
//                {
//                    case "EVM1":
//                        this.events.Add(new InvasionEvent(eventData, eventInfo, world));
//                        break;
//                    case "EV0002":
//                        this.events.Add(new E0002_Event(eventData, eventInfo, world));
//                        break;
                        
//                    case "EV0003":
//                        this.events.Add(new EV0003_Event(eventData, eventInfo, world));
//                        break;
//                }
//            }

//        }

//        private IRes Resource()
//        {
//            return this.world.Resource();
//        }

//        public void Initialize()
//        {
//            this.fiber.Start();
//            this.updateDisposable = this.fiber.ScheduleOnInterval(Update, 0, (int)Resource().ServerInputs.Inputs["update_events_interval"]);
//        }

//        private void Update()
//        {
            
//            float time = Time.curtime();
//            var actors = this.world.GetMmoActors(a => true);

//            foreach(WorldEvent worldEvent in this.Events)
//            {
//                if(!worldEvent.IsActive)
//                {
//                    if(worldEvent.TryStart(time))
//                    {
//                        log.InfoFormat("Event {0} started", worldEvent.Id);
//                    }
//                    continue;
//                }
//                else
//                {
//                    worldEvent.Update(time);
//                    foreach(var pActor in actors )
//                    {
//                        if(worldEvent.AddActor(pActor.Value))
//                        {
//                            log.InfoFormat("Actor {0} added to event: {1}", pActor.Value.nebulaObject.Id, worldEvent.Id);
//                        }
//                    }

//                    if(worldEvent.CheckTransition())
//                    {
//                        log.InfoFormat("Event {0} changed stage to {1}", worldEvent.Id, worldEvent.CurrentStage.StageId);
//                    }
//                }

//            }
//        }



//        public List<WorldEvent> Events 
//        {
//            get 
//            {
//                return this.events;
//            }
//        }

//        public List<WorldEvent> EventsForActor(string actorId )
//        {
//            return this.events.Where(evt => evt.ContainsActor(actorId)).ToList();
//        }

//        public IFiber Fiber
//        {
//            get
//            {
//                return this.fiber;
//            }
//        }



//        public void Dispose() {
//            if (!disposed) {
//                disposed = true;
//                if (this.updateDisposable != null) {
//                    log.Info("World Events updater will be disposed");
//                    this.updateDisposable.Dispose();
//                    this.updateDisposable = null;
//                }
//                if (this.fiber != null) {
//                    this.fiber.Dispose();
//                }
//            }
//        }
//    }
//}
