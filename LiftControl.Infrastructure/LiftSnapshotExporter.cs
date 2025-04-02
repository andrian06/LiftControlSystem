using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LiftControl.Domain;
using LiftControl.Shared;

namespace LiftControl.Infrastructure
{
    /// <summary>
    /// Responsible for exporting the current lift status as a JSON file
    /// that can be read by the Blazor UI.
    /// </summary>
    public static class LiftSnapshotExporter
    {
        /// <summary>
        /// Converts the internal LiftUnit state to a serializable LiftSnapshot
        /// and writes it as a JSON file to the Blazor UI's wwwroot folder.
        /// </summary>
        /// <param name="lifts">The current list of lifts.</param>
        /// <param name="path">The path to write the snapshot JSON file (default: Blazor's wwwroot).</param>
        public static void Export(List<LiftUnit> lifts, string relativePath = @"..\..\..\..\LiftControl.BlazorUI\wwwroot\snapshot.json")
        {
            try
            {
                // Resolve absolute path from current working directory
                string fullPath = Path.GetFullPath(relativePath);

                // Ensure the directory exists
                string? directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                // Prepare the snapshot data
                var snapshot = lifts.Select(lift => new LiftSnapshot
                {
                    LiftId = lift.Id,
                    CurrentFloor = lift.CurrentFloor,
                    IsInRoute = lift.IsInRoute,
                    Direction = lift.CurrentDirection.ToString(),
                    Queue = lift.Queue.Select(q => $"{q.PickupFloor}->{q.DestinationFloor}").ToList(),
                    Passengers = lift.Passengers.Select(p => $"{p.PickupFloor}->{p.DestinationFloor}").ToList()
                });

                var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });

                // Write to file
                File.WriteAllText(fullPath, json);

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"Snapshot exported to: {fullPath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Snapshot Export Error] {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
