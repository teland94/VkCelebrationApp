using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.Controllers
{
    [Route("api/CongratulationTemplates")]
    public class CongratulationTemplatesController : Controller
    {
        private ICongratulationTemplatesService CongratulationTemplatesService { get; }

        public CongratulationTemplatesController(ICongratulationTemplatesService congratulationTemplatesService)
        {
            CongratulationTemplatesService = congratulationTemplatesService;
        }

        [HttpGet]
        public IEnumerable<CongratulationTemplateDto> GetCongratulationTemplates()
        {
            return CongratulationTemplatesService.Get();
        }

        [HttpGet("{id}")]
        public IActionResult GetCongratulationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var translation = CongratulationTemplatesService.Get(id);

            return Ok(translation);
        }

        [HttpPut("{id}")]
        public IActionResult PutCongratulationTemplate([FromRoute] int id, [FromBody] CongratulationTemplateDto template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CongratulationTemplatesService.Update(template);

            return NoContent();
        }

        [HttpPost]
        public IActionResult PostCongratulationTemplate([FromBody] CongratulationTemplateDto template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CongratulationTemplatesService.Create(template);

            return CreatedAtAction("PostCongratulationTemplate", new { id = template.Id }, template);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCongratulationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CongratulationTemplatesService.Delete(id);

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

        [HttpGet("GetRandomCongratulationTemplates")]
        public async Task<IActionResult> GetRandomCongratulationTemplatesAsync(int? count = 5)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var templates = await CongratulationTemplatesService.GetRandomCongratulationTemplatesAsync(count);

            return Ok(templates);
        }
    }
}