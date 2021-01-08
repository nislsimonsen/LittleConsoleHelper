using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper
{
	public class SimpleColorScheme
	{
		public ConsoleColor Text { get; set; }
		public ConsoleColor Background { get; set; }

		public static SimpleColorScheme Default = new SimpleColorScheme { Text = Console.ForegroundColor, Background = Console.BackgroundColor };
		
	}
	public class ColorScheme : SimpleColorScheme
	{
		public ConsoleColor SelectedText { get; set; }
		public ConsoleColor SelectedBackground { get; set; }
		public ConsoleColor SecondaryText{ get; set; }
		public ConsoleColor SecondaryBackground { get; set; }

		public static new ColorScheme Default = new ColorScheme { Text = Console.ForegroundColor, SelectedText = Console.ForegroundColor, Background = Console.BackgroundColor, SelectedBackground = Console.BackgroundColor, SecondaryText = Console.ForegroundColor, SecondaryBackground = Console.BackgroundColor };
	}
}
