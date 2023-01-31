namespace TurboMeta.API.File
{
    public interface IFileLoader<T>
    {
        T Load(string path, IFileLoader<T> parent = null);
    }
}