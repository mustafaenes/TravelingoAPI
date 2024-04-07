using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelinGo.Business.Models
{
    public class TravelingoResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Result { get; set; }
    }
}
