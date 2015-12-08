namespace Nebula.Client.Utils {
    using System;
    using ExitGames.Client.Photon;


    public static class Extensions {

        //================================BYTE KEY========================================
        public static int GetValueInt(this Hashtable hash, byte key, int defValue = 0 ) {
            if(hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if(valType == typeof(int)) {
                    return (int)val;
                }else if(valType == typeof(byte)) {
                    return (int)(byte)val;
                } else if(valType == typeof(long)) {
                    return (int)(long)val;
                } else if(valType == typeof(short)) {
                    return (int)(short)val;
                } else if(valType == typeof(float)) {
                    return (int)(float)val;
                } else if(valType == typeof(double)) {
                    return (int)(double)val;
                }
            }
            return defValue;
        }


        public static float GetValueFloat(this Hashtable hash, byte key, float defValue = 0f ) {
            if(hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if(valType == typeof(float)) {
                    return (float)val;
                } else if(valType == typeof(double)) {
                    return (float)(double)val;
                } else if(valType == typeof(int)) {
                    return (float)(int)val;
                } else if(valType == typeof(byte)) {
                    return (float)(byte)val;
                } else if(valType == typeof(long)) {
                    return (float)(long)val;
                } else if(valType == typeof(short)) {
                    return (float)(short)val;
                }
            }
            return defValue;
        }

        public static byte GetValueByte(this Hashtable hash, byte key, byte defValue = 0 ) {
            if(hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if(valType == typeof(byte)) {
                    return (byte)val;
                } else if(valType == typeof(int)) {
                    return (byte)(int)val;
                } else if(valType == typeof(long)) {
                    return (byte)(long)val;
                } else if(valType == typeof(short)) {
                    return (byte)(short)val;
                }
            }
            return defValue;
        }

        public static string GetValueString(this Hashtable hash, byte key ) {
            if(hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if(val == null ) {
                    return string.Empty;
                }
                return val.ToString();
            }
            return string.Empty;
        }

        public static string GetValueString(this Hashtable hash, byte key, string defValue) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (val == null) {
                    return defValue;
                }
                return val.ToString();
            }
            return defValue;
        }

        public static bool GetValueBool(this Hashtable hash, byte key, bool defValue = false ) {
            if(hash.ContainsKey(key)) {
                return (bool)hash[key];
            }
            return defValue;
        }

        public static Hashtable GetValueHash(this Hashtable hash, byte key) {
            if(hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if(res == null ) {
                    res = new Hashtable();
                }
                return res;
            }
            return new Hashtable();
        }

        public static Hashtable GetValueHash(this Hashtable hash, byte key, Hashtable defValue) {
            if (hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static float[] GetValueFloatArray(this Hashtable hash, byte key) {
            if(hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if(res == null ) {
                    res = new float[] { 0, 0, 0 };
                }
                return res;
            }
            return new float[] { 0, 0, 0 };
        }

        public static float[] GetValueFloatArray(this Hashtable hash, byte key, float[] defValue) {
            if (hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static object[] GetValueObjectArray(this Hashtable hash, byte key) {
            if(hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if(res == null ) {
                    res = new object[] { };
                }
                return res;
            }
            return new object[] { };
        }

        public static object[] GetValueObjectArray(this Hashtable hash, byte key, object[] defValue) {
            if (hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static string[] GetValueStringArray(this Hashtable hash, byte key) {
            if(hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if(res == null ) {
                    res = new string[] { };
                }
                return res;
            }
            return new string[] { };
        }

        public static string[] GetValueStringArray(this Hashtable hash, byte key, string[] defValue) {
            if (hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }


        //=================================INT KEY==========================================
        public static int GetValueInt(this Hashtable hash, int key, int defValue = 0) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(int)) {
                    return (int)val;
                } else if (valType == typeof(byte)) {
                    return (int)(byte)val;
                } else if (valType == typeof(long)) {
                    return (int)(long)val;
                } else if (valType == typeof(short)) {
                    return (int)(short)val;
                } else if (valType == typeof(float)) {
                    return (int)(float)val;
                } else if (valType == typeof(double)) {
                    return (int)(double)val;
                }
            }
            return defValue;
        }


        public static float GetValueFloat(this Hashtable hash, int key, float defValue = 0f) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(float)) {
                    return (float)val;
                } else if (valType == typeof(double)) {
                    return (float)(double)val;
                } else if (valType == typeof(int)) {
                    return (float)(int)val;
                } else if (valType == typeof(byte)) {
                    return (float)(byte)val;
                } else if (valType == typeof(long)) {
                    return (float)(long)val;
                } else if (valType == typeof(short)) {
                    return (float)(short)val;
                }
            }
            return defValue;
        }

        public static byte GetValueByte(this Hashtable hash, int key, byte defValue = 0) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(byte)) {
                    return (byte)val;
                } else if (valType == typeof(int)) {
                    return (byte)(int)val;
                } else if (valType == typeof(long)) {
                    return (byte)(long)val;
                } else if (valType == typeof(short)) {
                    return (byte)(short)val;
                }
            }
            return defValue;
        }

        public static string GetValueString(this Hashtable hash, int key) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (val == null) {
                    return string.Empty;
                }
                return val.ToString();
            }
            return string.Empty;
        }

        public static string GetValueString(this Hashtable hash, int key, string defValue) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (val == null) {
                    return defValue;
                }
                return val.ToString();
            }
            return defValue;
        }

        public static bool GetValueBool(this Hashtable hash, int key, bool defValue = false) {
            if (hash.ContainsKey(key)) {
                return (bool)hash[key];
            }
            return defValue;
        }

        public static Hashtable GetValueHash(this Hashtable hash, int key) {
            if (hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if (res == null) {
                    res = new Hashtable();
                }
                return res;
            }
            return new Hashtable();
        }

        public static Hashtable GetValueHash(this Hashtable hash, int key, Hashtable defValue) {
            if (hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static float[] GetValueFloatArray(this Hashtable hash, int key) {
            if (hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if (res == null) {
                    res = new float[] { 0, 0, 0 };
                }
                return res;
            }
            return new float[] { 0, 0, 0 };
        }

        public static float[] GetValueFloatArray(this Hashtable hash, int key, float[] defValue) {
            if (hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static object[] GetValueObjectArray(this Hashtable hash, int key) {
            if (hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if (res == null) {
                    res = new object[] { };
                }
                return res;
            }
            return new object[] { };
        }

        public static object[] GetValueObjectArray(this Hashtable hash, int key, object[] defValue) {
            if (hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static string[] GetValueStringArray(this Hashtable hash, int key) {
            if (hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if (res == null) {
                    res = new string[] { };
                }
                return res;
            }
            return new string[] { };
        }

        public static string[] GetValueStringArray(this Hashtable hash, int key, string[] defValue) {
            if (hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        //=====================================STRING KEY=========================================
        public static int GetValueInt(this Hashtable hash, string key, int defValue = 0) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(int)) {
                    return (int)val;
                } else if (valType == typeof(byte)) {
                    return (int)(byte)val;
                } else if (valType == typeof(long)) {
                    return (int)(long)val;
                } else if (valType == typeof(short)) {
                    return (int)(short)val;
                } else if (valType == typeof(float)) {
                    return (int)(float)val;
                } else if (valType == typeof(double)) {
                    return (int)(double)val;
                }
            }
            return defValue;
        }


        public static float GetValueFloat(this Hashtable hash, string key, float defValue = 0f) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(float)) {
                    return (float)val;
                } else if (valType == typeof(double)) {
                    return (float)(double)val;
                } else if (valType == typeof(int)) {
                    return (float)(int)val;
                } else if (valType == typeof(byte)) {
                    return (float)(byte)val;
                } else if (valType == typeof(long)) {
                    return (float)(long)val;
                } else if (valType == typeof(short)) {
                    return (float)(short)val;
                }
            }
            return defValue;
        }

        public static byte GetValueByte(this Hashtable hash, string key, byte defValue = 0) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(byte)) {
                    return (byte)val;
                } else if (valType == typeof(int)) {
                    return (byte)(int)val;
                } else if (valType == typeof(long)) {
                    return (byte)(long)val;
                } else if (valType == typeof(short)) {
                    return (byte)(short)val;
                }
            }
            return defValue;
        }

        public static string GetValueString(this Hashtable hash, string key) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (val == null) {
                    return string.Empty;
                }
                return val.ToString();
            }
            return string.Empty;
        }

        public static string GetValueString(this Hashtable hash, string key, string defValue) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (val == null) {
                    return defValue;
                }
                return val.ToString();
            }
            return defValue;
        }

        public static bool GetValueBool(this Hashtable hash, string key, bool defValue = false) {
            if (hash.ContainsKey(key)) {
                return (bool)hash[key];
            }
            return defValue;
        }

        public static Hashtable GetValueHash(this Hashtable hash, string key) {
            if (hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if (res == null) {
                    res = new Hashtable();
                }
                return res;
            }
            return new Hashtable();
        }

        public static Hashtable GetValueHash(this Hashtable hash, string key, Hashtable defValue) {
            if (hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static float[] GetValueFloatArray(this Hashtable hash, string key) {
            if (hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if (res == null) {
                    res = new float[] { 0, 0, 0 };
                }
                return res;
            }
            return new float[] { 0, 0, 0 };
        }

        public static float[] GetValueFloatArray(this Hashtable hash, string key, float[] defValue) {
            if (hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static object[] GetValueObjectArray(this Hashtable hash, string key) {
            if (hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if (res == null) {
                    res = new object[] { };
                }
                return res;
            }
            return new object[] { };
        }

        public static object[] GetValueObjectArray(this Hashtable hash, string key, object[] defValue) {
            if (hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static string[] GetValueStringArray(this Hashtable hash, string key) {
            if (hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if (res == null) {
                    res = new string[] { };
                }
                return res;
            }
            return new string[] { };
        }

        public static string[] GetValueStringArray(this Hashtable hash, string key, string[] defValue) {
            if (hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        //=====================================short keys===========================================================
        public static int GetValueInt(this Hashtable hash, short key, int defValue = 0) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(int)) {
                    return (int)val;
                } else if (valType == typeof(byte)) {
                    return (int)(byte)val;
                } else if (valType == typeof(long)) {
                    return (int)(long)val;
                } else if (valType == typeof(short)) {
                    return (int)(short)val;
                } else if (valType == typeof(float)) {
                    return (int)(float)val;
                } else if (valType == typeof(double)) {
                    return (int)(double)val;
                }
            }
            return defValue;
        }


        public static float GetValueFloat(this Hashtable hash, short key, float defValue = 0f) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(float)) {
                    return (float)val;
                } else if (valType == typeof(double)) {
                    return (float)(double)val;
                } else if (valType == typeof(int)) {
                    return (float)(int)val;
                } else if (valType == typeof(byte)) {
                    return (float)(byte)val;
                } else if (valType == typeof(long)) {
                    return (float)(long)val;
                } else if (valType == typeof(short)) {
                    return (float)(short)val;
                }
            }
            return defValue;
        }

        public static byte GetValueByte(this Hashtable hash, short key, byte defValue = 0) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (valType == typeof(byte)) {
                    return (byte)val;
                } else if (valType == typeof(int)) {
                    return (byte)(int)val;
                } else if (valType == typeof(long)) {
                    return (byte)(long)val;
                } else if (valType == typeof(short)) {
                    return (byte)(short)val;
                }
            }
            return defValue;
        }

        public static string GetValueString(this Hashtable hash, short key) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (val == null) {
                    return string.Empty;
                }
                return val.ToString();
            }
            return string.Empty;
        }

        public static string GetValueString(this Hashtable hash, short key, string defValue) {
            if (hash.ContainsKey(key)) {
                object val = hash[key];
                Type valType = val.GetType();
                if (val == null) {
                    return defValue;
                }
                return val.ToString();
            }
            return defValue;
        }

        public static bool GetValueBool(this Hashtable hash, short key, bool defValue = false) {
            if (hash.ContainsKey(key)) {
                return (bool)hash[key];
            }
            return defValue;
        }

        public static Hashtable GetValueHash(this Hashtable hash, short key) {
            if (hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if (res == null) {
                    res = new Hashtable();
                }
                return res;
            }
            return new Hashtable();
        }

        public static Hashtable GetValueHash(this Hashtable hash, short key, Hashtable defValue) {
            if (hash.ContainsKey(key)) {
                Hashtable res = hash[key] as Hashtable;
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static float[] GetValueFloatArray(this Hashtable hash, short key) {
            if (hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if (res == null) {
                    res = new float[] { 0, 0, 0 };
                }
                return res;
            }
            return new float[] { 0, 0, 0 };
        }

        public static float[] GetValueFloatArray(this Hashtable hash, short key, float[] defValue) {
            if (hash.ContainsKey(key)) {
                float[] res = hash[key] as float[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static object[] GetValueObjectArray(this Hashtable hash, short key) {
            if (hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if (res == null) {
                    res = new object[] { };
                }
                return res;
            }
            return new object[] { };
        }

        public static object[] GetValueObjectArray(this Hashtable hash, short key, object[] defValue) {
            if (hash.ContainsKey(key)) {
                object[] res = hash[key] as object[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }

        public static string[] GetValueStringArray(this Hashtable hash, short key) {
            if (hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if (res == null) {
                    res = new string[] { };
                }
                return res;
            }
            return new string[] { };
        }

        public static string[] GetValueStringArray(this Hashtable hash, short key, string[] defValue) {
            if (hash.ContainsKey(key)) {
                string[] res = hash[key] as string[];
                if (res == null) {
                    res = defValue;
                }
                return res;
            }
            return defValue;
        }
    }
}