using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BikeStore.Models
{
    public class Bike : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public bool HasEnsurance { get; set; }
        public string Size { get; set; }
    }
}
