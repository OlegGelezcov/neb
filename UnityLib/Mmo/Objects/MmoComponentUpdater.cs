namespace Nebula.Mmo.Objects {
    using UnityEngine;
    using System.Collections;
    using Nebula.Mmo.Items;
    using Items.Components;

    public class MmoComponentUpdater : MonoBehaviour {

        private IMmoComponentContainer mItem;

        public void SetItem(IMmoComponentContainer it) {
            mItem = it;
        }

        private float mTimer;

        void Start() {
            mTimer = 1;
        }

        void Update() {
            if (mItem == null) { return; }

            mTimer -= Time.deltaTime;
            if (mTimer <= 0f) {

                mTimer = 2f;

                foreach (var comp in mItem.components) {
                    comp.Value.Update();
                }
            }
        }
    }
}
