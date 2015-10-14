namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    public class DecreasePrecision : UseSkill
    {
        public DecreasePrecision()
        {
            ID = "SK0102";
            _cooldown = 20.0f;
            active_time = 10.0f;
            effect = 0.2f;
            _energy = 100;
        }

        public override void Use(ICombatActor actor)
        {
            if (ready)
            {
                if (actor.ShipDestroyed == false)
                {

                    ICombatActor target = null;
                    if (actor.GetTarget.TargetIsICombatActor(out target))
                    {
                        if (actor.GetShip.Energy.EnoughAdd(_energy))
                        {
                            UseTimeUpdate();
                            ConsoleLogging.Get.Print("apply bonus from {0} to {1}", actor.Avatar.Id, target.Avatar.Id);
                            target.GetShip.GetBonuses().GetBonus(BonusType.decreasePrecision).ReplaceBuff(actor.Avatar.Id + ID, new Buff(effect, () =>
                            {
                                return active;
                            }));
                            StartClientEvent(actor, new Hashtable());
                        }
                        else
                        {
                            SendClientErrorEvent(actor, ReturnCode.Fatal, string.Format("source has {0} energy but need: {1}", actor.GetShip.Energy.Energy, _energy));
                        }
                    }
                    else
                    {
                        SendClientErrorEvent(actor, ReturnCode.Fatal, "Target not valid");
                    }
                }
                else
                {
                    SendClientErrorEvent(actor, ReturnCode.Fatal, "Actor ship destroyed");
                }
            }
            else
            {
                SendClientErrorEvent(actor, ReturnCode.Fatal, "Skill not ready");
            }
        }
    }

}
