using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class HiddenParameter : Parameter
	{
		public override bool IncludeInHelp { get; set; } = false;
		public HiddenParameter(string name, string defaultValue, bool required, params string[] tokens)
			: base(name, defaultValue, required, tokens)
		{ }
	}
}
