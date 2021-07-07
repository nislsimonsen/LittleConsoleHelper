using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class DirectoryParameter : Parameter
	{
		public override string ValueTypeName { get; protected set; } = "Directory path";
		public DirectoryParameter(string name, string defaultValue, bool required, params string[] tokens)
			: base(name, defaultValue, required, tokens)
		{ }

		public override bool Validate(out string validationError)
		{
			// since this may be validated in a shell, we cannot use standard fs methods to validate
			try
			{
				if (string.IsNullOrWhiteSpace(Value))
				{
					validationError = "Value is null or whitespace";
					return false;
				}

				var illegalChars = new List<char> { '!', '#', '/', '?', '*' };
				foreach (var c in illegalChars)
					if (Value.Contains(c))
					{
						validationError = $"'{c}' is not valid in a path";
						return false;
					}

				if (!Value.Contains("\\"))
				{
					validationError = "A valid path must have at least one '\\'";
					return false;
				}

				validationError = null;
				return true;
			}
			catch
			{
				validationError = $"'{Value}' is not a valid Directory";
				return false;
			}
		}
	}
}
