using LittleConsoleHelper.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.UserInput
{
	public static class MultiSelectList
	{
		public static List<Option> Select(string header, List<Option> items, string helpText = "Press up/down-key to navigate, spacebar to select/deselect and enter to accept", ColorScheme colorScheme = null)
		{
			if (colorScheme == null)
				colorScheme = ColorScheme.Default;
			PosLeft = Console.CursorLeft;
			PosTop = Console.CursorTop;
			int selectedIndex = 0;
			while (true)
			{
				Clear();
				WriteItems(items, selectedIndex, helpText, colorScheme);
				var key = Console.ReadKey();
				switch (key.Key)
				{
					case ConsoleKey.UpArrow:

						if (selectedIndex > 0)
							selectedIndex--;
						else
							selectedIndex = items.Count - 1;
						break;
					case ConsoleKey.DownArrow:
						if (selectedIndex < items.Count - 1)
							selectedIndex++;
						else
							selectedIndex = 0;
						break;
					case ConsoleKey.Spacebar:
						items[selectedIndex].Selected = !items[selectedIndex].Selected;
						break;
					case ConsoleKey.Enter:
						Clear();
						return items;
					default:
						break;
				}
			}
		}

		private static int PosLeft;
		private static int PosTop;
		private static List<string> WrittenLines = new List<string>();
		private static void WriteItems(List<Option> items, int selectedIndex, string helpText, ColorScheme colorScheme)
		{
			WrittenLines = new List<string>();
			for (var i = 0; i < items.Count; i++)
			{
				var item = items[i];

				if (i == selectedIndex)
				{
					if (item.Selected)
					{
						WriteLine(" + " + item.Text, colorScheme.SelectedText, colorScheme.SelectedBackground);
					}
					else
					{
						WriteLine("   " + item.Text, colorScheme.Text, colorScheme.SelectedBackground);
					}
				}
				else
				{
					if (item.Selected)
					{
						WriteLine(" + " + item.Text, colorScheme.SelectedText, colorScheme.Background);
					}
					else
					{
						WriteLine("   " + item.Text, colorScheme.SecondaryText, colorScheme.Background);
					}
				}
				WrittenLines.Add(item.Text);
			}
			if (!string.IsNullOrEmpty(helpText))
			{
				WriteLine(helpText, colorScheme.Text, colorScheme.Background);
				WrittenLines.Add(helpText);
			}
		}
		private static void WriteLine(string text, ConsoleColor color, ConsoleColor backgroundColor)
		{
			var resetColor = Console.ForegroundColor;
			var resetBgColor = Console.BackgroundColor;
			Console.ForegroundColor = color;
			Console.BackgroundColor = backgroundColor;
			Console.WriteLine(text);
			Console.ForegroundColor = resetColor;
			Console.BackgroundColor = resetBgColor;
		}

		private static void Clear()
		{
			Console.SetCursorPosition(PosLeft, PosTop);
			foreach (var line in WrittenLines)
			{
				for (var i = 0; i < Console.WindowWidth - 2; i++)
					Console.Write(" \b ");
				Console.WriteLine();
			}
			Console.SetCursorPosition(PosLeft, PosTop);
		}

		public class Option
		{
			public string Text { get; set; }
			public bool Selected { get; set; }
			public object Value { get; set; }
		}
	}
}
