namespace SecureLogin.Model
{
    public class Client
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; } = "";
        public Guid ApiKey { get; set; }
    }
}
