namespace Space.Game.Skills
{
    using System;
    using Common;
    using System.Collections;

    public class BlockResist : UseSkill
    {
        public BlockResist()
        {
            ID = "SK0108";
            _cooldown = 50.0f;
            active_time = 10.0f;
            effect = 1.0f;
            _energy = 100;
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
                            targetActor.GetShip.GetBonuses().GetBonus(BonusType.blockResist).ReplaceBuff(sourceActor.Avatar.Id + ID, new Buff(effect, () => { return active; }));
                            sourceActor.GetShip.GetBonuses().GetBonus(BonusType.blockResist).ReplaceBuff(sourceActor.Avatar.Id + ID, new Buff(effect, () => { return active; }));
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
