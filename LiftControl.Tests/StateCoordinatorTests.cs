using Xunit;
using System.Collections.Generic;
using LiftControl.Domain;
using LiftControl.Application;
using LiftControl.Config;

namespace LiftControl.Tests
{
    public class StateCoordinatorTests
    {
        [Fact]
        public void CanInitializeStateCoordinator()
        {
            var lifts = new List<LiftUnit> { new("LiftA"), new("LiftB") };

            // Basic sanity check to ensure coordinator can be constructed without errors
            var coordinator = new StateCoordinator(lifts);

            Assert.NotNull(coordinator);
        }

        [Fact]
        public void RouteManualRequest_InvalidDirection_TopFloorUp()
        {
            var lift = new LiftUnit("LiftTest") { CurrentFloor = SimulationConfig.NumberOfFloors };
            var lifts = new List<LiftUnit> { lift };
            var coordinator = new StateCoordinator(lifts);

            coordinator.RouteDirectionalRequest(SimulationConfig.NumberOfFloors, Direction.Up);

            // No request should be added due to invalid direction
            Assert.Empty(lift.Queue);
        }

        [Fact]
        public void RouteManualRequest_InvalidDirection_GroundFloorDown()
        {
            var lift = new LiftUnit("LiftTest") { CurrentFloor = 1 };
            var lifts = new List<LiftUnit> { lift };
            var coordinator = new StateCoordinator(lifts);

            coordinator.RouteDirectionalRequest(1, Direction.Down);

            Assert.Empty(lift.Queue);
        }

        [Fact]
        public void InitializeStateCoordinator_WithEmptyLiftList_ShouldNotThrow()
        {
            var lifts = new List<LiftUnit>();

            var exception = Record.Exception(() => new StateCoordinator(lifts));

            Assert.Null(exception);
        }
    }
}
