using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftControl.Config;

namespace LiftControl.Domain
{
    /// <summary>
    /// Represents a single lift in the system. Handles movement, passenger boarding/unboarding, and request queuing.
    /// </summary>
    public class LiftUnit
    {
        public string Id { get; }

        /// <summary>
        /// Current floor where the lift is located.
        /// </summary>
        public int CurrentFloor { get; set; }

        /// <summary>
        /// Current direction the lift is moving in.
        /// </summary>
        public Direction CurrentDirection { get; set; } = Direction.Idle;

        /// <summary>
        /// Passengers currently onboard the lift.
        /// </summary>
        public List<PassengerRequest> Passengers { get; } = new();

        /// <summary>
        /// Pickup requests waiting to be handled by this lift.
        /// </summary>
        public List<PassengerRequest> Queue { get; private set; } = new();

        /// <summary>
        /// Gets a value indicating whether this instance is moving.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is moving; otherwise, <c>false</c>.
        /// </value>
        public bool IsInRoute { get; private set; } = false;

        /// <summary>
        /// Initializes the lift with the given identifier and starts at floor 1.
        /// </summary>
        /// <param name="id">Unique identifier for this lift.</param>
        public LiftUnit(string id)
        {
            Id = id;
            CurrentFloor = 1;
        }

        /// <summary>
        /// Main loop that moves the lift, handles passengers, and reacts to requests.
        /// </summary>
        public async Task MoveAsync()
        {
            while (true)
            {
                // Always try to unload and board first
                UnloadArrivedPassengers();
                BoardWaitingPassengers();
                UpdateDirection();

                // Re-check if we have anything to do
                if (!Queue.Any() && !Passengers.Any())
                {
                    CurrentDirection = Direction.Idle;
                    IsInRoute = false;
                    await Task.Delay(500);
                    continue;
                }

                int? nextStop = GetNextStop();

                if (!nextStop.HasValue)
                {
                    IsInRoute = false;
                    await Task.Delay(500);
                    continue;
                }

                if (nextStop != CurrentFloor)
                {
                    IsInRoute = true; 
                }

                // Handle stop if we are already at the right floor
                if (nextStop == CurrentFloor)
                {
                    IsInRoute = false;
                    await Task.Delay(SimulationConfig.SecondsForLoading * 1000);
                    continue; // loop again to re-check state
                }

                // Simulate travel to next floor
                int floorsToMove = Math.Abs(CurrentFloor - nextStop.Value);
                Console.WriteLine($"{Id} moving from floor {CurrentFloor} to {nextStop.Value}");

                IsInRoute = true;
                await Task.Delay(SimulationConfig.SecondsPerFloor * 1000 * floorsToMove);
                CurrentFloor = nextStop.Value;

                UnloadArrivedPassengers();
                BoardWaitingPassengers();

                await Task.Delay(SimulationConfig.SecondsForLoading * 1000);
                UpdateDirection();
                IsInRoute = false;
            }
        }


        /// <summary>
        /// Assigns a new passenger request to this lift.
        /// </summary>
        /// <param name="request">The passenger request to queue.</param>
        public void AssignPassenger(PassengerRequest request)
        {
            if (IsDuplicateRequest(request))
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"{Id} duplicate ignored: {request.PickupFloor}->{request.DestinationFloor} (already in queue or onboard)");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"{Id} assigned passenger: {request.PickupFloor}->{request.DestinationFloor}");

            Queue.Add(request);
            SortQueueByDirection();

