using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Objects
{
    public abstract class NpcObjectOwner
    {
        public abstract void HandleDeath(NebulaObject obj);

        public abstract NpcObjectOwnerType GetOwnerType();

        public abstract string GetID();


        public static NpcObjectOwner CreateWorld(MmoWorld world)
        {
            return WorldNpcObjectOwner.Create(world);
        }

        public static NpcObjectOwner CreateGroup(NpcGroup npcGroup)
        {
            return GroupNpcObjectOwner.Create(npcGroup);
        }

        public static NpcObjectOwner CreateNone()
        {
            return NoneNpcObjectOwner.Create();
        }
    }
}
