namespace ToyCompiler.HostLayer
{
    public interface IObjectGetter
    {
        bool GetObject(string name, out object? obj);
    }
}
