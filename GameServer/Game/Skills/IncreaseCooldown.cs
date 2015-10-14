namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    /// <summary>
    /// decrease enemy cooldown
    /// </summary>
    public class IncreaseCooldown : UseSkill
    {

        public IncreaseCooldown()
        {
            ID = "SK0101";
            _cooldown = 20.0f;
            active_time = 10.0f;
            effect = 0.25f;
            _energy = 100;
        }

        public override void Use(ICombatActor actor)
        {
            if (ready)
            {
                if (!actor.ShipDestroyed)
                {
                    ICombatActor targetActor = null;
                    if (actor.GetTarget.TargetIsICombatActor(out targetActor))
                    {
                        if (actor.GetShip.Energy.EnoughAdd(_energy))
                        {
                            UseTimeUpdate();
                            targetActor.GetShip.GetBonuses().GetBonus(BonusType.increaseCooldown).ReplaceBuff(actor.Avatar.Id + ID, new Buff(effect, () => { return active; }));
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
