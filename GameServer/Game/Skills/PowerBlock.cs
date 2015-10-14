namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    /// <summary>
    /// Power block skill decrease target resist on 80% and decrease source damage  on 50%, duration 15 sec
    /// </summary>
    /// 
    public class PowerBlock : UseSkill
    {
        public PowerBlock()
        {
            ID = "SK0116";
            _cooldown = 25.0f;
            _energy = 21.0f;
            effect = 0.8f;
            active_time = 15.0f;
        }

        public override void Use(ICombatActor sourceActor)
        {
            if (ready)
            {
                if (!sourceActor.ShipDestroyed)
                {
                    ICombatActor targetActor = null;
                    if (sourceActor.GetTarget.TargetIsICombatActor(out targetActor))
                    {
                        if (sourceActor.GetShip.Energy.EnoughAdd(_energy))
                        {
                            UseTimeUpdate();
                            targetActor.GetShip.GetBonuses().GetBonus(BonusType.decreaseResists).ReplaceBuff(sourceActor.Avatar.Id + ID, new Buff(effect, () => { return active; }));
                            sourceActor.GetShip.GetBonuses().GetBonus(BonusType.decreaseDamage).ReplaceBuff(sourceActor.Avatar.Id + ID, new Buff(0.5f, () => { return active; }));
                            StartClientEvent(sourceActor, new Hashtable());
                        }
                        else
                            SendClientErrorEvent(sourceActor, ReturnCode.Fatal, string.Format("source has {0} energy but need: {1}", sourceActor.GetShip.Energy.Energy, _energy));
                    }
                    else
                        SendClientErrorEvent(sourceActor, ReturnCode.Fatal, "Target not valid");
                }
                else
                    SendClientErrorEvent(sourceActor, ReturnCode.Fatal, "Actor ship destroyed");
            }
            else
                SendClientErrorEvent(sourceActor, ReturnCode.Fatal, "Skill not ready");
        }
    }

}
