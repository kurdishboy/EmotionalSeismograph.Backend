namespace Domain.Models;

public class Emotion
{
    public Guid Id { get; init; }
    
    public string Name { get; set; }

    public string Description { get; set; }
}