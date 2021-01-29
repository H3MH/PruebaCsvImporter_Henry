namespace CsvImporter.Shell.Models
{
    using System;
    public class StockItem
    {
        public int Id { get; set; }
        public int PointOfSale { get; set; }
        public string Product { get; set; }
        public DateTime Date { get; set; }
        public int Stock { get; set; }
    }
}
