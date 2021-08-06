using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public class RootCommand<UContext> : BaseCommand<UContext>
	{
		public override string Name => throw new NotImplementedException();

		public RootCommand()
			: base()
		{
		}
		public RootCommand(params BaseCommand<UContext>[] subCommands)
		{
			SubCommands = subCommands.ToList();
		}

		public override List<BaseCommand<UContext>> SubCommands { get; set; }
		
		public void UseHelpCommand(string programName, string programVersion, string programCommandName, bool visibleInMenu = true)
		{
			var helpCommand = new HelpCommand<UContext>(programName, programVersion, programCommandName, this) { VisibleInMenu = visibleInMenu};
			SubCommands.Add(helpCommand);
		}
	}
}
