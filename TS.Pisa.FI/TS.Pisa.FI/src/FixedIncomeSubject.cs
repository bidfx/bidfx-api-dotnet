namespace TS.Pisa.FI
{
    public class FixedIncomeSubject
    {
        public string Venue { get; internal set; }
        public string Isin { get; internal set; }
        private readonly string _pisaSubject;

        public FixedIncomeSubject(string venue, string isin)
        {
            Venue = venue;
            Isin = isin.Trim();
            _pisaSubject = "AssetClass=FixedIncome,Exchange=" + Venue + ",Level=1,Source=Lynx,Symbol=" + Isin;
        }

        public override string ToString()
        {
            return "Venue=" + Venue + ",ISIN=" + Isin;
        }

        internal string PisaSubject()
        {
            return _pisaSubject;
        }
    }
}