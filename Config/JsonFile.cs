using System.Configuration;
using System.Xml;

namespace imgmrg.Config
{
    public class JsonFile : ConfigurationElement
    {
        protected override void DeserializeElement(XmlReader reader, bool s)
        {
            Name = reader.GetAttribute("name");
            Value = (reader.ReadElementContentAs(typeof(string), null) as string ?? "").Trim();
        }
        public string Name { get; private set; }
        public string Value { get; private set; }
    }
}