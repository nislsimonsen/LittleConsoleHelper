using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper
{
	public class MenuShowOptions
	{
		public ColorScheme ColorScheme { get; set; }
		public bool AllowEscape { get; set; }
		public bool AllowInteriorNodeSelect { get; set; }
		public string InteriorSuffix { get; set; }
		public string InteriorOpenSuffix { get; set; }
		public string Indentation { get; set; }
		public bool ClearOnSelect { get; set; }

		public MenuShowOptions()
		{
			ColorScheme = ColorScheme.Default;
			AllowEscape = false;
			AllowInteriorNodeSelect = false;
			InteriorSuffix = " >";
			InteriorOpenSuffix = " <";
			Indentation = "  ";
			ClearOnSelect = false;
		}
	}
}
