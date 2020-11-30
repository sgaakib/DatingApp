namespace API.Entities
{
    public class AppUser
    {
        public int id { get; set; }
        public string userName { get; set; }
        public byte[] passwordhash { get; set; }
        public byte[] passwordsalt { get; set; }
    }
}