using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
    }
}
