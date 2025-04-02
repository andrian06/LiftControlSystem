using Xunit;
using System.Collections.Generic;
using LiftControl.Domain;
using LiftControl.Application;

namespace LiftControl.Tests
{
    public class RequestRouterTests
    {
        [Fact]
        public void RoutePassenger_ShouldPreferNearestIdleLiftOverBusyOnes()
        {
            var lift1 = new LiftUnit("Lift1") { CurrentFloor = 1 };
            var lift2 = new LiftUnit("Lift2") { CurrentFloor = 8 };
            var lift3 = new LiftUnit("Lift3") { CurrentFloor = 4 }; // This is idle and closest

            // Simulate active lifts
            lift1.AssignPassenger(new PassengerRequest(1, 3));
            lift2.AssignPassenger(new PassengerRequest(8, 10));


            var lifts = new List<LiftUnit> { lift1, lift2, lift3 };
            var router = new RequestRouter(lifts);

            var request = new PassengerRequest(5, 7); // Up

            router.RoutePassenger(request);

            // The idle and nearby lift3 should be assigned
            Assert.Contains(request, lift3.Queue);
        }



        [Fact]
        public void RoutePassenger_ShouldPreferSameDirectionLift()
        {
            var lift = new LiftUnit("Lift1");

            // Simulate lift already going up
            lift.AssignPassenger(new PassengerRequest(2, 6));

            var lifts = new List<LiftUnit> { lift };
            var router = new RequestRouter(lifts);

            var request = new PassengerRequest(3, 5);

            router.RoutePassenger(request);

            // Should favor lift already heading up with this pickup on its path
            Assert.Contains(request, lift.Queue);
        }

        [Fact]
        public void RoutePassenger_WithTieBreaking_ChoosesFirstInList()
        {
            var lift1 = new LiftUnit("Lift1") { CurrentFloor = 3, CurrentDirection = Direction.Idle };
            var lift2 = new LiftUnit("Lift2") { CurrentFloor = 7, CurrentDirection = Direction.Idle };

            var lifts = new List<LiftUnit> { lift1, lift2 };
            var router = new RequestRouter(lifts);

            var request = new PassengerRequest(5, 8);
            router.RoutePassenger(request);

            // Both lifts are equally close, so tie-breaking should favor first in list
            Assert.Contains(request, lift1.Queue);
        }

        [Fact]
        public void RoutePassenger_IdleLiftPreferredOverMovingOpposite()
        {
            var idleLift = new LiftUnit("IdleLift") { CurrentFloor = 4, CurrentDirection = Direction.Idle };
            var movingLift = new LiftUnit("MovingLift") { CurrentFloor = 4, CurrentDirection = Direction.Down };

            var lifts = new List<LiftUnit> { movingLift, idleLift };
            var router = new RequestRouter(lifts);

            var request = new PassengerRequest(5, 9);
            router.RoutePassenger(request);

            // Idle lift should be chosen over lift going wrong direction
            Assert.Contains(request, idleLift.Queue);
        }

        [Fact]
        public void RoutePassenger_WithEmptyLiftList_ThrowsException()
        {
            var lifts = new List<LiftUnit>();
            var router = new RequestRouter(lifts);
            var request = new PassengerRequest(2, 4);

            Assert.Throws<System.InvalidOperationException>(() => router.RoutePassenger(request));
        }
    }
}
