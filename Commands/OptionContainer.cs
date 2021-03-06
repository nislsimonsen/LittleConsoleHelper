using LittleConsoleHelper.Commands.Parameters;
using LittleConsoleHelper.Display;
using LittleConsoleHelper.UserInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public abstract class OptionContainer
	{

		protected OptionContainer()
		{ }
		public OptionContainer(Dictionary<string, string> parameters, List<string> flags)
		{
			CreationParameters = parameters;
			CreationFlags = flags;
			foreach (var p in Parameters)
				ParseParameter(p, parameters);
			foreach (var f in Flags)
				ParseFlag(f, flags);
		}
		protected void ParseFlag(Flag f, List<string> args)
		{
			foreach (var t in f.Tokens)
				if (args.ToList().Contains(t))
				{
					f.IsSet = true;
					break;
				}
		}
		protected void ParseParameter(Parameter p, Dictionary<string, string> parameters)
		{
			foreach (var t in p.Tokens)
				if (parameters.ContainsKey(t))
				{
					p.Value = parameters[t];
					break;
				}
		}
		public abstract List<Flag> Flags { get; }
		public abstract List<Parameter> Parameters { get; }
		public List<BaseOption> Options
		{
			get
			{
				var r = new List<BaseOption>();
				r.AddRange(Flags.Cast<BaseOption>().ToList());
				r.AddRange(Parameters.Cast<BaseOption>().ToList());
				return r;

			}
		}
		protected List<string> CreationFlags { get; set; }
		protected Dictionary<string, string> CreationParameters { get; set; }

		private static Vocabulary AutocompleteVocabulary { get; set; }
		public static void UseVocabulary(Vocabulary vocabulary)
		{
			AutocompleteVocabulary = vocabulary;
		}

		public virtual bool EnsureRequiredParametersAndValidate()
		{
			Parameters.ForEach(p => p.EnsureRequired(AutocompleteVocabulary));
			var r = Validate(CreationParameters, CreationFlags);

			if (AutocompleteVocabulary != null)
			{
				foreach (var p in Parameters)
				{
					AutocompleteVocabulary.AddParameterValue(p);
				}
			}

			return r;
		}
		/// <summary>
		/// Validates registered parameters which have a value. Does not validate value-less parameters.
		/// </summary>
		/// <returns>False if any parameters fail validation</returns>
		public virtual bool Validate(Dictionary<string, string> parameters, List<string> flags)
		{
			foreach (var p in Parameters)
			{
				if (!p.Required && !p)
					return true;
				if ((p || p.Required) && !p.Validate(out var errorMessage))
				{
					var error = $"{{error}}Validation error for parameter '{p.Name}':";
					Formatter.WriteLines(error, $"{{error}}{errorMessage}");

					return false;
				}
			}

			if (!VerifyParametersAreKnown(parameters))
				return false;
			if (!VerifyFlagsAreKnown(flags))
				return false;
			return true;
		}

		private bool VerifyParametersAreKnown(Dictionary<string, string> parameters)
		{
			foreach (var p in parameters.Keys)
			{
				if (!Parameters.Any(P => P.Tokens.Contains(p)))
				{
					Formatter.WriteLines($"{{error}}Unknown parameter pair '{p}':'{parameters[p]}'");
					return false;
				}
			}
			return true;
		}

		private bool VerifyFlagsAreKnown(List<string> flags)
		{
			foreach (var f in flags)
			{
				if (!Flags.Any(F => F.Tokens.Contains(f)))
				{
					Formatter.WriteLines($"{{error}}Unknown flag '{f}'");
					return false;
				}
			}
			return true;
		}
	}
}
