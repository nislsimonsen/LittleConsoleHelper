using System;

namespace LittleConsoleHelper
{
	public static class Prompt
	{
		public static string ForString(string question, ColorScheme colorScheme)
		{
			var resetColors = InitializeColors(colorScheme);
			Console.WriteLine(question);

			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			var result = Console.ReadLine();

			SetColors(resetColors);

			return result;
		}

		private static string ForString(string question, string prefix, string postfix, ColorScheme colorScheme)
		{
			// postponed, so private..
			var resetColors = InitializeColors(colorScheme);
			Console.WriteLine(question);

			string input = string.Empty;

			Console.ForegroundColor = colorScheme.SecondaryText;
			Console.BackgroundColor = colorScheme.SecondaryBackground;
			Console.Write(prefix + postfix);
			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			var left = prefix.Length;
			var currentLength = postfix.Length;
			string part1 = "";
			string part2 = "";
			Console.SetCursorPosition(left, Console.CursorTop);

			var keyPressed = Console.ReadKey();
			while (keyPressed.Key != ConsoleKey.Enter && keyPressed.Key != ConsoleKey.Escape)
			{
				switch (keyPressed.Key)
				{
					case ConsoleKey.Backspace:
						if (left <= prefix.Length)
							break;
						part1 = input.Substring(0, left - 1-prefix.Length);
						part2 = input.Length > left ? input.Substring(left, input.Length - left) : string.Empty;
						input = part1 + part2;
						left--;
						break;
					case ConsoleKey.LeftArrow:
						if (left > prefix.Length)
							left--;
						break;
					case ConsoleKey.RightArrow:
						if (left < input.Length)
							left++;
						break;
					default:
						//if (char.IsLetterOrDigit(keyPressed.KeyChar))
						//{
						part1 = input.Substring(0, left-prefix.Length);
						part2 = input.Length > left-prefix.Length ? input.Substring(left-prefix.Length, input.Length - left) : string.Empty;
						input = part1 + keyPressed.KeyChar.ToString() + part2;
						left++;
						//}
						break;
				}
				Console.SetCursorPosition(0, Console.CursorTop);
				for (var i = 0; i < currentLength; i++)
					Console.Write(" \b ");
				Console.SetCursorPosition(0, Console.CursorTop);
				
				Console.ForegroundColor = colorScheme.SecondaryText;
				Console.BackgroundColor = colorScheme.SecondaryBackground;
				Console.Write(prefix);
				Console.ForegroundColor = colorScheme.SelectedText;
				Console.BackgroundColor = colorScheme.SelectedBackground;

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
			return input;
		}

		public static string ForStringWithPrefix(string question, string prefix, ColorScheme colorScheme)
		{
			var resetColors = InitializeColors(colorScheme);
			Console.WriteLine(question);

			Console.ForegroundColor = colorScheme.SecondaryText;
			Console.BackgroundColor = colorScheme.SecondaryBackground;
			Console.Write(prefix);

			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			var result = Console.ReadLine();

			SetColors(resetColors);

			return result;
		}

		public static string ForStringWithPostfix(string question, string postfix, ColorScheme colorScheme)
		{
			var resetColors = InitializeColors(colorScheme);
			Console.WriteLine(question);

			string input = string.Empty;

			Console.ForegroundColor = colorScheme.SecondaryText;
			Console.BackgroundColor = colorScheme.SecondaryBackground;
			Console.Write(postfix);
			Console.ForegroundColor = colorScheme.SelectedText;
			Console.BackgroundColor = colorScheme.SelectedBackground;

			var left = 0;
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
				for (var i = 0; i < currentLength; i++)
					Console.Write(" \b ");
				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write(input);

				Console.ForegroundColor = colorScheme.SecondaryText;
				Console.BackgroundColor = colorScheme.SecondaryBackground;
				Console.Write(postfix);
				Console.ForegroundColor = colorScheme.SelectedText;
				Console.BackgroundColor= colorScheme.SelectedBackground;

				currentLength = input.Length + postfix.Length;
				Console.SetCursorPosition(left, Console.CursorTop);
				keyPressed = Console.ReadKey();
			}
			SetColors(resetColors);
			Console.WriteLine();
			return input;
		}

		public static int? ForInt(string question, ColorScheme colorScheme)
		{
			var resetColors = InitializeColors(colorScheme);

			Console.WriteLine(question);

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
