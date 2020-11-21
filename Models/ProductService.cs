using System;
using System.Threading.Tasks;

namespace NetNote.Models
{
    public class ProductService
    {
        private static int stockCount = 10;

        public static int StockCount { get => stockCount; set => stockCount = value; }
        public async Task<bool> BuyAsync()
        {
            await Task.Delay(new Random().Next(100, 500));
            if (StockCount > 0) { StockCount--; return true; }
            return false;
        }
    }
}