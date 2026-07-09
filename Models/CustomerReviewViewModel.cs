namespace SunSolarAPI.Models
{
    public class CustomerReviewViewModel
    {
        public int ReviewID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? ReviewedPackage { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}