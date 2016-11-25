using System;
using System.Collections.Generic;
using System.Reflection;

namespace TS.Pisa.FI.Example
{
    internal class PisaFIExample
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            new PisaFIExample().RunTest();
        }

        private void RunTest()
        {
            var session = new FIPisaSession
            {
                Host = "ny-tunnel.uatdev.tradingscreen.com",
                Username = "axaapitest",
                Password = "B3CarefulWithThatAXAEug3n3!"
            };
            session.OnPriceUpdate += OnPriceUpdate;
            session.OnPriceStatus += OnPriceStatus;
            session.Start();
            SendSubscriptions(session);
        }

        private static void OnPriceUpdate(object source, PriceUpdateEventArgs args)
        {
            Log.Info("price update: " + args);
        }

        private static void OnPriceStatus(object source, FIPriceStatusEventArgs args)
        {
            Log.Info("price status: " + args);
        }

        private void SendSubscriptions(FIPisaSession session)
        {
            foreach (var isin in System.IO.File.ReadLines("ISIN_list_10.txt"))
            {
               session.Subscribe("SGC", isin);
            }
        }
    }
}