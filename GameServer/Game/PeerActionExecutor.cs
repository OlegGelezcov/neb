using Common;
using System.Collections;

namespace Space.Game {
    public class PeerActionExecutor
    {

        private Hashtable GetFailReturn(string message)
        {
            return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, message } };
        }

    }
}
