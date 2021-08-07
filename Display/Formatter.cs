using LittleConsoleHelper.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Display
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
		public static void WriteLine()
		{
			WriteLines(string.Empty);
		}
		public static void WriteLine(string text)
		{
			WriteLines(text);
		}
		public static void WriteLine(bool ensureNoBrokenWords, string text)
		{
			WriteLines(ensureNoBrokenWords, text);
		}
		public static void WriteLines(params string[] text)
		{
			WriteLines(true, text);
		}
		public static void WriteLines(bool ensureNoBrokenWords, params string[] text)
		{
			for (var i = 0; i < text.Length; i++)
			{
				if (text[i] == null)
					text[i] = string.Empty;

				Write(text[i], 0, false, ensureNoBrokenWords);
				Console.ForegroundColor = resetColor;
				Console.WriteLine();
			}
			Console.ForegroundColor = resetColor;
		}


		public static void Write(string text, int width = 0, bool skipInitialColor = false, bool ensureNonBrokenWords = true)
		{
			if (text == null)
				text = string.Empty;
			if (ColorScheme == null)
				ColorScheme = ColorScheme.Default;

			resetColor = ColorScheme.Text;
			if (!skipInitialColor)
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
						if (ensureNonBrokenWords)
							WriteToConsoleNonBroken(buffer.ToString());
						else
							Console.Write(buffer);
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
						buffer.Append(c);
						i++;
					}
					else if (c == '{')
					{
						isInFormatSpecifier = true;
						formatString = string.Empty;

					}
					else
					{
						buffer.Append(c);
					}
				}
			}
			if (ensureNonBrokenWords)
				WriteToConsoleNonBroken(buffer.ToString());
			else
				Console.Write(buffer);
			buffer.Clear();
			var numPaddingChars = Math.Max(0, width - GetUnformattedText(text).Length);
			Console.Write(string.Empty.PadRight(numPaddingChars));
		}
		private static void WriteToConsoleNonBroken(string text)
		{
			var remainingCharsOnThisLine = Console.WindowWidth - Console.CursorLeft;
			if (text.Length > remainingCharsOnThisLine)
			{
				var index = text[0..remainingCharsOnThisLine].LastIndexOf(' ');
				if (index > 0)
				{
					Console.Write(text[0..index]);
					Console.WriteLine();
					WriteToConsoleNonBroken(text[(index + 1)..^0]);
				}
				else
				{
					Console.WriteLine();
					WriteToConsoleNonBroken(text);
				}
			}
			else
				Console.Write(text);
		}
		public static string GetUnformattedText(string text)
		{
			if (text == null)
				return string.Empty;
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
			// Yes, this is terribad. Should be refactored, but has to be done alongside major overhaul to Configuration and ColorScheme classes.
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
			else
			{
				foreach (var c in ColorScheme.Custom)
					if (formatString.Equals(c.Key, StringComparison.InvariantCultureIgnoreCase))
						Console.ForegroundColor = c.Value;
			}
		}
	}
}
