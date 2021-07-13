using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class SubCommandParameter : HiddenParameter
	{
		public SubCommandParameter() : base("SubCommand", null, false, "subcommand")
		{
		}
		public SubCommandParameter(string usageDescription) : base("SubCommand", null, false, "subcommand")
		{
			UsageDescription = usageDescription;
		}
		public string UsageDescription { get; set; } = "value";
	}
}
