namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    /// <summary>
    /// Improved cooldown, decrease cooldown on 20%, decrease hit prob on 30%, duration 20 sec
    /// </summary>
    public class ImprovedCooldown : UseSkill
    {
        public ImprovedCooldown()
        {
            ID = "SK0117";
            _cooldown = 35.0f;
            _energy = 15.0f;
            active_time = 20.0f;
            effect = 0.2f;
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
                        /*
                        actor.GetShip.GetBonuses().GetBonus(BonusType.decreaseCooldown).ReplaceBuff(ID + actor.Avatar.Id, new Buff(effect, () =>
                        {
                            return active;
                        }));
                        actor.GetShip.GetBonuses().GetBonus(BonusType.decreaseResists).ReplaceBuff(ID + actor.Avatar.Id, new Buff(0.5f, () =>
                        {
                            return active;
                        }));*/

                        actor.GetShip.GetBonuses().GetBonus(BonusType.decreaseCooldown).ReplaceBuff(ID + actor.Avatar.Id, new Buff(effect, () => { return active; }));
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
