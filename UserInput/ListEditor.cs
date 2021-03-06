using LittleConsoleHelper.Config;
using LittleConsoleHelper.UserInput.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.UserInput
{
	public static class ListEditor
	{
		static int PosLeft;
		static int PosTop;
		static List<string> WrittenLines = new List<string>();
		/// <summary>
		/// Allows the user to manipulate a list
		/// </summary>
		/// <param name="items">The initial list of items. Will not be changed by this method.</param>
		/// <param name="header">Optional text to display</param>
		/// <param name="defaultItem">If present, must equal an item from items</param>
		/// <param name="promptForFirstItemIfEmpty">If this is true and items is empty, one item will automatically be inserted and the user will prompted to name it.</param>
		/// <param name="colorScheme">optional</param>
		/// <param name="helpText">Optional. Will be displayed below the list editor.</param>
		/// <param name="clearPreemptivelyToPreventBufferOverrunBug">Optional. In case of a visual buffer overrun, the method will clear the console prior to printing the items. Note that this is only to cover for a bug and will be fixed properly in a later release.</param>
		/// <returns>If the user exits by pressing Enter: A new list containing the accepted items. If the user exits by pressing Escape: A copy of items</returns>
		public static List<string> Edit(List<string> items, string header = null, string defaultItem = null, bool promptForFirstItemIfEmpty = true, ColorScheme colorScheme = null, string helpText = "Use: (shift+)Up/Down / Insert/Delete / Enter/Esc", bool clearPreemptivelyToPreventBufferOverrunBug = true)
		{
			var menuItems = items.Select(i => new MenuItem(i, null)).ToList();
			if (string.IsNullOrEmpty(defaultItem) && items.Count() > 0)
				defaultItem = items[0];
			MenuItem menuDefaultItem = null;
			if(menuItems.Any())
				menuDefaultItem = menuItems.First(i => i.Text.Equals(defaultItem));
			var result = EditList(header, menuItems, menuDefaultItem, colorScheme, helpText, promptForFirstItemIfEmpty, clearPreemptivelyToPreventBufferOverrunBug);
			if (result == null)
				return null;
			return result.Select(mi => mi.Text).ToList();
		}
		
		private static List<MenuItem> EditList(string header, List<MenuItem> items, MenuItem defaultItem, ColorScheme colorScheme, string helpText, bool promptForFirstItemIfEmpty, bool clearPreemptivelyToPreventBufferOverrunBug)
		{
			// HACK: This is a poor solution for a problem which only occurs once the consoles outputbuffer size (300 for cmd, a couple of thousand for ps) is overrun. Fix similar to that done for Menu and TempMsg
			var realEstate = Console.BufferHeight - Console.CursorTop;
			if (clearPreemptivelyToPreventBufferOverrunBug && realEstate < 20)
				Console.Clear();

			int selectedIndex;
			if (defaultItem == null || items.Count == 0)
				selectedIndex = 0;
			else
				selectedIndex = items.IndexOf(defaultItem);
			if (colorScheme == null)
				colorScheme = ColorScheme.Default;

			var workingCopy = new List<MenuItem>();
			foreach (var item in items)
				workingCopy.Add(item.Clone());

			if (!string.IsNullOrEmpty(header))
				WriteLine(header, colorScheme.Text);

			PosLeft = Console.CursorLeft;
			PosTop = Console.CursorTop;

			if (!items.Any() && promptForFirstItemIfEmpty)
				AddNewAndGetName(colorScheme, 0, workingCopy); 
			var key = DisplayAndGetCommand(workingCopy, ref selectedIndex, colorScheme, helpText);
			while (key != ConsoleKey.Escape && key != ConsoleKey.Enter)
			{
				switch (key)
				{
					case ConsoleKey.Delete:
						if (selectedIndex >= workingCopy.Count)
							break;
						workingCopy.RemoveAt(selectedIndex);
						if (selectedIndex > 0)
							selectedIndex--;
						break;
					case ConsoleKey.Insert:
						if (workingCopy.Any())
							selectedIndex++;
						AddNewAndGetName(colorScheme, selectedIndex, workingCopy);
						break;
					case ConsoleKey.UpArrow:
						if (selectedIndex > 0)
						{
							selectedIndex--;
							var temp = workingCopy[selectedIndex];
							workingCopy.RemoveAt(selectedIndex);
							workingCopy.Insert(selectedIndex + 1, temp);
						}
						break;
					case ConsoleKey.DownArrow:
						if (selectedIndex < workingCopy.Count - 1)
						{
							var temp = workingCopy[selectedIndex];
							workingCopy.RemoveAt(selectedIndex);
							selectedIndex++;
							workingCopy.Insert(selectedIndex, temp);
						}
						break;
					default:
						break;
				}

				key = DisplayAndGetCommand(workingCopy, ref selectedIndex, colorScheme, helpText);
			}

			if (key == ConsoleKey.Escape)
			{
				Console.Write("\b \b");
				ClearItems();
				return items;
			}
			if (key == ConsoleKey.Enter)
			{
				ClearItems();
				return workingCopy;
			}

			throw new Exception("The world is ending");
		}

		private static void AddNewAndGetName(ColorScheme colorScheme, int selectedIndex, List<MenuItem> workingCopy)
		{
			var placeholderText = "New item: ";
			var placeholder = new MenuItem(placeholderText, null);
			workingCopy.Insert(selectedIndex, placeholder);
			ClearItems();
			WriteItems(workingCopy, selectedIndex, colorScheme);
			Console.SetCursorPosition(placeholderText.Length, PosTop + selectedIndex);
			var resetColor = Console.ForegroundColor;
			Console.ForegroundColor = colorScheme.SelectedText;
			var name = Console.ReadLine();
			Console.ForegroundColor = resetColor;
			if (!string.IsNullOrEmpty(name))
				workingCopy.Insert(selectedIndex, new MenuItem(name, null));
			workingCopy.Remove(placeholder);
		}

		private static ConsoleKey DisplayAndGetCommand(List<MenuItem> items, ref int selectedIndex, ColorScheme colorScheme, string helpText)
		{
			while (true)
			{
				ClearItems();
				WriteItems(items, selectedIndex, colorScheme);
				if (!string.IsNullOrEmpty(helpText))
				{
					Console.WriteLine();
					WriteLine(helpText, colorScheme.SecondaryText);
					WrittenLines.Add(string.Empty);
					WrittenLines.Add(helpText);
				}
				var key = Console.ReadKey();
				if ((key.Modifiers & ConsoleModifiers.Shift) != 0)
					return key.Key;
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
					default:
						return key.Key;
				}
			}
		}

		private static void WriteItems(List<MenuItem> items, int selectedIndex, ColorScheme colorScheme)
		{
			WrittenLines = new List<string>();
			for (var i = 0; i < items.Count; i++)
			{
				var item = items[i];
				if (i == selectedIndex)
				{
					WriteLine(item.Text, colorScheme.SelectedText);
				}
				else
				{
					WriteLine(item.Text, colorScheme.Text);
				}
				WrittenLines.Add(item.Text);
			}
		}

		private static void ClearItems()
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

		private static void WriteLine(string text, ConsoleColor color)
		{
			var resetColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ForegroundColor = resetColor;
		}
	}
}
