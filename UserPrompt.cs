using System;

namespace LittleConsoleHelper
{
	public static class Prompt
	{
		public static string ForString(string question, ConsoleColor questionColor = ConsoleColor.White, ConsoleColor answerColor = ConsoleColor.Green)
		{
			ConsoleColor resetColor = Console.ForegroundColor;
			
			Console.ForegroundColor = questionColor;
			Console.WriteLine(question);

			Console.ForegroundColor = answerColor;
			var result = Console.ReadLine();
			
			Console.ForegroundColor = resetColor;
			
			return result;
		}
	}
}
