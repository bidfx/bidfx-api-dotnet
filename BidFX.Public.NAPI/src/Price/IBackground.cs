namespace BidFX.Public.NAPI.Price
{
    /// <summary>
    /// An interface used for stopping and starting API components with long lived background tasks.
    /// </summary>
    public interface IBackground
    {
        /// <summary>
        /// Starts background task, spining up any required threads.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops background tasks, shutting down any associated threads.
        /// </summary>
        void Stop();

        /// <summary>
        /// Checks if the background task is running (has been started and not stopped).
        /// </summary>
        bool Running { get; }
    }
}