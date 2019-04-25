namespace Template.Models.ViewModels
{
    public class SelectListItem
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public string Group { get; set; }

        public SelectListItem(string name, int value) : this (name, value, string.Empty) { }

        public SelectListItem(string name, int value, string group)
        {
            Name = name;
            Value = value;
            Group = group;
        }
    }
}
