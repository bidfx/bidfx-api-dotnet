
namespace TS.Pisa.Plugin.Puffin
{
    internal interface IPuffinRequestor : ISubscriber, IBackground
    {
    }

    internal class NullPuffinRequestor : IPuffinRequestor
    {
        public void Subscribe(string subject)
        {
        }

        public void Unsubscribe(string subject)
        {
        }

        public void Stop()
        {
        }

        public void Start()
        {
        }
    }
}