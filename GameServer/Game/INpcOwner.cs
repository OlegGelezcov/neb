using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game
{
    public interface INpcOwner
    {
        void HandleNpcDeath(string npcID);

        string GetID();
    }
}
