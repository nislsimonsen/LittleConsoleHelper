using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace LittleConsoleHelper
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
			ColorSubHeader = GetColor(doc, "/LittleConsoleHelper/ColorScheme/SubHeader");
			ColorNote = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Note");
			ColorSuccess = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Success");
			ColorWarning = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Warning");
			ColorError = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Error");
			ColorInput = GetColor(doc, "/LittleConsoleHelper/ColorScheme/Input");
			ColorSecondaryInput = GetColor(doc, "/LittleConsoleHelper/ColorScheme/SecondaryInput");

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
		internal static ConsoleColor? ColorSubHeader { get; private set; }
		internal static ConsoleColor? ColorNote { get; private set; }
		internal static ConsoleColor? ColorSuccess { get; private set; }
		internal static ConsoleColor? ColorWarning { get; private set; }
		internal static ConsoleColor? ColorError { get; private set; }
		internal static ConsoleColor? ColorInput { get; private set; }
		internal static ConsoleColor? ColorSecondaryInput { get; private set; }

	}
}
