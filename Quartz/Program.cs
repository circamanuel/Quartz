using Quartz.Models;
using Quartz.Services;

namespace Quartz
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var engineService = new EngineService();
            var menuOperations = new MenuOperation(engineService);
            
            // Starts application
            await menuOperations.ShowMainMenu();

        }
    }
}
