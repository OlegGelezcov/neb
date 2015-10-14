using Nebula.Mmo.Games;

namespace Nebula.Mmo {
    public interface IEngine {
        void OnGameBehaviourChanged(GameType gameType, GameState gameState);
    }
}
