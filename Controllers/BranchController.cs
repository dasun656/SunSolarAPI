using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SunSolarAPI.Models;

namespace SunSolarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly string _connectionString;

        public BranchController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SunSolarDB");
        }

        // GET: api/branch/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var branches = new List<object>();
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT BranchID, BranchName, Address, ContactNumber, GoogleMapLink FROM Branches";
                using (var cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                branches.Add(new
                                {
                                    BranchID = reader["BranchID"],
                                    BranchName = reader["BranchName"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    ContactNumber = reader["ContactNumber"].ToString(),
                                    GoogleMapLink = reader["GoogleMapLink"].ToString()
                                });
                            }
                        }
                        return Ok(branches);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { message = "Server error: " + ex.Message });
                    }
                }
            }
        }

        // POST: api/branch/add
        [HttpPost("add")]
        public IActionResult Add([FromBody] BranchModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.BranchName) || string.IsNullOrEmpty(model.Address))
                return BadRequest(new { message = "Branch name and address are required." });

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Branches (BranchName, Address, ContactNumber, GoogleMapLink)
                                 VALUES (@BranchName, @Address, @ContactNumber, @GoogleMapLink)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BranchName", model.BranchName);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@ContactNumber", model.ContactNumber ?? "");
                    cmd.Parameters.AddWithValue("@GoogleMapLink", (object)model.GoogleMapLink ?? DBNull.Value);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Branch added!" });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { message = "Server error: " + ex.Message });
                    }
                }
            }
        }

        // PUT: api/branch/update
        [HttpPut("update")]
        public IActionResult Update([FromBody] BranchModel model)
        {
            if (model == null || model.BranchID == 0)
                return BadRequest(new { message = "Invalid branch data." });

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Branches SET
                                    BranchName    = @BranchName,
                                    Address       = @Address,
                                    ContactNumber = @ContactNumber,
                                    GoogleMapLink = @GoogleMapLink
                                 WHERE BranchID = @BranchID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BranchID", model.BranchID);
                    cmd.Parameters.AddWithValue("@BranchName", model.BranchName);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@ContactNumber", model.ContactNumber ?? "");
                    cmd.Parameters.AddWithValue("@GoogleMapLink", (object)model.GoogleMapLink ?? DBNull.Value);
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0) return NotFound(new { message = "Branch not found." });
                        return Ok(new { success = true, message = "Branch updated!" });
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { message = "Server error: " + ex.Message });
                    }
                }
            }
        }

        // DELETE: api/branch/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Branches WHERE BranchID = @BranchID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BranchID", id);
                    try
                    {
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0) return NotFound(new { message = "Branch not found." });
                        return Ok(new { success = true, message = "Branch deleted!" });
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