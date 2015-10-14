namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    public class LimitedCooldown : UseSkill
    {
        public LimitedCooldown()
        {
            ID = "SK0109";
            _cooldown = 40.0f;
            active_time = 15.0f;
            _energy = 100;
            effect = 1;
        }

        public override void Use(ICombatActor actor)
        {
            if (ready)
            {
                if (!actor.ShipDestroyed)
                {
                    if (actor.GetShip.Energy.EnoughAdd(_energy))
                    {
                        UseTimeUpdate();
                        actor.GetShip.GetBonuses().GetBonus(BonusType.decreaseCooldown).ReplaceBuff(ID + actor.Avatar.Id, new Buff(effect, () =>
                        {
                            return active;
                        }));
                        actor.GetShip.GetBonuses().GetBonus(BonusType.decreaseResists).ReplaceBuff(ID + actor.Avatar.Id, new Buff(0.5f, () =>
                        {
                            return active;
                        }));
                        StartClientEvent(actor, new Hashtable());
                    }
                    else
                        SendClientErrorEvent(actor, ReturnCode.Fatal, string.Format("source has {0} energy but need: {1}", actor.GetShip.Energy.Energy, _energy));
                }
                else
                    SendClientErrorEvent(actor, ReturnCode.Fatal, "Actor ship destroyed");
            }
            else
                SendClientErrorEvent(actor, ReturnCode.Fatal, "Skill not ready");
        }
    }

}
