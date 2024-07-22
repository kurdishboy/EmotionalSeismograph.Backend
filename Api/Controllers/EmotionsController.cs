using Application.EmotionsModule;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/emotions")]
public class EmotionsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetById([FromServices] EmotionServices emotionServices, Guid id)
    {
        var emotion = emotionServices.GetById(id);
        
        return Ok(emotion);
    }
    
    [HttpGet]
    public IActionResult GetAll([FromServices] EmotionServices emotionServices)
    {
        var emotions = emotionServices.GetAll();
        
        return Ok(emotions);
    }
    
    [HttpPost]
    public IActionResult Create([FromServices] EmotionServices emotionServices, EmotionCreateAndUpdateRequest request)
    {
        var emotionId = emotionServices.Create(request);
        
        return CreatedAtAction(nameof(GetById), new { id = emotionId });
    }
    
    [HttpPut("{id}")]
    public IActionResult Update([FromServices] EmotionServices emotionServices, Guid id, EmotionCreateAndUpdateRequest request)
    {
        emotionServices.Update(id, request);
        
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete([FromServices] EmotionServices emotionServices, Guid id)
    {
        emotionServices.DeleteEmotion(id);
        
        return NoContent();
    }
}