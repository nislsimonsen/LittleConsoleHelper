using System;

namespace LittleConsoleHelper
{
	public static class TemporaryMessage
	{
		private static string CurrentMessage { get; set; }
		private static int Top { get; set; }
		private static int Left { get; set; }
		
		public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Gray)
		{
			if (!string.IsNullOrEmpty(CurrentMessage))
				CurrentMessage = string.Empty;
			Top = Console.CursorTop;
			Left = Console.CursorLeft;
			var previousColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = previousColor;
			CurrentMessage = message;
		}

		public static void Update(string message, ConsoleColor color = ConsoleColor.Gray)
		{
			Clear();
			WriteLine(message, color); 
		}

		public static void Clear()
		{
			Console.SetCursorPosition(Left, Top);
			for (int i = 0; i < CurrentMessage.Length; i++)
				Console.Write(" \b ");
			Console.SetCursorPosition(Left, Top);
			CurrentMessage = string.Empty;
		}
	}
}
