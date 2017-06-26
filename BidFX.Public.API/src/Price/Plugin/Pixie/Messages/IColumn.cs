namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public interface IColumn
    {
        int Size();

        object Get(int i);
    }
}