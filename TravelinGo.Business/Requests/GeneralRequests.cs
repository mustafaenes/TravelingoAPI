using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelinGo.Business.Requests
{
    public class GeneralRequests
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; } 
    }

    public class GetRestaurantDetails
    {
        public int LocationId { get; set; }
    }

    public class Comments
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string AuthorName { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
    }

}
