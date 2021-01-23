using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper
{
	public class SimpleColorScheme
	{
		public ConsoleColor Text { get; set; }
		public ConsoleColor Background { get; set; }

		public static SimpleColorScheme Empty = new SimpleColorScheme { Text = Console.ForegroundColor, Background = Console.BackgroundColor };
		public static SimpleColorScheme Default = new SimpleColorScheme { Text = Configuration.ColorText ?? ConsoleColor.White, Background = Configuration.ColorTextBG ?? Console.BackgroundColor };

	}
	public class ColorScheme : SimpleColorScheme
	{
		public ConsoleColor SelectedText { get; set; }
		public ConsoleColor SelectedBackground { get; set; }
		public ConsoleColor SecondaryText{ get; set; }
		public ConsoleColor SecondaryBackground { get; set; }
		public ConsoleColor Header { get; set; }
		public ConsoleColor SubHeader { get; set; }

		public static new ColorScheme Empty = new ColorScheme
		{
			Text = Console.ForegroundColor,
			SelectedText = Console.ForegroundColor,
			Background = Console.BackgroundColor,
			SelectedBackground = Console.BackgroundColor,
			SecondaryText = Console.ForegroundColor,
			SecondaryBackground = Console.BackgroundColor,
			Header = Console.ForegroundColor,
			SubHeader = Console.ForegroundColor
		};
		public static new ColorScheme Default = new ColorScheme 
		{ 
			Text = Configuration.ColorText ?? Console.ForegroundColor, 
			SelectedText = Configuration.ColorSelectedText ?? ConsoleColor.White,
			SecondaryText = Configuration.ColorSecondaryText ?? Console.ForegroundColor,
			Background = Configuration.ColorTextBG ?? Console.BackgroundColor, 
			SelectedBackground = Configuration.ColorSelectedTextBG ?? Console.BackgroundColor, 
			SecondaryBackground = Configuration.ColorSecondaryTextBG ?? Console.BackgroundColor,
			Header = Configuration.ColorHeader ?? ConsoleColor.Yellow,
			SubHeader = Configuration.ColorSubHeader ?? ConsoleColor.Yellow
		};
	}
}
