﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	//public abstract class BaseCommand : BaseCommand<NullLogger, NullContext>
	//{ 
	//	public virtual bool Execute(Dictionary<string, string> parameters, List<string> flags) 
	//	{ 
	//		return Execute(new NullContext(), parameters, flags); 
	//	}
	//}
	public class RootCommand<TLogger, UContext> : BaseCommand<TLogger, UContext>
		where TLogger : ILogger
	{
		public override string Name => throw new NotImplementedException();

		public override bool IsExecutable => throw new NotImplementedException();
		public RootCommand()
			: base()
		{
		}
		public RootCommand(params BaseCommand<TLogger, UContext>[] subCommands)
		{
			SubCommands = subCommands.ToList();
		}

		public override List<BaseCommand<TLogger, UContext>> SubCommands { get; set; }
	}

	public abstract class BaseCommand<TLogger, UContext>
		where TLogger : ILogger
	{
		public abstract string Name { get; }
		public abstract bool IsExecutable { get; }
		public virtual string FriendlyName { get { return null; } }
		public virtual string ShortDescription { get { return string.Empty; } }

		protected List<BaseCommand<TLogger, UContext>> _subCommands;
		public virtual List<BaseCommand<TLogger, UContext>> SubCommands 
		{ 
			get 
			{ 
				if(_subCommands == null)
					_subCommands = new List<BaseCommand<TLogger, UContext>>();
				return _subCommands;
			} 
			set 
			{
				_subCommands = value;
			} 
		}
		protected TLogger Logger { get; set; }
		public void SetLogger(TLogger logger)
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

		public virtual void DisplayHelp()
		{
		}
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
