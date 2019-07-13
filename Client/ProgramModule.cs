using Autofac;
using Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Client
{
    class ProgramModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // scan current directory for IObservable<StockUpdate> service dll:
            var observableStockUpdateTypes = Directory
                .EnumerateFiles(Environment.CurrentDirectory)
                .Where(f => f.EndsWith(".dll"))
                .Select(f => Assembly.LoadFrom(f))
                .SelectMany(assembly => assembly.GetTypes().Where(type => typeof(IObservable<StockUpdate>).IsAssignableFrom(type) && type.IsClass));

            if (observableStockUpdateTypes.Count() == 0)
                throw new CannotFindStockPriceServiceException();
            else if (observableStockUpdateTypes.Count() > 1)
                Debug.WriteLine("WARNING: More than one IObservable<StockUpdate> type found in the current folder. The last one wins.");
            else
                foreach(var t in observableStockUpdateTypes)
                    builder.RegisterType(t).As<IObservable<StockUpdate>>();
        }
    }
}
