using System;
using System.IO;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class PixieProtocolOptions
    {
        private int _version = PixieVersion.CurrentVersion;
        private int _heartbeat = 15;
        private int _idle = 120;
        private int _minti = 0;
        private long _subscriptionInterval = 250;
        private bool _compressSubscriptions = true;

        public bool CompressSubscriptions
        {
            get { return _compressSubscriptions; }
            set { _compressSubscriptions = value; }
        }

        public long SubscriptionInterval
        {
            get { return _subscriptionInterval; }
            set { _subscriptionInterval = Params.InRange(value, 25L, 5000L); }
        }

        public int Version
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
            set { _heartbeat = Params.InRange(value, 1, 90); }
        }

        public int Idle
        {
            get { return _idle; }
            set { _idle = Params.InRange(value, 5, 600); }
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
            string signature = "pixie://localhost:9902?version=" + Version + "&heartbeat=" + Heartbeat + "&idle=" +
                            Idle;
            if (Minti != 0)
            {
                signature += "&minti=" + Minti;
            }

            return signature + '\n';
        }
    }
}