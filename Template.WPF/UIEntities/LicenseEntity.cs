namespace Template.WPF.UIEntities
{
    public class LicenseEntity
    {
        public string Name { get; }
        public string Sentence { get; }

        public LicenseEntity(string name, string sentence)
        {
            Name = name;
            Sentence = sentence;
        }
    }
}
