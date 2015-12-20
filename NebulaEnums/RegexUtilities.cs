using System;
using System.Text.RegularExpressions;

namespace ServerClientCommon {
    public class RegexUtilities {
        private bool invalid = false;

        public bool IsValidEmail(string strIn) {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            //try {
            //    strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            //} catch (Exception) {
            //    return false;
            //}

            if(invalid) {
                return false;
            }

            try {
                /*
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);*/

                return Regex.IsMatch(strIn, @"^\S+@\S+$", RegexOptions.IgnoreCase);

            } catch (Exception) {
                return false;
            }
        }

        /*
        private string DomainMapper(Match match) {
            IdnMapping idn = new IdnMapping();
            string domainName = match.Groups[2].Value;
            try {
                domainName = idn.GetAscii(domainName);
            } catch (ArgumentException) {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }*/
    }
}
