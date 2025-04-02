using System.Threading.Tasks;
using LiftControl.ConsoleApp;
using LiftControl.Infrastructure;

class Program
{
    static async Task Main()
    {
        bool runManualTest = false; // Set to false to run simulation

        if (runManualTest)
            await ManualTest.Run();
        else
            ConsoleRelay.StartSimulation();
    }
}
