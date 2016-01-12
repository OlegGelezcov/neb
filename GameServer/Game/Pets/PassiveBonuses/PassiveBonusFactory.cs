using Nebula.Engine;
using Nebula.Pets;

namespace Nebula.Game.Pets.PassiveBonuses {
    public class PassiveBonusFactory {

        public PassivePetBonus Create(PetPassiveBonusInfo bonus, NebulaObject source ) {
            if(bonus == null ) {
                return new EmptyPetBonus(bonus, source);
            }

            switch(bonus.id) {
                case 2:
                    return new PvpPointsPetBonus(bonus, source);
                case 3:
                    return new CreditsPetBonus(bonus, source);
                case 4:
                    return new SpeedPetBonus(bonus, source);
                case 5:
                    return new InputHealingPetBonus(bonus, source);
                case 6:
                    return new OutputHealingPetBonus(bonus, source);
                case 7:
                    return new DamagePetBonus(bonus, source);
                case 8:
                    return new InputDamagePetBonus(bonus, source);
                case 9:
                    return new EnergyRegenPetBonus(bonus, source);
                case 10:
                    return new OptimalDistancePetBonus(bonus, source);
                case 11:
                    return new AutoLootPetBonus(bonus, source);
                default:
                    return new EmptyPetBonus(bonus, source);
            }
        }
    }

}
