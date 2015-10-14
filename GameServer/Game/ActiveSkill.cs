
using Common;
using Photon.SocketServer;
using Space.Server;
using Photon.SocketServer.Rpc;
using Space.Mmo.Server;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Space.Server.Messages;

namespace Space.Game
{

    public enum SkillUnit { Percent, Units }

	public abstract class ActiveSkill : Skill
	{


		public float use_time;
		public float _cooldown;
		public float active_time;
        public float _energy;

        public ActiveSkill()
        {
            use_time = 0;
        }

        public bool active
        {
            get
            {
                return ((Time.time - use_time) < active_time);
            }
        }

        public virtual bool ready
        {
            get
            {
                return ((Time.time - use_time) > _cooldown);
            }
        }


        public virtual void UseTimeUpdate()
		{
            use_time = Time.time;
		}


        public virtual void StartClientEvent( ICombatActor actor, Hashtable additionalInfo )
        {
            Hashtable data = new Hashtable { 
                {SkillProp.SKILL_ID, this.ID},
                {SkillProp.COOLDOWN, _cooldown},
                {SkillProp.ACTIVE_TIME, active_time},
                {SkillProp.SOURCE_ACTOR_ID, actor.Avatar.Id },
                {SkillProp.TARGET_ACTOR_ID, actor.GetTarget.TargetId},
                {SkillProp.ACTOR_TYPE, actor.Avatar.Type },
                {SkillProp.TARGET_TYPE, actor.GetTarget.Type },
                {SkillProp.ADDITIONAL_INFO, (additionalInfo != null ) ? additionalInfo : new Hashtable() },
                {SkillProp.RETURN_CODE, (byte)ReturnCode.Ok }
            };

            SendParameters sendParameters = new SendParameters
            {
                Unreliable = true,
                ChannelId = Settings.ItemEventChannel
            };

            UseSkillEventData evtInstance = new UseSkillEventData
            {
                Properties = data
            };

            EventData eventData = new EventData((byte)EventCode.UseSkill, evtInstance);
            var message = new ItemEventMessage(actor.Avatar, eventData, sendParameters);
            ((MmoItem)actor.Avatar).ReceiveEvent(eventData, sendParameters);
            actor.Avatar.EventChannel.Publish(message);
        }

        public void SendClientErrorEvent(ICombatActor actor, ReturnCode code, string reason) {
            Hashtable data = new Hashtable { 
                {SkillProp.RETURN_CODE, (byte)code},
                {SkillProp.REASON_MESSAGE, reason }
            };
            SendParameters sendParameters = new SendParameters
            {
                Unreliable = true,
                ChannelId = Settings.ItemEventChannel
            };

            UseSkillEventData evtInstance = new UseSkillEventData
            {
                Properties = data
            };

            EventData eventData = new EventData((byte)EventCode.UseSkill, evtInstance);
            var message = new ItemEventMessage(actor.Avatar, eventData, sendParameters);
            ((MmoItem)actor.Avatar).ReceiveEvent(eventData, sendParameters);
            actor.Avatar.EventChannel.Publish(message);
        }

        public virtual void updateCooldown(ICombatActor actor) { }
	}

    public class UseSkillEventData {

        [DataMember(Code = (byte)ParameterCode.Properties)]
        public Hashtable Properties { get; set; }
    }
}
