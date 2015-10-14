using Common;

namespace Nebula.UI {
    public interface ICombatObjectInfo : IIconObjectInfo {
        int Level { get; }
        Race Race { get; }
        float CurrentHealth { get; }
        float MaxHealth { get; }
        float HitProb { get; }
    }
}
