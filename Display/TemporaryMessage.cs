using LittleConsoleHelper.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LittleConsoleHelper.Display
{
	public static class TemporaryMessage
	{
		private static List<string> CurrentMessages { get; set; } = new List<string>();
		private static int Top { get; set; }
		private static int Left { get; set; }
		private static bool IsInUpdatable { get; set; }

		/// <summary>
		/// Starts a new TemporaryMessage section. 
		/// Similar to calling WriteLine(message, refresh = false, startNew = true);
		/// </summary>
		/// <param name="message"></param>
		/// <param name="colorScheme"></param>
		/// <param name="fasterButNoFormat"></param>
		public static void StartNew(string message, SimpleColorScheme colorScheme = null, bool fasterButNoFormat = false)
		{
			WriteLine(message, false, true, colorScheme, fasterButNoFormat);
		}
		public static void StartNew()
		{
			WriteLine(null, false, true);
		}

		/// <summary>
		/// Clears an existing TemporaryMessage section
		/// Similar to calling WriteLine(message, refresh = true, startNew = false);
		/// </summary>
		/// <param name="message"></param>
		/// <param name="colorScheme"></param>
		/// <param name="fasterButNoFormat"></param>
		public static void Refresh(string message, SimpleColorScheme colorScheme = null, bool fasterButNoFormat = false)
		{
			WriteLine(message, true, false, colorScheme, fasterButNoFormat);
		}
		public static void Refresh()
		{
			WriteLine(null, true, false);
		}
		/// <summary>
		/// Writes a line to an updatable section. The section is terminated by calling Clear. Multiple calls to this method result in a multiline-section
		/// Usage:
		/// WriteLine(line1, colorScheme, false, true);
		/// WriteLine(line2, colorScheme, false);
		/// WriteLine(line3, colorScheme, false);
		/// .. wait for some action
		/// WriteLine(updatedline1, colorScheme, false, true);
		/// WriteLine(updatedline2, colorScheme, false);
		/// .. optionally call Clear() to remove the updated lines at the end
		/// </summary>
		/// <param name="message">The string to be written</param>
		/// <param name="colorScheme">(optional) SimpleColorScheme which specifies text and background color</param>
		/// <param name="refresh">(optional) clears the previous messages</param>
		/// <param name="startNew">(optional) indicates that this call will start a new section</param>
		public static void WriteLine(string message, bool refresh = false, bool startNew = false, SimpleColorScheme colorScheme = null, bool fasterButNoFormat = false)
		{
			if (colorScheme == null)
				colorScheme = SimpleColorScheme.Empty;
			
			if (startNew)
			{
				CurrentMessages = new List<string>();
				Top = Console.CursorTop;
				Left = Console.CursorLeft;
			}
			if(refresh)
			{
				Clear();
			}
			if (Console.CursorTop >= (Console.BufferHeight-1))
				Top--;
			var previousColor = Console.ForegroundColor;
			var previousBgColor = Console.BackgroundColor;
			Console.ForegroundColor = colorScheme.Text;
			Console.BackgroundColor = colorScheme.Background;

			if (message == null)
				return;
			if(fasterButNoFormat)
				Console.WriteLine(message);
			else
				Formatter.WriteLines(message);

			Console.ForegroundColor = previousColor;
			Console.BackgroundColor = previousBgColor;
			
			CurrentMessages.Add(fasterButNoFormat ? message : Formatter.GetUnformattedText(message));
		}

		public static void Clear()
		{
			Console.SetCursorPosition(Left, Top);
			for (int i = 0; i < CurrentMessages.Count; i++)
			{
				// todo: This can be a lot faster
				StringBuilder clear = new StringBuilder();
				for (int j = 0; j < CurrentMessages[i].Length; j++)
					clear.Append(" \b ");
				Console.Write(clear.ToString());
				Console.WriteLine();
			}
			Console.SetCursorPosition(Left, Top);
			CurrentMessages = new List<string>();
		}

	}
}
