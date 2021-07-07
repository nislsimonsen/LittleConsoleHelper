﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public class EnumParameter<T> : Parameter
		where T : struct
	{
		public override string ValueTypeName
		{
			get
			{
				return typeof(T).ToString();
			}
			protected set { }
		}
		public EnumParameter(string name, string defaultValue, bool required, params string[] tokens)
			: base(name, defaultValue, required, tokens)
		{ }
		protected internal override void EnsureRequired()
		{
			if (Required && string.IsNullOrWhiteSpace(Value))
			{
				Value = Menu.SelectEnumMember<T>($"Select value for '{Name}'", options: null).Value.ToString();
			}
		}
		public T EnumValue
		{
			get
			{
				return Enum.Parse<T>(Value, true);
			}
		}
		public override bool Validate(out string validationError)
		{
			if (Enum.TryParse<T>(Value, true, out var t))
			{
				validationError = null;
				return true;
			}
			validationError = $"Could not parse '{Value} as type {ValueTypeName}'";
			return false;
		}
	}
}