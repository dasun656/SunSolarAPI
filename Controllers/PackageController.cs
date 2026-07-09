using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SunSolarAPI.Models;

namespace SunSolarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly string _connectionString;

        public PackageController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SunSolarDB")!;
        }

        // GET: api/package/all
        [HttpGet("all")]
        public IActionResult GetAllPackages()
        {
            var list = new List<object>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT PackageID, PackageName, Price, CapacityKW, 
                                           Description, IsNewArrival, DiscountPercentage 
                                    FROM SolarPackages";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader()) // ✅ using block
                        {
                            while (reader.Read())
                            {
                                list.Add(new
                                {
                                    packageID = reader["PackageID"],
                                    packageName = reader["PackageName"].ToString(),
                                    price = reader["Price"],
                                    capacityKW = reader["CapacityKW"],
                                    description = reader["Description"]?.ToString(),
                                    isNewArrival = reader["IsNewArrival"],
                                    discountPercentage = reader["DiscountPercentage"]
                                });
                            }
                        }
                    }
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message }); // ✅ error message
            }
        }

        // POST: api/package/add
        [HttpPost("add")]
        public IActionResult AddPackage([FromBody] SolarPackageModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.PackageName))
                    return BadRequest(new { error = "Package name is required" });

                using (var conn = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO SolarPackages 
                                       (PackageName, Price, CapacityKW, Description, IsNewArrival, DiscountPercentage) 
                                     VALUES 
                                       (@n, @p, @c, @d, @isNew, @dis)";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@n", model.PackageName);
                        cmd.Parameters.AddWithValue("@p", model.Price);
                        cmd.Parameters.AddWithValue("@c", model.CapacityKW);
                        cmd.Parameters.AddWithValue("@d", (object?)model.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@isNew", model.IsNewArrival ? 1 : 0); // ✅ fixed
                        cmd.Parameters.AddWithValue("@dis", model.DiscountPercentage);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return Ok(new { success = true, message = "Package added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message }); // ✅ error message
            }
        }

        // DELETE: api/package/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult DeletePackage(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    string query = "DELETE FROM SolarPackages WHERE PackageID = @id";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                            return NotFound(new { error = "Package not found" }); // ✅ 404
                    }
                }
                return Ok(new { success = true, message = "Package deleted" });
            }
            catch (SqlException ex) when (ex.Number == 547) // FK constraint
            {
                return BadRequest(new { error = "Cannot delete: package is used in existing orders" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}