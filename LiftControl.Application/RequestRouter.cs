using System;
using System.Collections.Generic;
using System.Linq;
using LiftControl.Config;
using LiftControl.Domain;

namespace LiftControl.Application
{
    /// <summary>
    /// Handles assigning passenger requests to the most suitable lift.
    /// </summary>
    public class RequestRouter
    {
        private readonly List<LiftUnit> _lifts;

        /// <summary>
        /// Initializes the router with available lift units.
        /// </summary>
        /// <param name="lifts">List of lifts currently managed by the router.</param>
        public RequestRouter(List<LiftUnit> lifts)
        {
            _lifts = lifts;
        }

        /// <summary>
        /// Routes a passenger request to the most suitable lift based on scoring.
        /// </summary>
        /// <param name="request">The passenger's pickup and destination request.</param>
        public void RoutePassenger(PassengerRequest request)
        {
            // Pick the lift with the lowest score (most optimal for this request)
            var bestLift = _lifts
                            .OrderBy(l => ComputeScore(l, request))
                            .ThenBy(l => l.Passengers.Count + l.Queue.Count)
                            .ThenBy(l => l.Passengers.Count + l.Queue.Count) // Prefer least loaded
                            .First();


            // Log the assignment for visibility
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\"{request.Direction.ToString().ToLower()}\" request on floor {request.PickupFloor} received");
            Console.ResetColor();

            bestLift.AssignPassenger(request);
        }

        // Computes a "how good is this lift for this request" score — lower is better
        private int ComputeScore(LiftUnit lift, PassengerRequest request)
        {
            int load = lift.Passengers.Count + lift.Queue.Count;

            // Reject lift if it exceeds load threshold
            if (load > SimulationConfig.MaxLiftLoadThreshold)
                return int.MaxValue;

            // Reject lift if it already has requests going in a different direction
            if (lift.Queue.Any() && lift.Queue.Any(q => q.Direction != request.Direction))
                return int.MaxValue;

            // Reject lift if it is moving in the opposite direction
            if (lift.CurrentDirection != Direction.Idle && lift.CurrentDirection != request.Direction)
                return int.MaxValue;

            // Reject if lift already passed the pickup floor (even if direction is correct)
            if (lift.CurrentDirection == Direction.Up && lift.CurrentFloor > request.PickupFloor)
                return int.MaxValue;

            if (lift.CurrentDirection == Direction.Down && lift.CurrentFloor < request.PickupFloor)
                return int.MaxValue;

            // Reject lift if it is already in route (has decided to move) — prevents late assignments
            if (lift.IsInRoute)
                return int.MaxValue;

            // Perfect match: lift is idle and already at pickup floor
            if (lift.CurrentDirection == Direction.Idle && lift.CurrentFloor == request.PickupFloor && !lift.IsInRoute)
            {
                int worstDistance = SimulationConfig.NumberOfFloors;
                int worstLoadPenalty = SimulationConfig.MaxLiftLoadThreshold * SimulationConfig.LoadPenaltyWeight;
                int worstBatchingBonus = Math.Min(SimulationConfig.BatchingBonusWeight, 0);

                return worstDistance + worstLoadPenalty + worstBatchingBonus - SimulationConfig.PerfectMatchBonusWeight;
            }

            // Priority 2: Idle lift gets big bonus (encourages fairness and energy savings)
            int idleBonus = lift.CurrentDirection == Direction.Idle ? SimulationConfig.IdleBonusWeight : 0;

            int distance = Math.Abs(lift.CurrentFloor - request.PickupFloor);

            // Apply bonus if request is in the same direction and on the way
            int batchingBonus = (lift.CurrentDirection == request.Direction &&
                                ((request.Direction == Direction.Up && request.PickupFloor >= lift.CurrentFloor) ||
                                 (request.Direction == Direction.Down && request.PickupFloor <= lift.CurrentFloor)))
                                ? SimulationConfig.BatchingBonusWeight : 0;

            int loadPenalty = load * SimulationConfig.LoadPenaltyWeight;

            return distance + loadPenalty + batchingBonus + idleBonus;
        }


    }
}
