using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public class Flag : BaseOption
	{
		public bool IsSet { get; set; }
		public Flag(string name, params string[] tokens)
		{
			Name = name;
			Tokens = tokens.ToList();
		}

		private Flag() { }

		public static implicit operator bool(Flag f)
		{
			return f.IsSet;
		}
	}
}
