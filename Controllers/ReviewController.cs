using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SunSolarAPI.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SunSolarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly string _connectionString;

        public ReviewController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SunSolarDB");
        }

        // GET: api/review/all
        [HttpGet("all")]
        public IActionResult GetAllReviews()
        {
            var reviewsList = new List<object>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // ඔබගේ View එකේ නම පරීක්ෂා කරගන්න (vw_CustomerReviews)
                string query = "SELECT ReviewID, CustomerName, ReviewedPackage, Rating, Comment, SubmittedAt FROM vw_CustomerReviews";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviewsList.Add(new
                            {
                                ReviewID = reader["ReviewID"],
                                CustomerName = reader["CustomerName"]?.ToString(),
                                ReviewedPackage = reader["ReviewedPackage"]?.ToString(),
                                Rating = reader["Rating"],
                                Comment = reader["Comment"]?.ToString(),
                                SubmittedAt = reader["SubmittedAt"]
                            });
                        }
                    }
                }
            }
            return Ok(reviewsList);
        }

        // POST: api/review/add
        [HttpPost("add")]
        public IActionResult AddReview([FromBody] ReviewModel model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Reviews (UserID, PackageID, Rating, Comment) VALUES (@UserID, @PackageID, @Rating, @Comment)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", model.UserID);
                    cmd.Parameters.AddWithValue("@PackageID", (object)model.PackageID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Rating", model.Rating);
                    cmd.Parameters.AddWithValue("@Comment", (object)model.Comment ?? DBNull.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return Ok(new { success = true });
                }
            }
        }

        // DELETE: api/review/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteReview(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Reviews WHERE ReviewID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0 ? Ok(new { success = true }) : NotFound();
                }
            }
        }
    }
}