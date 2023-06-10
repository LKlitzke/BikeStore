namespace BikeStore.Models
{
    public class Speed : Bike
    {
        public string RunsAt() => "Highways only!";
        public bool HasDiscount() => true;
    }
}
