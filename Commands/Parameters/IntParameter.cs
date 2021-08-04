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
			if (Value == null || !int.TryParse(Value, out int f))
			{
				validationError = $"'{Value??"NULL"}' cannot be parsed to an integer";
				return false;
			}
			validationError = null;
			return true;
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
