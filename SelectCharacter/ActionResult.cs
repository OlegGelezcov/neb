using Common;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter {
    public class ActionResult : IInfoSource {
        public ReturnCode code { get; private set; }
        public Hashtable info { get; private set; }

        public ActionResult(ReturnCode inCode, Hashtable inInfo = null ) {
            code = inCode;
            info = inInfo;
        }

        public Hashtable GetInfo() {
            Hashtable hash = new Hashtable {
                {(int)SPC.ReturnCode, (int)code },

            };
            if(info != null ) {
                foreach(DictionaryEntry de in info ) {
                    hash.Add(de.Key, de.Value);
                }
            }
            return hash;
        }
    }
}
