namespace TS.Pisa.FI.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var session = new FixedIncomeSession
            {
                Host = "ny-tunnel.qadev.tradingscreen.com",
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