using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SunSolarAPI.Models;

namespace SunSolarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly string _connectionString;
        public PromotionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SunSolarDB");
        }

        [HttpGet("all")]
        public IActionResult GetAllPromotions()
        {
            var list = new List<object>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Promotions";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            PromoID = reader["PromoID"],
                            PromoName = reader["PromoName"],
                            DiscountPercentage = reader["DiscountPercentage"],
                            StartDate = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd"),
                            EndDate = Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd"),
                            Description = reader["Description"]
                        });
                    }
                }
            }
            return Ok(list);
        }

        [HttpPost("add")]
        public IActionResult AddPromotion([FromBody] PromotionModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Promotions (PromoName, DiscountPercentage, StartDate, EndDate, Description) VALUES (@N, @D, @S, @E, @Desc)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@N", model.PromoName);
                cmd.Parameters.AddWithValue("@D", model.DiscountPercentage);
                cmd.Parameters.AddWithValue("@S", model.StartDate);
                cmd.Parameters.AddWithValue("@E", model.EndDate);
                cmd.Parameters.AddWithValue("@Desc", (object)model.Description ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Ok(new { success = true });
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeletePromotion(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Promotions WHERE PromoID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Ok(new { success = true });
            }
        }

        [HttpPut("update")]
        public IActionResult UpdatePromotion([FromBody] PromotionModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Promotions SET PromoName=@N, DiscountPercentage=@D, StartDate=@S, EndDate=@E, Description=@Desc WHERE PromoID=@ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", model.PromoID);
                cmd.Parameters.AddWithValue("@N", model.PromoName);
                cmd.Parameters.AddWithValue("@D", model.DiscountPercentage);
                cmd.Parameters.AddWithValue("@S", model.StartDate);
                cmd.Parameters.AddWithValue("@E", model.EndDate);
                cmd.Parameters.AddWithValue("@Desc", (object)model.Description ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Ok(new { success = true });
            }
        }
    }
}