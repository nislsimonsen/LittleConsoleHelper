﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public class HelpCommand<T> : BaseCommand<T>
	{
		public override string Name => "Help";
		public override bool CaptureSubcommand => true;

		public string ProgramName { get; set; }
		public string ProgramVersion { get; set; }
		public string ProgramCommandName { get; set; }
		public RootCommand<T> RootCommand { get; set; }
		public HelpCommand(string programName, string programVersion, string programCommandName, RootCommand<T> rootCommand)
		{
			ProgramName = programName;
			ProgramVersion = programVersion;
			ProgramCommandName = programCommandName;
			RootCommand = rootCommand;
		}
		public override string ShortHelpText => "Provides help for the programs commands";
		public override List<string> LongHelpText => new List<string> 
		{
			"The help command can provide instructions for given commands.",
			"Invoke it by prepending 'help' before the command specifier or by appending a /? flag",
			"E.g.",
			"(programName) help (commandName) [(subCommandName)]",
			"(programName) (commandName [(subCommandName)] /?"
		};
		public override bool Execute(T context, Dictionary<string, string> parameters, List<string> flags)
		{
			var options = new HelpOptions(parameters, flags);
			
			if (!options.SubCommand || options.SubCommand.Value.Length == 0)
			{
				Formatter.WriteLines("{header}" + ProgramName + " help{/header}", "{secondarytext}Version: {selectedtext}" + ProgramVersion);
				Formatter.WriteLines("", "{secondarytext}Available commands:{reset}", "");

				foreach (var command in RootCommand.SubCommands)
				{
					Formatter.WriteLines($"  {{selectedtext}}{command.Name,-15}{{reset}}{command.ShortHelpText}");
				}
				Formatter.WriteLines("", "----------------------------------", "");
				Formatter.WriteLines("{selectedtext}" + ProgramCommandName + " help [command name]  {text}Displays further help on the specified command");
			}
			else
			{
				BaseCommand<T> command = RootCommand;
				var subcommands = options.SubCommand.Value.Split(new char[] { ',' });
				var path = new List<string> { ProgramCommandName };
				foreach (var subcommand in subcommands)
				{
					command = command.SubCommands.Where(c => c.Name.Equals(subcommand, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
					if (command == null)
					{
						Formatter.WriteLines($"{{error}}Unknown command '{subcommand}'");
						return false;
					}
					else
						path.Add(command.Name.ToLower());
				}

				if (command != null)
				{
					var commandPath = string.Join(' ', path.ToArray());
					var commandOptions = command.GetEmptyExecutionOptionsForHelp();

					string usageLabel = "{text}usage";
					string usage = "{selectedtext}" + commandPath;

					

					if (commandOptions == null)
					{
						Formatter.WriteLines(usageLabel, usage, "");

						if (command.LongHelpText != null)
							Formatter.WriteLines(command.LongHelpText.ToArray());
						
						if (command.SubCommands.Count > 0)
						{
							Formatter.WriteLines("{secondarytext}Available subcommands", "");

							var columns = new List<string> { "Command", "Description" };
							var values = new List<List<string>>();
							command.SubCommands.ForEach(sc => values.Add(new List<string> { $"{{selectedtext}}{commandPath} {sc.Name.ToLower()}", sc.ShortHelpText }));
							Table.Display(new TableData(columns, values));
						}
					}
					else
					{
						var p = commandOptions.Parameters.Where(p => p.IncludeInHelp).OrderBy(p => p.Required).ThenBy(p => p.Name).ToList();
						var f = commandOptions.Flags.Where(p => p.IncludeInHelp).OrderBy(f => f.Name).ToList();
						var allOptions = new List<BaseOption>().Union(p).Union(f);

						var requiredParametersUsage = String.Join(' ', p.Where(p => p.Required).Select(p => "-" + p.Tokens.First().ToLower() + " (value)").ToArray());
						var optionalParametersUsage = String.Join(' ', p.Where(p => !p.Required).Select(p => "[-" + p.Tokens.First().ToLower() + " (value)]").ToArray());
						var flagsUsage = String.Join(' ', f.Select(p => "[/" + p.Tokens.First().ToLower() + "]").ToArray());

						usage += " " + requiredParametersUsage;
						usage += (requiredParametersUsage.Length > 0 ? " " : string.Empty) + optionalParametersUsage;
						usage += (optionalParametersUsage.Length > 0 ? " " : string.Empty) + flagsUsage;

						Formatter.WriteLines(usageLabel, usage, "", "Parameters and flags:");

						var columns = new List<string> { "Name", "Required", "Type", "Token(s)" };
						var values = new List<List<string>>();
						p.Where(p => p.Required).ToArray().ToList().ForEach(p => values.Add(new List<string> { p.Name, "yes", p.ValueTypeName, "-" + string.Join(" -", p.Tokens) }));
						p.Where(p => !p.Required).ToArray().ToList().ForEach(p => values.Add(new List<string> { p.Name, "no", p.ValueTypeName, "-" + string.Join(" -", p.Tokens) }));
						f.ForEach(f => values.Add(new List<string> { f.Name, "no", string.Empty, "/" + string.Join(" /", f.Tokens) }));
						Table.Display(new TableData(columns, values));

						if (command.LongHelpText != null)
							Formatter.WriteLines(command.LongHelpText.ToArray());
					}
				}
			}
			return true;
		}

	}
	public class HelpOptions : OptionContainer
	{
		public Parameter SubCommand = new Parameter("SubCommand", null, false, "subcommand");

		public HelpOptions(Dictionary<string, string> parameters, List<string> flags)
			: base(parameters, flags)
		{
		}
		public override List<Flag> Flags => new List<Flag>();
		public override List<Parameter> Parameters => new List<Parameter> { SubCommand };
	}
}