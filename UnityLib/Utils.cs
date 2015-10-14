using Common;
using System.Collections;
using UnityEngine;

namespace Nebula {

    public static class Utils {
        private static Matrix4x4 _savedMatr;
        private static Color _savedColor;


        public static void SaveMatrix() {
            _savedMatr = GUI.matrix;
            GUI.matrix = GameMatrix();
        }

        public static Matrix4x4 GameMatrix() {
            return Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(ResX, ResY, 1));
        }

        public static Matrix4x4 SkewMatrix(float sx, float sy) {
            var mat = Matrix4x4.identity;
            mat.SetRow(0, new Vector4(1, sy, 0, 0));
            mat.SetRow(1, new Vector4(sx, 1, 0, 0));
            return mat;
        }
        public static void RestoreMatrix() {
            GUI.matrix = _savedMatr;
        }

        public static float NativeWidth {
            get {
                return 1920;
            }
        }
        public static float NativeHeight {
            get {
                return 1080;
            }
        }
        public static float ResX {
            get {
                return Screen.width / NativeWidth;
            }
        }
        public static float ResY {
            get {
                return Screen.height / NativeHeight;
            }
        }

        public static Color HexString2Color(string colorString) {
            if (string.IsNullOrEmpty(colorString))
                return Color.black;
            else {
                string numberString = string.Empty;
                if (colorString.StartsWith("#"))
                    numberString = colorString.Substring(1);
                else
                    numberString = colorString;
                string hexR = numberString.Substring(0, 2);
                string hexG = numberString.Substring(2, 2);
                string hexB = numberString.Substring(4, 2);
                string hexA = "FF";
                if (numberString.Length > 6)
                    hexA = numberString.Substring(6);
                float r = Mathf.Clamp01((float)System.Convert.ToInt32(hexR, 16) / 256.0f);
                float g = Mathf.Clamp01((float)System.Convert.ToInt32(hexG, 16) / 256.0f);
                float b = Mathf.Clamp01((float)System.Convert.ToInt32(hexB, 16) / 256.0f);
                float a = Mathf.Clamp01((float)System.Convert.ToInt32(hexA, 16) / 256.0f);
                return new Color(r, g, b, a);
            }
        }

        public static float[] GetInRange(float min, float max, int dimen = 3) {
            float[] result = new float[dimen];
            for (int i = 0; i < dimen; i++) {
                result[i] = Random.value * (max - min) + min;
            }
            return result;
        }

