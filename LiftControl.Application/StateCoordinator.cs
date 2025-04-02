using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftControl.Config;
using LiftControl.Domain;

namespace LiftControl.Application
{
    /// <summary>
    /// Keeps all the lifts in sync and allows sending passenger requests manually or from simulation.
    /// </summary>
    public class StateCoordinator
    {
        private readonly List<LiftUnit> _liftUnits;

        /// <summary>
        /// Initializes the coordinator with the list of managed lift units.
        /// </summary>
        /// <param name="liftUnits">All the lift units in the system.</param>
        public StateCoordinator(List<LiftUnit> liftUnits)
        {
            _liftUnits = liftUnits;
        }

        /// <summary>
        /// Starts all lifts asynchronously. This usually keeps running unless the system stops.
        /// </summary>
        /// <returns>A Task representing the combined lift operations.</returns>
        public async Task RunAsync()
        {
            var tasks = _liftUnits.Select(lift => lift.MoveAsync());
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends a request from a pickup floor in a specified direction, generating a destination realistically.
        /// Used by both manual and automated scenarios.
        /// </summary>
        /// <param name="pickupFloor">The floor where the user is requesting a lift.</param>
        /// <param name="direction">Direction the user wants to go (up or down).</param>
        public void RouteDirectionalRequest(int pickupFloor, Direction direction)
        {
            var router = new RequestRouter(_liftUnits);

            // Prevent invalid requests like "go up" from top floor
            if (!IsValidDirection(pickupFloor, direction))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Can't go {direction} from floor {pickupFloor} — no floors in that direction.");
                Console.ResetColor();
                return;
            }

            // Simulate a realistic destination in the requested direction
            int destination = direction switch
            {
                Direction.Up => new Random().Next(pickupFloor + 1, SimulationConfig.NumberOfFloors + 1),
                Direction.Down => new Random().Next(1, pickupFloor),
                _ => pickupFloor
            };

            var request = new PassengerRequest(pickupFloor, destination);
            router.RoutePassenger(request);
        }

        /// <summary>
        /// Determines whether the requested direction is valid from the given floor.
        /// </summary>
        private bool IsValidDirection(int pickupFloor, Direction direction)
        {
            return direction switch
            {
                Direction.Up => pickupFloor < SimulationConfig.NumberOfFloors,
                Direction.Down => pickupFloor > 1,
                _ => false
            };
        }

    }
}
