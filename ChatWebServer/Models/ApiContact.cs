namespace ChatWebServer.Models
{
    public class ApiContact
    {
        public string id { get; set; }
        public string name { get; set; }
        public string server { get; set; } 
        public string last { get; set; } = null;
        public string lastdate { get; set; } = null;
    }
}
