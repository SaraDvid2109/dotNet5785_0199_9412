using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

internal static class Class1 
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return string.Empty;

        var properties = typeof(T).GetProperties();
        var sb = new StringBuilder();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(t, null);
            if (value is System.Collections.IEnumerable enumerable && !(value is string))
            {
                sb.AppendLine($"{prop.Name}: [");
                foreach (var item in enumerable)
                {
                    sb.AppendLine(item.ToStringProperty());
                }
                sb.AppendLine("]");
            }
            else
            {
                sb.AppendLine($"{prop.Name}: {value}");
            }
        }

        return sb.ToString();
    }
}
