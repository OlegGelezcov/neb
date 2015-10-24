using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nebula {
    public class SystemInfo {
        public static string languageString {
            get {
                if(Application.systemLanguage == SystemLanguage.Russian) {
                    return "ru";
                }
                return "en";
            }
        }
    }
}
