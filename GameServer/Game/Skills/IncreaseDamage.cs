namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    public class IncreaseDamage : UseSkill
    {
        public IncreaseDamage()
        {
            ID = "SK0104";
            _cooldown = 40.0f;
            active_time = 10.0f;
            effect = 0.2f;
            _energy = 100.0f;
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
                        actor.GetShip.GetBonuses().GetBonus(BonusType.increaseDamage).ReplaceBuff(ID, new Buff(effect, () =>
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
