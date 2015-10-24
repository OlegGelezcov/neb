using UnityEngine.Events;

namespace UnityEngine.UI {

    /// <summary>
    /// Extensions for Unity UI classes
    /// </summary>
    public static class UnityUIExtensions {

        /// <summary>
        /// Delete all existing listeners and add single parameter listener
        /// </summary>
        public static void SetSingleListener(this Button button, UnityAction action) {
            button.onClick.RemoveAllListeners();
            if (action != null) {
                button.onClick.AddListener(action);
            }
        }

        /// <summary>
        /// Delete all previous listeners and add single parameter listener
        /// </summary>
        public static void SetSingleListener(this Toggle toggle, UnityAction<bool> action) {
            toggle.onValueChanged.RemoveAllListeners();
            if (action != null) {
                toggle.onValueChanged.AddListener(action);
            }
        }
    }
}
