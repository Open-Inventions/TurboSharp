namespace TurboCompile.API
{
    public interface IRefLoader<out T>
    {
        T LoadFrom(byte[] bytes);

        T LoadFrom(string path);
    }
}