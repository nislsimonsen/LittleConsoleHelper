using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace LittleConsoleHelper.Config
{
	public static class Configuration
	{

		static string configFileName = "LittleConsoleHelper.config";
		static ConsoleColor defaultColor;
		static Configuration()
		{
			defaultColor = Console.ForegroundColor;
			LoadConfigFile();
		}
		static void Reset()
		{
			Console.ForegroundColor = defaultColor;
		}

		static void LoadConfigFile()
		{
			var dir = new DirectoryInfo(Environment.CurrentDirectory);

			string path = null;
			while (path == null)
			{
				if (File.Exists(dir + "\\" + configFileName))
					path = dir + "\\" + configFileName;
				try
				{
					if (dir.Parent == null)
						break;
					dir = dir.Parent;
				}
				catch { break; }
			}
			if (path == null)
				return;
			XmlDocument doc = new XmlDocument();
			doc.Load(path);

			ColorText = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Text");
			ColorSelectedText = GetColor(doc, "/LittleConsoleHelper/ColorScheme/SelectedText");
			ColorSecondaryText = GetColor(doc, "/LittleConsoleHelper/ColorScheme/SecondaryText");
			ColorTextBG = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Background");
			ColorSelectedTextBG = GetColor(doc, "/LittleConsoleHelper/ColorScheme/SelectedBackground");
			ColorSecondaryTextBG = GetColor(doc, "/LittleConsoleHelper/ColorScheme/SecondaryBackground");
			ColorHeader = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Header");
			ColorSuccess = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Success");
			ColorWarning = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Warning");
			ColorError = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Error");
			ColorInput = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Input");

			//TODO: Redo this so that mapping and "used" are not stored twice
			var staticColorPaths = new List<string>
			{
				"Text",
				"SelectedText",
				"SecondaryText",
				"Background",
				"SelectedBackground",
				"SecondaryBackground",
				"Header",
				"Success",
				"Warning",
				"Error",
				"Input",
			};
			var colorNodes = doc.SelectSingleNode("/LittleConsoleHelper/ColorScheme").ChildNodes;
			for(var i=0;i<colorNodes.Count;i++)
			{
				var node = colorNodes[i];
				var name = node.Name;
				if (staticColorPaths.Contains(name))
					continue;
				if (CustomColors.ContainsKey(node.Name))
					continue;
				if (!Enum.TryParse(node.InnerText, out ConsoleColor color))
					continue;

				CustomColors.Add(name, color);
			}
		}
		static ConsoleColor? GetColor(XmlDocument doc, string xpath)
		{
			var node = doc.SelectSingleNode(xpath);
			if (node != null)
			{
				ConsoleColor r;
				if (Enum.TryParse(node.InnerText, out r))
					return r;
			}
			return null;
		}

		internal static ConsoleColor? ColorText { get; private set; }
		internal static ConsoleColor? ColorSelectedText { get; private set; }
		internal static ConsoleColor? ColorSecondaryText { get; private set; }
		internal static ConsoleColor? ColorTextBG { get; private set; }
		internal static ConsoleColor? ColorSelectedTextBG { get; private set; }
		internal static ConsoleColor? ColorSecondaryTextBG { get; private set; }
		internal static ConsoleColor? ColorHeader { get; private set; }
		internal static ConsoleColor? ColorSuccess { get; private set; }
		internal static ConsoleColor? ColorWarning { get; private set; }
		internal static ConsoleColor? ColorError { get; private set; }
		internal static ConsoleColor? ColorInput { get; private set; }
		internal static Dictionary<string, ConsoleColor> CustomColors { get; private set; } = new Dictionary<string, ConsoleColor>();
	}
}
