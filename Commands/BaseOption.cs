using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public abstract class BaseOption
	{
		public string Name { get; set; }
		public List<string> Tokens { get; set; }
		public string ShortDescription { get; set; }
		public virtual bool IncludeInHelp { get; set; } = true;
		public string ExtendedHelp { get; set; } = "No description available";

	}
}
