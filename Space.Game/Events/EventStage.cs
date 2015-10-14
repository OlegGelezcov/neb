//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Common;
//using System.Collections;
//using ServerClientCommon;

//namespace Space.Game.Events
//{
//    public class EventStage : IInfoSource
//    {
//        //private readonly IEvent ownedEvent;
//        private readonly int stageId;
//        private readonly string startTextId;
//        private readonly string taskTextId;
//        private readonly bool isFinal;
//        private readonly bool isSuccess;
//        private readonly List<EventStageTransition> transitions;
//        private readonly int timeout;



//        public EventStage(int stageId, string startTextId, string taskTextId, bool isFinal, bool isSuccess, int timeout, List<EventStageTransition> transitions)
//        {
//            this.stageId = stageId;
//            this.startTextId = startTextId;
//            this.taskTextId = taskTextId;
//            this.isFinal = isFinal;
//            this.isSuccess = isSuccess;
//            this.transitions = transitions;
//            this.timeout = timeout;
//        }

//        public Hashtable GetInfo()
//        {
//            Hashtable info = new Hashtable();
//            info.Add((int)SPC.StageId, stageId);
//            info.Add((int)SPC.StartText, startTextId);
//            info.Add((int)SPC.TaskText, taskTextId);
//            info.Add((int)SPC.IsFinal, isFinal);
//            info.Add((int)SPC.IsSuccess, isSuccess);
//            info.Add((int)SPC.Timeout, timeout);
//            return info;
//        }

//        public void StartStage(IEvent ownedEvent)
//        {
//            if(!string.IsNullOrEmpty(startTextId))
//            {
//                ownedEvent.SendWorldEvent(CustomEventCode.WorldEventStageChanged);

//                if(isFinal )
//                {
//                    ownedEvent.SetCompleted();
//                }
//            }
//        }

//        public bool CheckTransition(IEvent ownedEvent, out string transitionText)
//        {
//            foreach(var transition in transitions )
//            {
//                if(transition.CheckConditions(ownedEvent))
//                {
//                    if(ownedEvent.GotoStage(transition.ToStage))
//                    {
//                        transitionText = transition.Text;
//                        return true;
//                    }
//                }
//            }
//            transitionText = string.Empty;
//            return false;
//        }

//        //private bool CheckConditions(IEvent ownedEvent, List<EventCondition> conditions)
//        //{
//        //    bool result = true;
//        //    foreach(var condition in conditions)
//        //    {
//        //        if(!condition.Check(ownedEvent))
//        //        {
//        //            result = false;
//        //            break;
//        //        }
//        //    }
//        //    return result;
//        //}

//        public int StageId
//        {
//            get
//            {
//                return this.stageId;
//            }
//        }

//        public string StartTextId
//        {
//            get
//            {
//                return this.startTextId;
//            }
//        }

//        public string TaskTextId
//        {
//            get
//            {
//                return this.taskTextId;
//            }
//        }

//        public bool IsFinal
//        {
//            get
//            {
//                return this.isFinal;
//            }
//        }

//        public bool IsSuccess
//        {
//            get
//            {
//                return this.isSuccess;
//            }
//        }

//        public int Timeout
//        {
//            get
//            {
//                return this.timeout;
                 
//            }
//        }

//        public List<EventStageTransition> Transitions
//        {
//            get
//            {
//                return this.transitions;
//            }
//        }


        
//        public List<EventCondition> ConditionsFor(int transitionId )
//        {
//            foreach(var tr in this.transitions)
//            {
//                if(tr.ToStage == transitionId)
//                {
//                    return tr.Conditions;
//                }
//            }
//            throw new Exception("not founded transition for: {0}".f(transitionId));
//        }


//    }
//}
