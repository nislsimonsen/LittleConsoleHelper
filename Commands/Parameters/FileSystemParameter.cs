using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LittleConsoleHelper.Commands.Parameters
{
	public abstract class FileSystemParameter : Parameter
	{
		public FileSystemValidationMode FolderValidation { get; set; }
		public FileSystemParameter(string name, string defaultValue, bool required, FileSystemValidationMode folderValidation, params string[] tokens) : base(name, defaultValue, required, tokens)
		{
			FolderValidation = folderValidation;
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
			//TODO: Check if this really works as intended - a file parameter can be a full path (= validation should validate existing/new/none folder AND existing/new/none file)
			var directory = Path.GetDirectoryName(Value);
			switch (FolderValidation)
			{
				case FileSystemValidationMode.NoValidation:
					break;
				case FileSystemValidationMode.Exists:
					if (!Directory.Exists(directory) && !Directory.Exists(Path.Combine(Environment.CurrentDirectory, Value)))
					{
						validationError = $"The folder '{directory}' does not exist";
						return false;
					}
					break;
				case FileSystemValidationMode.DoesNotExist:
					if (Directory.Exists(directory) || Directory.Exists(Path.Combine(Environment.CurrentDirectory, Value)))
					{
						validationError = $"The folder '{directory}' already exists";
						return false;
					}
					break;
				default:
					break;
			}

			return true;
		}
	}

	public enum FileSystemValidationMode
	{ 
		NoValidation,
		Exists,
		DoesNotExist
	}
	public class FileParameter : FileSystemParameter
	{
		public FileSystemValidationMode FileValidation { get; private set; }
		

		public FileParameter(string name, string defaultValue, FileSystemValidationMode fileValidation, FileSystemValidationMode folderValidation, bool required, params string[] tokens) : base(name, defaultValue, required, folderValidation, tokens)
		{
			FileValidation = fileValidation;
		}
		public override string ValueTypeName { get; protected set; } = "File path";

		public override bool Validate(out string validationError)
		{
			var baseValid = base.Validate(out validationError);
			if (!baseValid)
				return false;

			switch (FileValidation)
			{
				case FileSystemValidationMode.NoValidation:
					break;
				case FileSystemValidationMode.Exists:
					if (!File.Exists(Value) && !File.Exists(Path.Combine(Environment.CurrentDirectory, Value)))
					{
						validationError = $"The folder '{Value}' does not exist";
						return false;
					}
					break;
				case FileSystemValidationMode.DoesNotExist:
					if (File.Exists(Value) || File.Exists(Path.Combine(Environment.CurrentDirectory, Value)))
					{
						validationError = $"The folder '{Value}' already exists";
						return false;
					}
					break;
				default:
					break;
			}

			return true;
		}
	}

	public class DirectoryParameter : FileSystemParameter
	{
		public DirectoryParameter(string name, string defaultValue, bool required, FileSystemValidationMode folderValidation, params string[] tokens) : base(name, defaultValue, required, folderValidation, tokens)
		{
		}
		public override string ValueTypeName { get; protected set; } = "Directory path";
		public override bool Validate(out string validationError)
		{
			var baseValid = base.Validate(out validationError);
			if (!baseValid)
				return false;
			try
			{
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
