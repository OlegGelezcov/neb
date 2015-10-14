namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Common;

    public interface IMmoComponentContainer : IItemProperty {
        void UpdateMmoComponent(MmoBaseComponent component);
        Dictionary<ComponentID, MmoBaseComponent> components {
            get;

        }
    }
}
