using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class BoolParameter : Parameter
	{
		public override string ValueTypeName { get; set; } = "Boolean";

		public BoolParameter(string name, string defaultValue, bool required, params string[] tokens)
			: base(name, defaultValue, required, tokens)
		{ }

		public override bool Validate(out string validationError)
		{
			if (InternalParse().HasValue)
			{
				validationError = null;
				return true;
			}
			validationError = $"'{Value}' cannot be parsed to a boolean";
			return false;
		}
		private bool? InternalParse()
		{
			if (Value == null)
				return null;
			var variations = new List<(string token, bool value)>
			{
				("true", true),
				("false", false),
				("yes", true),
				("no", false),
				("1", true),
				("0", false)
			};

			foreach (var v in variations)
				if (Value.Equals(v.token, StringComparison.InvariantCultureIgnoreCase))
					return v.value;

			return null;
		}

		public bool BoolValue
		{
			get
			{
				return InternalParse().Value;
			}
			set
			{
				Value = value.ToString();
			}
		}
		
	}
}
