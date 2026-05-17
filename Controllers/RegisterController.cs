using FINANCETRACKER.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FINANCETRACKER.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public IActionResult Register(RegisterModel request)
        {
            using (var conn = new NpgsqlConnection(
                _configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                // Check if username exists
                string checkQuery = @"SELECT COUNT(*) 
                                      FROM ""USERS""
                                      WHERE ""USERNAME"" = @USERNAME";

                using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@USERNAME", request.username);

                    int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (userExists > 0)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Username already exists"
                        });
                    }
                }

                // Insert user
                string insertQuery = @"INSERT INTO ""USERS""(""NAME"", ""USERNAME"", ""PASSWORD"", ""CREATED_DATE"")
                                       VALUES(@NAME, @USERNAME, @PASSWORD, NOW())";

                using (var cmd = new NpgsqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@NAME", request.Name);
                    cmd.Parameters.AddWithValue("@USERNAME", request.username);

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);
                    cmd.Parameters.AddWithValue("@PASSWORD", hashedPassword);

                    cmd.ExecuteNonQuery();
                }

                return Ok(new
                {
                    success = true,
                    message = "User Registered Successfully"
                });
            }
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            using (var conn = new NpgsqlConnection(
                _configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                string query = @"SELECT ID, NAME, PASSWORD
                                 FROM ""USERS""
                                 WHERE ""USERNAME"" = @Username";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", request.Username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedPassword = reader["PASSWORD"].ToString();

                            bool isPasswordValid =
                                BCrypt.Net.BCrypt.Verify(request.Password, storedPassword);

                            if (isPasswordValid)
                            {
                                var claims = new[]
                                {
                                    new Claim("UserId", reader["ID"].ToString()),
                                    new Claim("Username", request.Username)
                                };

                                var key = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                                );

                                var creds = new SigningCredentials(
                                    key,
                                    SecurityAlgorithms.HmacSha256
                                );

                                var token = new JwtSecurityToken(
                                    issuer: _configuration["Jwt:Issuer"],
                                    audience: _configuration["Jwt:Audience"],
                                    claims: claims,
                                    expires: DateTime.UtcNow.AddDays(1),
                                    signingCredentials: creds
                                );

                                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                                return Ok(new
                                {
                                    success = true,
                                    message = "Login Successful",
                                    token = jwt,
                                    userId = reader["ID"],
                                    username = request.Username
                                });
                            }
                        }
                    }
                }

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid Username or Password"
                });
            }
        }
    }
}