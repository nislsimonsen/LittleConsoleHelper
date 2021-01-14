﻿using System;
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
		public static MenuShowOptions DefaultMenuShowOptions { get; set; }

		public static MenuItem SelectFromList(IEnumerable<string> listOfItems, string defaultItem = null)
		{
			return SelectFromList(listOfItems, MenuShowOptions.Default);
		}
		public static MenuItem SelectFromList(IEnumerable<MenuItem>  listOfItems, MenuItem defaultItem = null)
		{
			return SelectFromList(listOfItems, MenuShowOptions.Default);
		}

		public static MenuItem SelectFromList(IEnumerable<MenuItem> listOfItems, MenuShowOptions options)
		{
			return SelectFromList(listOfItems, null, options);
		}
		public static MenuItem SelectFromList(IEnumerable<MenuItem> listOfItems, MenuItem defaultItem, MenuShowOptions options)
		{
			return SelectFromList(null, listOfItems, defaultItem, options);
		}
		public static MenuItem SelectFromList(string headerText, IEnumerable<MenuItem> listOfItems, MenuShowOptions options)
		{
			return SelectFromList(headerText, listOfItems, null, options);
		}
		public static MenuItem SelectFromList(string headerText, IEnumerable<MenuItem> listOfItems, MenuItem defaultItem, MenuShowOptions options)
		{
			if (!string.IsNullOrEmpty(headerText))
				WriteLine(headerText, options.ColorScheme.Text);
			MenuItem rootNode = new MenuItem("root");
			foreach (var item in listOfItems)
			{
				rootNode.Children.Add(item);
				item.Parent = rootNode;
			}
			return Select(rootNode, null, defaultItem, options);
		}

		public static MenuItem SelectFromList(IEnumerable<string> listOfItems, MenuShowOptions options)
		{
			return SelectFromList(null, listOfItems, options);
		}
		public static MenuItem SelectFromList(string headerText, IEnumerable<string> listOfItems, MenuShowOptions options)
		{
			if (options == null)
				options = MenuShowOptions.Default;
			if (!string.IsNullOrEmpty(headerText))
				WriteLine(headerText, options.ColorScheme.Text);
			MenuItem rootNode = new MenuItem("root");
			foreach (var item in listOfItems)
				new MenuItem(item, rootNode);
			return Select(rootNode, options);
		}

		public static MenuItem SelectEnumMember<T>(MenuShowOptions options = null)
		{
			return SelectEnumMember<T>(null, default(T), options);
		}
		public static MenuItem SelectEnumMember<T>(string headerText, MenuShowOptions options = null)
		{
			return SelectEnumMember<T>(headerText, default(T), options);
		}
		public static MenuItem SelectEnumMember<T>(string headerText, T selectedItem, MenuShowOptions options = null)
		{
			if (options == null)
				options = MenuShowOptions.Default;
			if (!string.IsNullOrEmpty(headerText))
				WriteLine(headerText, options.ColorScheme.Text);

			var items = ((T[])Enum.GetValues(typeof(T))).ToList().Select(a => new MenuItem(a.ToString(), null, a)).ToArray();
			var menuItemSelected = items.Where(i => ((T)i.Value).Equals(selectedItem)).Single();
			return SelectFromList(items, menuItemSelected, options);
		}
		public static bool SelectBool(string trueText, string falseText, MenuShowOptions options)
		{
			return SelectBool(null, options, trueText, falseText);
		}
		public static bool SelectBool(string headerText = null, MenuShowOptions options = null, string trueText = "Yes", string falseText = "No")
		{
			if (options == null)
				options = MenuShowOptions.Default;
			if (!string.IsNullOrEmpty(headerText))
				WriteLine(headerText, options.ColorScheme.Text);
			return (bool)SelectFromList(new List<MenuItem> { new MenuItem(trueText, null, true), new MenuItem(falseText, null, false) }, options).Value;
		}
		public static MenuItem Select(MenuItem rootNode, MenuShowOptions options = null)
		{
			return Select(rootNode, null, null, options);
		}
		public static MenuItem Select(MenuItem rootNode, string headerText, MenuShowOptions options = null)
		{
			return Select(rootNode, headerText, null, options);
		}
		public static MenuItem Select(MenuItem rootNode, string headerText, MenuItem selectedItem, MenuShowOptions options = null)
		{
			if (options == null)
				options = MenuShowOptions.Default;
			
			return Select(headerText, rootNode, selectedItem, options.ColorScheme, options.AllowEscape, options.AllowInteriorNodeSelect, options.InteriorSuffix, options.InteriorOpenSuffix, options.Indentation, options.ClearOnSelect);
		}
		private static MenuItem Select(string headerText, MenuItem rootNode, MenuItem selectedItem, ColorScheme colorScheme = null, bool allowEscape = true, bool allowInteriorNodeSelect = false, string interiorSuffix = " >", string interiorOpenSuffix = " <", string indentation = "\t", ClearOnSelectMode clearOnSelect = ClearOnSelectMode.ClearUnselected)
		{
			if (colorScheme == null)
				colorScheme = ColorScheme.Empty;
			if (!string.IsNullOrEmpty(headerText))
				WriteLine(headerText, colorScheme.Text);
			if (selectedItem == null)
			{
				currentIndex = 0;
				itemSelected = rootNode.Children[currentIndex];
			}
			else
			{
				currentIndex = 0;
				foreach (var c in selectedItem.Parent.Children)
					if (c == selectedItem)
						break;
					else
						currentIndex++;
				var p = selectedItem.Parent;
				while (p != null && p != rootNode)
				{
					p.IsExpanded = true;
					p = p.Parent;
				}
				itemSelected = selectedItem;
			}
			left = Console.CursorLeft;
			top = Console.CursorTop;
			resetColor = Console.ForegroundColor;
			resetBgColor = Console.BackgroundColor;

			bool cont = true;
			while (cont)
			{
				Clear();
				linesWritten = 0;
				PrintChoices(rootNode.Children, colorScheme.SecondaryText, colorScheme.SelectedText, colorScheme.SecondaryBackground, colorScheme.SelectedBackground, interiorSuffix, interiorOpenSuffix, indentation);

				var key = Console.ReadKey();
				switch (key.Key)
				{
					case ConsoleKey.UpArrow:
						if (itemSelected.Parent.IsExpanded && currentIndex == 0)
						{
							itemSelected = itemSelected.Parent;
						}
						else
						{
							currentIndex--;
							if (currentIndex < 0)
								currentIndex = itemSelected.Parent.Children.Count - 1;
							itemSelected = itemSelected.Parent.Children[currentIndex];
						}
						break;
					case ConsoleKey.DownArrow:
						if (itemSelected.IsExpanded)
						{
							itemSelected = itemSelected.Children.First();
							currentIndex = 0;
						}
						else
						{
							currentIndex++;
							if (currentIndex >= itemSelected.Parent.Children.Count)
								currentIndex = 0;
							itemSelected = itemSelected.Parent.Children[currentIndex];
						}
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
						{
							Console.Write("\b \b");
							break;
						}
					default:
						Console.Write("\b \b");
						break;
				}
			}
			Console.ForegroundColor = resetColor;
			if (clearOnSelect == ClearOnSelectMode.ClearAll)
				Clear();
			else if (clearOnSelect == ClearOnSelectMode.ClearUnselected)
			{
				Clear();
				PrintChoices(new List<MenuItem> { itemSelected }, colorScheme.SecondaryText, colorScheme.SelectedText, colorScheme.SecondaryBackground, colorScheme.SelectedBackground, interiorSuffix, interiorOpenSuffix, indentation);
			}
			return itemSelected;
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
				for (var j = 0; j < 60; j++)// todo shold not use fixed number here
				{
					Console.Write(" \b ");
				}
				if (i < linesWritten - 1)
					Console.WriteLine();
			}
			Console.SetCursorPosition(left, top);
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
