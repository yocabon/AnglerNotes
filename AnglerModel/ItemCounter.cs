using System.Collections.Generic;

namespace AnglerModel
{
    /// <summary>
    /// Simply store a list of <see cref="Item"/>
    /// </summary>
    public class ItemCounter
    {
        /// <summary>
        /// Simple non ordered list of <see cref="Item"/>
        /// </summary>
        public List<Item> ItemList { get; set; }

        /// <summary>
        /// Creates a new instance of ItemCounter with an empty list
        /// </summary>
        public ItemCounter()
        {
            ItemList = new List<Item>();
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
