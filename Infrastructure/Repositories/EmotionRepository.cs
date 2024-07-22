using Domain.Models;
using Domain.Repositories;

namespace Infrastructure.Repositories;

public class EmotionRepository(ApplicationDbContext dbContext) : IEmotionsRepository
{
    public Emotion? GetById(Guid id)
    {
        return dbContext.Emotions.SingleOrDefault(c => c.Id == id);
    }

    public List<Emotion> GetAll()
    {
        return dbContext.Emotions.ToList();
    }

    public void Add(Emotion emotion)
    {
        dbContext.Emotions.Add(emotion);
    }

    public void Delete(Guid id)
    {
        dbContext.Emotions.Remove(dbContext.Emotions.Single(c => c.Id == id));
    }
}