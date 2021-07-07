using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public static class Command
	{
		public static bool Run<T, U>(string[] programArgs, RootCommand<U> rootCommand, T logger, U context)
			where T : ILogger
		{
			var commandParts = CLIParser.Parse(programArgs);
			var commandToRun = GetCommandToRun(commandParts.Commands, rootCommand.SubCommands);
			
			if (commandToRun.CaptureSubcommand)
			{
				try
				{
					var commandIndex = commandParts.Commands.Select(c => c.ToLower()).ToList().IndexOf(commandToRun.Name.ToLower());
					if (commandParts.Commands.Count > commandIndex)
					{
						var subcommand = string.Join(',', commandParts.Commands.ToArray()[(commandIndex + 1)..^0]);
						commandParts.Parameters.Add("subcommand", subcommand);
					}
				}
				catch (Exception e)
				{
					throw new ArgumentException($"Unable to correctly parse subcommand(s) from command list: {string.Join(',', commandParts.Commands.ToArray())}", e);
				}
			}
			
			commandToRun.SetLogger(logger);

			return commandToRun.Execute(context, commandParts.Parameters, commandParts.Flags);
		}

		private static BaseCommand<U> GetCommandToRun<U>(List<string> commands, List<BaseCommand<U>> subCommands)
		{
			BaseCommand<U> command = null;

			if (commands.Any())
			{
				string commandName = commands.First();
				command = subCommands.FirstOrDefault(sc => sc.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));
			}
			else
			{
				command = Menu.SelectFromList(subCommands.Select(sc => new MenuItem(sc.Name, null, sc)).ToList()).Value as BaseCommand<U>;
			}
			if (command == null)
				throw new NullReferenceException(nameof(command));

			if (command.IsExecutable)
			{
				return command;
			}
			else
			{
				if (commands.Any())
					return GetCommandToRun(commands.ToArray()[1..^0].ToList(), command.SubCommands);
				else if (command.SubCommands.Any())
					return GetCommandToRun(new List<string>(), command.SubCommands);
				else
					throw new NullReferenceException($"Command {command.Name} is not executable and has no subcommands");
				
			}
		}
	}
}
