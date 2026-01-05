using System.ComponentModel.DataAnnotations;

namespace CityBreakBooking.Web.Models
{
    public class Destination
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "City")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        // Navigation
        public ICollection<Trip>? Trips { get; set; }
        public bool IsActive { get; set; }
    }
}