namespace Percolore.Core
{
    public class ComboBoxItem
    {
        public int Value { get; set; }
        public string Display { get; set; }

        public ComboBoxItem(int value, string display)
        {
            Value = value;
            Display = display;
        }

        public override string ToString()
        {
            return Display;
        }
    }
}