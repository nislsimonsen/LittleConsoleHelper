using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class IntParameter : Parameter
	{
		public override string ValueTypeName { get; set; } = "Integer";
		public IntParameter(string name, string defaultValue, bool required, params string[] tokens)
			: base(name, defaultValue, required, tokens)
		{ }

		public override bool Validate(out string validationError)
		{
			if (int.TryParse(Value, out int f))
			{
				validationError = null;
				return true;
			}
			validationError = $"'{Value}' cannot be parsed to an integer";
			return false;
		}

		public int IntValue
		{
			get
			{
				return int.Parse(Value);
			}
			set
			{
				Value = value.ToString();
			}
		}
	}
}
