using System;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Text;
using System.Linq;
#if UP
using System.Xml;
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif
using GameMath;
using System.Globalization;

namespace Common {
    public static class CommonExtensions {

        public static string ReplaceVariables(this string sourceString, Hashtable variablesInfo) {
            string result = sourceString;
            foreach (System.Collections.DictionaryEntry entry in variablesInfo) {
                string varName = "{" + entry.Key.ToString() + "}";
                result = result.Replace(varName, entry.Value.ToString());
            }
            return result;
        }




        public static T GetValue<T>(this Hashtable ht, byte key, T defaultValue) {
            if (ht.ContainsKey(key)) {
                if (ht[key].GetType() == typeof(double)) {
                    double d = (double)ht[key];
                    float f = (float)d;
                    return (T)(object)f;
                }

                if (typeof(T) == typeof(byte) && ht[key].GetType() != typeof(byte)) {
                    int ib = (int)ht[key];
                    byte b = (byte)ib;
                    return (T)(object)b;

                }

                if (typeof(T) == typeof(int) && ht[key].GetType() == typeof(byte)) {
                    int i = (byte)ht[key];
                    return (T)(object)i;
                }
            }
            return ht.ContainsKey(key) ? (T)ht[key] : defaultValue;
        }

        public static T GetValue<T>(this Hashtable ht, int key, T defaultValue) {
            if (ht.ContainsKey(key)) {
                if (ht[key].GetType() == typeof(double)) {
                    double d = (double)ht[key];
                    float f = (float)d;
                    return (T)(object)f;
                }

                if (typeof(T) == typeof(byte) && ht[key].GetType() != typeof(byte)) {
                    int ib = (int)ht[key];
                    byte b = (byte)ib;
                    return (T)(object)b;

                }

                if (typeof(T) == typeof(int) && ht[key].GetType() == typeof(byte)) {
                    int i = (byte)ht[key];
                    return (T)(object)i;
                }
            }
            return ht.ContainsKey(key) ? (T)ht[key] : defaultValue;
        }

        public static T GetValue<T>(this Hashtable ht, string key, T defaultValue) {
            if (ht.ContainsKey(key)) {
                if (ht[key].GetType() == typeof(double)) {
                    double d = (double)ht[key];
                    float f = (float)d;
                    return (T)(object)f;
                }

                if (typeof(T) == typeof(byte) && ht[key].GetType() != typeof(byte)) {
                    int ib = (int)ht[key];
                    byte b = (byte)ib;
                    return (T)(object)b;

                }

                if (typeof(T) == typeof(int) && ht[key].GetType() == typeof(byte)) {
                    int i = (byte)ht[key];
                    return (T)(object)i;
                }
            }

            return ht.ContainsKey(key) ? (T)ht[key] : defaultValue;
        }

        public static T Value<T>(this Hashtable hash, object key, T defaultValue = default(T)) {
            if (hash.ContainsKey(key)) {
                if (hash[key].GetType() == typeof(double)) {
                    double d = (double)hash[key];
                    float f = (float)d;
                    return (T)(object)f;
                }
                if (typeof(T) == typeof(byte) && hash[key].GetType() != typeof(byte)) {
                    int ib = (int)hash[key];
                    byte b = (byte)ib;
                    return (T)(object)b;
                }
                if (typeof(T) == typeof(int) && hash[key].GetType() == typeof(byte)) {
                    int i = (byte)hash[key];
                    return (T)(object)i;
                }
            }
            if (hash.ContainsKey(key)) {
                return (T)hash[key];
            }
            return defaultValue;
        }

        //public static T Value<T>(this Hashtable hash, byte key, T defaultValue = default(T)) {

        //    return hash.GetValue<T>(key, defaultValue);
        //}

        //public static T Value<T>(this Hashtable hash, int key, T defaultValue = default(T)) {

        //    return hash.GetValue<T>(key, defaultValue);
        //}

        //public static T Value<T>(this Hashtable hash, string key, T defaultValue = default(T)) {

        //    return hash.GetValue<T>(key, defaultValue);
        //}

        public static T GetValueOrDefault<T>(this Hashtable hash, string key) {
            if (typeof(T) == typeof(string)) {
                return (T)(object)hash.GetValue<string>(key, string.Empty);
            } else if (typeof(T) == typeof(int)) {
                return (T)(object)hash.GetValue<int>(key, 0);
            } else if (typeof(T) == typeof(float)) {
                return (T)(object)hash.GetValue<float>(key, 0.0f);
            } else if (typeof(T) == typeof(double)) {
                return (T)(object)hash.GetValue<double>(key, 0.0);
            } else if (typeof(T) == typeof(bool)) {
                return (T)(object)hash.GetValue<bool>(key, false);
            } else if (typeof(T) == typeof(byte)) {
                return (T)(object)hash.GetValue<byte>(key, 0);
            } else if (typeof(T) == typeof(Hashtable)) {
                return (T)(object)hash.GetValue<Hashtable>(key, new Hashtable());
            }
            return hash.GetValue<T>(key, default(T));
        }

