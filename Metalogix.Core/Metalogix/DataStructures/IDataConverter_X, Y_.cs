namespace Metalogix.DataStructures
{
    public interface IDataConverter<X, Y>
    {
        Y Convert(X oValue);
    }
}