#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DA_Assets.Shared
{
    public class DAFormatter
    {
        static List<string> GetJsonPropertyNames<T>()
        {
            List<string> propertyNames = new List<string>();

            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
#if JSONNET_EXISTS
                JsonPropertyAttribute attribute = field.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute != null)
                {
                    propertyNames.Add(attribute.PropertyName ?? field.Name);
                }
#endif
            }

            foreach (var property in properties)
            {
#if JSONNET_EXISTS
                JsonPropertyAttribute attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute != null)
                {
                    propertyNames.Add(attribute.PropertyName ?? property.Name);
                }
#endif
            }

            return propertyNames;
        }

        private const int indentLength = 4;

        private static string Repeat(int n) => new string(' ', n * indentLength);

        public static JFResult Format<T>(string str)
        {
            JFResult jsonFormatResult = new JFResult();

            List<string> typeNames = GetJsonPropertyNames<T>();

            foreach (string typeName in typeNames)
            {
                if (str.Contains($"\"{typeName}\""))
                {
                    jsonFormatResult.MatchTargetType = true;
                    break;
                }
            }

            bool hasOpenBrace = false;
            bool hasCloseBrace = false;

            int indent = 0;
            bool quoted = false;

            StringBuilder sb = new StringBuilder();

            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        if (ch == '{')
                            hasOpenBrace = true;

                        sb.Append(ch);
                        if (quoted == false)
                        {
                            sb.AppendLine();
                            sb.Append(Repeat(++indent));
                        }
                        break;
                    case '}':
                    case ']':
                        if (ch == '}')
                            hasCloseBrace = true;

                        if (quoted == false)
                        {
                            sb.AppendLine();
                            sb.Append(Repeat(--indent));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (escaped == false)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (quoted == false)
                        {
                            sb.AppendLine();
                            sb.Append(Repeat(indent));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (quoted == false)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            jsonFormatResult.Json = sb.ToString();
            jsonFormatResult.IsValid = hasOpenBrace && hasCloseBrace;

            return jsonFormatResult;
        }
    }

    public struct JFResult
    {
        public bool IsValid { get; set; }
        public string Json { get; set; }
        public bool MatchTargetType { get; set; }
    }
}