        public static Color ItemColor(this ObjectColor c) {
            switch (c) {
                case ObjectColor.blue:
                    return Color.blue;
                case ObjectColor.green:
                    return Color.green;
                case ObjectColor.orange:
                    return new Color(219f / 256, 129f / 256, 15f / 256);
                case ObjectColor.yellow:
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }

        public static Rect WorldPos2ScreenRect(Vector3 position, Vector2 rectSize, Camera camera = null) {
            if (!camera)
                camera = Camera.main;
            if (!camera)
                return new Rect(0, 0, 0, 0);

            if (Vector3.Dot(camera.transform.forward, position - camera.transform.position) >= 0) {
                Vector2 screenPoint = camera.WorldToScreenPoint(position);
                screenPoint.x = Mathf.Clamp(screenPoint.x, 0.0f, Screen.width);
                screenPoint.y = Mathf.Clamp(screenPoint.y, 0.0f, Screen.height);
                Vector2 scaledPoint = new Vector2(screenPoint.x / ResX, NativeHeight - screenPoint.y / ResY);
                scaledPoint.x = Mathf.Clamp(scaledPoint.x, rectSize.x * 0.5f, NativeWidth - rectSize.x * 0.5f);
                scaledPoint.y = Mathf.Clamp(scaledPoint.y, rectSize.y * 0.5f, NativeHeight - rectSize.y * 0.5f);

                return new Rect(scaledPoint.x - rectSize.x * 0.5f, scaledPoint.y - rectSize.y * 0.5f, rectSize.x, rectSize.y);
            } else {
                return new Rect((NativeWidth - rectSize.x * 0.1f) * rectSize.x, (NativeHeight - rectSize.y * 0.1f) * 0.5f, rectSize.x * 0.1f, rectSize.y * 0.1f);
            }
            /*
            if (Vector3.Dot(camera.transform.forward, position - camera.transform.position) >= 0)
            {
                Vector2 screenPoint = camera.WorldToScreenPoint(position);
                Vector2 scaledPoint = new Vector2(screenPoint.x  / ResX, NativeHeight - screenPoint.y / ResY);
                scaledPoint.x = Mathf.Clamp(scaledPoint.x, rectSize.x * 0.5f, NativeWidth - rectSize.x * 0.5f);
                scaledPoint.y = Mathf.Clamp(scaledPoint.y, rectSize.y * 0.5f, NativeHeight - rectSize.y * 0.5f);

                return new Rect(scaledPoint.x - rectSize.x * 0.5f, scaledPoint.y - rectSize.y * 0.5f, rectSize.x, rectSize.y);
            }
            else
            {
                return new Rect(0.5f * rectSize.x, (NativeHeight - rectSize.y) * 0.5f, rectSize.x, rectSize.y);
            }*/
        }

        public static Rect GetCenterRect(Vector2 size) {
            float x = (NativeWidth - size.x) * 0.5f;
            float y = (NativeHeight - size.y) * 0.5f;
            return new Rect(x, y, size.x, size.y);
        }

        public static Rect GetDownCenterRect(Vector2 size) {
            float x = (NativeWidth - size.x) * 0.5f;
            float y = (NativeHeight - size.y);
            return new Rect(x, y, size.x, size.y);
        }

        public static long GetTime() {
            return System.DateTime.UtcNow.Ticks;
        }

        public static void SetColor(Color color) {
            _savedColor = GUI.color;
            GUI.color = color;
        }

        public static void RestoreColor() {
            GUI.color = _savedColor;
        }

        public static Vector2 ConvertedMousePosition {
            get {
                float xk = NativeWidth / Screen.width;
                float yk = NativeHeight / Screen.height;
                Vector2 sourceMousePos = Input.mousePosition;
                Vector2 convertedMousePos = new Vector2(sourceMousePos.x * xk, (Screen.height - sourceMousePos.y) * yk);
                return convertedMousePos;
            }
        }

        public static Vector2 UVVector(Vector3 worldPos) {
            float xk = NativeWidth / Screen.width;
            float yk = NativeHeight / Screen.height;
            var sp = Camera.main.WorldToScreenPoint(worldPos);
            Vector2 convertedSP = new Vector2(sp.x * xk, (Screen.height - sp.y) * yk);
            return new Vector2(convertedSP.x / NativeWidth, convertedSP.y / NativeHeight);
        }

        public static Color RandomColor {
            get {
                return new Color(Random.value, Random.value, Random.value, Random.value);
            }
        }

        public static bool MouseOverGUI {
            get {
                return Time.renderedFrameCount <= _lastFrameOverGUI + 1;
            }

            set {
                if (value) {
                    _lastFrameOverGUI = Time.renderedFrameCount;
                }
            }
        }
        private static int _lastFrameOverGUI;

        public static bool InRange(float x, float min, float max) {
            return x >= min && x <= max;
        }

        public static bool InRange01(float x) {
            return InRange(x, 0, 1);
        }

        public static T RandomEnum<T>() {
            T[] values = (T[])System.Enum.GetValues(typeof(T));
            return values[Random.Range(0, values.Length)];
        }

        //public static Rect Arrange(Vector2 size, Vector2 listSize, int index, UIDirection arrangement)
        //{
        //    Rect rct = new Rect(0, 0, size.x, size.y);
        //    if (arrangement == UIDirection.Horizontal)
        //    {
        //        int xCountMax = (int)(listSize.x / size.x);
        //        int line = (index / xCountMax);
        //        rct.y = line * size.y;
        //        rct.x = (index - (line * xCountMax)) * size.x; 
        //    }
        //    if (arrangement == UIDirection.Vertical)
        //    {
        //        int yCountMax = (int)(listSize.x / size.y);
        //        rct.x = (int)(index / yCountMax) * size.x;
        //        rct.y = (index - ((int)(index / yCountMax) * yCountMax)) * size.y;
        //    }

        //    return rct;
        //}



        public static ObjectColor Difficulty2ObjectColor(Difficulty difficulty) {
            switch (difficulty) {
                case Difficulty.medium:
                case Difficulty.none:
                    return ObjectColor.blue;
                case Difficulty.hard:
                    return ObjectColor.yellow;
                case Difficulty.boss:
                    return ObjectColor.green;
                case Difficulty.boss2:
                    return ObjectColor.orange;
                default:
                    return ObjectColor.white;
            }
        }
        public static Color RGBA(int r, int g, int b, int a) {
            return new Color((float)r / 256.0f, (float)g / 256.0f, (float)b / 256.0f, (float)a / 256.0f);
        }

        public static Color RGB(int r, int g, int b) {
            return RGBA(r, g, b, 256);
        }

        public static Color GetColor(ObjectColor color) {
            switch (color) {
                case ObjectColor.white:
                    return Color.white;
                case ObjectColor.blue:
                    return Color.blue;
                case ObjectColor.green:
                    return Color.green;
                case ObjectColor.yellow:
                    return Color.yellow;
                case ObjectColor.orange:
                    return new Color(1, 0.5f, 0, 1);
                default:
                    return Color.white;
            }
        }




        public static Color RaceColor(Race race) {
            switch (race) {
                case Race.Humans: return new Color(173f / 256f, 216f / 256f, 230f / 256f, 1f);
                case Race.Borguzands: return Color.red;
                case Race.Criptizoids: return Color.green;
                default: return Color.white;
            }
        }

        public static float ClampAngle(float angle, float min, float max) {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;

            //if (angle >= 0 && angle < 180) {
            //    return Mathf.Clamp(angle, 0, max);
            //} else if( angle >= 180 ) {
            //    return Mathf.Clamp(angle, 360 + min, 360);
            //}
            if (angle > 270) {
                angle -= 380;
            }
            return Mathf.Clamp(angle, min, max);
            //return angle; Mathf.Clamp(angle, min, max);
        }

        public static GUISkin LoadDebugSkin() {
            return UnityEngine.Resources.Load<GUISkin>("UI/Skins/game");
        }

        public static GUIStyle LoadDebugLabelStyle() {
            return LoadDebugSkin().GetStyle("font_upper_left");
        }
    }

