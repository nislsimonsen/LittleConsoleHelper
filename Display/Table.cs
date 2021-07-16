using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Display
{
	public static class Table
	{
		// TODO: Integrate the table specific color settings with ColorScheme and configuration files
		static TableStyle TableStyleNone = new TableStyle { BorderStyle = TableBorderStyle.None, Padding = 1, HeaderColor = ConsoleColor.Gray, FirstColumnColor = ConsoleColor.Gray, OtherColumnsColor = ConsoleColor.Gray, BorderColor = ConsoleColor.Gray };
		static TableStyle TableStyleDefault = new TableStyle { BorderStyle = TableBorderStyle.HeaderSeperated, Padding = 2, HeaderColor = ConsoleColor.White, FirstColumnColor = ConsoleColor.White, OtherColumnsColor = ConsoleColor.Gray, BorderColor = ConsoleColor.Gray };
		public static void Display(List<List<string>> data, TableStyle tableStyle = null)
		{
			Display(null, data, tableStyle);
		}
		public static void Display(List<string> headers, List<List<string>> data, TableStyle tableStyle = null)
		{
			var table = new TableData(headers, data);
			Display(table, tableStyle);
		}

		public static void Display(TableData table, TableStyle tableStyle = null)
		{
			if (tableStyle == null)
				tableStyle = TableStyleDefault;

			var maximumColumns = Math.Max(table.ColumnHeaders?.Count??0, table.Values.Max(row => row.Count));
			var columnLengths = new int[maximumColumns];

			bool anyHeaders = false;
			if (table.ColumnHeaders != null)
			{
				for (var i = 0; i < table.ColumnHeaders.Count; i++)
				{
					var chl = Formatter.GetUnformattedText(table.ColumnHeaders[i]).Length;
					if (chl > 0)
						anyHeaders = true;
					columnLengths[i] = Math.Max(columnLengths[i], chl);
				}
			}
			for (var i = 0; i < table.Values.Count; i++)
			{
				var row = table.Values[i];
				for (var j = 0; j < row.Count; j++)
				{
					var dl = Formatter.GetUnformattedText(row[j]).Length;
					columnLengths[j] = Math.Max(columnLengths[j], dl);
				}
			}

			string borderRow = null;
			if (tableStyle.BorderStyle != TableBorderStyle.None)
			{
				var totalWidth = 0;
				for (var i = 0; i < columnLengths.Length; i++)
				{
					totalWidth += columnLengths[i];
					if ((i + 1) < columnLengths.Length)
					{
						totalWidth += tableStyle.Padding;
					}
				}
				borderRow = string.Empty.PadRight(totalWidth, '-');
			}

			if (anyHeaders)
			{
				Console.ForegroundColor = tableStyle.HeaderColor;
				if (table.ColumnHeaders != null)
				{
					for (var i = 0; i < table.ColumnHeaders.Count; i++)
					{
						Formatter.Write(table.ColumnHeaders[i], columnLengths[i] + tableStyle.Padding, true);
					}
				}
				Formatter.WriteLine(string.Empty);

				Console.ForegroundColor = tableStyle.BorderColor;
				if ((tableStyle.BorderStyle & TableBorderStyle.HeaderSeperated) == TableBorderStyle.HeaderSeperated)
				{
					Formatter.WriteLine(borderRow);
				}
			}

			for (var i = 0; i < table.Values.Count; i++)
			{
				for (var j = 0; j < table.Values[i].Count; j++)
				{
					Console.ForegroundColor = j == 0 ? tableStyle.FirstColumnColor : tableStyle.OtherColumnsColor;
					Formatter.Write(table.Values[i][j], columnLengths[j] + tableStyle.Padding, true);
				}
				Formatter.WriteLine(string.Empty);
			}
		}
	}

	public class TableData
	{
		internal List<string> ColumnHeaders = new List<string>();
		internal List<List<string>> Values = new List<List<string>>();

		public TableData() { }
		public TableData(List<string> columns, List<List<string>> values)
		{
			ColumnHeaders = columns;
			Values = values;
		}
		

	}

	public class TableStyle
	{
		public int Padding { get; set; }
		public TableBorderStyle BorderStyle { get; set; } = TableBorderStyle.None;
		public ConsoleColor HeaderColor { get; set; }
		public ConsoleColor FirstColumnColor { get; set; }
		public ConsoleColor OtherColumnsColor { get; set; }
		public ConsoleColor BorderColor { get; set; }
	}

	[Flags]
	public enum TableBorderStyle
	{ 
		None = 0,
		HeaderSeperated = 1
	}
}
