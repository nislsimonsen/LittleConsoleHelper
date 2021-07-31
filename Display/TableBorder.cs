using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Display
{

	[Flags]
	public enum TableBorder
	{
		None = 0,
		HeaderSeparated = 1,
		RowsSeparated = 2,
		ColumnsSeparated = 4,
		Inside = 7,
		Outside = 8,
		All = 15
	}

	public class TableBorderType
	{
		TableBorderType() { }

		public static TableBorderType SingleLine = new TableBorderType
		{
			HorizontalBorderChar = '─',
			VerticalBorderChar = '│',
			OutsideTopLeftBorderChar = '┌',
			OutsideTopRightBorderChar = '┐',
			OutsideBottomLeftBorderChar = '└',
			OutsideBottomRightBorderChar = '┘',
			InsideCrossBorderChar = '┼',
			OutsideToInsideBorderCharTop = '┬',
			OutsideToInsideBorderCharBottom = '┴',
			OutsideToInsideBorderCharLeft = '├',
			OutsideToInsideBorderCharRight = '┤',
		};

		public static TableBorderType DoubleLine = new TableBorderType
		{
			HorizontalBorderChar = '═',
			VerticalBorderChar = '║',
			OutsideTopLeftBorderChar = '╔',
			OutsideTopRightBorderChar = '╗',
			OutsideBottomLeftBorderChar = '╚',
			OutsideBottomRightBorderChar = '╝',
			InsideCrossBorderChar = '╬',
			OutsideToInsideBorderCharTop = '╦',
			OutsideToInsideBorderCharBottom = '╩',
			OutsideToInsideBorderCharLeft = '╠',
			OutsideToInsideBorderCharRight = '╣',
		};
		public static TableBorderType Asterisk = new TableBorderType
		{
			HorizontalBorderChar = '*',
			VerticalBorderChar = '*',
			OutsideTopLeftBorderChar = '*',
			OutsideTopRightBorderChar = '*',
			OutsideBottomLeftBorderChar = '*',
			OutsideBottomRightBorderChar = '*',
			InsideCrossBorderChar = '*',
			OutsideToInsideBorderCharTop = '*',
			OutsideToInsideBorderCharBottom = '*',
			OutsideToInsideBorderCharLeft = '*',
			OutsideToInsideBorderCharRight = '*',
		};
		public static TableBorderType Dot = new TableBorderType
		{
			//■
			HorizontalBorderChar = '·',
			VerticalBorderChar = '·',
			OutsideTopLeftBorderChar = '·',
			OutsideTopRightBorderChar = '·',
			OutsideBottomLeftBorderChar = '·',
			OutsideBottomRightBorderChar = '·',
			InsideCrossBorderChar = '·',
			OutsideToInsideBorderCharTop = '·',
			OutsideToInsideBorderCharBottom = '·',
			OutsideToInsideBorderCharLeft = '·',
			OutsideToInsideBorderCharRight = '·',
		};
		public static TableBorderType Dash = new TableBorderType
		{
			HorizontalBorderChar = '-',
			VerticalBorderChar = '|',
			OutsideTopLeftBorderChar = '-',
			OutsideTopRightBorderChar = '-',
			OutsideBottomLeftBorderChar = '-',
			OutsideBottomRightBorderChar = '-',
			InsideCrossBorderChar = '-',
			OutsideToInsideBorderCharTop = '-',
			OutsideToInsideBorderCharBottom = '-',
			OutsideToInsideBorderCharLeft = '-',
			OutsideToInsideBorderCharRight = '-',
		};
		public char HorizontalBorderChar { get; set; } = '─';
		public char VerticalBorderChar { get; set; } = '│';
		public char OutsideTopLeftBorderChar { get; set; } = '┌';
		public char OutsideTopRightBorderChar { get; set; } = '┐';
		public char OutsideBottomLeftBorderChar { get; set; } = '└';
		public char OutsideBottomRightBorderChar { get; set; } = '┘';
		public char InsideCrossBorderChar { get; set; } = '┼';
		public char OutsideToInsideBorderCharTop { get; set; } = '┬';
		public char OutsideToInsideBorderCharBottom { get; set; } = '┴';
		public char OutsideToInsideBorderCharLeft { get; set; } = '├';
		public char OutsideToInsideBorderCharRight { get; set; } = '┤';
	}
}
