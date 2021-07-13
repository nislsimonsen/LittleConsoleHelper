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
				//if (_value == null && DefaultValue != null)
				//	return DefaultValue;
				//return _value;
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

		protected internal virtual void EnsureRequired()
		{
			if (Required && string.IsNullOrWhiteSpace(Value))
			{
				Value = Prompt.ForString(Name);
			}
		}

		public virtual bool Validate(out string validationError)
		{
			validationError = null;
			if (Required)
			{
				if (String.IsNullOrWhiteSpace(Value))
				{
					validationError = "cannot be empty";
					return false;
				}
				return true;
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
