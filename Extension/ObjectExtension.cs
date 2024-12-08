using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.Shared.Storage;
using ExtendedXmlSerializer;

public static class ObjectExtension
{
    public static List<string> GetFieldNames<T>(this T @this, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
    where T : notnull
    {
        List<string> output = new();
        var fields = typeof(T).GetFields(flags);
        foreach (var item in fields)
        {
            output.Add(item.Name);
        }
        return output;
    }

    public static Dictionary<string, string> GetFieldValuesAsDict<T>(this T @this, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
    where T : notnull
    {
        Dictionary<string, string> output = new();
        var fields = typeof(T).GetFields(flags);
        List<string> names = @this.GetFieldNames(flags);
        foreach (var field in fields)
        {
            Type type = field.FieldType;
            object? value = field.GetValue(@this);
            if (value is not null)
            {
                output[field.Name] = value.ToString();
            }
        }
        return output;
    }
}
