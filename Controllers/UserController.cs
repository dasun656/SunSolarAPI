using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SunSolarAPI.Models;
using System.Data;

namespace SunSolarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SunSolarDB");
        }

        // 1. User Registration
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { message = "Please provide all required fields." });
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber) VALUES (@FullName, @Email, @Password, @PhoneNumber)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", model.FullName);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return Ok(new { success = true, message = "Registration successful!" });
                    }
                    catch (SqlException ex)
                    {
                        return StatusCode(500, new { message = "Database error: " + ex.Message });
                    }
                }
            }
        }

        // 2. User Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // ✅ Role column එකත් select කරනවා
                string query = "SELECT UserID, FullName, Email, Role FROM Users WHERE Email = @Email AND PasswordHash = @Password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Password", model.Password);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ✅ Role සහිතව response return කරනවා
                                return Ok(new
                                {
                                    success = true,
                                    message = "Login successful!",
                                    userID = reader["UserID"].ToString(),
                                    fullName = reader["FullName"].ToString(),
                                    email = reader["Email"].ToString(),
                                    role = reader["Role"].ToString()
                                });
                            }
                            else
                            {
                                return Unauthorized(new { message = "Invalid email or password." });
                            }
                        }
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