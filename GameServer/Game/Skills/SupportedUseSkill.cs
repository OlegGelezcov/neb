namespace Space.Game.Skills
{
    using System;

    public abstract class SupportedUseSkill : UseSkill
    {
        protected bool toggled;

        public bool Toggled {
            get {
                return toggled;
            }
        }
    }
}
