using System;
using SQLite;

namespace ShoppingList
{
    public class ShoppingItem
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set;}
        public string Item { get; set;}
        public decimal Quantity {get;set;}
        public string Notes {get;set;}
        public string Location {get;set;}
		public bool Completed {get;set;}
        public string ImagePath {get;set;}
        public byte[] Image {get;set;}
    }
}

