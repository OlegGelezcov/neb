using System;

namespace Nebula.Client.Utils {
    public static class StringUtils {

        /// <summary>
        /// Format to localized string interval in seconds. Result string has form
        /// 'AAd BBh' or 'AAh BBm' or 'AAm BBs' or 'AAs' with localized suffixes
        /// </summary>
        /// <param name="iseconds">Input interval in seconds</param>
        /// <param name="lang">Input language (en or ru)</param>
        /// <returns>Localized string of time interval</returns>
        public static string FormatIntervalSeconds(int iseconds, string lang) {
            string result = string.Empty;

            TimeSpan timeSpan = TimeSpan.FromSeconds(iseconds);
            int days = timeSpan.Days;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if(days > 0) {
                result += days.ToString("00") + dl(lang) + " ";
            }

            if(days > 0 ) {
                result += hours.ToString("00") + hl(lang) + " ";
                return result;
            } else {

                if(hours > 0 ) {
                    result += hours.ToString("00") + hl(lang) + " ";
                } 

                if(hours > 0 ) {
                    result += minutes.ToString("00") + ml(lang) + " ";
                    return result;
                } else {

                    if(minutes > 0 ) {
                        result += minutes.ToString("00") + ml(lang) + " ";
                    }

                    if(minutes > 0 ) {
                        result += seconds.ToString("00") + sl(lang) + " ";
                        return result;
                    } else {
                        result += seconds.ToString("00") + sl(lang);
                    }
                }
            }
            return result;
        }

        private static string dl(string lang) {
            return (lang == "ru") ? "д" : "d";
        }

        private static string hl(string lang) {
            return (lang == "ru") ? "ч" : "h";
        }

        private static string ml(string lang) {
            return (lang == "ru") ? "м" : "m";
        }

        private static string sl(string lang) {
            return (lang == "ru") ? "с" : "s";
        }
    }
}
