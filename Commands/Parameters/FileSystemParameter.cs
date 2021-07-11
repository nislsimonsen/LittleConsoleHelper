using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public abstract class FileSystemParameter : Parameter
	{
		public bool ValidateExistingDirectory { get; protected set; }
		public FileSystemParameter(string name, string defaultValue, bool required, params string[] tokens) : base(name, defaultValue, required, tokens)
		{
		}
		public override bool Validate(out string validationError)
		{
			validationError = null;
			if (string.IsNullOrWhiteSpace(Value))
			{
				validationError = "Value cannot be null or empty";
				return false;
			}

			var illegalChars = new List<char> { '!', '#', '%', '&', '?', '*' };
			if (Value.ToCharArray().Any(c => illegalChars.Contains(c)))
			{
				validationError = $"'{Value}' is not a valid path";
				return false;
			}
			return true;
		}
	}

	public class FileParameter : FileSystemParameter
	{
		public bool ValidateExistingFile { get; private set; }

		public FileParameter(string name, string defaultValue, bool validateAgainstExistingLocalFile, bool validateAgainstExistingLocalDirectory, bool required, params string[] tokens) : base(name, defaultValue, required, tokens)
		{
			ValidateExistingFile = validateAgainstExistingLocalFile;
			ValidateExistingDirectory = validateAgainstExistingLocalDirectory;
		}
		public override string ValueTypeName { get; protected set; } = "File path";

		public override bool Validate(out string validationError)
		{
			var baseValid = base.Validate(out validationError);
			if (!baseValid)
				return false;


			try
			{
				if (ValidateExistingFile)
				{
					if (File.Exists(Value) || File.Exists(Path.Combine(Environment.CurrentDirectory, Value)))
						return true;
					else
					{
						validationError = $"The file '{Value}' does not exist";
						return false;
					}
				}
				if (ValidateExistingDirectory)
				{
					if (Directory.Exists(Value) || Directory.Exists(Path.Combine(Environment.CurrentDirectory, Value)))
						return true;
					else
					{
						validationError = $"The directory '{Value}' does not exist";
						return false;
					}
				}
			}
			catch (Exception e)
			{
				validationError = e.ToString();
				return false;
			}

			return true;
		}
	}

	public class DirectoryParameter : FileSystemParameter
	{
		public DirectoryParameter(string name, string defaultValue, bool validateAgainstExistingLocalDirectory, bool required, params string[] tokens) : base(name, defaultValue, required, tokens)
		{
			ValidateExistingDirectory = validateAgainstExistingLocalDirectory;
		}
		public override string ValueTypeName { get; protected set; } = "Directory path";
		public override bool Validate(out string validationError)
		{
			var baseValid = base.Validate(out validationError);
			if (!baseValid)
				return false;
			try
			{
				if (ValidateExistingDirectory)
				{
					if (Directory.Exists(Value) || Directory.Exists(Path.Combine(Environment.CurrentDirectory, Value)))
						return true;
					else
					{
						validationError = $"The directory '{Value}' does not exist";
						return false;
					}
				}
			}
			catch (Exception e)
			{
				validationError = e.ToString();
				return false;
			}
			return true;
		}
	}
}
