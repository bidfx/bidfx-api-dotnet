namespace TS.Pisa.FI.Example
{
    public class Program
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            var session = new FixedIncomeSession
            {
                Host = "ny-tunnel.qadev.tradingscreen.com",
//                Host = "ny-tunnel.uatdev.tradingscreen.com",
//                Host="localhost",Port=9901,Tunnel=false,
                Username = "axaapitest",
                Password = "B3CarefulWithThatAXAEug3n3!"
            };
            var test = 2;
            switch (test)
            {
                case 1:
                    new FieldAccessExample(session).Run();
                    break;
                case 2:
                    new SnapshotTimingExample(session).Run();
                    break;
            }
        }
    }
}