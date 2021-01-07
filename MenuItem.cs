using System.Collections.Generic;

namespace LittleConsoleHelper
{
	public class MenuItem
	{
		public string Text{ get; set;  }
		public object Value { get; set; }
		public MenuItem Parent { get; set; }
		public List<MenuItem> Children { get; set; }
		internal bool IsExpanded { get; set; }
		public MenuItem(string text, MenuItem parent = null, object value = null)
		{
			Text = text;
			if (value != null)
				Value = value;
			else
				Value = text;
			Parent = parent;
			Children = new List<MenuItem>();
			if (parent != null)
				parent.Children.Add(this);
		}

		public bool IsRoot()
		{
			return Parent == null;
		}
		public int GetLevel(int i = -1)
		{
			if (Parent == null)
				return i;
			return Parent.GetLevel() + 1;
		}
	}
}
