using Model;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Service
{
    public class StockPriceService :IObservable<StockUpdate>
    {
        private class Unsubscriber : IDisposable
        {
            private ConcurrentDictionary<IObserver<StockUpdate>, object> _observers;
            private IObserver<StockUpdate> _observer;

            public Unsubscriber(ConcurrentDictionary<IObserver<StockUpdate>, object> observers, IObserver<StockUpdate> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                object o;
                if (_observer != null && _observers != null)
                    try
                    {
                        _observers.TryRemove(_observer, out o);
                    }
                    catch
                    { 
                        // swallow 
                    }
            }
        }

        readonly string stock1 = "Stock1";
        readonly string stock2 = "Stock2";
        readonly double stock1Max = 240.00;
        readonly double stock1Min = 270.00;
        readonly double stock2Max = 180.00;
        readonly double stock2Min = 210.00;
        int generationIntervalMilliseconds = 1000;
               
        ConcurrentDictionary<IObserver<StockUpdate>, object> observers;

        public StockPriceService()
        {
            observers = new ConcurrentDictionary<IObserver<StockUpdate>, object> ();
            StartGeneration();
        }

        private void StartGeneration()
        {
            System.Timers.Timer t = new System.Timers.Timer(generationIntervalMilliseconds);
            t.Elapsed += (s, e) => {

                var time = DateTime.Now;
                try
                {
                    // generate stock1 and stock2 price updates:
                    var stock1Update = new StockUpdate() { Price = GetRandomNumber(stock1Min, stock1Max), Stock = stock1, Time = DateTime.Now };
                    var stock2Update = new StockUpdate() { Price = GetRandomNumber(stock2Min, stock2Max), Stock = stock2, Time = DateTime.Now };
                    foreach (var kv in observers)
                    {
                        kv.Key.OnNext(stock1Update);
                        kv.Key.OnNext(stock2Update);
                    }
                }
                catch ( Exception ex)
                {
                    foreach (var kv in observers)
                    {
                        kv.Key.OnError(ex);
                    }
                }

                Debug.WriteLine($"{DateTime.Now}: stock service heartbeat");
            };
            t.Start();

            // notify observers
            foreach (var kv in observers)
                kv.Key.OnCompleted();

            // clear observers collection:
            while (observers.Count > 0)
            {
                observers.Clear();
            }
        }

        public double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            var value = random.NextDouble() * (maximum - minimum) + minimum;
            return Math.Round(value, 2);
        }

        public IDisposable Subscribe(IObserver<StockUpdate> observer)
        {  
            if (!observers.ContainsKey(observer))
            {
                observers.TryAdd(observer, null);
            }

            return new Unsubscriber(observers, observer);
        }
    }
}
