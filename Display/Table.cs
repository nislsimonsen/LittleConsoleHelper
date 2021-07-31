using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Display
{
	public static class Table
	{
		// TODO: Integrate the table specific color settings with ColorScheme and configuration files
		static TableStyle TableStyleNone = new TableStyle { Border = TableBorder.None, Padding = 1, HeaderColor = ConsoleColor.Gray, FirstColumnColor = ConsoleColor.Gray, OtherColumnsColor = ConsoleColor.Gray, BorderColor = ConsoleColor.Gray };
		static TableStyle TableStyleDefault = new TableStyle { Border = TableBorder.HeaderSeparated, Padding = 2, HeaderColor = ConsoleColor.White, FirstColumnColor = ConsoleColor.White, OtherColumnsColor = ConsoleColor.Gray, BorderColor = ConsoleColor.Gray };
		
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

			bool anyHeaders = HasHeaders(table);
			var columnLengths = CalculateColumnLengths(table);

			CreateBorderRows(tableStyle, columnLengths, out var topBorderRow, out var insideBorderRow, out var bottomBorderRow, out var insideVerticalBorder);

			PrintTopOutsideBorder(tableStyle, topBorderRow);
			PrintHeaderRow(table, tableStyle, columnLengths, anyHeaders, insideBorderRow, insideVerticalBorder);
			PrintDataRows(table, tableStyle, columnLengths, insideBorderRow, insideVerticalBorder);

			PrintBottomOutsideBorder(tableStyle, bottomBorderRow);
		}

		private static int[] CalculateColumnLengths(TableData table)
		{
			var maximumColumns = Math.Max(table.ColumnHeaders?.Count ?? 0, table.Rows.Max(row => row.Count));
			int[] columnLengths = new int[maximumColumns];

			if (table.ColumnHeaders != null)
			{
				for (var i = 0; i < table.ColumnHeaders.Count; i++)
				{
					var chl = Formatter.GetUnformattedText(table.ColumnHeaders[i]).Length;
					columnLengths[i] = Math.Max(columnLengths[i], chl);
				}
			}
			for (var i = 0; i < table.Rows.Count; i++)
			{
				var row = table.Rows[i];
				for (var j = 0; j < row.Count; j++)
				{
					var dl = Formatter.GetUnformattedText(row[j]).Length;
					columnLengths[j] = Math.Max(columnLengths[j], dl);
				}
			}
			return columnLengths;
		}
		private static bool HasHeaders(TableData table)
		{
			if (table.ColumnHeaders != null)
			{
				for (var i = 0; i < table.ColumnHeaders.Count; i++)
				{
					var chl = Formatter.GetUnformattedText(table.ColumnHeaders[i]).Length;
					if (chl > 0)
						return true;
				}
			}
			return false;
		}
		private static void CreateBorderRows(TableStyle tableStyle, int[] columnLengths, out string topBorderRow, out string insideBorderRow, out string bottomBorderRow, out string insideVerticalBorder)
		{
			topBorderRow = string.Empty;
			insideBorderRow = string.Empty;
			bottomBorderRow = string.Empty;
			if (tableStyle.Border != TableBorder.None)
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
				if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
					totalWidth += tableStyle.Padding;

				if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
				{
					topBorderRow += tableStyle.BorderType.OutsideTopLeftBorderChar;
					if ((tableStyle.Border & TableBorder.Inside) == TableBorder.Inside)
						insideBorderRow += tableStyle.BorderType.OutsideToInsideBorderCharLeft;
					else
						insideBorderRow += tableStyle.BorderType.VerticalBorderChar;
					bottomBorderRow += tableStyle.BorderType.OutsideBottomLeftBorderChar;
				}
				for (var i = 0; i < columnLengths.Length; i++)
				{
					var segmentLength = columnLengths[i] + tableStyle.Padding;
					var segmentPart = string.Empty.PadRight(segmentLength, tableStyle.BorderType.HorizontalBorderChar);

					topBorderRow += segmentPart;
					insideBorderRow += segmentPart;
					bottomBorderRow += segmentPart;

					if ((tableStyle.Border & TableBorder.ColumnsSeparated) == TableBorder.ColumnsSeparated && i < columnLengths.Length - 1)
					{
						topBorderRow += tableStyle.BorderType.OutsideToInsideBorderCharTop;
						insideBorderRow += tableStyle.BorderType.InsideCrossBorderChar;
						bottomBorderRow += tableStyle.BorderType.OutsideToInsideBorderCharBottom;
					}
				}
				if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
				{
					topBorderRow += tableStyle.BorderType.OutsideTopRightBorderChar;
					if ((tableStyle.Border & TableBorder.Inside) == TableBorder.Inside)
						insideBorderRow += tableStyle.BorderType.OutsideToInsideBorderCharRight;
					else
						insideBorderRow += tableStyle.BorderType.VerticalBorderChar;
					bottomBorderRow += tableStyle.BorderType.OutsideBottomRightBorderChar;
				}
			}

			if ((tableStyle.Border & TableBorder.ColumnsSeparated) == TableBorder.ColumnsSeparated)
				insideVerticalBorder = tableStyle.BorderType.VerticalBorderChar.ToString();
			else
				insideVerticalBorder = string.Empty;
		}
		private static void PrintTopOutsideBorder(TableStyle tableStyle, string topBorderRow)
		{
			if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
			{
				Console.ForegroundColor = tableStyle.BorderColor;
				Console.Write(topBorderRow);
				Console.WriteLine();
			}
		}
		private static void PrintHeaderRow(TableData table, TableStyle tableStyle, int[] columnLengths, bool anyHeaders, string insideBorderRow, string insideVerticalBorder)
		{
			if (anyHeaders)
			{
				if (table.ColumnHeaders != null)
				{
					if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
					{
						Console.ForegroundColor = tableStyle.BorderColor;
						Console.Write(tableStyle.BorderType.VerticalBorderChar);
					}
					Console.ForegroundColor = tableStyle.HeaderColor;
					for (var i = 0; i < table.ColumnHeaders.Count; i++)
					{
						Console.ForegroundColor = tableStyle.HeaderColor;
						Formatter.Write(table.ColumnHeaders[i], columnLengths[i] + tableStyle.Padding, true);
						if (i < table.ColumnHeaders.Count - 1)
						{
							Console.ForegroundColor = tableStyle.BorderColor;
							Console.Write(insideVerticalBorder);
						}
					}
					if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
					{
						Console.ForegroundColor = tableStyle.BorderColor;
						Console.Write(tableStyle.BorderType.VerticalBorderChar);
					}
				}
				Formatter.WriteLine(string.Empty);

				if ((tableStyle.Border & TableBorder.HeaderSeparated) == TableBorder.HeaderSeparated)
				{
					Console.ForegroundColor = tableStyle.BorderColor;
					Console.Write(insideBorderRow);
					Console.WriteLine();
				}
			}
		}
		private static void PrintDataRows(TableData table, TableStyle tableStyle, int[] columnLengths, string insideBorderRow, string insideVerticalBorder)
		{
			for (var i = 0; i < table.Rows.Count; i++)
			{
				if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
				{
					Console.ForegroundColor = tableStyle.BorderColor;
					Console.Write(tableStyle.BorderType.VerticalBorderChar);
				}
				for (var j = 0; j < table.Rows[i].Count; j++)
				{
					Console.ForegroundColor = j == 0 ? tableStyle.FirstColumnColor : tableStyle.OtherColumnsColor;
					Formatter.Write(table.Rows[i][j], columnLengths[j] + tableStyle.Padding, true);
					if (j < table.Rows[i].Count - 1)
					{
						Console.ForegroundColor = tableStyle.BorderColor;
						Console.Write(insideVerticalBorder);
					}
				}
				if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
				{
					Console.ForegroundColor = tableStyle.BorderColor;
					Console.Write(tableStyle.BorderType.VerticalBorderChar);
				}

				Formatter.WriteLine(string.Empty);
				if ((tableStyle.Border & TableBorder.RowsSeparated) == TableBorder.RowsSeparated && i < table.Rows.Count - 1)
				{
					Console.ForegroundColor = tableStyle.BorderColor;
					Console.Write(insideBorderRow);
					Console.WriteLine();

				}
			}
		}
		private static void PrintBottomOutsideBorder(TableStyle tableStyle, string bottomBorderRow)
		{
			if ((tableStyle.Border & TableBorder.Outside) == TableBorder.Outside)
			{
				Console.ForegroundColor = tableStyle.BorderColor;
				Console.Write(bottomBorderRow);
				Console.WriteLine();
			}
		}
	}

}
