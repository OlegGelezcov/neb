using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public enum BotAIState : byte
    {
        Patrol = 0,
        Pursuit = 1,
        Fire = 2,
        Dead = 3
    }
}

