namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    public class PriorityDamage : UseSkill
    {

        public PriorityDamage()
        {
            ID = "SK0110";
            _cooldown = 20;
            active_time = 10;
            effect = 0.2f;
            _energy = 16;
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
                        actor.GetShip.GetBonuses().GetBonus(BonusType.increaseDamage).ReplaceBuff(ID + actor.Avatar.Id, new Buff(effect, () => { return active; }));
                        actor.GetShip.GetBonuses().GetBonus(BonusType.decreasePrecision).ReplaceBuff(ID + actor.Avatar.Id, new Buff(0.3f, () => { return active; }));
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
