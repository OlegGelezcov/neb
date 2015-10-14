namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;


    public class InterferenceSystemPrecision : SupportedUseSkill
    {
        private ICombatActor target;
        private ICombatActor source;

        public InterferenceSystemPrecision() {
            ID = "SK0122";
            _cooldown = 2.0f;
            _energy = 8.0f;
            effect = 0.05f;
            active_time = 15;
        }

        public override void Use(ICombatActor source)
        {
            this.source = source;
            source.GetShip.Energy.RemoveEnergyBuf(ID);

            if (toggled) 
            {
                toggled = false;
                target = null;
                this.StartClientEvent(this.source, new Hashtable() { { GenericEventProps.status, this.toggled } });
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
                                target.GetShip.GetBonuses().GetBonus(BonusType.decreasePrecision).ReplaceBuff(source.Avatar.Id + ID, new Buff(effect, () => toggled));
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
