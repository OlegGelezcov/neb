namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    public class OverheatingGuns : UseSkill
    {
        public OverheatingGuns()
        {
            ID = "SK0106";
            _cooldown = 15.0f;
            effect = 0.2f;
            _energy = 100.0f;
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

                        //apply overheating status for player
                        actor.ApplyOverheatingGun();

                        //add such bonus to player with effect
                        actor.GetShip.GetBonuses().GetBonus(BonusType.overheatingGuns).ReplaceBuff(ID, new Buff(effect, () =>
                        {
                            return actor.GetOverheatingStatus();
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
