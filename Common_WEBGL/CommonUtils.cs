using System;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Text;
using GameMath;

namespace Common {

    public static class CommonUtils {
        public static readonly DateTime START_DATE = new DateTime(1970, 1, 1);
        private static readonly Dictionary<Race, List<Workshop>> raceWorkshops;

        static CommonUtils() {
            raceWorkshops = new Dictionary<Race, List<Workshop>>{
                { Race.Humans, new List<Workshop> { Workshop.DarthTribe, Workshop.Equilibrium, Workshop.RedEye, Workshop.BigBang } },
                { Race.Borguzands, new List<Workshop> { Workshop.Rakhgals, Workshop.Phelpars, Workshop.Zoards, Workshop.Lerjees } },
                { Race.Criptizoids, new List<Workshop> { Workshop.Yshuar, Workshop.KrolRo, Workshop.Arlen, Workshop.Dyneira} }
            };
        }

        public static Race RaceForWorkshop(Workshop w) {
            if (raceWorkshops[Race.Humans].Contains(w)) {
                return Race.Humans;
            } else if (raceWorkshops[Race.Borguzands].Contains(w)) {
                return Race.Borguzands;
            } else if (raceWorkshops[Race.Criptizoids].Contains(w)) {
                return Race.Criptizoids;
            }
            return Race.None;
        }

        public static int[] GenerateNumbers(int numberCount, int numberSum) {
            if (numberCount == 0)
                numberCount = 2;

            if (numberCount == 1)
                return new int[] { numberSum };

            int[] result = new int[numberCount];
            int points = numberSum;
            while (points > 0) {
                for (int i = 0; i < result.Length; i++) {
                    if (Rand.Float01() < 0.5f && points > 0) {
                        result[i] += 1;
                        points--;
                    }
                }
            }

            //int[] generatedPoint = new int[numberCount - 1];
            //for (int i = 0; i < generatedPoint.Length; i++)
            //{
            //    generatedPoint[i] = Rand.Int(numberSum); 
            //}

            //Array.Sort(generatedPoint);

            //int[] result = new int[numberCount];
            //for (int i = 0; i < numberCount; i++)
            //{
            //    if (i == 0)
            //    {
            //        result[i] = generatedPoint[0] - 0;
            //    }
            //    else if (i > 0 && i < generatedPoint.Length)
            //        result[i] = generatedPoint[i] - generatedPoint[i - 1];
            //    else
            //        result[i] = numberSum - generatedPoint[generatedPoint.Length - 1];
            //}
            return result;
        }

