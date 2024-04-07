using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelinGo.Business.Models;

namespace TravelinGo.Business
{
    public interface IAuthenticationManager
    {
        User Authenticate(string email, string password);
    }
}
