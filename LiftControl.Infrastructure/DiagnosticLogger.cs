using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftControl.Domain;

namespace LiftControl.Infrastructure
{
    /// <summary>
    /// Periodically logs the state of each lift in the simulation for monitoring/debugging.
    /// </summary>
    public static class DiagnosticLogger
    {
        /// <summary>
        /// Starts logging the current state of each lift at regular intervals.
        /// </summary>
        /// <param name="lifts">The list of lifts to monitor.</param>
        /// <param name="intervalSeconds">How often to log (in seconds).</param>
        public static void Start(List<LiftUnit> lifts, int intervalSeconds = 10)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("=== Lift Status Snapshot ===");

                    foreach (var lift in lifts)
                    {
                        // format pickup queue
                        string pickupQueue = lift.Queue.Any()
                            ? string.Join(", ", lift.Queue.Select(p => $"{p.PickupFloor}->{p.DestinationFloor}"))
                            : "None";

                        // Format onboard passengers
                        string onboard = lift.Passengers.Any()
                            ? string.Join(", ", lift.Passengers.Select(p => $"{p.PickupFloor}->{p.DestinationFloor}"))
                            : "None";

                        Console.WriteLine($"[{lift.Id}] Floor: {lift.CurrentFloor} | IsInRoute: {lift.IsInRoute}  | Dir: {lift.CurrentDirection} | Queue: {pickupQueue} | Passenger: {onboard}");
                    }

                    Console.ResetColor();

                    // Used for Frontend UI
                    LiftSnapshotExporter.Export(lifts);

                    await Task.Delay(intervalSeconds * 1000);
                }
            });
        }


    }
}
