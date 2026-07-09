using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SunSolarAPI.Models;

namespace SunSolarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopController : ControllerBase
    {
        private readonly string _connectionString;

        public WorkshopController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SunSolarDB");
        }

        // GET: api/workshop/all
        [HttpGet("all")]
        public IActionResult GetAllWorkshops()
        {
            var workshops = new List<object>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT WorkshopID, Topic, Area, WorkshopDate,
                                        WorkshopTime, MaxParticipants
                                 FROM Workshops";
                using (var cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                workshops.Add(new
                                {
                                    WorkshopID = reader["WorkshopID"],
                                    Topic = reader["Topic"].ToString(),
                                    Area = reader["Area"].ToString(),
                                    WorkshopDate = reader["WorkshopDate"].ToString(),
                                    WorkshopTime = reader["WorkshopTime"].ToString(),
                                    MaxParticipants = reader["MaxParticipants"]
                                });
                            }
                        }
                        return Ok(workshops);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { message = "Server error: " + ex.Message });
                    }
                }
            }
        }

        // POST: api/workshop/add
        [HttpPost("add")]
        public IActionResult AddWorkshop([FromBody] Workshop workshop)
        {
            if (workshop == null || string.IsNullOrEmpty(workshop.Topic))
                return BadRequest(new { message = "Topic is required." });

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Workshops
                                    (Topic, Area, WorkshopDate, WorkshopTime, MaxParticipants)
                                 VALUES
                                    (@Topic, @Area, @WorkshopDate, @WorkshopTime, @MaxParticipants)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Topic", workshop.Topic);
                    cmd.Parameters.AddWithValue("@Area", workshop.Area);
                    cmd.Parameters.AddWithValue("@WorkshopDate", workshop.WorkshopDate);
                    cmd.Parameters.AddWithValue("@WorkshopTime", workshop.WorkshopTime);
                    cmd.Parameters.AddWithValue("@MaxParticipants", workshop.MaxParticipants);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Workshop scheduled!" });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { message = "Server error: " + ex.Message });
                    }
                }
            }
        }

        // PUT: api/workshop/update
        [HttpPut("update")]
        public IActionResult UpdateWorkshop([FromBody] Workshop workshop)
        {
            if (workshop == null || workshop.WorkshopID == 0)
                return BadRequest(new { message = "Invalid workshop data." });

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Workshops SET
                                    Topic           = @Topic,
                                    Area            = @Area,
                                    WorkshopDate    = @WorkshopDate,
                                    WorkshopTime    = @WorkshopTime,
                                    MaxParticipants = @MaxParticipants
                                 WHERE WorkshopID = @WorkshopID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@WorkshopID", workshop.WorkshopID);
                    cmd.Parameters.AddWithValue("@Topic", workshop.Topic);
                    cmd.Parameters.AddWithValue("@Area", workshop.Area);
                    cmd.Parameters.AddWithValue("@WorkshopDate", workshop.WorkshopDate);
                    cmd.Parameters.AddWithValue("@WorkshopTime", workshop.WorkshopTime);
                    cmd.Parameters.AddWithValue("@MaxParticipants", workshop.MaxParticipants);
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0) return NotFound(new { message = "Workshop not found." });
                        return Ok(new { success = true, message = "Workshop updated!" });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { message = "Server error: " + ex.Message });
                    }
                }
            }
        }

        // DELETE: api/workshop/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteWorkshop(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Workshops WHERE WorkshopID = @WorkshopID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@WorkshopID", id);
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0) return NotFound(new { message = "Workshop not found." });
                        return Ok(new { success = true, message = "Workshop deleted!" });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { message = "Server error: " + ex.Message });
                    }
                }
            }
        }
    }
}