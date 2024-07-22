using Domain.Models;

namespace Domain.Repositories;

public interface IEmotionsRepository
{
    Emotion? GetById(Guid id);

    List<Emotion> GetAll();
    
    void Add(Emotion emotion);
    
    void Delete(Guid id);
}