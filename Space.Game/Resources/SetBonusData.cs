using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Space.Game.Resources
{
    public class SetBonusData
    {
        private ModuleSetBonusType bonusType;
        private object value;

        public SetBonusData(ModuleSetBonusType bonusType, object value)
        {
            this.bonusType = bonusType;
            this.value = value;
        }

        public ModuleSetBonusType BonusType
        {
            get
            {
                return this.bonusType;
            }
        }

        public T GetValue<T>()
        {
            return (T)value;
        }
    }
}
