using Autofac;
using Model;
using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Stock Price Client in C#\r");
            Console.WriteLine("------------------------\n");

            // dynamically load service from current application folder:
            IContainer container = null;
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<ProgramModule>();
            try
            {
                container = containerBuilder.Build();
            }
            catch (CannotFindStockPriceServiceException e)
            {
                Console.WriteLine($"Cannot Find Stock Price Service dll in the folder {Environment.CurrentDirectory}");
                Console.WriteLine("Please copy Service.dll or any other compatible dll to the folder");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            var service =  container.Resolve<IObservable<StockUpdate>>();

            // Ask the user to type the stock to track.
            ConsoleKeyInfo key;
            while (true)
            {
                Console.WriteLine(@"Type ""1"" or ""2"" to track ""Stock1"" or ""Stock2"" respectively");
                key = Console.ReadKey();
                if(key.KeyChar!='1' && key.KeyChar != '2')
                {
                    Console.WriteLine();
                    Console.WriteLine("Incorrect key. Please try again\n");
                    continue;
                }
                break;
            }

            string stock = key.KeyChar == '1' ? "Stock1" : "Stock2";

            Console.WriteLine();
            Console.WriteLine($"Subscribing to track {stock} ...\n");

            var client = new StockPriceClient(stock);
            IDisposable unsubscribe = service.Subscribe(client);
            Console.WriteLine("Subscribed!");
            Console.WriteLine("Press any key to unsubscribe ...\n");
            Console.ReadKey();

            unsubscribe.Dispose();

            Console.WriteLine("Subsciption ended, but the stock service is still running");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }
    }
}
