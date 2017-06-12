using System;
using System.Globalization;
using System.Net.NetworkInformation;

namespace BidFX.Public.API.Price.Tools
{
    public class ServiceProperties
    {
        public static string Environment(string host)
        {
            if (host != null)
            {
                if (host.Contains(".prod.")) return "PROD";
                if (host.Contains(".uatprod.")) return "UATPROD";
                if (host.Contains(".uatdev.")) return "UATDEV";
                if (host.Contains(".qaprod.")) return "QAPROD";
                if (host.Contains(".qadev.")) return "QADEV";
            }
            return "DEV";
        }

        public static string Host()
        {
            try
            {
                return IPGlobalProperties.GetIPGlobalProperties().HostName;
            }
            catch (Exception)
            {
                return "unknown";
            }
        }

        public static string Username()
        {
            try
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch (Exception)
            {
                return "unknown";
            }
        }

        public static string City()
        {
            try
            {
                var timeZone = TimeZoneInfo.Local.DisplayName;
                var split = timeZone.IndexOf('/');
                return split == -1 ? timeZone : timeZone.Substring(++split);
            }
            catch (Exception)
            {
                return "unknown";
            }
        }

        public static string Locale()
        {
            try
            {
                return CultureInfo.CurrentCulture.EnglishName;
            }
            catch (Exception)
            {
                return "unknown";
            }
        }
    }
}