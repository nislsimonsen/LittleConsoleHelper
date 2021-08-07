using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Config
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
		public ConsoleColor Success { get; set; }
		public ConsoleColor Warning { get; set; }
		public ConsoleColor Error { get; set; }
		public ConsoleColor Input { get; set; }
		
		public Dictionary<string, ConsoleColor> Custom { get; set; } = new Dictionary<string, ConsoleColor>();

		public static new ColorScheme Empty = new ColorScheme
		{
			Text = Console.ForegroundColor,
			SelectedText = Console.ForegroundColor,
			Background = Console.BackgroundColor,
			SelectedBackground = Console.BackgroundColor,
			SecondaryText = Console.ForegroundColor,
			SecondaryBackground = Console.BackgroundColor,
			Header = Console.ForegroundColor,
			Success = Console.ForegroundColor,
			Warning = Console.ForegroundColor,
			Error = Console.ForegroundColor,
			Input = Console.ForegroundColor,
		};
		public static new ColorScheme Default = new ColorScheme 
		{ 
			Text = Configuration.ColorText ?? Console.ForegroundColor, 
			SelectedText = Configuration.ColorSelectedText ?? ConsoleColor.White,
			SecondaryText = Configuration.ColorSecondaryText ?? ConsoleColor.DarkGray,
			Background = Configuration.ColorTextBG ?? Console.BackgroundColor, 
			SelectedBackground = Configuration.ColorSelectedTextBG ?? Console.BackgroundColor, 
			SecondaryBackground = Configuration.ColorSecondaryTextBG ?? Console.BackgroundColor,
			Header = Configuration.ColorHeader ?? ConsoleColor.Yellow,
			Success = Configuration.ColorSuccess ?? ConsoleColor.Green,
			Warning = Configuration.ColorWarning ?? ConsoleColor.Yellow,
			Error = Configuration.ColorError ?? ConsoleColor.Red,
			Input = Configuration.ColorInput ?? ConsoleColor.Cyan,
			Custom = Configuration.CustomColors
		};
		public ConsoleColor GetCustomColor(string configKey)
		{
			if (Custom.ContainsKey(configKey))
				return Custom[configKey];
			throw new ArgumentException($"'{configKey}' is not correctly defined as LittleConsoleHelper.config");
		}

	}
}