            UpdateDirection();
        }

        /// <summary>
        /// Unloads passengers whose destination matches the current floor.
        /// </summary>
        public void UnloadArrivedPassengers()
        {
            var departing = Passengers.Where(p => p.DestinationFloor == CurrentFloor).ToList();
            foreach (var p in departing)
            {
                Passengers.Remove(p);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{Id} unloaded passenger at floor {CurrentFloor} (from {p.PickupFloor})");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Boards passengers waiting on the current floor.
        /// </summary>
        public void BoardWaitingPassengers()
        {
            var boarding = Queue
                .Where(p => p.PickupFloor == CurrentFloor &&
                            (CurrentDirection == Direction.Idle || p.Direction == CurrentDirection))
                .ToList();

            foreach (var p in boarding)
            {
                Passengers.Add(p);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{Id} boarded passenger at floor {CurrentFloor} (to {p.DestinationFloor})");
                Console.ResetColor();
            }

            // Remove boarded passengers from the list
            Queue.RemoveAll(p => boarding.Contains(p));
        }


        // Determines the next floor to stop at (either for pickup or drop-off)
        private int? GetNextStop()
        {
            // Priority: current floor first
            if (Passengers.Any(p => p.DestinationFloor == CurrentFloor) ||
                Queue.Any(p => p.PickupFloor == CurrentFloor))
            {
                return CurrentFloor;
            }

            // Then choose based on direction-aware batching logic
            var possibleStops = Passengers.Select(p => p.DestinationFloor)
                .Concat(Queue.Select(p => p.PickupFloor))
                .Distinct()
                .ToList();

            if (!possibleStops.Any())
                return null;

            return possibleStops
                .OrderBy(f => Math.Abs(CurrentFloor - f))
                .First();
        }


        // Updates the direction.
        private void UpdateDirection()
        {
            // If at floor 1, force direction up
            if (CurrentFloor == 1)
            {
                CurrentDirection = Direction.Up;
                return;
            }

            // If at top floor, force direction down
            if (CurrentFloor == SimulationConfig.NumberOfFloors)
            {
                CurrentDirection = Direction.Down;
                return;
            }

            if (CurrentDirection != Direction.Idle &&
                (Passengers.Any(p => IsInCurrentDirection(p.DestinationFloor)) ||
                 Queue.Any(p => IsInCurrentDirection(p.PickupFloor))))
            {
                return; // Keep current direction
            }

            if (Passengers.Any())
            {
                var dest = Passengers.First().DestinationFloor;
                CurrentDirection = dest > CurrentFloor ? Direction.Up : Direction.Down;
            }
            else if (Queue.Any())
            {
                var next = Queue.First().PickupFloor;
                CurrentDirection = next > CurrentFloor ? Direction.Up : Direction.Down;
            }
            else
            {
                CurrentDirection = Direction.Idle;
            }
        }



        // Determines whether [is in current direction] [the specified target floor].
        private bool IsInCurrentDirection(int targetFloor)
        {
            return CurrentDirection == Direction.Up && targetFloor > CurrentFloor ||
                   CurrentDirection == Direction.Down && targetFloor < CurrentFloor;
        }

        /// <summary>
        /// Sorts the queue by direction.
        /// </summary>
        private void SortQueueByDirection()
        {
            if (!Queue.Any())
                return;

            if (CurrentDirection == Direction.Up)
            {
                Queue = Queue
                    .Where(q => q.Direction == Direction.Up)
                    .OrderBy(q => q.PickupFloor)
                    .Concat(Queue.Where(q => q.Direction != Direction.Up)) // keep the rest at the end
                    .ToList();
            }
            else if (CurrentDirection == Direction.Down)
            {
                Queue = Queue
                    .Where(q => q.Direction == Direction.Down)
                    .OrderByDescending(q => q.PickupFloor)
                    .Concat(Queue.Where(q => q.Direction != Direction.Down))
                    .ToList();
            }
            else // idle
            {
                Queue = Queue
                    .OrderBy(q => Math.Abs(CurrentFloor - q.PickupFloor))
                    .ToList();
            }
        }

        // <summary>
        // Determines whether [is duplicate request] [the specified request].
        // </summary>
        // <param name="request">The request.</param>
        // <returns>
        //   <c>true</c> if [is duplicate request] [the specified request]; otherwise, <c>false</c>.
        // </returns>
        private bool IsDuplicateRequest(PassengerRequest request)
        {
            return Queue.Any(p =>
                        p.PickupFloor == request.PickupFloor &&
                        p.DestinationFloor == request.DestinationFloor)
                || Passengers.Any(p =>
                        p.PickupFloor == request.PickupFloor &&
                        p.DestinationFloor == request.DestinationFloor);
        }

    }
}
