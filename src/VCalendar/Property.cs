namespace Acklann.VCalendar
{
    public struct Property
    {
        public Property(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
    }
}