        public static T GetValueOrDefault<T>(this Hashtable hash, int key) {
            if (typeof(T) == typeof(string)) {
                return (T)(object)hash.GetValue<string>(key, string.Empty);
            } else if (typeof(T) == typeof(int)) {
                return (T)(object)hash.GetValue<int>(key, 0);
            } else if (typeof(T) == typeof(float)) {
                return (T)(object)hash.GetValue<float>(key, 0.0f);
            } else if (typeof(T) == typeof(double)) {
                return (T)(object)hash.GetValue<double>(key, 0.0);
            } else if (typeof(T) == typeof(bool)) {
                return (T)(object)hash.GetValue<bool>(key, false);
            } else if (typeof(T) == typeof(byte)) {
                return (T)(object)hash.GetValue<byte>(key, 0);
            } else if (typeof(T) == typeof(Hashtable)) {
                return (T)(object)hash.GetValue<Hashtable>(key, new Hashtable());
            }
            return hash.GetValue<T>(key, default(T));
        }

        public static T GetValue<T>(this System.Collections.IDictionary ht, byte key, T defaultValue) {
            return ht.Contains(key) ? (T)ht[key] : defaultValue;
        }


        public static ItemType toItemType(this byte b) {
            return (ItemType)b;
        }

        public static byte toByte(this ItemType type) {
            return (byte)type;
        }

        public static byte toByte(this Enum e) {
            return (byte)(object)e;
        }

        public static T toEnum<T>(this byte b) {
            return (T)(object)b;
        }

        public static string f(this string str, params object[] args) {
            return string.Format(str, args);
        }

        public static T GetValue<T>(this Dictionary<byte, object> dict, ParameterCode code) {
            return (T)dict[(byte)code];
        }

        public static StringBuilder ToStringBuilder(this Hashtable hash) {
            StringBuilder sb = new StringBuilder();
            CommonUtils.ConstructHashString(hash, 1, ref sb);
            return sb;
        }

        public static StringBuilder ToStringBuilder(this object[] arr) {
            StringBuilder sb = new StringBuilder();
            CommonUtils.ConstructArrayString(arr, 1, ref sb);
            return sb;
        }

        public static Hashtable toHash<T, U>(this Dictionary<T, U> dict) {
            Hashtable result = new Hashtable();
            foreach (var pair in dict) {
                result.Add(pair.Key, pair.Value);
            }
            return result;
        }

        public static Dictionary<T, U> toDict<T, U>(this Hashtable hash) {
            Dictionary<T, U> result = new Dictionary<T, U>();
            foreach (System.Collections.DictionaryEntry entry in hash) {
                result.Add((T)entry.Key, (U)entry.Value);
            }
            return result;
        }

        public static IEnumerable<U> SmartCast<T, U>(this IEnumerable<T> source) where U : class {
            return source.Where(s => (s is U)).Select(s => (s as U));
        }

        /// <summary>
        /// Check that class relaize interface, null parameter is allowed
        /// </summary>
        public static bool Check<T>(this object obj) where T : class {
            if (obj == null)
                return false;
            return (obj is T);
        }

