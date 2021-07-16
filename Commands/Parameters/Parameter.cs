using LittleConsoleHelper.Display;
using LittleConsoleHelper.UserInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class Parameter : BaseOption
	{
		public bool Required { get; set; }
		public string DefaultValue { get; set; }
		private string _value;
		public string Value
		{
			get
			{
				return _value ?? DefaultValue;
			}
			set
			{
				_value = value;
			}
		}
		public virtual string ValueTypeName { get; set; } = "String";
		public virtual string ValueTypeDescription { get { return ValueTypeName; } }

		public Parameter(string name, string defaultValue, bool required, params string[] tokens)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (required && defaultValue != null)
				throw new ArgumentException("Cannot set default value with required parameter");
			if (tokens.Length == 0)
				throw new ArgumentException(nameof(tokens));

			Name = name;
			DefaultValue = defaultValue;
			Required = required;
			Tokens = tokens.ToList();
		}

		private Parameter() { }

		/// <summary>
		/// This method will be called to ensure that required parameters have a value
		/// </summary>
		protected internal virtual void EnsureRequired(Vocabulary vocabulary = null)
		{
			if (Required && string.IsNullOrWhiteSpace(Value))
			{
				do
				{
					var promptHeader = $"{{secondarytext}}Parameter {{selectedtext}}{Name}{{secondarytext}} ({{selectedtext}}{ValueTypeName}{{secondarytext}}) is required. Please enter a value:";
					if (vocabulary == null)
						Value = Prompt.ForString(promptHeader);
					else
						Value = Prompt.ForString(promptHeader, (s) => vocabulary.GetByType(ValueTypeName, s));

					if (Validate(out var validationError))
						break;
					else
						Formatter.WriteLine($"{{error}}The supplied value '{Value}' fails validation: {validationError}");
				}
				while (true);

				if (vocabulary != null)
					//vocabulary.AddText(Value);
					vocabulary.Add(ValueTypeName, Value);
			}
		}

		/// <summary>
		/// Only call on parameters which have a value. 
		/// </summary>
		public virtual bool Validate(out string validationError)
		{
			validationError = null;
			if (String.IsNullOrWhiteSpace(Value))
			{
				validationError = "cannot be empty";
				return false;
			}
			return true;
			
		}
		public virtual bool IsDefined()
		{
			return !string.IsNullOrEmpty(Value);
		}
		public static implicit operator bool(Parameter p)
		{
			return p.Value != null;
		}
		public static implicit operator string(Parameter p)
		{
			return p.ToString();
		}
		public override string ToString()
		{
			return Value ?? DefaultValue;
		}
	}
}
