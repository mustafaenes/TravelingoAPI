using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TravelinGo.Business.Models;
using TravelinGo.Business.Requests;

namespace TravelinGo.Business
{
    public class GeneralManager : IGeneralManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GeneralManager(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
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
        public async Task<string> GetChatGPTResponse(GptRequest request)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];
                var httpClient = _httpClientFactory.CreateClient();

                // API anahtarını Authorization başlığı olarak ekleyin
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[] { new { role = "user", content = request.message } }
                }), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    throw new Exception("ChatGPT API error");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ChatGPT API error: " + ex.Message);
            }
        }

        public string GetAllRestraurantsInformations(string City)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

            using SqlConnection connection = new(connectionString);
            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@CITY", City);

            var result = connection.QueryFirstOrDefault<string>("GetAllRestaurantsInformations", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

        public string GetAllRestraurantsFeatures(string City)
        {
            string connectionString = _configuration.GetConnectionString("MyDatabaseConnection");

            using SqlConnection connection = new(connectionString);
            connection.Open();

            var parameters = new DynamicParameters();
            parameters.Add("@CITY", City);

            var result = connection.QueryFirstOrDefault<string>("GetRestaurantsFeatures", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

    }
}
