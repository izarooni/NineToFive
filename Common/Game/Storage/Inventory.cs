﻿using System.Collections.Generic;
using NineToFive.Constants;

namespace NineToFive.Game.Storage {
    public enum InventoryType {
        Equipped,
        Equip,
        Use,
        Setup,
        Etc,
        Cash
    }

    public class Inventory {
        private Dictionary<short, Item> _items = new Dictionary<short, Item>();

        public Inventory(InventoryType type, int size = 32) {
            Type = type;
            Size = size;
        }

        public InventoryType Type { get; }
        public int Size { get; }
        public Dictionary<short, Item>.ValueCollection Items => _items.Values;

        public bool EquipItem(Equip equip, bool replace = false) {
            equip.BagIndex = (short) -ItemConstants.GetBodyPartFromId(equip.Id);
            if (replace) _items.Remove(equip.BagIndex);
            return _items.TryAdd(equip.BagIndex, equip);
        }

        public Item this[short bagIndex] {
            get {
                _items.TryGetValue(bagIndex, out Item item);
                return item;
            }
        }
    }

    enum EntityType { }
}