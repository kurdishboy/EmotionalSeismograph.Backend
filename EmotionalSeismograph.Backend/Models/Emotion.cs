namespace EmotionalSeismograph.Backend.Models
{
    public class Emotion
    {
        public long id { get; set; }
        public long userId { get; set; }
        public required string emotion { get; set; }
        public string? status { get; set; }
        public required string regdate { get; set; }
    }
}
