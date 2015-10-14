//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


//namespace Space.Game.Events
//{
//    public class EventStageTransition
//    {
//        private int toStage;
//        private string transitionText;
//        private List<EventCondition> conditions;

//        public EventStageTransition(int stage, string text, List<EventCondition> conditions)
//        {
//            this.toStage = stage;
//            this.transitionText = text;
//            this.conditions = conditions;
//        }

//        public int ToStage
//        {
//            get
//            {
//                return this.toStage;
//            }
//        }

//        public string Text
//        {
//            get
//            {
//                if (this.transitionText == null)
//                    this.transitionText = string.Empty;
//                return this.transitionText;
//            }
//        }

//        public bool CheckConditions(IEvent ownedEvent)
//        {
//            bool result = true;
//            foreach(var condition in this.conditions)
//            {
//                if(!condition.Check(ownedEvent))
//                {
//                    result = false;
//                    break;
//                }
//            }
//            return result;
//        }

//        public List<EventCondition> Conditions
//        {
//            get
//            {
//                return this.conditions;
//            }
//        }
//    }
//}
