using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper
{
	public static class Formatter
	{
		static ConsoleColor resetColor;
		public static ColorScheme ColorScheme { get; set; }
		public static void WriteAddLineBreak(params string[] text)
		{
			WriteLines(text);
			Console.WriteLine();
		}
		public static void WriteAndWaitAddLineBreak(params string[] text)
		{
			WriteAndWait(text);
			Console.WriteLine();
		}
		public static void WriteAndWait(params string[] text)
		{
			WriteLines(text);
			Console.ReadLine();
			Console.SetCursorPosition(0, Console.CursorTop - 1);
		}
		public static void WriteLines(params string[] text)
		{
			if (ColorScheme == null)
				ColorScheme = ColorScheme.Default;

			resetColor = ColorScheme.Text;
			Console.ForegroundColor = resetColor;
			var isInFormatSpecifier = false;
			var formatString = string.Empty;
			for (var j = 0; j < text.Length; j++)
			{
				for (var i = 0; i < text[j].Length; i++)
				{
					var c = text[j][i];
					if (isInFormatSpecifier)
					{
						if (c == '}')
						{
							isInFormatSpecifier = false;
							HandleFormat(formatString);
						}
						else
						{
							formatString += c;
						}
					}
					else
					{
						if (c == '}' && i != text[j].Length && text[j][i + 1] == '}'
							|| c == '{' && i != text[j].Length && text[j][i + 1] == '{')
						{
							if (IsAlternating && c != ' ')
							{
								Console.ForegroundColor = AlternationColors[AlternationIndex++ % AlternationColors.Count];
								Console.Write(c);
							}
							else
							{
								Console.Write(c);
							}
							i++;
						}
						else if (c == '{')
						{
							isInFormatSpecifier = true;
							formatString = string.Empty;
							IsAlternating = false;
						}
						else
						{
							if (IsAlternating && c != ' ')
							{
								Console.ForegroundColor = AlternationColors[AlternationIndex++ % AlternationColors.Count];
								Console.Write(c);
							}
							else
							{
								Console.Write(c);
							}
						}
					}
				}
				Console.ForegroundColor = resetColor;
				IsAlternating = false;
				Console.WriteLine();
			}
			Console.ForegroundColor = resetColor;
		}
		private static List<ConsoleColor> AlternationColors;
		private static int AlternationIndex;
		private static bool IsAlternating;
		private static void HandleFormat(string formatString)
		{
			ConsoleColor literalColor;
			if (Enum.TryParse<ConsoleColor>(formatString, true, out literalColor))
			{
				Console.ForegroundColor = literalColor;
			}
			else if (formatString.StartsWith("/")
				|| formatString.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = resetColor;
			}
			else if (formatString.Equals("text", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.Text;
			}
			else if (formatString.Equals("selectedtext", StringComparison.InvariantCultureIgnoreCase)
				|| formatString.Equals("selected", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.SelectedText;
			}
			else if (formatString.Equals("secondarytext", StringComparison.InvariantCultureIgnoreCase)
				|| formatString.Equals("secondary", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.SecondaryText;
			}
			else if (formatString.Equals("header", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.Header;
			}
			else if (formatString.Equals("subheader", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.SubHeader;
			}
			else if (formatString.StartsWith("alternate", StringComparison.InvariantCultureIgnoreCase))
			{
				AlternationColors = new List<ConsoleColor>();
				var colorStrings = formatString.Substring(10, formatString.Length - 11).Split(",");
				foreach (var colorString in colorStrings)
				{
					ConsoleColor color;
					if (Enum.TryParse<ConsoleColor>(colorString, true, out color))
						AlternationColors.Add(color);
					else
						Formatter.WriteAndWaitAddLineBreak("Error: Unknown ConsoleColor '" + colorString + "'");
				}
				AlternationIndex = 0;
				IsAlternating = true;
			}
			else if (Enum.TryParse<ConsoleColor>(formatString, true, out var result))
			{
				Console.ForegroundColor = result;
			}
		}
	}
}