        public static float ToFloat(this string str) {
            if (string.IsNullOrEmpty(str))
                return 0.0f;
            float result = 0.0f;
            float.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static int ToInt(this string str) {
            if (string.IsNullOrEmpty(str))
                return 0;
            int result = 0;
            int.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static int[] ToIntArray(this string str) {
            if (string.IsNullOrEmpty(str)) { return new int[] { }; }
            string[] tokens = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> result = new List<int>();
            foreach (string token in tokens) {
                int temp;
                if (int.TryParse(token, out temp)) {
                    result.Add(temp);
                }
            }
            return result.ToArray();
        }

        public static float[] ToFloatArray(this string str) {
            if (string.IsNullOrEmpty(str)) { return new float[] { }; }
            string[] tokens = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<float> result = new List<float>();
            foreach (string token in tokens) {
                float temp;
                if (float.TryParse(token, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out temp)) {
                    result.Add(temp);
                }
            }
            return result.ToArray();
        }

        public static bool ToBool(this string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            if (str.ToLower().Trim() == "true")
                return true;
            return false;
        }

        public static float[] ToVector(this string str) {
            float[] result = new float[] { 0.0f, 0.0f, 0.0f };
            if (string.IsNullOrEmpty(str))
                return result;
            string[] tokens = str.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < System.Math.Min(tokens.Length, result.Length); i++) {
                result[i] = tokens[i].ToFloat();
            }
            return result;
        }

        public static MinMax ToMinMax(this string source) {
            float[] minArr = new float[] { 0f, 0f, 0f };
            float[] maxArr = new float[] { 0f, 0f, 0f };
            if (string.IsNullOrEmpty(source)) {
                return new MinMax(new Vector3(minArr), new Vector3(maxArr));
            }
            string[] minMaxTokens = source.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (minMaxTokens.Length > 0) {
                minArr = minMaxTokens[0].ToFloatArray3();
            }
            if (minMaxTokens.Length > 0) {
                maxArr = minMaxTokens[1].ToFloatArray3();
            }
            return new MinMax(new Vector3(minArr), new Vector3(maxArr));
        }

        public static float[] ToFloatArray3(this string source) {
            float[] result = new float[] { 0f, 0f, 0f };
            if (string.IsNullOrEmpty(source)) {
                return result;
            }
            string[] tokens = source.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Math.Min(result.Length, tokens.Length); i++) {
                result[i] = tokens[i].ToFloat();
            }
            return result;
        }

        public static Dictionary<T, U> ToDictionary<T, U>(this Hashtable hash) {
            Dictionary<T, U> dict = new Dictionary<T, U>();
            foreach (System.Collections.DictionaryEntry entry in hash) {
                dict.Add((T)entry.Key, (U)entry.Value);
            }
            return dict;
        }

        public static List<string> ToList(this string str) {
            if (string.IsNullOrEmpty(str))
                return new List<string>();
            string[] sArr = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return sArr.ToList();
        }

        public static string AsString<T, U>(this KeyValuePair<T, U> pair) {
            return "{0}:{1}".f(pair.Key.ToString(), pair.Value.ToString());
        }

