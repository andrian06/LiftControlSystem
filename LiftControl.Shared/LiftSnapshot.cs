namespace LiftControl.Shared
{
    public class LiftSnapshot
    {
        public string LiftId { get; set; }
        public int CurrentFloor { get; set; }
        public string Direction { get; set; }
        public bool IsInRoute { get; set; }
        public List<string> Queue { get; set; } = new();
        public List<string> Passengers { get; set; } = new();
    }

}
