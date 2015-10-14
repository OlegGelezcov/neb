using UnityEngine;
using System.Collections;

namespace Nebula.Mmo.Games {
    /// <summary>
    /// Game type. Foreach game type we have peer for connecting to server
    /// </summary>
    public enum GameType {
        //master game
        Master,
        //login game
        Login,
        //select character game
        SelectCharacter,
        //main game
        Game,
        None
    }
}