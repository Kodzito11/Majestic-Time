using System;
using System.Collections.Generic;
using System.Linq;

namespace WatchLibrary.Models
{
    public class CartItem
    {
        public int WatchId { get; set; }
        public int Quantity { get; set; }
        
        public decimal TotalPrice { get; set; }
        public int UserId { get; set; }

      
        private void ValidateCartItem()
        {
            if (Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Quantity), "You must add at least one item to your cart.");
            }

            
            if (TotalPrice <= 0)
            {
                throw new ArgumentException("Total Price must be greater than 0 and cannot be negative.", nameof(TotalPrice));
            }
        }


        public void Validate()
        {
            ValidateCartItem();

            // Tillad UserId = 0 for gæster, men ikke negative værdier
            if (UserId < 0)
            {
                throw new ArgumentException("Invalid UserId.", nameof(UserId));
            }

            if (WatchId <= 0)
            {
                throw new ArgumentException("Invalid WatchId.", nameof(WatchId));
            }

            if (Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Quantity), "Quantity must be greater than zero.");
            }
        }


        public void CalculateTotalPrice(decimal pricePerItem)
        {
            if (pricePerItem <= 0)
            {
                throw new ArgumentException("Price per item must be greater than zero.", nameof(pricePerItem));
            }

            TotalPrice = pricePerItem * Quantity;
        }

    
        public override string ToString()
        {
            return $"WatchId: {WatchId}, Quantity: {Quantity}, TotalPrice: {TotalPrice:C}, UserId: {UserId}";
        }
    }
}
