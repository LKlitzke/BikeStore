namespace BikeStore.Models
{
    public class Hardtrail : Bike
    {
        public string RunsAt() => "Everywhere!";
        public bool HasDiscount() => true;
    }
}
