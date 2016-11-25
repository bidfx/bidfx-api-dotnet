using System;

namespace TS.Pisa.Plugin.Puffin
{
    internal interface IPuffinRequestor
    {
        /// <summary>
        /// Subscribe to price information.
        /// </summary>
        /// <param name="subject">the subject of the instrument to subscribe to.</param>
        void Subscribe(string subject);

        /// <summary>
        /// Unsubscribe to price information.
        /// </summary>
        /// <param name="subject">the subject of the instrument to unsubscribe from.</param>
        void Unsubscribe(string subject);

        /// <summary>
        /// Close the puffin session.
        /// </summary>
        void CloseSession();

        /// <summary>
        /// Check that the puffin session is still active, if not close it.
        /// </summary>
        void CheckSessionIsActive();
    }

    public class NullPuffinRequestor : IPuffinRequestor
    {
        public void Subscribe(string subject)
        {
            //do nothing
        }

        public void Unsubscribe(string subject)
        {
            //do nothing
        }

        public void CloseSession()
        {
            //do nothing
        }

        public void CheckSessionIsActive()
        {
            //do nothing
        }
    }
}