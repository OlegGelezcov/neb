using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Objects
{
    public sealed class NoneNpcObjectOwner : NpcObjectOwner
    {
        public NoneNpcObjectOwner() { }
        public override void HandleDeath(NebulaObject obj)
        {
            
        }

        public override NpcObjectOwnerType GetOwnerType()
        {
            return NpcObjectOwnerType.None;
        }

        public static NoneNpcObjectOwner Create()
        {
            return new NoneNpcObjectOwner();
        }

        public override string GetID()
        {
            return string.Empty;
        }
    }
}
