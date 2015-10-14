namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    public class Jump : UseSkill
    {
        public Jump()
        {
            ID = "SK1103";
            _cooldown = 10;
            active_time = 0;
            effect = 0;
            _energy = 100;
        }

        public override void Use(ICombatActor actor)
        {
            if (ready && (actor.ShipDestroyed == false))
            {

                if (actor.GetShip.Energy.EnoughAdd(_energy))
                {
                    UseTimeUpdate();
                    StartClientEvent(actor, new Hashtable());
                }
                else
                {
                    SendClientErrorEvent(actor, ReturnCode.Fatal, string.Format("source has {0} energy but need: {1}", actor.GetShip.Energy.Energy, _energy));
                }
            }
            else
            {
                SendClientErrorEvent(actor, ReturnCode.Fatal, "Skill not ready or actor ship destroyed");
            }
        }
    }
	
}
