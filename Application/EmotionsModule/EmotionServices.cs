using Application.Abstractions;
using Domain.Models;
using Domain.Repositories;

namespace Application.EmotionsModule;

public class EmotionServices(IUnitOfWork unitOfWork, IEmotionsRepository emotionsRepository)
{
    public List<EmotionResponse> GetAll()
    {
        var result = emotionsRepository.GetAll()
            .Select(e => new EmotionResponse(e.Id, e.Name, e.Description))
            .ToList();
        
        return result;
    }

    public EmotionResponse? GetById(Guid id)
    {
        var entity = emotionsRepository.GetById(id);
        if (entity is null) throw new Exception("Emotion not found");
        
        return new EmotionResponse(entity.Id, entity.Name, entity.Description);
    }

    public Guid Create(EmotionCreateAndUpdateRequest requesr)
    {
        var emotion = new Emotion
        {
            Id = Guid.NewGuid(),
            Name = requesr.Name,
            Description = requesr.Description
        };

        emotionsRepository.Add(emotion);
        unitOfWork.SaveChanges();

        return emotion.Id;
    }

    public void Update(Guid id, EmotionCreateAndUpdateRequest emotion)
    {
        var entity = emotionsRepository.GetById(id);
        if (entity is null) throw new Exception("Emotion not found");
        
        entity.Name = emotion.Name;
        entity.Description = emotion.Description;
        
        unitOfWork.SaveChanges();
    }

    public void DeleteEmotion(Guid id)
    {
        var entity = emotionsRepository.GetById(id);
        if (entity is null) throw new Exception("Emotion not found");
        
        emotionsRepository.Delete(id);
        unitOfWork.SaveChanges();
    }
}