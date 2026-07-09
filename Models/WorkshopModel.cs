namespace SunSolarAPI.Models
{
    public class Workshop
    {
        public int WorkshopID { get; set; }  
        public string Topic { get; set; }
        public string Area { get; set; }
        public string WorkshopDate { get; set; }
        public string WorkshopTime { get; set; }
        public int MaxParticipants { get; set; }
    }
}