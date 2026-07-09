namespace SunSolarAPI.Models
{
    public class PromotionModel
    {
        public int PromoID { get; set; } 
        public string PromoName { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
    }
}