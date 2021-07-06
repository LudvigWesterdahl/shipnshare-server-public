namespace ShipWithMeInfrastructure.Models
{
    public sealed class ChatUser
    {
        public long UserId { get; set; }

        public User User { get; set; }

        public string ChatId { get; set; }

        public Chat Chat { get; set; }

        public bool Active { get; set; }
    }
}
