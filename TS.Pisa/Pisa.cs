using System.Reflection;

namespace TS.Pisa
{
    public class Pisa
    {
        public const string Name = "Pisa.NET";
        public static readonly Assembly Reference = typeof(Pisa).Assembly;
        public static readonly string Package = Reference.GetName().Name;
        public static readonly string Version = Reference.GetName().Version.ToString();
    }
}