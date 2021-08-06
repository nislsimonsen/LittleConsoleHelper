using LittleConsoleHelper.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LittleConsoleHelper.UserInput
{
	public class Vocabulary
	{
		[XmlIgnore]
		public Dictionary<string, List<string>> WordsByType = new Dictionary<string, List<string>>();
		public List<string> SerializedInnerWordsByType = new List<string>();
		private char SerializationDivider = '¤';
		public void Init()
		{
			foreach (var pair in SerializedInnerWordsByType)
			{
				var parts = pair.Split(SerializationDivider);
				var key = parts[0];
				var value = parts[1];
				if (!WordsByType.ContainsKey(key))
					WordsByType.Add(key, new List<string>());
				WordsByType[key].Add(value);
			}
		}
		public void PrepareForWrite()
		{
			SerializedInnerWordsByType.Clear();
			foreach (var key in WordsByType.Keys)
			{
				foreach (var item in WordsByType[key])
				{
					SerializedInnerWordsByType.Add(key + SerializationDivider + item);
				}
			}
		}
		public List<string> GetByType(string type, string searchPattern)
		{
			if (!WordsByType.ContainsKey(type))
				return new List<string>();
			return WordsByType[type].Where(w => w.ToLower().Contains(searchPattern.ToLower())).ToList();
		}
		public void Add(string type, string newItem, bool bufferWrite = false)
		{
			if (!WordsByType.ContainsKey(type))
				WordsByType.Add(type, new List<string>());
			var existing = WordsByType[type].FirstOrDefault(i => i.ToLower().Equals(newItem.ToLower()));
			if (existing == null)
				WordsByType[type].Add(newItem);
			if(!bufferWrite)
				WriteToFile(FilePath);
		}

		internal void AddParameterValue(Parameter p)
		{
			if(p.IsDefined())
				Add(p.ValueTypeName, p.Value);
		}

		private string FilePath { get; set; }
		public static Vocabulary ReadFromFile(string filePath)
		{
			if (!File.Exists(filePath))
				return new Vocabulary() { FilePath = filePath };
			Vocabulary r;
			XmlSerializer serializer = new XmlSerializer(typeof(Vocabulary));
			using (var reader = new StreamReader(filePath))
				r = (Vocabulary)serializer.Deserialize(reader);
			r.FilePath = filePath;
			r.Init();
			return r;
		}
		public void WriteToFile(string filePath)
		{
			PrepareForWrite();
			XmlSerializer serializer = new XmlSerializer(this.GetType());
			using (var writer = new StreamWriter(new FileStream(filePath, FileMode.Create)))
			{
				serializer.Serialize(writer, this);
				writer.Flush();
				writer.Close();
			}
		}
	}

}
