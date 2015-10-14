namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    //toggled on\of skill which affect target range
    public class InterferenceSystemRange : SupportedUseSkill
    {
        private ICombatActor target;
        private ICombatActor source;

        public InterferenceSystemRange() {
            ID = "SK0123";
            _cooldown = 7.0f;
            _energy = 15.0f;
            effect = 0.2f;
            active_time = 15;
        }

        public override void Use(ICombatActor source)
        {
            this.source = source;

            source.GetShip.Energy.RemoveEnergyBuf(ID);

            if (toggled) 
            {
                Release();
                return;
            }
            else
            {
                if (ready) 
                {
                    if (!source.ShipDestroyed)
                    {
                        if (source.GetShip.Energy.MaxEnergy > _energy)
                        {
                            if (source.GetTarget.TargetIsICombatActor(out target))
                            {
                                toggled = true;
                                target.GetShip.GetBonuses().GetBonus(BonusType.decreaseRange).ReplaceBuff(source.Avatar.Id + ID, new Buff(effect, () => toggled));
                                source.GetShip.Energy.SetEnergyBuf(ID, -_energy);
                                this.StartClientEvent(this.source, new Hashtable() { { GenericEventProps.status, this.toggled } });
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
            if (source != null) {
                source.GetShip.Energy.RemoveEnergyBuf(ID);
                this.StartClientEvent(this.source, new Hashtable() { { GenericEventProps.status, this.toggled } });
            }
        }

        public override void Touch()
        {
            if( toggled )
            {
                if( target != null && source != null )
                {
                    ICombatActor newTarget;
                    if(source.GetTarget.TargetIsICombatActor(out newTarget))
                    {
                        if( newTarget.Avatar.Id != target.Avatar.Id )
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
