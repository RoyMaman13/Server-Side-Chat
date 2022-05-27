using System.ComponentModel.DataAnnotations;

namespace ChatWebServer.Models
{
    public class Rate
    {
        public int Id { get; set; }
        [Range(1, 5), Required]
        public int Grade { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Name { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;   

    }
}

