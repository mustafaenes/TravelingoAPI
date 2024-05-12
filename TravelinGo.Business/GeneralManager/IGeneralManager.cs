using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelinGo.Business.Models;
using TravelinGo.Business.Requests;

namespace TravelinGo.Business
{
    public interface IGeneralManager
    {
        TravelingoResponse GetCities();
        TravelingoResponse SignUpUser(GeneralRequests requests);
        string GetRestaurantDetails(int LocationId);
        string GetCommentsByLocationId(int locationId);
        TravelingoResponse AddOrUpdateComment(Comments comment);
        Task<string> GetChatGPTResponse(GptRequest request);
        string GetAllRestraurantsInformations(string City);
        string GetAllRestraurantsFeatures(string City);
        string GetAllHotelsInformations(string City);
        string GetAllDestinationsInformations(string City);
        string GetHotelDetails(int LocationId);
        string GetDestinationDetails(int LocationId);
    }
}
