using System;
using System.Collections.Generic;
using System.Linq;

namespace LittleConsoleHelper
{
	public static class Menu
	{
		static int left;
		static int top;
		static MenuItem itemSelected;
		static int currentIndex;
		static ConsoleColor resetColor;
		static ConsoleColor resetBgColor;
		static int linesWritten;

		public static MenuItem ShowFlat(MenuShowOptions options, params string[] flatList)
		{
			MenuItem rootNode = new MenuItem("root");
			foreach (var item in flatList)
				new MenuItem(item, rootNode);
			return Show(rootNode, options);
		}
		public static MenuItem Show(MenuItem rootNode, MenuShowOptions options = null)
		{
			if (options == null)
				options = new MenuShowOptions();

			return Show(rootNode, options.ColorScheme, options.AllowEscape, options.AllowInteriorNodeSelect, options.InteriorSuffix, options.InteriorOpenSuffix, options.Indentation, options.ClearOnSelect);
		}
		public static MenuItem Show(MenuItem rootNode, ColorScheme colorScheme = null, bool allowEscape = true, bool allowInteriorNodeSelect = false, string interiorSuffix = " >", string interiorOpenSuffix = " <", string indentation = "\t", bool clearOnSelect = false)
		{
			if (colorScheme == null)
				colorScheme = GetDefaultColorScheme();
			currentIndex = 0;
			itemSelected = rootNode.Children[currentIndex];
			left = Console.CursorLeft;
			top = Console.CursorTop;
			resetColor = Console.ForegroundColor;
			resetBgColor = Console.BackgroundColor;

			bool cont = true;
			while (cont)
			{
				Clear();
				linesWritten = 0;
				PrintChoices(rootNode.Children, colorScheme.Text.Value, colorScheme.SelectedText.Value, colorScheme.Background.Value, colorScheme.SelectedBackground.Value, interiorSuffix, interiorOpenSuffix, indentation);

				var key = Console.ReadKey();
				switch (key.Key)
				{
					case ConsoleKey.UpArrow:
						currentIndex--;
						if (currentIndex < 0)
							currentIndex = itemSelected.Parent.Children.Count - 1;
						itemSelected = itemSelected.Parent.Children[currentIndex];
						break;
					case ConsoleKey.DownArrow:
						currentIndex++;
						if (currentIndex >= itemSelected.Parent.Children.Count)
							currentIndex = 0;
						itemSelected = itemSelected.Parent.Children[currentIndex];
						break;
					case ConsoleKey.LeftArrow:
						if (itemSelected.IsExpanded)
							itemSelected.IsExpanded = false;
						else if (!itemSelected.Parent.IsRoot())
						{
							for (var i = 0; i < itemSelected.Parent.Parent.Children.Count; i++)
							{
								if (itemSelected.Parent == itemSelected.Parent.Parent.Children[i])
								{
									currentIndex = i;
									break;
								}
							}
							itemSelected = itemSelected.Parent;

						}
						break;
					case ConsoleKey.RightArrow:
						if (itemSelected.IsExpanded)
						{
							itemSelected = itemSelected.Children[0];
							currentIndex = 0;
						}
						else
						{
							itemSelected.IsExpanded = true;
						}
						break;
					case ConsoleKey.Enter:
						if (itemSelected.Children.Count == 0)
							cont = false;
						else if (allowInteriorNodeSelect)
							cont = false;
						else
						{
							itemSelected.IsExpanded = true;
							itemSelected = itemSelected.Children[0];
							currentIndex = 0;
						}
						break;
					case ConsoleKey.Escape:
						if (allowEscape)
							return null;
						else
							break;
					default:
						break;
				}
			}
			Console.ForegroundColor = resetColor;
			if (clearOnSelect)
				Clear();

			return itemSelected;
		}

		internal static ColorScheme GetDefaultColorScheme()
		{
			return new ColorScheme
			{
				Text = ConsoleColor.White,
				SelectedText = ConsoleColor.Green,
				Background = ConsoleColor.Black,
				SelectedBackground = ConsoleColor.Black
			};
		}

		private static void PrintChoices(List<MenuItem> choices, ConsoleColor normal, ConsoleColor selected, ConsoleColor normalBg, ConsoleColor selectedBg, string interiorSuffix, string interiorOpenSuffix, string indentation)
		{
			for (var i = 0; i < choices.Count; i++)
			{
				linesWritten++;
				var item = choices[i];
				var suffix = string.Empty;
				if (item.Children.Count > 0)
					suffix = item.IsExpanded ? interiorOpenSuffix : interiorSuffix;
				if (item == itemSelected)
				{
					Console.ForegroundColor = selected;
					Console.BackgroundColor = selectedBg;
					for (var j = 0; j < item.GetLevel(); j++)
						Console.Write(indentation);
					Console.WriteLine(item.Text + suffix);
				}
				else
				{
					Console.ForegroundColor = normal;
					Console.BackgroundColor = normalBg;
					for (var j = 0; j < item.GetLevel(); j++)
						Console.Write(indentation);
					Console.WriteLine(item.Text + suffix);
				}
				if (item.IsExpanded)
				{
					PrintChoices(item.Children, normal, selected, normalBg, selectedBg, interiorSuffix, interiorOpenSuffix, indentation);
				}
			}
			Console.ForegroundColor = resetColor;
			Console.BackgroundColor = resetBgColor;
		}
		private static void Clear()
		{
			Console.SetCursorPosition(left, top);
			for (var i = 0; i < linesWritten; i++)
			{
				for (var j = 0; j < 60; j++)// todo not fixed number here
				{
					Console.Write(" \b ");
				}
				if (i < linesWritten - 1)
					Console.WriteLine();
			}
			Console.SetCursorPosition(left, top);
		}
	}

	public class MenuShowOptions
	{
		public ColorScheme ColorScheme { get; set; }
		public bool AllowEscape { get; set; }
		public bool AllowInteriorNodeSelect { get; set; }
		public string InteriorSuffix { get; set; }
		public string InteriorOpenSuffix { get; set; }
		public string Indentation { get; set; }
		public bool ClearOnSelect { get; set; }

		public MenuShowOptions()
		{
			ColorScheme = Menu.GetDefaultColorScheme();
			AllowEscape = false;
			AllowInteriorNodeSelect = false;
			InteriorSuffix = " >";
			InteriorOpenSuffix = " <";
			Indentation = "  ";
			ClearOnSelect = false;
		}
	}
}
