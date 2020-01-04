using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnglerModel
{
    /// <summary>
    /// Simply store a list of <see cref="Item"/>
    /// </summary>
    public class ItemCounter
    {
        private const string Pattern = @"^(?<name>.*)(\ ---\ )(?<count>\d+)$";

        /// <summary>
        /// Sync to human readble text file. This is the filename.
        /// </summary>
        public string SyncFilename { get; set; }

        /// <summary>
        /// Simple non ordered list of <see cref="Item"/>
        /// </summary>
        public List<Item> ItemList { get; set; }

        /// <summary>
        /// Creates a new instance of ItemCounter with an empty list
        /// </summary>
        public ItemCounter()
        {
            SyncFilename = "";
            ItemList = new List<Item>();
        }

        public override string ToString()
        {
            return string.Join("", ItemList.Select(item => item.Name + " --- " + item.Count + "\n"));
        }

        public void LoadFromString(string content)
        {
            if (string.IsNullOrWhiteSpace(SyncFilename))
                return;
            
            string[] lines = content.Split( new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            ItemList = new List<Item>();
            foreach(string line in lines)
            {
                var match = Regex.Match(line, Pattern);
                if (match.Success)
                {
                    ItemList.Add(new Item() { Name = match.Groups["name"].Value, Count = int.Parse(match.Groups["count"].Value) });
                }
            }
        }
    }

    /// <summary>
    /// A pair (Name, Count)
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Creates a new instance of Item with given values
        /// </summary>
        public Item(string name, int count)
        {
            Name = name;
            Count = count;
        }

        /// <summary>
        /// Creates a new instance of Item with value ("", 0)
        /// </summary>
        public Item()
        {
            Name = "";
            Count = 0;
        }

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A number associated with the name
        /// </summary>
        public int Count { get; set; }
    }
}
