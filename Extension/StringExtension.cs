using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.Storage;
using ExtendedXmlSerializer;

public static class StringExtension
{
    public static string Format(this string text, Dictionary<string, string> replacements, string delimiterStart = "{", string delimitedEnd = "}")
    {
		string output = text;
		foreach (var item in replacements)
		{
			text.Replace($"{delimiterStart}{item.Key}{delimitedEnd}", item.Value);
		}
		return output;
    }

	public static string NewLine(this string text, string line, bool ignoreEmpty = true)
	{
		string output = text;
		if (line == "" && ignoreEmpty) return output;
		else return output + line + "\n";
	}

}
