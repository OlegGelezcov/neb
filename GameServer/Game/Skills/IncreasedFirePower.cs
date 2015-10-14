

namespace Space.Game.Skills
{
    using System;
    using System.Collections;
    using Common;

    /// <summary>
    /// Increase damage on ship on 5% when toggled on
    /// </summary>
    public class IncreasedFirePower : SupportedUseSkill
    {
        private ICombatActor source;

        public IncreasedFirePower() {
            ID = "SK0124";
            _cooldown = 5.0f;
            _energy = 10.0f;
            effect = 0.05f;
            active_time = 0;
        }

        public override void Use(ICombatActor source)
        {
            this.source = source;
            this.source.GetShip.Energy.RemoveEnergyBuf(ID);

            if (this.toggled)
            {
                this.source.SendServiceMessage( ServiceMessageType.Info, "TOGGLE OFF");
                Release();
            }
            else 
            {
                if (UseAllowed()) 
                {
                    this.toggled = true;
                    this.source.GetShip.GetBonuses()
                        .GetBonus(BonusType.increaseDamage)
                        .ReplaceBuff(this.source.Avatar.Id + ID,
                        new Buff(effect, () => this.toggled));
                    this.source.GetShip.Energy.SetEnergyBuf(ID, -_energy);
                    this.StartClientEvent(this.source, new Hashtable() { { GenericEventProps.status, this.toggled } });
                    this.source.SendServiceMessage(ServiceMessageType.Info, "TOGGLE ON");
                }
                else
                {
                    this.source.SendServiceMessage(ServiceMessageType.Warning, GetServiceMessage());
                }
            }
        }

        private bool UseAllowed() {
            return (this.ready) && (false == this.source.ShipDestroyed) &&
                (this.source.GetShip.Energy.MaxEnergy >= _energy);
        }

        private string GetServiceMessage() 
        {
            if (this.source != null)
            {
                return string.Format("skill ready: {0}; source ship destroyed: {1};" +
                    " source max energy: {2}; skill required energy: {3}",
                    this.ready, this.source.ShipDestroyed,
                    this.source.GetShip.Energy.MaxEnergy, this._energy);
            }
            else 
            {
                return "source object is null";    
            }
        }

        public override void Release()
        {

            this.toggled = false;
            if (this.source != null) 
            {
                this.source.SendServiceMessage(ServiceMessageType.Info, "Release() SK0124 called");
                this.source.GetShip.GetBonuses().RemoveBuff(BonusType.increaseDamage, this.source.Avatar.Id + ID);
                this.source.GetShip.Energy.RemoveEnergyBuf(ID);
                this.StartClientEvent(this.source, new Hashtable() { { GenericEventProps.status, this.toggled } });
            }
            
        }


    }
}
