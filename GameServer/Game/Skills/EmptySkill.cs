namespace Space.Game.Skills
{
    using System;

    public class EmptySkill : UseSkill
    {
        public EmptySkill()
        {
            this.ID = string.Empty;
        }

        public override void Use(ICombatActor actor)
        {
            //Debug.Log("use Empty skill ");
        }

        public override bool IsEmpty
        {
            get
            {
                return true;
            }
        }

        private static EmptySkill instance = new EmptySkill();

        public static EmptySkill Get
        {
            get
            {
                return instance;
            }
        }
    }
}
