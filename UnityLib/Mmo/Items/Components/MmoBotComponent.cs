namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoBotComponent : MmoBaseComponent {

        public BotItemSubType? botSubType {
            get {
                if (item != null) {
                    byte subType;
                    if (item.TryGetProperty<byte>((byte)PS.SubType, out subType)) {
                        return (BotItemSubType)subType;
                    }
                }
                return null;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Bot;
            }
        }
    }
}
