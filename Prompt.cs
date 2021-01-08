using System;

namespace LittleConsoleHelper
{
	public static class Prompt
	{
		public static string ForString(string question, ColorScheme colorScheme)
		{
			var resetColors = InitializeColors(colorScheme);

			Console.WriteLine(question);

			Console.ForegroundColor = colorScheme.SecondaryText;
			Console.ForegroundColor = colorScheme.SecondaryBackground;

			var result = Console.ReadLine();

			SetColors(resetColors);
			
			return result;
		}

		public static int? ForInt(string question, ColorScheme colorScheme)
		{
			var resetColors = InitializeColors(colorScheme);
			Console.WriteLine(question);

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
