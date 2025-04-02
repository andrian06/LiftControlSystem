using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiftControl.Application;
using LiftControl.Config;
using LiftControl.Domain;

namespace LiftControl.Infrastructure
{
    /// <summary>
    /// Manages simulation setup, startup, and runtime flow.
    /// </summary>
    public class ConsoleRelay
    {
        public static List<LiftUnit> Lifts { get; private set; }
        public static StateCoordinator Coordinator { get; private set; }

        public static void StartSimulation()
        {
            Lifts = new();

            for (int i = 1; i <= SimulationConfig.NumberOfLifts; i++)
            {
                Lifts.Add(new LiftUnit($"LiftUnit-{i}"));
            }

            Coordinator = new StateCoordinator(Lifts);

            DiagnosticLogger.Start(Lifts);

            foreach (var lift in Lifts)
            {
                Task.Run(() => lift.MoveAsync());
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    // Skip request generation if all lifts are overloaded
                    bool anyAvailable = Lifts.Any(l =>
                        l.Passengers.Count + l.Queue.Count < SimulationConfig.MaxLiftLoadThreshold);

                    if (!anyAvailable)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("[Simulation] All lifts are at full capacity. Skipping request.");
                        Console.ResetColor();
                        await Task.Delay(1000); // Retry after 1 second
                        continue;
                    }

                    // Proceed with generating a random request
                    int pickup = Random.Shared.Next(1, SimulationConfig.NumberOfFloors + 1);
                    Direction direction = pickup == 1 ? Direction.Up :
                                          pickup == SimulationConfig.NumberOfFloors ? Direction.Down :
                                          (Random.Shared.Next(2) == 0 ? Direction.Up : Direction.Down);

                    Coordinator.RouteDirectionalRequest(pickup, direction);
                    await Task.Delay(SimulationConfig.RequestIntervalSeconds * 1000);
                }
            });

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
