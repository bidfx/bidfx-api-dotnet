namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// Allows to access data from a grid column received by the Pixie connection
    /// </summary>
    public interface IColumn
    {
        int Size();

        object Get(int i);
    }
}