    public static class Extensions {
        //public static float[] ToArray(this UnityEngine.Vector3 v) { return new float[] { v.x, v.y, v.z }; }

        public static Vector2 mul(this Vector2 orig, float mulx, float muly) {
            Vector2 f = orig;
            f.x *= mulx;
            f.y *= muly;
            return f;
        }

        public static Vector2 mul(this Vector2 orig, Vector2 mul) {
            Vector2 f = orig;
            f.x *= mul.x;
            f.y *= mul.y;
            return f;
        }

        public static Rect addPos(this Rect from, Vector2 to) {
            Rect f = from;
            f.x += to.x;
            f.y += to.y;
            return f;
        }

        public static Rect moveVert(this Rect source, float y) {
            return new Rect(source.x, source.y + y, source.width, source.height);
        }

        public static Rect addPos(this Rect from, Rect to) {
            Rect f = from;
            f.x += to.x;
            f.y += to.y;
            return f;
        }

        public static Rect addPos(this Rect from, float x, float y) {
            Rect f = from;
            f.x += x;
            f.y += y;
            return f;
        }

        public static Rect addPosToCenter(this Rect from, Vector2 to) {
            Rect f = new Rect(from.center.x + to.x, from.center.y + to.y, from.width, from.height);
            return f;

        }

