using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands
{
	public static class CLIParser
	{
		public static CLIParserResult Parse(params string[] args)
		{
			if (args.Any(a => a == null))
				throw new ArgumentNullException(nameof(args));

			var r = new CLIParserResult();

			for (var i = 0; i < args.Length; i++)
			{
				var a = args[i];
				if (a.StartsWith("/"))
				{
					r.Flags.Add(a[1..^0]);
				}
				else if (a.StartsWith("-"))
				{
					if (i < args.Length - 1)
					{
						var b = args[i + 1];
						if (!b.StartsWith("-") && !b.StartsWith("/"))
						{
							r.Parameters.Add(a[1..^0], b);
							i++;
						}
						else
							r.Parameters.Add(a[1..^0], "");
					}
					else
						r.Parameters.Add(a[1..^0], "");
				}
				else
				{
					r.Commands.Add(a);
				}
			}
			return r;
		}

	}
	public class CLIParserResult
	{
		public List<string> Commands { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
		public List<string> Flags { get; set; }

		public CLIParserResult()
		{
			Commands = new List<string>();
			Parameters = new Dictionary<string, string>();
			Flags = new List<string>();
		}
	}
}
