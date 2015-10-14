using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SpaceExtensions {

    public static float[] GetArray(this Vector3 vec) {
        return new float[] { vec.x, vec.y, vec.z };
    }

    public static Vector3 GetVector(this float[] array) {
        if (array.Length == 0)
            return Vector3.zero;
        if (array.Length == 1)
            return new Vector3(array[0], 0, 0);
        if (array.Length == 2)
            return new Vector3(array[0], array[1], 0);
        return new Vector3(array[0], array[1], array[2]);
    }

    public static int GetInt(this Hashtable hashTable, string key) {
        if (hashTable.ContainsKey(key)) {
            foreach (DictionaryEntry entry in hashTable) {
                if (entry.Key.ToString() == key) {
                    return (int)entry.Value;
                }
            }
        }
        Debug.LogError(string.Format("hash not cointains key: {0}", key));
        return 0;
    }

    public static byte GetByte(this Hashtable hashTable, string key) {
        if (hashTable.ContainsKey(key)) {
            foreach (DictionaryEntry entry in hashTable) {
                if (entry.Key.ToString() == key) {
                    return (byte)entry.Value;
                }
            }
        }
        Debug.LogError(string.Format("hash not cointains key: {0}", key));
        return (byte)0;
    }

    public static string GetString(this Hashtable hashTable, string key) {
        if (hashTable.ContainsKey(key)) {
            foreach (DictionaryEntry entry in hashTable) {
                if (entry.Key.ToString() == key) {
                    return (string)entry.Value;
                }
            }
        }
        Debug.LogError(string.Format("hash not cointains key: {0}", key));
        return null;
    }

    public static float GetFloat(this Hashtable hashTable, string key) {
        if (hashTable.ContainsKey(key)) {
            foreach (DictionaryEntry entry in hashTable) {
                if (entry.Key.ToString() == key) {
                    return (float)entry.Value;
                }
            }
        }
        Debug.LogError(string.Format("hash not cointains key: {0}", key));
        return 0.0f;
    }

    public static bool GetBool(this Hashtable hashTable, string key) {
        if (hashTable.ContainsKey(key)) {
            return (bool)hashTable[key];
        }
        Debug.LogError(string.Format("hash not cointains key: {0}", key));
        return false;
    }

    public static bool GetBool(this Hashtable hashTable, int key) {
        if (hashTable.ContainsKey(key)) {
            return (bool)hashTable[key];
        }
        Debug.LogError(string.Format("hash not cointains key: {0}", key));
        return false;
    }

    public static bool TryGetEnum<T>(Hashtable hashTable, string key, out T value) {
        value = default(T);
        foreach (DictionaryEntry entry in hashTable) {
            if (entry.Key.ToString() == key) {
                value = (T)entry.Value;
                return true;
            }
        }
        return false;
    }

    public static object GetValue(this Hashtable source, object key) {
        foreach (DictionaryEntry entry in source) {
            if (entry.Key.ToString() == key.ToString()) {
                return entry.Value;
            }
        }
        return null;
    }

    public static T ActivateSelf<T>(this T behaviour) where T : MonoBehaviour {
        if (!behaviour.gameObject.activeSelf) {
            behaviour.gameObject.SetActive(true);
        }
        return behaviour;
    }

    public static GameObject ActivateSelf(this GameObject go) {
        if (!go.activeSelf) {
            go.SetActive(true);
        }
        return go;
    }

    public static T DeactivateSelf<T>(this T behaviour) where T : MonoBehaviour {
        if (behaviour.gameObject.activeSelf) {
            behaviour.gameObject.SetActive(false);
        }
        return behaviour;
    }

    public static GameObject DeactivateSelf(this GameObject go) {
        if (go.activeSelf) {
            go.SetActive(false);
        }
        return go;
    }

    public static Hashtable AsHashtable(this Dictionary<string, Hashtable> dict) {
        Hashtable result = new Hashtable();
        foreach (var de in dict) {
            result.Add(de.Key, de.Value);
        }
        return result;
    }

    public static Vector3 toVector(this float[] array) {
        if (array == null)
            return Vector3.zero;
        if (array.Length == 0)
            return Vector3.zero;
        else if (array.Length == 1)
            return new Vector3(array[0], 0, 0);
        else if (array.Length == 2)
            return new Vector3(array[0], array[1], 0);
        else
            return new Vector3(array[0], array[1], array[2]);
    }

    public static float[] toArray(this Vector3 vec) {
        return new float[] { vec.x, vec.y, vec.z };
    }

    public static Hashtable toHash(this Dictionary<string, Hashtable> dict) {
        Hashtable result = new Hashtable();
        foreach (var pair in dict)
            result.Add(pair.Key, pair.Value);
        return result;
    }

    public static bool TryGetValue<T>(this Hashtable hash, string key, out T value) {
        value = default(T);
        if (hash.ContainsKey(key)) {
            value = (T)hash[key];
            return true;
        }
        return false;
    }

    public static string CustomJoin(this string[] strArray, int startIndex = 0, string concatString = "") {
        if (startIndex < 0 || startIndex >= strArray.Length)
            return string.Empty;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = startIndex; i < strArray.Length; i++) {
            sb.Append((i != strArray.Length - 1) ? strArray[i] + concatString : strArray[i]);
        }
        return sb.ToString();
    }

    public static void SetStyleTexture(this GUIStyle style, Texture2D tex) {
        style.normal.background = style.hover.background = style.active.background = style.focused.background =
            style.onNormal.background = style.onHover.background = style.onActive.background = style.onFocused.background = tex;
    }

    public static void SetLayerRecursively(this GameObject go, int layer) {
        go.layer = layer;
        foreach (Transform t in go.transform) {
            t.gameObject.SetLayerRecursively(layer);
        }
    }

    public static void ResetTransform(this GameObject go) {
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
    }

    public static GameObject GetChildrenWithName(this GameObject go, string name) {
        if (go.name == name)
            return go;
        foreach (Transform tChild in go.transform) {
            GameObject result = tChild.gameObject.GetChildrenWithName(name);
            if (result)
                return result;
        }
        return null;
    }

    public static Color ToColor(this string str) {
        if (string.IsNullOrEmpty(str))
            return Color.white;
        if (str == "black")
            return Color.black;
        if (str == "blue")
            return Color.blue;
        if (str == "clear")
            return Color.clear;
        if (str == "cyan")
            return Color.cyan;
        if (str == "gray")
            return Color.gray;
        if (str == "green")
            return Color.green;
        if (str == "grey")
            return Color.grey;
        if (str == "magenta")
            return Color.magenta;
        if (str == "red")
            return Color.red;
        if (str == "white")
            return Color.white;
        if (str == "yellow")
            return Color.yellow;
        if (str == "orange")
            return new Color(221f / 256f, 163f / 256f, 11f / 256f, 1.0f);

        string[] tokens = str.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        float r = 1f, g = 1f, b = 1f, a = 1f;

        if (tokens.Length > 0) {
            r = Mathf.Clamp01((float)int.Parse(tokens[0]) / 256f);
        }
        if (tokens.Length > 1) {
            g = Mathf.Clamp01((float)int.Parse(tokens[1]) / 256f);
        }
        if (tokens.Length > 2) {
            b = Mathf.Clamp01((float)int.Parse(tokens[2]) / 256f);
        }
        if (tokens.Length > 3) {
            a = Mathf.Clamp01((float)int.Parse(tokens[3]) / 256f);
        }

        return new Color(r, g, b, a);
    }

    public static void AddRange<T, U>(this Dictionary<T, U> to, Dictionary<T, U> from) {
        foreach (var kv in from) {
            if (!to.ContainsKey(kv.Key))
                to.Add(kv.Key, kv.Value);
            else
                Debug.LogError("Duplicate key: " + kv.Key + " when merging dictionaries");
        }
    }

    public static string ToDrawString(this Vector3 vec) {
        return string.Format("[ {0:F1},{1:F1},{2:F1} ]", vec.x, vec.y, vec.z);
    }

    public static string ToIntString(this Vector3 vec) {
        return string.Format("[{0},{1},{2}]", Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }

    public static void RemoveChildrens(this Transform parent) {
        for (int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Return printable hash string from hash table of any hierarchy level
    /// </summary>
    public static string ToHashString(this Hashtable hash) {
        if (hash == null) {
            return string.Empty;
        }
        var stringBuilder = new System.Text.StringBuilder();
        Common.CommonUtils.ConstructHashString(hash, 1, ref stringBuilder);
        return stringBuilder.ToString();
    }
}
