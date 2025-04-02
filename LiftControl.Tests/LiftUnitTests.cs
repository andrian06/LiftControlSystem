using Xunit;
using LiftControl.Domain;
using System.Collections.Generic;
using LiftControl.Config;
using System.Threading.Tasks;

namespace LiftControl.Tests
{
    public class LiftUnitTests
    {
        [Fact]
        public void AssignPassenger_AddsRequestToQueue()
        {
            var lift = new LiftUnit("LiftA");
            var request = new PassengerRequest(2, 6);

            lift.AssignPassenger(request);

            Assert.Contains(request, lift.Queue);
        }

        [Fact]
        public void BoardWaitingPassengers_MovesRequestToPassengerList()
        {
            var lift = new LiftUnit("LiftA");
            var request = new PassengerRequest(3, 6);

            lift.AssignPassenger(request);
            lift.BoardWaitingPassengers(); // Not at pickup floor yet, should not board

            // Set lift on correct pickup floor
            lift.CurrentFloor = 3;

            // Simulate a matching condition
            lift.Queue.Add(request); // Ensure it's in queue again
            lift.BoardWaitingPassengers();

            Assert.DoesNotContain(request, lift.Queue);
            Assert.Contains(request, lift.Passengers);
        }


        [Fact]
        public void UnloadArrivedPassengers_RemovesFromPassengerList()
        {
            var lift = new LiftUnit("LiftA");
            var request = new PassengerRequest(2, 5);
            lift.Passengers.Add(request);

            lift.CurrentFloor = 5; // Match destination floor

            lift.UnloadArrivedPassengers();

            Assert.DoesNotContain(request, lift.Passengers);
        }

                [Fact]
        public void UpdateDirection_WithPassengers_SetsCorrectDirection()
        {
            var lift = new LiftUnit("LiftTest");
            lift.CurrentFloor = 2;
            lift.Passengers.Add(new PassengerRequest(2, 8));

            lift.AssignPassenger(new PassengerRequest(3, 9));

            Assert.Equal(Direction.Up, lift.CurrentDirection);
        }

        [Fact]
        public void UpdateDirection_WithQueueOnly_SetsCorrectDirection()
        {
            var lift = new LiftUnit("LiftTest");
            lift.CurrentFloor = 5;

            // This triggers UpdateDirection internally
            lift.AssignPassenger(new PassengerRequest(7, 3));

            Assert.Equal(Direction.Up, lift.CurrentDirection);
        }


        [Fact]
        public void GetNextStop_ReturnsNull_WhenNoPassengersOrQueue()
        {
            var lift = new LiftUnit("LiftTest");

            // Use reflection to call private GetNextStop
            var method = typeof(LiftUnit).GetMethod("GetNextStop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = method.Invoke(lift, null);

            Assert.Null(result);
        }

        [Fact]
        public void AssignPassenger_WithSamePickupAndDestination_ShouldHandleGracefully()
        {
            var lift = new LiftUnit("LiftTest");
            var request = new PassengerRequest(3, 3);

            lift.AssignPassenger(request);

            Assert.Contains(request, lift.Queue);
        }

    }
}
