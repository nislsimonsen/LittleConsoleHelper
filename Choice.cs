namespace LittleConsoleHelper
{
	public class Choice
	{
		public string Text { get; set; }
		public object Value { get; set; }
		public Choice(string text, object value = null)
		{
			Text = text;
			Value = value;
		}
	}
}
