using System;

namespace BidFX.Public.API
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public String ProductSerial { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Https
        {
            get { return Port == 443; }
        }


        public UserInfo()
        {
            Port = 443;
        }
    }
}