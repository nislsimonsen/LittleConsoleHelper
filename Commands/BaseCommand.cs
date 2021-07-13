using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	
	public abstract class BaseCommand<UContext>
	{
		public abstract string Name { get; }
		public virtual bool IsExecutable { get; } = true;
		public virtual string ShortHelpText { get; set; } = null;
		public virtual List<string> LongHelpText { get; set; } = null;
		public virtual string FriendlyName { get { return Name; } }

		protected List<BaseCommand<UContext>> _subCommands;
		public virtual List<BaseCommand<UContext>> SubCommands 
		{ 
			get 
			{ 
				if(_subCommands == null)
					_subCommands = new List<BaseCommand<UContext>>();
				return _subCommands;
			} 
			set 
			{
				_subCommands = value;
			} 
		}
		protected ILogger Logger { get; set; }
		public void SetLogger(ILogger logger)
		{
			Logger = logger;
		}
		public BaseCommand()
		{
		}
		public virtual OptionContainer GetEmptyExecutionOptionsForHelp() { return null; }
		protected Func<UContext, Dictionary<string, string>, List<string>, bool> _executionMethod;
		public virtual bool Execute(UContext context, Dictionary<string, string> parameters, List<string> flags) 
		{
			if (_executionMethod != null)
				return _executionMethod(context, parameters, flags);
			else
				return true; 
		}

		/// <summary>
		/// Converts any text supplied after the command name as parameters to a "subcommand" parameter
		/// </summary>
		/// <returns></returns>
		public virtual bool CaptureSubcommand{ get; set; } = false;
	}

	public class NullLogger : ILogger
	{
		public IDisposable BeginScope<TState>(TState state)
		{
			return new Foo();
		}
		private class Foo : IDisposable
		{
			public void Dispose()
			{
			}
		}
		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
		}
	}
	public class NullContext
	{ }
}
