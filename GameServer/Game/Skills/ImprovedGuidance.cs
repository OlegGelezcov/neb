namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    public class ImprovedGuidance : UseSkill
    {

        public ImprovedGuidance()
        {
            ID = "SK0114";
            _cooldown = 35;
            _energy = 17;
            active_time = 20;
            effect = 0.15f;
        }

        public override void Use(ICombatActor actor)
        {
            if (ready)
            {
                if (actor.ShipDestroyed == false)
                {
                    if (actor.GetShip.Energy.EnoughAdd(_energy))
                    {
                        UseTimeUpdate();

                        actor.GetShip.GetBonuses().GetBonus(BonusType.increaseRange).ReplaceBuff(actor.Avatar.Id + ID, new Buff(effect, () => { return active; }));
                        actor.GetShip.GetBonuses().GetBonus(BonusType.increasePrecision).ReplaceBuff(actor.Avatar.Id + ID, new Buff(effect, () => { return active; }));
                        actor.GetShip.GetBonuses().GetBonus(BonusType.decreaseSpeed).ReplaceBuff(actor.Avatar.Id + ID, new Buff(effect, () => { return active; }));
                        actor.GetShip.Touch();

                        StartClientEvent(actor, new Hashtable());
                    }
                    else
                    {
                        SendClientErrorEvent(actor, ReturnCode.Fatal, string.Format("source has {0} energy but need: {1}", actor.GetShip.Energy.Energy, _energy));
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
