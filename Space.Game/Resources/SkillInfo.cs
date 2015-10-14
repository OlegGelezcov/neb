namespace Space.Game.Skills
{
    using System;
    using Common;

    public class SkillInfo
    {
        public string id;
        public ShipModelSlotType type;

        public override string ToString()
        {
            return string.Format("{0}:{1}", id, type);
        }
    }


}
