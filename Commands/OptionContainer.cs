using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public abstract class OptionContainer
	{
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
		public abstract List<Flag> Flags { get; }
		public abstract List<Parameter> Parameters { get; }
		protected List<string> CreationFlags { get; set; }
		protected Dictionary<string, string> CreationParameters { get; set; }

		[Obsolete("Use parameterless overload instead")]
		public virtual bool EnsureRequiredParametersAndValidate(Dictionary<string, string> parameters, List<string> flags)
		{
			Parameters.ForEach(p => p.EnsureRequired());
			return Validate(CreationParameters, CreationFlags);
		}
		public virtual bool EnsureRequiredParametersAndValidate()
		{
			Parameters.ForEach(p => p.EnsureRequired());
			var r = Validate(CreationParameters, CreationFlags);

			return r;
		}
		public virtual bool Validate(Dictionary<string, string> parameters, List<string> flags)
		{
			foreach (var p in Parameters)
			{
				if (p && !p.Validate(out var errorMessage))
				{
					var error = $"Validation error for parameter '{p.Name}':";
					Formatter.WriteLines(error, errorMessage);
					
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
