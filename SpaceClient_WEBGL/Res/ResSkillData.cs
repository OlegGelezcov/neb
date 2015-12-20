

namespace Nebula.Client.Res
{
    using Common;
    using ExitGames.Client.Photon;
    using global::Common;

    public class ResSkillData
    {
        public int id;
        public string name;
        public float durability;
        public SkillType type;
        public float energy;
        public float cooldown;
        public string description;
        public Hashtable inputs;
        public SkillTargetType targetType;
    }

    public enum SkillTargetType {
        enemy,
        friend,
        self
    }
}
