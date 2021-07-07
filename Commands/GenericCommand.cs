using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public class GenericCommand<UContext> : BaseCommand<UContext>
	{
		public GenericCommand(string name, List<BaseCommand<UContext>> subCommands)
		{
			_name = name;
			_isExecutable = false;
			SubCommands = subCommands;
		}
		public GenericCommand(string name, Func<UContext, Dictionary<string, string>, List<string>, bool> executionMethod)
		{
			_name = name;
			_isExecutable = true;
			_executionMethod = executionMethod;
		}
		private string _name;
		public override string Name { get { return _name; } }

		private bool _isExecutable;
		public override bool IsExecutable { get { return _isExecutable; } }

	}
}
