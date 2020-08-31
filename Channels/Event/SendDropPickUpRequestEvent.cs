﻿using System;
using System.Numerics;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using NineToFive.Net;

namespace NineToFive.Event {
    public class SendDropPickUpRequestEvent : PacketEvent {

        private uint _objectId;
        
        public SendDropPickUpRequestEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadByte();  // type  
            p.ReadInt();   // update time
            p.ReadShort(); // player location x
            p.ReadShort(); // player location y
            _objectId = p.ReadUInt();   // v9
            p.ReadInt();   // dwID
            
            return true;
        }

        public override void OnHandle() {
            User user = Client.User;
            Drop drop = user.Field.LifePools[EntityType.Drop][_objectId] as Drop;
            if (drop == null) return;

            InventoryType inventoryType = ItemConstants.GetInventoryType((int) drop.Id);
            Inventory inventory = user.Inventories[inventoryType];
            
            if (drop.Money > 0) {
                
            } else if (inventory.AddItem(drop.Item)) {
                
            }
        }
    }
}