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
		public static void WriteLine(string text)
		{
			WriteLines(text);
		}
		public static void WriteLines(params string[] text)
		{
			for (var j = 0; j < text.Length; j++)
			{
				Write(text[j], 0);
				Console.ForegroundColor = resetColor;
				IsAlternating = false;
				Console.WriteLine();
			}
			Console.ForegroundColor = resetColor;
		}

		public static void Write(string text, int width = 0, bool skipInitialColor = false)
		{
			if (ColorScheme == null)
				ColorScheme = ColorScheme.Default;

			resetColor = ColorScheme.Text;
			if(!skipInitialColor)
				Console.ForegroundColor = resetColor;

			var isInFormatSpecifier = false;
			var formatString = string.Empty;
			StringBuilder buffer = new StringBuilder();

			for (var i = 0; i < text.Length; i++)
			{
				var c = text[i];
				if (isInFormatSpecifier)
				{
					if (c == '}')
					{
						Console.Write(buffer.ToString());
						buffer.Clear();
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
					if (c == '}' && i != text.Length && text[i + 1] == '}'
						|| c == '{' && i != text.Length && text[i + 1] == '{')
					{
						if (IsAlternating && c != ' ')
						{
							Console.ForegroundColor = AlternationColors[AlternationIndex++ % AlternationColors.Count];
							Console.Write(c);
						}
						else
						{
							//Console.Write(c);
							buffer.Append(c);
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
							//Console.Write(c);
							buffer.Append(c);
						}
					}
				}
			}
			Console.Write(buffer);
			buffer.Clear();
			var numPaddingChars = Math.Max(0, width - GetUnformattedText(text).Length);
			Console.Write(string.Empty.PadRight(numPaddingChars));
			//for (var i = GetUnformattedText(text).Length; i++ < width; Console.Write(' ')) { }
		}
		public static string GetUnformattedText(string text)
		{
			StringBuilder r = new StringBuilder();
			var isInFormatSpecifier = false;
			for (var i = 0; i < text.Length; i++)
			{
				var c = text[i];
				if (isInFormatSpecifier)
				{
					if (c == '}')
					{
						isInFormatSpecifier = false;
					}
				}
				else
				{
					if (c == '{')
					{
						isInFormatSpecifier = true;
					}
					else
						r.Append(c);
				}
			}
			return r.ToString();
		}
		private static List<ConsoleColor> AlternationColors;
		private static int AlternationIndex;
		private static bool IsAlternating;
		private static void HandleFormat(string formatString)
		{
			if (Enum.TryParse<ConsoleColor>(formatString, true, out var literalColor))
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
			else if (formatString.Equals("note", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.Note;
			}
			else if (formatString.Equals("success", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.Success;
			}
			else if (formatString.Equals("warning", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.Warning;
			}
			else if (formatString.Equals("error", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.Error;
			}
			else if (formatString.Equals("input", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.Input;
			}
			else if (formatString.Equals("secondaryinput", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.ForegroundColor = ColorScheme.SecondaryInput;
			}
		}
	}
}
