namespace SunSolarAPI.Models
{
    public class ReviewModel
    {
        public int UserID { get; set; }
        public int? PackageID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}