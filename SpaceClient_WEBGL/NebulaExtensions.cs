
namespace Nebula.Client {
    using System.Collections.Generic;
    using System.Text;
    public static class NebulaExtensions {
        public static string ToNewLineSeparatedString<T>(this List<T> source) {
            if (source == null) {
                return string.Empty;
            }
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < source.Count; i++) {
                if (i != source.Count - 1) {
                    stringBuilder.AppendLine(source[i].ToString());
                } else {
                    stringBuilder.Append(source[i].ToString());
                }
            }
            return stringBuilder.ToString();
        }

        public static string ToCharacterSeparatedString<T>(this List<T> source, char ch) {
            if(source == null ) {
                return string.Empty;
            }
            var stringBuilder = new StringBuilder();
            foreach(var str in source ) {
                stringBuilder.Append(str + ch.ToString() + " ");
            }
            return stringBuilder.ToString();
        }
    }
}
