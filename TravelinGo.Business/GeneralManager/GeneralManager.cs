using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelinGo.Business.Models;
using TravelinGo.Business.Requests;

namespace TravelinGo.Business
{
    public class GeneralManager : IGeneralManager
    {
        private readonly IConfiguration _configuration;

        public GeneralManager(IConfiguration configuration)
        {
            _configuration = configuration; 
        }



        public TravelingoResponse GetCities()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

                using SqlConnection connection = new(connectionString);
                connection.Open();

                var sqlQuery = @"
                    SELECT CAST((
                        SELECT ID, SEHIR_ADI FROM T_SEHIRLER FOR JSON AUTO
                    ) AS NVARCHAR(MAX))";

                var result = connection.QueryFirstOrDefault<string>(sqlQuery);

                if (!string.IsNullOrEmpty(result))
                {
                    return new TravelingoResponse
                    {
                        Result = result,
                        ErrorCode = "200",  // Başarılı durumu temsil eden bir kod
                        ErrorDescription = "Success"  // Başarılı durumu temsil eden bir açıklama
                    };
                }
                else
                {
                    return new TravelingoResponse
                    {
                        ErrorCode = "500",
                        ErrorDescription = "No city found"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return new TravelingoResponse
                {
                    ErrorCode = "500",
                    ErrorDescription = "Internal Server Error"
                };
            }
        }

        public TravelingoResponse SignUpUser(GeneralRequests requests)
        {
            TravelingoResponse result = new TravelingoResponse();

            try
            {
                string strCon = _configuration.GetConnectionString("MyDatabaseConnection");
                string sqlStr = null;
                sqlStr = "[dbo].[SignUpUser]";
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    using (SqlCommand Cmd = new SqlCommand(sqlStr, con))
                    {
                        Cmd.CommandType = CommandType.StoredProcedure;
                        {
                            Cmd.Parameters.AddWithValue("@ISIM", requests.Name);
                            Cmd.Parameters.AddWithValue("@SOYISIM", requests.Surname);
                            Cmd.Parameters.AddWithValue("@DOGUM_TARIHI", requests.BirthDate);
                            Cmd.Parameters.AddWithValue("@MAIL", requests.Email);
                            Cmd.Parameters.AddWithValue("@SIFRE", requests.Password);
                            Cmd.Parameters.AddWithValue("@CEP_TEL", requests.PhoneNumber);
                            Cmd.Parameters.AddWithValue("@CINSIYET", requests.Gender);
                            Cmd.Parameters.AddWithValue("@ADRES", requests.Address);
                        }
                        Cmd.Connection.Open();
                        Cmd.ExecuteNonQuery();
                    }
                }
                result = new TravelingoResponse
                {
                    ErrorCode = "0",
                    ErrorDescription = "İşleminiz Başarı ile gerçekleşmiştir."
                };
                ;
            }
            catch (Exception ex)
            {
                result = new TravelingoResponse
                {
                    ErrorCode = "1000",
                    ErrorDescription = ex.Message.ToString()
                };
            }
            return result;

        }

        public string GetRestaurantDetails(int LocationId)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

            using SqlConnection connection = new(connectionString);
            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@LOCATION_ID", LocationId);

            var result = connection.QueryFirstOrDefault<string>("GetRestaurantDetails", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

        public string GetCommentsByLocationId(int locationId)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

            using SqlConnection connection = new(connectionString);
            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@LocationId", locationId);

            return connection.QueryFirstOrDefault<string>("GetCommentsByLocationId", parameters, commandType: CommandType.StoredProcedure);
        }

        public TravelingoResponse AddOrUpdateComment(Comments comment)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

                using SqlConnection connection = new(connectionString);
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@LocationId", comment.LocationId);
                parameters.Add("@AuthorName", comment.AuthorName);
                parameters.Add("@CommentText", comment.CommentText);
                parameters.Add("@CommentDate", comment.CommentDate);

                connection.Execute("AddOrUpdateComment", parameters, commandType: CommandType.StoredProcedure);

                return new TravelingoResponse
                {
                    ErrorCode = "0",
                    ErrorDescription = "Yorum başarıyla eklendi."
                };
            }
            catch (Exception ex)
            {
                return new TravelingoResponse
                {
                    ErrorCode = "1000",
                    ErrorDescription = ex.Message
                };
            }
        }

    }
}
