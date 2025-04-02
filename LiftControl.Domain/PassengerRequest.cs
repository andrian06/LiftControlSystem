namespace LiftControl.Domain
{
    /// <summary>
    /// Represents a complete passenger journey request, from pickup to destination.
    /// </summary>
    public class PassengerRequest
    {
        /// <summary>
        /// Floor where the passenger starts.
        /// </summary>
        public int PickupFloor { get; }

        /// <summary>
        /// Floor where the passenger wants to go.
        /// </summary>
        public int DestinationFloor { get; }

        /// <summary>
        /// Direction of travel based on pickup and destination.
        /// </summary>
        public Direction Direction => DestinationFloor > PickupFloor ? Direction.Up : Direction.Down;

        /// <summary>
        /// Creates a new passenger request with specified pickup and destination floors.
        /// </summary>
        /// <param name="pickup">Floor where the passenger is waiting.</param>
        /// <param name="destination">Floor the passenger wants to go to.</param>
        public PassengerRequest(int pickup, int destination)
        {
            PickupFloor = pickup;
            DestinationFloor = destination;
        }

        /// <summary>
        /// String representation of the request in the form "X->Y".
        /// </summary>
        //public override string ToString() => $"{PickupFloor}->{DestinationFloor}";
    }
}
