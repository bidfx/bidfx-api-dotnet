using System.Text.RegularExpressions;

namespace TS.Pisa.FI
{
    /// <summary>
    /// A subject used to identify a fixed income subscription.
    /// </summary>
    public class FixedIncomeSubject
    {
        private static readonly Regex VenueRegex = new Regex(@"^\w+$");
        private static readonly Regex IsinRegex = new Regex(@"^\w{12}$");

        public string Venue { get; internal set; }
        public string Isin { get; internal set; }
        private readonly string _pisaSubject;

        /// <summary>
        /// Creates a new instance of a fixed income subject.
        /// </summary>
        /// <param name="venue">The trading venue that will publish pricing</param>
        /// <param name="isin">The ISIN code of the instrument</param>
        /// <exception cref="IllegalSubjectException">is the given parameters are invalid</exception>
        public FixedIncomeSubject(string venue, string isin)
        {
            Venue = Validate(venue, VenueRegex, "venue");
            Isin = Validate(isin, IsinRegex, "ISIN");
            _pisaSubject = "AssetClass=FixedIncome,Exchange=" + Venue + ",Level=1,Source=Lynx,Symbol=" + Isin;
        }

        private static string Validate(string code, Regex regex, string codeType)
        {
            if (code == null)
            {
                throw new IllegalSubjectException("null is not a valid " + codeType + " code");
            }
            if (regex.IsMatch(code))
            {
                return code;
            }
            throw new IllegalSubjectException("given code \"" + code + "\" is not a valid " + codeType + " code");
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