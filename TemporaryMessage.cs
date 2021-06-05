using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LittleConsoleHelper
{
	public static class TemporaryMessage
	{
		private static List<string> CurrentMessages { get; set; }
		private static int Top { get; set; }
		private static int Left { get; set; }
		private static bool IsInUpdatable { get; set; }

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
		public static void WriteLine(string message, SimpleColorScheme colorScheme = null, bool refresh = false, bool startNew = false, bool fasterButNoFormat = false)
		{
			if (colorScheme == null)
				colorScheme = SimpleColorScheme.Empty;
			if (CurrentMessages == null)
				CurrentMessages = new List<string>();
			
			if (startNew)
			{
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
