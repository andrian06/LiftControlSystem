using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiftControl.Application;
using LiftControl.Config;
using LiftControl.Domain;
using LiftControl.Infrastructure;

namespace LiftControl.ConsoleApp
{
    public static class ManualTest
    {
        public static async Task Run()
        {
            List<LiftUnit> lifts = new();

            // Create and name each lift unit
            for (int i = 1; i <= SimulationConfig.NumberOfLifts; i++)
            {
                lifts.Add(new LiftUnit($"LiftUnit-{i}"));
            }


            // This handles routing requests to the best lift
            var coordinator = new StateCoordinator(lifts);

            // Start logging each lift’s activity to the console (so we can see what’s going on)
            DiagnosticLogger.Start(lifts);

            // Kick off each lift so it can start moving independently
            foreach (var lift in lifts)
            {
               Task.Run(() => lift.MoveAsync());
            }

            
            // Manually throw in some test requests, just like real passengers pressing buttons
            coordinator.RouteDirectionalRequest(3, Direction.Up);
            await Task.Delay(5000);
            coordinator.RouteDirectionalRequest(5, Direction.Down);
            await Task.Delay(5000);
            coordinator.RouteDirectionalRequest(9, Direction.Up);
            await Task.Delay(5000);
            coordinator.RouteDirectionalRequest(1, Direction.Up);
            await Task.Delay(5000);
            coordinator.RouteDirectionalRequest(10, Direction.Down);

            // Keep the app running so we can watch the simulation do its thing
            await Task.Delay(Timeout.Infinite);
        }

    }
}
