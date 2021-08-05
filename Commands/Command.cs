﻿using LittleConsoleHelper.Display;
using LittleConsoleHelper.UserInput.Menu;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public static class Command
	{
		public static bool Run<U>(string[] programArgs, RootCommand<U> rootCommand, ILogger logger, U context)
		{
			var commandParts = CLIParser.Parse(programArgs);

			BaseCommand<U> commandToRun;
			var helpCommand = rootCommand.SubCommands.FirstOrDefault(c => c.Name.Equals("help", StringComparison.InvariantCultureIgnoreCase));
			if (commandParts.Flags.Contains("?") && helpCommand != null)
			{
				commandToRun = helpCommand;
			}
			else
			{
				commandToRun = GetCommandToRun(commandParts.Commands, rootCommand.SubCommands, logger);
			}

			if (commandToRun == null)
			{
				var errorMessage = "Unable to parse supplied parameters to a command";
				logger.LogError(errorMessage, programArgs);
				Formatter.WriteLine("{error}" + errorMessage);
				return false;
			}

			if (commandToRun.CaptureSubcommand)
			{
				try
				{
					var commandIndex = commandParts.Commands.Select(c => c.ToLower()).ToList().IndexOf(commandToRun.Name.ToLower());
					if (commandParts.Commands.Count > commandIndex)
					{
						var subcommand = string.Join(',', commandParts.Commands.ToArray()[(commandIndex + 1)..^0]);
						var subcommandParameterKey = "subcommand";
						if (commandParts.Parameters.ContainsKey(subcommandParameterKey))
							commandParts.Parameters[subcommandParameterKey] = subcommand;
						else
							commandParts.Parameters.Add(subcommandParameterKey, subcommand);
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

		private static BaseCommand<U> GetCommandToRun<U>(List<string> commands, List<BaseCommand<U>> subCommands, ILogger logger)
		{
			BaseCommand<U> command = null;

			if (commands.Any())
			{
				string commandName = commands.First();
				command = subCommands.FirstOrDefault(sc => sc.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));
			}
			else
			{
				command = Menu.SelectFromList(subCommands.Select(sc => new MenuItem(sc.FriendlyName ?? sc.Name, null, sc)).ToList()).Value as BaseCommand<U>;
			}
			if (command == null)
			{
				return null;
			}

			if (command.IsExecutable)
			{
				return command;
			}
			else
			{
				if (commands.Any())
					return GetCommandToRun(commands.ToArray()[1..^0].ToList(), command.SubCommands, logger);
				else if (command.SubCommands.Any())
					return GetCommandToRun(new List<string>(), command.SubCommands, logger);
				else
				{ }
				throw new NullReferenceException($"Command {command.Name} is not executable and has no subcommands");

			}
		}
	}
}
