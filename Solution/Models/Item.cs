using System.Collections.Generic;

namespace Park
{
    public class Item
    {
        public string _itemName { get; set; }
        public string _itemIcon { get; set; }
        public int _itemCount { get; set; }

        public Item(string itemName, string itemIcon, int itemCount)
        {
            _itemName = itemName;
            _itemIcon = itemIcon;
            _itemCount = itemCount;
        }
    }
}