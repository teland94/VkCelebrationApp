using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.Controllers
{
    [Route("api/CongratulationTemplates")]
    public class CongratulationTemplatesController : Controller
    {
        private ICrudService<CongratulationTemplateDto> CongratulationTemplatesCrudService { get; }
        private ICongratulationTemplatesService CongratulationTemplatesService { get; }

        public CongratulationTemplatesController(ICrudService<CongratulationTemplateDto> congratulationTemplatesCrudService,
            ICongratulationTemplatesService congratulationTemplatesService)
        {
            CongratulationTemplatesCrudService = congratulationTemplatesCrudService;
            CongratulationTemplatesService = congratulationTemplatesService;
        }

        [HttpGet]
        public IEnumerable<CongratulationTemplateDto> GetCongratulationTemplates()
        {
            return CongratulationTemplatesCrudService.Get();
        }

        [HttpGet("{id}")]
        public IActionResult GetCongratulationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var translation = CongratulationTemplatesCrudService.Get(id);

            return Ok(translation);
        }

        [HttpPut("{id}")]
        public IActionResult PutCongratulationTemplate([FromRoute] int id, [FromBody] CongratulationTemplateDto template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CongratulationTemplatesCrudService.Update(template);

            return NoContent();
        }

        [HttpPost]
        public IActionResult PostCongratulationTemplate([FromBody] CongratulationTemplateDto template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CongratulationTemplatesCrudService.Create(template);

            return CreatedAtAction("PostCongratulationTemplate", new { id = template.Id }, template);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCongratulationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CongratulationTemplatesCrudService.Delete(id);

            return NoContent();
        }

        [HttpGet("Find")]
        public IActionResult Find(string text, int? maxItems = 5)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var congratulationTemplatesService = CongratulationTemplatesService.Find(text, maxItems);

            return Ok(congratulationTemplatesService);
        }
    }
}