        public static T GetRandomElement<T>(this T[] arr) {
            if (arr.Length == 0)
                return default(T);
            int rIndex = Rand.Int(0, arr.Length - 1);
            return arr[rIndex];
        }

#if UP
        public static int ToInt(this XmlAttribute attr) {
            return int.Parse(attr.Value);
        }
        public static float ToFloat(this XmlAttribute attr) {
            return float.Parse(attr.Value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static bool ToBool(this XmlAttribute attr) {
            return bool.Parse(attr.Value);
        }
#else
        public static int ToInt(this XAttribute attr) {
            return int.Parse(attr.Value);
        }

        public static float ToFloat(this XAttribute attr) {
            return float.Parse(attr.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static bool ToBool(this XAttribute attr) {
            return bool.Parse(attr.Value);
        }
#endif

        public static float[] RandomizeInRadius(this float[] center, float radius) {
            float val = Rand.Float(radius);
            float[] vec = CommonUtils.RandomUnitVector3();
            for (int i = 0; i < vec.Length; i++)
                vec[i] *= val;

            float[] result = new float[center.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = center[i] + vec[i];
            return result;
        }

        public static float[] RandomizeInMinMax(this float[] center, MinMax minmax) {
            float x = Rand.Float(minmax.Min.X, minmax.Max.X);
            float y = Rand.Float(minmax.Min.Y, minmax.Max.Y);
            float z = Rand.Float(minmax.Min.Z, minmax.Max.Z);
            float[] arr = new float[] { x, y, z };
            float[] result = new float[center.Length];
            for (int i = 0; i < result.Length; i++) {
                result[i] = center[i] + arr[i];
            }
            return result;
        }

        public static Vector ToVector(this float[] vector, bool roundComponentsToInt) {
            if (!roundComponentsToInt) {
                if (vector == null) {
                    return new Vector { X = 0f, Y = 0f, Z = 0f };
                }
                if (vector.Length == 1) {
                    return new Vector { X = vector[0], Y = 0, Z = 0 };
                }
                if (vector.Length == 2) {
                    return new Vector { X = vector[0], Y = vector[1], Z = 0f };
                }
                if (vector.Length > 2) {
                    return new Vector { X = vector[0], Y = vector[1], Z = vector[2] };
                }
                return new Vector { X = 0f, Y = 0f, Z = 0f };
            } else {
                switch (vector.Length) {
                    case 0:
                        {
                            return new Vector();
                        }

                    case 1:
                        {
                            return new Vector { X = Convert.ToInt32(vector[0]) };
                        }

                    case 2:
                        {
                            return new Vector { X = Convert.ToInt32(vector[0]), Y = Convert.ToInt32(vector[1]) };
                        }

                    default:
                        {
                            return new Vector { X = Convert.ToInt32(vector[0]), Y = Convert.ToInt32(vector[1]), Z = Convert.ToInt32(vector[2]) };
                        }
                }
            }
        }


#region Public Methods

        /// <summary>
        ///   Converts a <see cref = "Vector" /> to a float array.
        ///   Each value is devided by 100.
        /// </summary>
        /// <param name = "vector">
        ///   The vector.
        /// </param>
        /// <param name = "dimensions">
        ///   The dimensions.
        /// </param>
        /// <returns>
        ///   A new float array.
        /// </returns>
        public static float[] ToFloatArray(this Vector vector, byte dimensions) {
            switch (dimensions) {
                case 0:
                    {
                        return new float[0];
                    }

                case 1:
                    {
                        return new[] { Convert.ToSingle(vector.X) };
                    }

                case 2:
                    {
                        return new[] { Convert.ToSingle(vector.X), Convert.ToSingle(vector.Y) };
                    }

                case 3:
                    {
                        return new[] { Convert.ToSingle(vector.X), Convert.ToSingle(vector.Y), Convert.ToSingle(vector.Z) };
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException("dimensions");
                    }
            }
        }



        public static string[] AsKeyArray(this Hashtable hashTable) {
            string[] keys = new string[hashTable.Count];
            int counter = 0;
            foreach (System.Collections.DictionaryEntry entry in hashTable) {
                keys[counter++] = entry.Key.ToString();
            }
            return keys;
        }
        public static List<string> AsKeyList(this Hashtable hashTable) {
            List<string> result = new List<string>();
            foreach (System.Collections.DictionaryEntry entry in hashTable) {
                result.Add(entry.Key.ToString());
            }
            return result;
        }

        public static void AddHashtable(this Hashtable meHashtable, Hashtable target) {
            foreach (System.Collections.DictionaryEntry entry in target) {
                if (!meHashtable.ContainsKey(entry.Key))
                    meHashtable.Add(entry.Key, entry.Value);
            }
        }

        public static object GetValue(this Hashtable hashTable, string key) {
            if (hashTable.ContainsKey(key)) {
                foreach (System.Collections.DictionaryEntry entry in hashTable) {
                    if (entry.Key.ToString() == key)
                        return entry.Value;
                }
            }
            return null;
        }


        public static Vector3 ToVector3(this float[] array) {
            if (array == null)
                return Vector3.Zero;

            if (array.Length == 0)
                return Vector3.Zero;

            if (array.Length == 1)
                return new Vector3(array[0], 0, 0);

            if (array.Length == 2)
                return new Vector3(array[0], array[1], 0);

            return new Vector3(array[0], array[1], array[2]);
        }

        public static float[] ToArray(this Vector3 vector) {
            return new float[] { vector.X, vector.Y, vector.Z };
        }

        public static Vector ToVector(this Vector3 vec) {
            return new Vector { X = vec.X, Y = vec.Y, Z = vec.Z };
        }

        /// <summary>
        ///   Converts a float array to a <see cref = "Vector" />.
        ///   Each value is multiplied with 100.
        /// </summary>
        /// <param name = "vector">
        ///   The vector.
        /// </param>
        /// <returns>
        ///   A new <see cref = "Vector" />.
        /// </returns>

        #endregion

#if UP
        public static float GetFloat(this UPXElement element, string name) {
            return element.GetAttributeFloat(name);
        }
        public static float[] GetFloatArray(this UPXElement element, string name) {
            string arrStr = element.GetAttributeString(name);
            string[] arr = arrStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0) {
                return new float[] { 0, 0, 0 };
            } else if (arr.Length == 1) {
                return new float[] { float.Parse(arr[0], CultureInfo.InvariantCulture), 0, 0 };
            } else if (arr.Length == 2) {
                return new float[] { float.Parse(arr[0], CultureInfo.InvariantCulture), float.Parse(arr[1], CultureInfo.InvariantCulture), 0 };
            } else {
                return new float[]
                {
                    float.Parse(arr[0], CultureInfo.InvariantCulture),
                    float.Parse(arr[1], CultureInfo.InvariantCulture),
                    float.Parse(arr[2], CultureInfo.InvariantCulture)
                };
            }
        }

        public static List<Vector3> ToVector3List(this UPXElement element, string name) {
            string source = element.GetAttributeString(name);
            List<Vector3> result = new List<Vector3>();
            string[] sourceArr = source.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in sourceArr) {
                result.Add(s.ToFloatArray3().ToVector3());
            }
            return result;
        }
        public static T GetEnum<T>(this UPXElement e, string name) {
            return (T)System.Enum.Parse(typeof(T), e.GetAttributeString(name));
        }

        public static int GetInt(this UPXElement e, string name) {
            return e.GetAttributeInt(name);
        }
        public static string GetString(this UPXElement e, string name) {
            return e.GetAttributeString(name);
        }
        public static bool GetBool(this UPXElement e, string name) {
            return e.GetAttributeBool(name);
        }
        public static bool HasAttribute(this UPXElement element, string name) {
            return element.HasAtt(name);
        }
#else

        public static float GetFloat(this XElement element, string name) {
            return float.Parse(element.Attribute(name).Value, CultureInfo.InvariantCulture);
        }



        public static float[] GetFloatArray(this XElement element, string name) {
            string arrStr = element.Attribute(name).Value;
            string[] arr = arrStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0) {
                return new float[] { 0, 0, 0 };
            } else if (arr.Length == 1) {
                return new float[] { float.Parse(arr[0], CultureInfo.InvariantCulture), 0, 0 };
            } else if (arr.Length == 2) {
                return new float[] { float.Parse(arr[0], CultureInfo.InvariantCulture), float.Parse(arr[1], CultureInfo.InvariantCulture), 0 };
            } else {
                return new float[]
                {
                    float.Parse(arr[0], CultureInfo.InvariantCulture),
                    float.Parse(arr[1], CultureInfo.InvariantCulture),
                    float.Parse(arr[2], CultureInfo.InvariantCulture)
                };
            }
        }


        public static List<Vector3> ToVector3List(this XElement element, string name) {
            List<Vector3> result = new List<Vector3>();
            string source = element.GetString(name);
            string[] sourceArr = source.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in sourceArr) {
                result.Add(s.ToFloatArray3().ToVector3());
            }
            return result;
        }


        public static T GetEnum<T>(this XElement e, string name) {
            return (T)System.Enum.Parse(typeof(T), e.Attribute(name).Value);
        }



        public static int GetInt(this XElement e, string name) {
            return int.Parse(e.Attribute(name).Value);
        }

        public static string GetString(this XElement e, string name) {
            return e.Attribute(name).Value;
        }

        public static bool GetBool(this XElement e, string name) {
            return bool.Parse(e.Attribute(name).Value);
        }

        public static bool HasAttribute(this XElement element, string attrName) {
            foreach (var attr in element.Attributes()) {
                if (attr.Name == attrName)
                    return true;
            }
            return false;
        }
#endif


        public static void AddVector3(this float[] arr, Vector3 vec) {
            arr[0] += vec.X;
            arr[1] += vec.Y;
            arr[2] += vec.Z;
        }

        public static Hashtable HashDeepCopy(this Hashtable source) {
            if (source == null)
                return new Hashtable();
            Hashtable result = new Hashtable();
            foreach (System.Collections.DictionaryEntry entry in source) {
                if (entry.Value != null && entry.Value is Hashtable) {
                    result.Add(entry.Key, (entry.Value as Hashtable).HashDeepCopy());
                } else {
                    result.Add(entry.Key, entry.Value);
                }
            }
            return result;
        }

        public static int FromHex(this string source) {
            if (string.IsNullOrEmpty(source)) { return 0; }

            int num;
            if (int.TryParse(source, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num)) {
                return num;
            }
            return 0;
        }

        public static int GetIntParameter(this Dictionary<byte, object> source, ParameterCode key ) {
            if(source.ContainsKey((byte)key)) {
                return (int)source[(byte)key];
            }
            return 0;
        }

        public static float GetFloatParameter(this Dictionary<byte, object> source, ParameterCode key) {
            if(source.ContainsKey((byte)key)) {
                return (float)source[(byte)key];
            }
            return 0f;
        }

        public static string GetStringParameter(this Dictionary<byte, object> source, ParameterCode key) {
            if(source.ContainsKey((byte)key)) {
                return (string)source[(byte)key];
            }
            return string.Empty;
        }

        public static bool GetBoolParameter(this Dictionary<byte, object> source, ParameterCode key) {
            if(source.ContainsKey((byte)key)) {
                return (bool)source[(byte)key];
            }
            return false;
        }

        public static Hashtable GetHashParameter(this Dictionary<byte, object> source, ParameterCode key) {
            if(source.ContainsKey((byte)key)) {
                Hashtable hash = source[(byte)key] as Hashtable;
                if(hash == null ) {
                    hash = new Hashtable();
                }
                return hash;
            }
            return new Hashtable();
        }
    }


}

