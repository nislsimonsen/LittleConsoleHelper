using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Display
{
	public class TableData
	{
		internal List<string> ColumnHeaders = new List<string>();
		internal List<List<string>> Rows = new List<List<string>>();

		public TableData() { }
		public TableData(List<string> columns, List<List<string>> rows)
		{
			ColumnHeaders = columns;
			Rows = rows;
		}
	}
}
