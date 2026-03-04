using Quartz.Models;
using Quartz.Services;

namespace Quartz
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // TODO: Boot up menu whit multiple options selected by arrow key.
            // Selection will be highlightet
            // Options: Start, Timeline and exit

            var engineService = new EngineService();
            var menuOperations = new MenuOperation(engineService);

            await menuOperations.ShowMainMenu();

        }
    }
}
