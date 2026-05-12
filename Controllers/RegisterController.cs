using FINANCETRACKER.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace FINANCETRACKER.Controllers
{

    [Route("api/auth/")] //base URL So your API starts with:
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterModel request)
        {
            using (SqlConnection conn =
                   new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                string checkQuery = @"SELECT COUNT(*) 
                              FROM USERS
                              WHERE USERNAME = @USERNAME";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);

                checkCmd.Parameters.AddWithValue("@USERNAME", request.username);

                int userExists = (int)checkCmd.ExecuteScalar();

                if (userExists > 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Username already exists"
                    });
                }

                string insertQuery = @"INSERT INTO USERS(NAME, USERNAME, PASSWORD,CREATED_DATE)
                               VALUES(@NAME, @USERNAME, @PASSWORD,GETDATE())";

                SqlCommand cmd = new SqlCommand(insertQuery, conn); 

                cmd.Parameters.AddWithValue("@NAME", request.Name);
                cmd.Parameters.AddWithValue("@USERNAME", request.username);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);

                cmd.Parameters.AddWithValue("@PASSWORD", hashedPassword);

                cmd.ExecuteNonQuery();

                return Ok(new
                {
                    success = true,
                    message = "User Registered Successfully"
                });
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            using (SqlConnection conn =
                   new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                string query = @"SELECT ID, NAME,PASSWORD
                         FROM USERS
                         WHERE USERNAME COLLATE SQL_Latin1_General_CP1_CS_AS = @Username";
                         

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Username", request.Username);
                

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedPassword = reader["PASSWORD"].ToString();

                    bool isPasswordValid =
                        BCrypt.Net.BCrypt.Verify(
                            request.Password,
                            storedPassword
                        );

                    if (isPasswordValid)
                    {
                        var claims = new[]
                        {
        new Claim("UserId", reader["ID"].ToString()),
        new Claim("Username", request.Username)
    };

                        var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                _configuration["Jwt:Key"]
                            )
                        );

                        var creds = new SigningCredentials(
                            key,
                            SecurityAlgorithms.HmacSha256
                        );

                        var token = new JwtSecurityToken(
                            issuer: _configuration["Jwt:Issuer"],
                            audience: _configuration["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: creds
                        );

                        var jwt = new JwtSecurityTokenHandler()
                            .WriteToken(token);

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

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid Email or Password"
                });
            }
        }
    }
}
