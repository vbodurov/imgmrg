using System;
using System.Configuration;

namespace imgmrg.Config
{
    public class JsonFileCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new JsonFile();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JsonFile) element).Name;
        }
        public new string this[string name]
        {
            get
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var file = (JsonFile)e.Current;
                    if (string.Equals(name, file.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return file.Value;
                    }
                }
                return null;
            }
        }
    }
}