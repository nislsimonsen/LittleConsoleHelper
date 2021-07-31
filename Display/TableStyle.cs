using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Display
{
	public class TableStyle
	{
		public int Padding { get; set; } = 2;
		public TableBorder Border { get; set; } = TableBorder.HeaderSeparated;
		public ConsoleColor HeaderColor { get; set; } = ConsoleColor.Gray;
		public ConsoleColor FirstColumnColor { get; set; } = ConsoleColor.Gray;
		public ConsoleColor OtherColumnsColor { get; set; } = ConsoleColor.Gray;
		public ConsoleColor BorderColor { get; set; } = ConsoleColor.Gray;

		public TableBorderType BorderType { get; set; } = TableBorderType.SingleLine;
	}
}
