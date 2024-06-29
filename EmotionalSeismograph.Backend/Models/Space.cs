namespace EmotionalSeismograph.Backend.Models
{
    public class Space
    {
        public long id { get; set; }
        public required string name { get; set; }
        public string? description { get; set; }
        public long ownerUserId { get; set; }
    }

    public class SpaceUser
    {
        public long id { get; set; }
        public long spaceId { get; set; }
        public long userId { get; set; }
    }
}
