
namespace TS.Pisa.Plugin.Puffin
{
    internal interface IPuffinRequestor
    {
        void Subscribe(string subject);
        void Unsubscribe(string subject);
        void CloseSession();
        void CheckSessionIsActive();
    }

    internal class NullPuffinRequestor : IPuffinRequestor
    {
        public void Subscribe(string subject)
        {
        }

        public void Unsubscribe(string subject)
        {
        }

        public void CloseSession()
        {
        }

        public void CheckSessionIsActive()
        {
        }
    }
}