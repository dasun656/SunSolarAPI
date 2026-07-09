namespace SunSolarAPI.Models
{
    public class SolarPackageModel
    {
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public decimal Price { get; set; }
        public decimal CapacityKW { get; set; }
        public string Description { get; set; }
        public bool IsNewArrival { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}