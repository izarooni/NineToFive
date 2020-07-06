﻿using NineToFive.Constants;

namespace NineToFive.Game.Entity {
    public class Pet : Meta.Entity {
        private int ID { get; set; }
        
        public Pet(int ID, int itemId) : base(EntityType.Pet) {
            this.ID = ID;
        }
    }
}