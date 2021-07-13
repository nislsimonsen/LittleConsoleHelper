using LittleConsoleHelper.Commands.Parameters;
using System;
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
			"Invoke it by prepending 'help' before the command or by appending a /? flag",
			"E.g. one of:",
			$"{{selectedtext}}{ProgramCommandName} help commandName",
			$"{{selectedtext}}{ProgramCommandName} commandName /?",
			$"{{selectedtext}}{ProgramCommandName} commandName subCommandName /?",
		};
		public override bool Execute(T context, Dictionary<string, string> parameters, List<string> flags)
		{
			var options = new HelpOptions(parameters, flags);

			flags.Remove("?");
			parameters.Remove("subcommand");

			if (!options.SubCommand || options.SubCommand.Value.Length == 0)
			{
				DisplayGenericProgramHelp();
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
					DisplayHelpForCommandOrCommandToken(parameters, flags, command, path);
				}
			}
			return true;
		}

		private static void DisplayHelpForCommandOrCommandToken(Dictionary<string, string> parameters, List<string> flags, BaseCommand<T> command, List<string> path)
		{
			var commandPath = string.Join(' ', path.ToArray());
			var commandOptions = command.GetEmptyExecutionOptionsForHelp();

			string usageLabel = "{secondarytext}usage:";
			string usage = "{selectedtext}" + commandPath;

			if (commandOptions == null)
			{
				DisplayHelpForCommandWithNoOptions(command, commandPath, usageLabel, usage);
			}
			else
			{
				string tokenToProvideHelpFor = null;
				BaseOption optionToProvideHelpFor = null;
				bool tokenIsParameter = false;
				bool tokenIsFlag = false;
				if (parameters.Any())
				{
					tokenToProvideHelpFor = parameters.First().Key;
					optionToProvideHelpFor = commandOptions.Parameters.FirstOrDefault(o => o.Tokens.Contains(tokenToProvideHelpFor));
					tokenIsParameter = true;
				}
				else if (flags.Any())
				{
					tokenToProvideHelpFor = flags.First();
					optionToProvideHelpFor = commandOptions.Flags.FirstOrDefault(o => o.Tokens.Contains(tokenToProvideHelpFor));
					tokenIsFlag = true;
				}

				if (optionToProvideHelpFor != null && optionToProvideHelpFor.IncludeInHelp)
				{
					DisplayHelpForSpecificOption(command, usage, tokenToProvideHelpFor, optionToProvideHelpFor, tokenIsParameter, tokenIsFlag);
				}
				else
				{
					DisplayHelpForCommandWithOptions(command, commandOptions, usageLabel, usage);
				}
			}
		}

		private void DisplayGenericProgramHelp()
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
		private static void DisplayHelpForCommandWithNoOptions(BaseCommand<T> command, string commandPath, string usageLabel, string usage)
		{
			Formatter.WriteLines(usageLabel + " " + usage, "");

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
		private static void DisplayHelpForCommandWithOptions(BaseCommand<T> command, OptionContainer commandOptions, string usageLabel, string usage)
		{
			Formatter.WriteLines($"{{secondarytext}}Displaying help for the {{selectedtext}}{command.FriendlyName}{{reset}} command", string.Empty);

			var p = commandOptions.Parameters.Where(p => p.IncludeInHelp).OrderBy(p => p.Required).ThenBy(p => p.Name).ToList();
			var f = commandOptions.Flags.Where(p => p.IncludeInHelp).OrderBy(f => f.Name).ToList();
			var allOptions = new List<BaseOption>().Union(p).Union(f);

			var subCommandParameter = commandOptions.Parameters.FirstOrDefault(o => o is SubCommandParameter) as SubCommandParameter;
			var subCommandParameterUsage = string.Empty;
			if (subCommandParameter != null)
			{
				subCommandParameterUsage = (subCommandParameter.Required ? "":"[") + subCommandParameter.UsageDescription.Replace(' ','_') + (subCommandParameter.Required ? "" : "]");
			}
			var requiredParametersUsage = 
				subCommandParameterUsage
				+ String.Join(' ', p.Where(p => p.Required).Select(p => "-" + p.Tokens.First().ToLower() + " (value)").ToArray());
			var optionalParametersUsage = String.Join(' ', p.Where(p => !p.Required).Select(p => "[-" + p.Tokens.First().ToLower() + " (value)]").ToArray());
			var flagsUsage = String.Join(' ', f.Select(p => "[/" + p.Tokens.First().ToLower() + "]").ToArray());

			usage += " " + requiredParametersUsage;
			usage += (requiredParametersUsage.Length > 0 ? " " : string.Empty) + optionalParametersUsage;
			usage += (optionalParametersUsage.Length > 0 ? " " : string.Empty) + flagsUsage;

			Formatter.WriteLines(usageLabel + " " + usage, "", "Parameters and flags:");

			var columns = new List<string> { "Name", "Required", "Type", "Token(s)" };
			var values = new List<List<string>>();
			p.Where(p => p.Required).ToArray().ToList().ForEach(p => values.Add(new List<string> { p.Name, "yes", p.ValueTypeName, "-" + string.Join(" -", p.Tokens) }));
			p.Where(p => !p.Required).ToArray().ToList().ForEach(p => values.Add(new List<string> { p.Name, "no", p.ValueTypeName, "-" + string.Join(" -", p.Tokens) }));
			f.ForEach(f => values.Add(new List<string> { f.Name, "no", string.Empty, "/" + string.Join(" /", f.Tokens) }));
			Table.Display(new TableData(columns, values));

			if (command.LongHelpText != null)
			{
				Formatter.WriteLine(string.Empty);
				Formatter.WriteLines(command.LongHelpText.ToArray());
			}
		}
		private static void DisplayHelpForSpecificOption(BaseCommand<T> command, string usage, string tokenToProvideHelpFor, BaseOption optionToProvideHelpFor, bool tokenIsParameter, bool tokenIsFlag)
		{
			var headers = new List<string> { string.Empty, string.Empty };
			var data = new List<List<string>>();

			data.Add(new List<string> { "Command", command.FriendlyName });

			data.Add(new List<string> { "Option", optionToProvideHelpFor.Name });

			if (tokenIsParameter)
				usage += " -" + tokenToProvideHelpFor + " " + "(value)";
			else if (tokenIsFlag)
				usage += " /" + tokenToProvideHelpFor;
			data.Add(new List<string> { "Usage", usage });

			if (tokenIsParameter)
			{
				Parameter parameterToProvideHelpFor = optionToProvideHelpFor as Parameter;
				data.Add(new List<string> { "Parameter type", parameterToProvideHelpFor.ValueTypeDescription });
			}
			data.Add(new List<string> { "Purpose", optionToProvideHelpFor.ShortDescription });

			Formatter.WriteLines($"{{secondarytext}}Displaying help for the {{selectedtext}}{optionToProvideHelpFor.Name}{{reset}} option", string.Empty);
			Table.Display(new TableData(headers, data));

			Formatter.WriteLine();
			Formatter.WriteLines(optionToProvideHelpFor.ExtendedHelp.ToArray());
		}
	}
	public class HelpOptions : OptionContainer
	{
		public Parameter SubCommand = new SubCommandParameter();
		public HelpOptions(Dictionary<string, string> parameters, List<string> flags)
			: base(parameters, flags)
		{
		}
		public override List<Flag> Flags => new List<Flag>();
		public override List<Parameter> Parameters => new List<Parameter> { SubCommand };
	}
}
