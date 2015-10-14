namespace Space.Game.Skills
{
    using System;
    using Common;
    using System.Collections;

    public class InterferenceSystemRecharge : SupportedUseSkill
    {

        private ICombatActor target;
        private ICombatActor actor;

        public InterferenceSystemRecharge()
        {
            ID = "SK0121";
            _cooldown = 5.0f;
            _energy = 12;
            effect = 0.1f;
            active_time = 15;
        }

        public override void Use(ICombatActor actor)
        {
            this.actor = actor;
            actor.GetShip.Energy.RemoveEnergyBuf(ID);

            if (toggled)
            {
                toggled = false;
                target = null;
                this.StartClientEvent(this.actor, new Hashtable() { { GenericEventProps.status, this.toggled } });
                return;
            }
            else
            {
                if (ready)
                {
                    if (!actor.ShipDestroyed)
                    {
                        if (actor.GetShip.Energy.MaxEnergy > _energy)
                        {
                            if (actor.GetTarget.TargetIsICombatActor(out target))
                            {
                                ConsoleLogging.Get.Print("Apply SK0121 to obj {0}", (ItemType)target.Avatar.Type);
                                toggled = true;
                                target.GetShip.GetBonuses().GetBonus(BonusType.increaseCooldown).ReplaceBuff(actor.Avatar.Id + ID, new Buff(effect, () => { return toggled; }));
                                actor.GetShip.Energy.SetEnergyBuf(ID, -_energy);
                                this.StartClientEvent(this.actor, new Hashtable() { { GenericEventProps.status, this.toggled } });
                            }
                        }
                    }
                }
            }
        }

        public override void Release()
        {
            target = null;
            toggled = false;
            if (actor != null) {
                actor.GetShip.Energy.RemoveEnergyBuf(ID);
                this.StartClientEvent(this.actor, new Hashtable() { { GenericEventProps.status, this.toggled } });
            }
        }

        public override void Touch()
        {
            if (toggled)
            {
                if (target != null && actor != null)
                {
                    ICombatActor newTarget;
                    if (actor.GetTarget.TargetIsICombatActor(out newTarget))
                    {
                        if (newTarget.Avatar.Id != target.Avatar.Id)
                        {
                            Release();
                        }
                    }
                    else
                    {
                        Release();
                    }
                }
                else
                {
                    Release();
                }
            }
        }

    }
    
}
