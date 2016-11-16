using System;
using System.Reflection;

namespace TS.Pisa
{
    public class PisaVersion
    {
        public static readonly Assembly Reference = typeof(PisaVersion).Assembly;
        public static readonly Version Version = Reference.GetName().Version;
    }
}