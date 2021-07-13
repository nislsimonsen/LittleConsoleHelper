using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class UrlParameter : Parameter
	{
		public override string ValueTypeName { get; set; } = "Url";
		public UrlParameter(string name, string defaultValue, bool required, params string[] tokens)
			: base(name, defaultValue, required, tokens)
		{ }

		public override bool Validate(out string validationError)
		{
			try
			{
				var uri = new Uri(Value);
				validationError = null;
				return true;
			}
			catch
			{
				validationError = $"'{Value}' is not a valid URI";
				return false;
			}
		}
	}
}
