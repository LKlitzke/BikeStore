namespace BikeStore.Models
{
    public class Bmx : Bike
    {
        public string RunsAt() => "Streets Only!";
        public bool HasDiscount() => false;
    }
}
