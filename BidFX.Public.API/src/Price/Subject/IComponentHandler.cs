namespace BidFX.Public.API.Price.Subject
{
    internal interface IComponentHandler
    {
        /// <summary>
        /// Handles a subject components.
        /// </summary>
        /// <param name="key">the subject components key</param>
        /// <param name="value">the subject components value</param>
        void SubjectComponent(string key, string value);
    }
}