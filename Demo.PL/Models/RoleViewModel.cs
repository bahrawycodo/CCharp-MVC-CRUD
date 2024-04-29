using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class RoleViewModel
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
