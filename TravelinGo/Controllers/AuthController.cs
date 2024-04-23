using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TravelinGo.Business;
using TravelinGo.Business.Models;
using TravelinGo.Business.Requests;

namespace TravelinGo.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthenticationManager authenticationManager,IConfiguration configuration)
        {
            _authenticationManager = authenticationManager;
            _configuration = configuration;
        }

        [HttpPost("SignInUser")]
        public IActionResult SignInUser([FromBody] LoginRequests requests)
        {
            var user = _authenticationManager.Authenticate(requests.email, requests.password);

            if(user == null)
            {
                return Unauthorized(new { message = "Geçersiz email adresi veya şifre!..." });
            }
            else
            {
                //return Ok(new { user });
                return Ok(new { AccessToken = GenerateAccessToken(user) });
            }

        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // appsettings.json dosyasındaki Jwt:Key değeri
            var tokenDescriptor = new SecurityTokenDescriptor
            {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanıcı ID'si
                new Claim(ClaimTypes.Name, user.Name), // Kullanıcı adı
                new Claim(ClaimTypes.Surname, user.Surname), // Kullanıcı soyadı
                new Claim(ClaimTypes.Email, user.Email), // Kullanıcı e-posta adresi
                new Claim("PhoneNumber", user.PhoneNumber),
                new Claim(ClaimTypes.Gender, user.Gender),
                new Claim("StreetAddress", user.Address),
                new Claim("DateOfBirth", user.BirthDate.ToString()), // Kullanıcı doğum tarihi
                new Claim("RegistrationDate", user.RegistrationDate.ToString()), // Kullanıcı kayıt tarihi
                // Diğer kullanıcı özelliklerini de ekleyebilirsiniz
            }),
                Expires = DateTime.UtcNow.AddHours(1), // Token'ın geçerlilik süresi, burada iki saat olarak ayarlanmıştır
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



    }
}
