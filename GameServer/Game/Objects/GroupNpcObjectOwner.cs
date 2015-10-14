using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Objects
{
    public sealed class GroupNpcObjectOwner : NpcObjectOwner
    {
        private NpcGroup npcGroup;

        private GroupNpcObjectOwner(NpcGroup npcGroup)
        {
            this.npcGroup = npcGroup;
        }

        public override void HandleDeath(NebulaObject obj)
        {
            if(this.npcGroup != null )
            {
                this.npcGroup.HandleNpcDeath(obj.Id);
            }
        }

        public override NpcObjectOwnerType GetOwnerType()
        {
            return NpcObjectOwnerType.NpcGroup;
        }

        public static GroupNpcObjectOwner Create(NpcGroup npcGroup)
        {
            return new GroupNpcObjectOwner(npcGroup);
        }

        public override string GetID()
        {
            return this.npcGroup.GetID();
        }
    }
}
