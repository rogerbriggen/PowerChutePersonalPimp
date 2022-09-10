// Roger Briggen license this file to you under the MIT license.
//

using Microsoft.Extensions.Logging;

namespace RogerBriggen.PowerChutePersonalPimpLib;
public class PowerChuteWatcherService
{
    private readonly ILogger<PowerChuteWatcherService> _logger;
    public PowerChuteWatcherService(ILogger<PowerChuteWatcherService> logger)
    {
        _logger = logger;
    }

    public bool InitFromFile(string configFile)
    {
        return false;
    }

    public void StartWatching()
    {
        
    }

    public void StopWatching()
    {

    }
}
