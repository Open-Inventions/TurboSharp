namespace TurboCompile.API.External
{
    public interface IExtRefResolver
    {
        string Locate(IExternalRef external);
    }
}