        public static float GetFloat(string text) {
            return float.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static T GetEnum<T>(string text) {
            return (T)System.Enum.Parse(typeof(T), text);
        }


        public static T RandomEnumValue<T>() {
            string[] names = Enum.GetNames(typeof(T));
            int index = Rand.Int() % names.Length;
            T result = (T)Enum.Parse(typeof(T), names[index]);
            return result;
        }

        public static Workshop RandomWorkshop(Race race) {
            List<Workshop> workshops = raceWorkshops[race];
            return workshops[Rand.Int(workshops.Count - 1)];
        }

        public static Workshop RandomWorkshop() {
            return RandomEnumValue<Workshop>();
        }

        public static ShipModelSlotType RandomSlotType() {
            return RandomEnumValue<ShipModelSlotType>();
        }

        public static Race RandomRace() {
            List<Race> races = new List<Race> { Race.Humans, Race.Borguzands, Race.Criptizoids };
            return races[Rand.Int(races.Count - 1)];
        }

        public static List<T> GetEnumValues<T>(List<T> except) {
            string[] names = Enum.GetNames(typeof(T));
            List<T> result = new List<T>();
            foreach (string name in names) {
                T nValue = (T)Enum.Parse(typeof(T), name);
                if (false == except.Contains(nValue))
                    result.Add(nValue);
            }
            return result;
        }

        public static T GetRandomEnumValue<T>(List<T> except) {
            List<T> allowedValues = GetEnumValues<T>(except);
            if (allowedValues.Count == 0)
                throw new InvalidOperationException("no enum values");
            return allowedValues[Rand.Int() % allowedValues.Count];
        }

        public static void ConstructHashString(Hashtable hash, int level, ref StringBuilder sb) {
            sb.AppendLine(new string(' ', level) + "{");
            foreach (System.Collections.DictionaryEntry entry in hash) {
                if ((entry.Value is Hashtable) == true) {
                    sb.Append(new string(' ', level) + entry.Key.ToString() + " : ");
                    ConstructHashString(entry.Value as Hashtable, level + 1, ref sb);

                } else if (entry.Value is object[]) {
                    sb.Append(new string(' ', level) + entry.Key.ToString() + " : ");
                    ConstructArrayString(entry.Value as object[], level + 1, ref sb);
                } else {
                    sb.AppendLine(new string(' ', level) + entry.Key.ToString() + " : " + entry.Value);
                }
            }
            sb.AppendLine(new string(' ', level) + "}");
        }

        public static void ConstructArrayString(object[] array, int level, ref StringBuilder sb) {
            sb.AppendLine(new string(' ', level) + "[");
            for (int i = 0; i < array.Length; i++) {
                if (true == (array[i] is Hashtable)) {
                    ConstructHashString(array[i] as Hashtable, level, ref sb);
                } else if (true == (array[i] is object[])) {
                    ConstructArrayString((array[i] as object[]), level, ref sb);
                } else {
                    sb.Append(new string(' ', level) + array[i].ToString() + (i == array.Length - 1 ? System.Environment.NewLine : ", "));
                }
            }
            sb.AppendLine(new string(' ', level) + "]");
        }

        public static float[] RandomUnitVector3() {
            float x = Rand.Float(-1, 1);
            float y = Rand.Float(-1, 1);
            float z = Rand.Float(-1, 1);
            float len = (float)Math.Sqrt(x * x + y * y + z * z);
            return new float[] { x / len, y / len, z / len };
        }

        public static object ParseValue(string value, TagType type) {
            switch (type) {
                case TagType.Integer:
                    {
                        return value.ToInt();
                    }
                case TagType.Float:
                    {
                        return value.ToFloat();
                    }
                case TagType.String:
                    {
                        return value;
                    }
                case TagType.Bool:
                    {
                        return value.ToBool();
                    }
                case TagType.StringArray:
                    {
                        return value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    }
                case TagType.IntegerArray:
                    {
                        return value.ToIntArray();
                    }
                case TagType.FloatArray:
                    {
                        return value.ToFloatArray();
                    }
                default:
                    throw new Exception(string.Format("unsupported tag type = {0}", type));
            }
        }

        public static object ParseValue(string value, string type) {
            switch (type.Trim().ToLower()) {
                case "float":
                    return value.ToFloat();
                case "int":
                    return value.ToInt();
                case "bool":
                    return value.ToBool();
                case "string":
                    return value;
                case "vector":
                    return value.ToVector();
                case "stringarray":
                    {
                        string[] result = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        return result;
                    }
                case "logfilterarray":
                    {
                        string[] strArr = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        List<LogFilter> filters = new List<LogFilter>();
                        foreach (var s in strArr) {
                            try {
                                LogFilter lf = (LogFilter)Enum.Parse(typeof(LogFilter), s);
                                filters.Add(lf);
                            } catch (Exception) {

                            }
                        }
                        return filters.ToArray();
                    }
                case "weapontype":
                    {
                        WeaponType wt = (WeaponType)Enum.Parse(typeof(WeaponType), value);
                        return wt;
                    }
                case "race":
                    {
                        Race race = (Race)Enum.Parse(typeof(Race), value);
                        return race;
                    }
                case "minmax":
                    {
                        return value.ToMinMax();
                    }
                case "hexint":
                    {
                        return int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                    }
                default:
                    goto case "string";
            }
        }

        public static bool AnyFrom<T>(this T toCheck, params T[] args) {
            if (args == null || args.Length == 0)
                return false;

            foreach (T a in args) {
                if (a.ToString() == toCheck.ToString())
                    return true;
            }

            return false;
        }

        public static int SecondsFrom1970() {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static int SecondsFrom1970(DateTime date) {
            return (int)(date - START_DATE).TotalSeconds;
        }


        public static bool AllNonEmty(string[] args) {
            bool nonEmpty = true;
            foreach (var s in args) {
                if (string.IsNullOrEmpty(s)) {
                    nonEmpty = false;
                    break;
                }
            }
            return nonEmpty;
        }
    }
}
