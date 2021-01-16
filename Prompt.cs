using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper
{
	/// <summary>
	/// Remark: All ColorScheme parameters can be omitted and will use a LittleConfigHelper.config file (if it exists) or bland default colors otherwise
	/// </summary>
	public static class Prompt
	{
		/// <summary>
		/// Prompts the user for a string.
		/// </summary>
		/// <param name="header">Will be written as a message to the user. Empty or null to disable</param>
		/// <param name="colorScheme">Optional</param>
		/// <returns></returns>
		public static string ForString(string header, ColorScheme colorScheme = null)
		{
			if (colorScheme == null)
				colorScheme = ColorScheme.Default;
			var resetColors = InitializeColors(colorScheme);
			Console.WriteLine(header);

			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			var result = Console.ReadLine();

			SetColors(resetColors);

			return result;
		}

		public static string ForString(string header, List<string> autocompleteOptions, ColorScheme colorScheme = null)
		{
			return ForString(header, (s) => autocompleteOptions.Where(a => a.StartsWith(s, StringComparison.InvariantCultureIgnoreCase)).ToList(), colorScheme);
		}
		public static string ForString(string header, Func<string, List<string>> autocompleteFunction, ColorScheme colorScheme = null)
		{
			var r = new StringBuilder(100);

			if (colorScheme == null)
				colorScheme = ColorScheme.Default;
			var resetColors = InitializeColors(colorScheme);
			if (!string.IsNullOrEmpty(header))
				Console.WriteLine(header);

			Console.ForegroundColor = colorScheme.SelectedText;

			var posLeft = 0;
			var posTop = Console.CursorTop;
			List<string> matches = new List<string>();
			var keyPressed = Console.ReadKey();
			int selectedMatch = -1;
			while (keyPressed.Key != ConsoleKey.Enter)
			{
				switch (keyPressed.Key)
				{
					case ConsoleKey.Tab:
						if (selectedMatch > -1)
						{
							r = new StringBuilder(matches[selectedMatch]);
							posLeft = r.Length;
							selectedMatch = -1;
						}
						break;
					case ConsoleKey.Escape:
						selectedMatch = -1;
						ClearAutocompleteInput(posTop, r.Length);
						ClearAutocompleteMatches(matches, posLeft, posTop);
						Console.SetCursorPosition(0, posTop);
						return null;
					case ConsoleKey.RightArrow:
						if (posLeft < r.Length)
							posLeft++;
						break;
					case ConsoleKey.LeftArrow:
						if (posLeft > 0)
							posLeft--;
						break;
					case ConsoleKey.Backspace:
						if (posLeft > 0)
						{
							r.Remove(r.Length - 1, 1);
							Console.Write(" \b");
							posLeft--;
						}
						break;
					case ConsoleKey.Delete:
						if (posLeft < r.Length)
						{
							r.Remove(posLeft, 1);
							ClearAutocompleteInput(posTop, 80);
						}
						break;
					case ConsoleKey.DownArrow:
						selectedMatch++;
						if (selectedMatch >= matches.Count)
							selectedMatch = matches.Count - 1;
						break;
					case ConsoleKey.UpArrow:
						selectedMatch--;
						if (selectedMatch < -1)
							selectedMatch = -1;
						break;
					default:
						r.Insert(posLeft, keyPressed.KeyChar.ToString());
						posLeft++;
						break;
				}

				ClearAutocompleteMatches(matches, posLeft, posTop);
				matches = new List<string>();
				foreach (var ac in autocompleteFunction(r.ToString()))
				{
					if (ac.StartsWith(r.ToString(), StringComparison.InvariantCultureIgnoreCase))
						matches.Add(ac);
				}
				if (selectedMatch > matches.Count)
					selectedMatch = -1;
				if (matches.Any())
				{
					Console.WriteLine();

					for (var i = 0; i < Math.Min(matches.Count, 10); i++)
					{
						Console.ForegroundColor = i == selectedMatch ? colorScheme.Text : colorScheme.SecondaryText;
						Console.WriteLine(matches[i]);
					}
					Console.ForegroundColor = colorScheme.SelectedText;
				}

				ClearAutocompleteInput(posTop, r.Length);
				Console.Write(r.ToString());
				if (matches.Any() && selectedMatch > -1)
				{
					if (selectedMatch >= matches.Count)
					{
						selectedMatch = -1;
					}
					else
					{
						var selected = matches[selectedMatch];
						var postfill = selected.Substring(r.Length, selected.Length - r.Length);
						Console.ForegroundColor = colorScheme.SecondaryText;
						Console.Write(postfill);
						Console.ForegroundColor = colorScheme.SelectedText;
					}
				}

				Console.SetCursorPosition(posLeft, posTop);
				keyPressed = Console.ReadKey();
			}

			if (selectedMatch > -1 && matches.Count > selectedMatch)
			{
				ClearAutocompleteMatches(matches, posLeft, posTop);
				ClearAutocompleteInput(posTop, r.Length);
				return matches[selectedMatch];
			}
			return r.ToString();
		}
		private static void ClearAutocompleteInput(int posTop, int inputLength)
		{
			Console.SetCursorPosition(0, posTop);
			//for (var i = 0; i < inputLength + 1; i++)
				Console.WriteLine("                                                          ");
				
			Console.SetCursorPosition(0, posTop);
		}
		private static void ClearAutocompleteMatches(List<string> matches, int posLeft, int posTop )
		{
			if (matches.Any())
			{
				foreach (var match in matches)
				{
					Console.WriteLine();
					for (var i = 0; i < match.Length; i++)
						Console.Write(" \b ");
				}
				Console.SetCursorPosition(posLeft, posTop);
			}
		}

		/// <summary>
		/// Prompts the user for a string.
		/// IMPORTANT: NOT using SecureString, just masking what the user enters
		/// </summary>
		/// <param name="header">Will be written as a message to the user. Empty or null to disable</param>
		/// <param name="colorScheme"></param>
		/// <returns></returns>
		public static string ForStringMasked(string header, char? maskChar = '*', ColorScheme colorScheme = null)
		{
			if (colorScheme == null)
				colorScheme = ColorScheme.Default;
			var resetColors = InitializeColors(colorScheme);
			if (!string.IsNullOrEmpty(header))
				Console.WriteLine(header);

			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;
			StringBuilder s = new StringBuilder();

			var result = string.Empty;
			var posLeft = 0;
			var posTop = Console.CursorTop;
			var leftPart = string.Empty;
			var rightPart = string.Empty;
			var keyPressed = Console.ReadKey();
			while (keyPressed.Key != ConsoleKey.Enter && keyPressed.Key != ConsoleKey.Escape)
			{
				switch (keyPressed.Key)
				{
					case ConsoleKey.LeftArrow:
						//if (posLeft > 0)
						//	posLeft--;
						break;
					case ConsoleKey.RightArrow:
						//if (posLeft < result.Length)
						//	posLeft++;
						break;
					case ConsoleKey.Delete:
						break;
					case ConsoleKey.Backspace:
						break;
					case ConsoleKey.Home:
						break;
					case ConsoleKey.End:
						break;

					default:
						Console.SetCursorPosition(0, posTop);
						for (var i = 0; i < result.Length + 1; i++)
							Console.Write(" \b ");
						Console.SetCursorPosition(0, posTop);
						if (maskChar.HasValue)
						{
							for (var i = 0; i < result.Length + 1; i++)
								Console.Write(maskChar);
							posLeft++;
						}
						leftPart += keyPressed.KeyChar.ToString();

						break;
				}
				Console.SetCursorPosition(posLeft, posTop);

				result = leftPart + rightPart;
				keyPressed = Console.ReadKey();
			}

			SetColors(resetColors);
			return result;
		}

		/// <summary>
		/// Prompts the user for a string.
		/// </summary>
		/// <param name="header">Will be written as a message to the user. Empty or null to disable</param>
		/// <param name="prefix">Will be prepended to the string the user enters. Cannot be overwritten by the user</param>
		/// <param name="colorScheme">Optional</param>
		/// <returns></returns>
		public static string ForString(string header, string prefix, ColorScheme colorScheme = null)
		{
			if (colorScheme == null)
				colorScheme = ColorScheme.Default;
			var resetColors = InitializeColors(colorScheme);
			Console.WriteLine(header);

			Console.ForegroundColor = colorScheme.SecondaryText;
			Console.BackgroundColor = colorScheme.SecondaryBackground;
			Console.Write(prefix);

			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			var result = Console.ReadLine();

			SetColors(resetColors);

			return result;
		}
		/// <summary>
		/// Prompts the user for a string.
		/// </summary>
		/// <param name="header">Will be written as a message to the user. Empty or null to disable</param>
		/// <param name="postfix">Will be appended to the string the user enters. Cannot be overwritten by the user</param>
		/// <param name="defaultValue">Can be overwritten. Optional, pass empty or null to disable</param>
		/// <param name="colorScheme">Optional</param>
		/// <returns></returns>
		public static string ForString(string header, string postfix, string defaultValue, ColorScheme colorScheme = null)
		{
			if (postfix == null)
				postfix = string.Empty;
			if (colorScheme == null)
				colorScheme = ColorScheme.Default;
			var resetColors = InitializeColors(colorScheme);

			if (!string.IsNullOrEmpty(header))
				Console.WriteLine(header);

			string input = string.Empty;
			if (!string.IsNullOrEmpty(defaultValue))
			{
				input = defaultValue;
				Console.ForegroundColor = colorScheme.SelectedText;
				Console.BackgroundColor = colorScheme.SelectedBackground;
				Console.Write(defaultValue);
			}
			Console.ForegroundColor = colorScheme.SecondaryText;
			Console.BackgroundColor = colorScheme.SecondaryBackground;
			Console.Write(postfix);
			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			var left = input.Length;
			var currentLength = postfix.Length;
			var part1 = string.Empty;
			var part2 = string.Empty;

			Console.SetCursorPosition(left, Console.CursorTop);

			var keyPressed = Console.ReadKey();
			while (keyPressed.Key != ConsoleKey.Enter && keyPressed.Key != ConsoleKey.Escape)
			{
				switch (keyPressed.Key)
				{
					case ConsoleKey.Backspace:
						if (left <= 0)
							break;
						part1 = input.Substring(0, left - 1);
						part2 = input.Length > left ? input.Substring(left, input.Length - left) : string.Empty;
						input = part1 + part2;
						left--;
						break;
					case ConsoleKey.Delete:
						if (left >= input.Length)
							break;
						part1 = input.Substring(0, left);
						part2 = input.Substring(left + 1, input.Length - left - 1);
						input = part1 + part2;
						break;
					case ConsoleKey.LeftArrow:
						if (left > 0)
							left--;
						break;
					case ConsoleKey.RightArrow:
						if (left < input.Length)
							left++;
						break;
					case ConsoleKey.UpArrow:
					case ConsoleKey.DownArrow:
						break;
					case ConsoleKey.Home:
						left = 0;
						break;
					case ConsoleKey.End:
						left = input.Length;
						break;
					default:
						//if (char.IsLetterOrDigit(keyPressed.KeyChar))
						//{
						part1 = input.Substring(0, left);
						part2 = input.Length > left ? input.Substring(left, input.Length - left) : string.Empty;
						input = part1 + keyPressed.KeyChar.ToString() + part2;
						left++;
						//}
						break;
				}
				Console.SetCursorPosition(0, Console.CursorTop);
				for (var i = 0; i < currentLength + 1; i++)
					Console.Write(" \b ");
				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write(input);

				Console.ForegroundColor = colorScheme.SecondaryText;
				Console.BackgroundColor = colorScheme.SecondaryBackground;
				Console.Write(postfix);
				Console.ForegroundColor = colorScheme.SelectedText;
				Console.BackgroundColor = colorScheme.SelectedBackground;

				currentLength = input.Length + postfix.Length;
				Console.SetCursorPosition(left, Console.CursorTop);
				keyPressed = Console.ReadKey();
			}
			SetColors(resetColors);
			Console.WriteLine();
			if (keyPressed.Key == ConsoleKey.Escape)
				return null;
			return input;
		}

		/// <summary>
		/// Prompts the user for an integer value. 
		/// Only numbers are allowed
		/// </summary>
		/// <param name="header">Optional</param>
		/// <param name="colorScheme">Optional</param>
		/// <returns>Null if the user ESCapes, otherwise the integer entered</returns>
		public static int? ForInt(string header = null, ColorScheme colorScheme = null)
		{
			if (colorScheme == null)
				colorScheme = ColorScheme.Default;
			var resetColors = InitializeColors(colorScheme);

			if (!string.IsNullOrEmpty(header))
				Console.WriteLine(header);

			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			string input = string.Empty;

			var keyPressed = Console.ReadKey();
			while (keyPressed.Key != ConsoleKey.Enter && keyPressed.Key != ConsoleKey.Escape)
			{
				if (char.IsDigit(keyPressed.KeyChar))
				{
					input += keyPressed.KeyChar.ToString();
				}
				else if (keyPressed.Key == ConsoleKey.Backspace)
				{
					input = input.Substring(0, input.Length - 1);
					Console.Write(" \b");
				}
				else
					Console.Write("\b \b");

				keyPressed = Console.ReadKey();
			}

			SetColors(resetColors);
			Console.WriteLine();
			if (keyPressed.Key == ConsoleKey.Escape)
				return null;

			int result;
			if (int.TryParse(input, out result))
				return result;
			return null;
		}

		private static ColorScheme InitializeColors(ColorScheme colorScheme)
		{
			var r = new ColorScheme { Text = Console.ForegroundColor, Background = Console.BackgroundColor };
			SetColors(colorScheme);
			return r;
		}
		private static void SetColors(ColorScheme colorScheme)
		{
			Console.ForegroundColor = colorScheme.Text;
			Console.BackgroundColor = colorScheme.Background;
		}
	}
}
