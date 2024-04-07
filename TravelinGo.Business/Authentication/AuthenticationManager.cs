using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelinGo.Business.Models;

namespace TravelinGo.Business
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly IConfiguration _configuration;

        public AuthenticationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public User Authenticate(string email, string password)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");
                string sqlStr = "[dbo].[IsUserValidControl]";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlStr, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MAIL", email);
                        command.Parameters.AddWithValue("@PASSWORD", password);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            // Kullanıcı bulundu, verileri oku
                            reader.Read();
                            User user = new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Surname = reader.GetString(2),
                                Email = reader.GetString(3),
                                Password = reader.GetString(4),
                                PhoneNumber = reader.GetString(5),
                                BirthDate = reader.GetDateTime(6),
                                Gender = reader.GetString(7),
                                Address = reader.GetString(8),
                                RegistrationDate = reader.GetDateTime(9)
                                // Diğer kullanıcı özelliklerini okuyun
                            };

                            return user;
                        }
                        else
                        {
                            // Kullanıcı bulunamadı
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda null döndür
                return null;
            }
        }
    }
}