        public static Rect addSize(this Rect from, Vector2 to) {
            Rect f = from;
            f.width += to.x;
            f.height += to.y;
            return f;
        }

        public static Rect addSize(this Rect from, Rect to) {
            Rect f = from;
            f.width += to.width;
            f.height += to.height;
            return f;
        }

        public static Rect addSize(this Rect from, float x, float y) {
            Rect f = from;
            f.width += x;
            f.height += y;
            return f;
        }

        public static string inLocation(this string path) {
            //if (Application.systemLanguage == SystemLanguage.Russian)
            //{
            //    path = "ru/" + path;
            //}
            return "en/" + path;

        }

        public static string Color(this string text, string color) {
            return "<color=" + color + ">" + text + "</color>";
        }

        public static string Color(this string text, Color color) {
            string str = string.Format("<color=#{0}>{1}</color>", color.HexString(), text);
            //Debug.Log("COLOR: " + color.HexString());
            return str;
        }

        public static string Bold(this string text) {
            return string.Format("<b>{0}</b>", text);
        }

        public static string Italics(this string text) {
            return string.Format("<i>{0}</i>", text);
        }

        public static string Size(this string text, int size) {
            return string.Format("<size={0}>{1}</size>", size, text);
        }
        private static string HexString(this Color color) {

            int iR = Mathf.Clamp((int)(color.r * 256), 0, 255);
            int iG = Mathf.Clamp((int)(color.g * 256), 0, 255);
            int iB = Mathf.Clamp((int)(color.b * 256), 0, 255);
            int iA = Mathf.Clamp((int)(color.a * 256), 0, 255);
            return iR.ToString("X2") + iG.ToString("X2") + iB.ToString("X2") + iA.ToString("X2");
        }

        public static Rect addOffset(this Rect source, float x, float y) {
            return new Rect(source.x + x, source.y + y, source.width, source.height);
        }

        public static Rect addOffset(this Rect source, Vector2 offset) {
            return addOffset(source, offset.x, offset.y);
        }

        public static void Print(this Hashtable hash, int level) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Common.CommonUtils.ConstructHashString(hash, level, ref sb);
            string str = sb.ToString();
            Debug.Log(str);
        }

        public static void Print(this string str) {
            Debug.Log(str);
        }

        public static void Print(this object[] array, int level) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Common.CommonUtils.ConstructArrayString(array, level, ref sb);
            string str = sb.ToString();
            Debug.Log(str);
        }

        /// <summary>
        /// Get new rect of new size, which center match with source rect center
        /// </summary>
        public static Rect GetRectCenteredAtThis(this Rect rect, Size size) {
            float cx = rect.center.x;
            float cy = rect.center.y;
            float nx = cx - size.Width * 0.5f;
            float ny = cy - size.Height * 0.5f;
            return new Rect(nx, ny, size.Width, size.Height);
        }

        public static Color SetAlpha(this Color c, float alpha) {
            return new Color(c.r, c.g, c.b, alpha);
        }
        public static string FormatBraces(this string s) {
            return s.Replace("{", "<").Replace("}", ">");
        }

        public static void LayerCullingShow(this Camera cam, int layerMask) {
            cam.cullingMask |= layerMask;
        }

        public static void LayerCullingShow(this Camera cam, string layer) {
            LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));
        }

        public static void LayerCullingHide(this Camera cam, int layerMask) {
            cam.cullingMask &= ~layerMask;
        }

        public static void LayerCullingHide(this Camera cam, string layer) {
            LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));
        }

        public static RectTransform ToRectTransform(this Transform t) {
            return t as RectTransform;
        }


    }
}