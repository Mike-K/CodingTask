using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class StockPriceClient : IObserver<StockUpdate>
    {
        public string StockName { get; private set; }
        public List<StockUpdate> PriceHistory { get; private set; }

        public StockPriceClient(string stockName)
        {
            StockName = stockName;
            PriceHistory = new List<StockUpdate>();
        }

        public void OnCompleted()
        {
            // do nothing
        }

        public void OnError(Exception error)
        {
            // do nothing
        }

        public void OnNext(StockUpdate value)
        {
            if (value.Stock == this.StockName)
            {
                PriceHistory.Add(value);
                Console.WriteLine($"{value.Time}: {value.Stock} price: {value.Price}");
            }
        }
    }
}
