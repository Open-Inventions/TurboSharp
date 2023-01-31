using TurboRepo.API.External;

namespace TurboRepo.API
{
    public interface IExtRefResolver
    {
        string Locate(IExternalRef external);
    }
}