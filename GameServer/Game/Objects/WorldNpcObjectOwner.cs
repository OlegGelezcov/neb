using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Objects
{
    public sealed class WorldNpcObjectOwner : NpcObjectOwner
    {
        private MmoWorld world;

        private  WorldNpcObjectOwner(MmoWorld world)
        {
            this.world = world;
        }

        public override void HandleDeath(NebulaObject obj)
        {
            if(this.world != null )
            {
                world.HandleNpcDeath(obj.Id);
            }
        }

        public override NpcObjectOwnerType GetOwnerType()
        {
            return NpcObjectOwnerType.World;
        }

        public static WorldNpcObjectOwner Create(MmoWorld world)
        {
            return new WorldNpcObjectOwner(world);
        }

        public override string GetID()
        {
            return this.world.GetID();
        }
    }
}
