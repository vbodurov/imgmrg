using System.Configuration;

namespace imgmrg.Config
{
    public class JsonConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("jsonFiles", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(JsonFileCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public JsonFileCollection JsonFiles => (JsonFileCollection)this["jsonFiles"];
    }
}