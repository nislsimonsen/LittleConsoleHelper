using System;
using System.Collections.Generic;
using System.Linq;

namespace LittleConsoleHelper
{
	public static class ChoiceSelector
	{
		static int left;
		static int top;
		static int currentChoice = 0;
		static bool first;
		static ConsoleColor resetColor;
		static ConsoleColor resetBgColor;
		public static bool ClearOptionsOnSelection { get; set; }

		static ChoiceSelector()
		{
			ClearOptionsOnSelection = true;
		}

		public static Choice Choose(params string[] choices)
		{
			return Choose(choices.Select(c => new Choice(c)).ToList());
		}
		public static Choice Choose(List<Choice> choices, bool allowEscape, ConsoleColor regularColor, ConsoleColor highlightColor)
		{
			return Choose(choices, allowEscape, regularColor, highlightColor, Console.BackgroundColor, Console.BackgroundColor);
		}
		public static Choice Choose(List<Choice> choices, bool allowEscape = true, ConsoleColor regularColor = ConsoleColor.DarkGreen, ConsoleColor highlightColor = ConsoleColor.Green, ConsoleColor regularBgColor = ConsoleColor.Black, ConsoleColor highlightBgColor = ConsoleColor.Black)
		{
			currentChoice = 0;
			first = true;
			left = Console.CursorLeft;
			top = Console.CursorTop;
			resetColor = Console.ForegroundColor;
			resetBgColor = Console.BackgroundColor;

			bool cont = true;
			while (cont)
			{
				PrintChoices(choices, onlySelected: false, regularColor, highlightColor, regularBgColor, highlightBgColor);

				var key = Console.ReadKey();
				switch (key.Key)
				{
					case ConsoleKey.UpArrow:
						currentChoice--;
						if (currentChoice < 0)
							currentChoice = choices.Count - 1;
						break;
					case ConsoleKey.DownArrow:
						currentChoice++;
						if (currentChoice >= choices.Count)
							currentChoice = 0;
						break;
					case ConsoleKey.Enter:
						cont = false;
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
			if (ClearOptionsOnSelection)
			{
				Clear(choices);
				PrintChoices(choices, onlySelected: true, regularColor, highlightColor, regularBgColor, highlightBgColor);
			}
			return choices[currentChoice];
		}

		private static void PrintChoices(List<Choice> choices, bool onlySelected, ConsoleColor normal, ConsoleColor selected, ConsoleColor normalBg, ConsoleColor selectedBg)
		{
			if (!first)
				Clear(choices);
			else
				first = false;

			for (var i = 0; i < choices.Count; i++)
			{
				if (i == currentChoice)
				{
					Console.ForegroundColor = selected;
					Console.BackgroundColor = selectedBg;
					Console.WriteLine(choices[i].Text);
				}
				else if(!onlySelected)
				{
					Console.ForegroundColor = normal;
					Console.BackgroundColor = normalBg;
					Console.WriteLine(choices[i].Text);
				}
				
			}
			Console.ForegroundColor = resetColor;
			Console.BackgroundColor = resetBgColor;
		}
		private static void Clear(List<Choice> choices)
		{
			Console.SetCursorPosition(left, top);
			for (var i = 0; i < choices.Count; i++)
			{
				for (var j = 0; j < choices[i].Text.Length; j++)
					Console.Write(" \b ");
				if (i < choices.Count - 1)
					Console.WriteLine();
			}
			Console.SetCursorPosition(left, top);
		}
	}
}
