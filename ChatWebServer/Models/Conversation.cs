using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebServer.Models
{
    public class Conversation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Relations
        public string UserId { get; set; }
        public User User { get; set; }
        public int ContactId { get; set; }

        public Contact Contact { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
