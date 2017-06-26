using System;
using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    public class PixieProtocolOptions
    {
        private uint _version = 3;
        private int _heartbeat = 15;
        private int _idle = 120;
        private int _minti = 0;

        public uint Version
        {
            get { return _version; }
            set
            {
                if (value > Version)
                {
                    throw new ArgumentException("protocol version (" + value + ") > " + Version);
                }
                _version = value;
            }
        }

        public int Heartbeat
        {
            get { return _heartbeat; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("heartbeat interval (" + value + ") < 1 second");
                }
                if (value > 90)
                {
                    throw new ArgumentException("heartbeat interval (" + value + ") > 90 seconds");
                }
                _heartbeat = value;
            }
        }

        public int Idle
        {
            get { return _idle; }
            set
            {
                if (value < 5)
                {
                    throw new ArgumentException("idle interval (" + value + ") < 5 seconds");
                }
                if (value > 600)
                {
                    throw new ArgumentException("idle interval (" + value + ") > 10 minutes");
                }
                _idle = value;
            }
        }

        public int Minti
        {
            get { return _minti; }
            set
            {
                if (value < 10)
                {
                    throw new ArgumentException("minimum throttle interval (" + value + ") < 10 milliseconds");
                }
                _minti = value;
            }
        }

        public void ConfigureStream(Stream stream)
        {
            stream.ReadTimeout = Idle * 1000;
        }

        public string GetProtocolSignature()
        {
            var signature = "pixie://localhost:9902?version=" + Version + "&heartbeat=" + Heartbeat + "&idle=" +
                            Idle;
            if (Minti != 0) signature += "&minti=" + Minti;
            return signature + '\n';
        }
    }
}