using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [Route("api/CongratulationTemplates")]
    public class CongratulationTemplatesController : ControllerBase
    {
        private ICongratulationTemplatesService CongratulationTemplatesService { get; }

        public CongratulationTemplatesController(ICongratulationTemplatesService congratulationTemplatesService)
        {
            CongratulationTemplatesService = congratulationTemplatesService;
        }

        [HttpGet]
        public async Task<IEnumerable<CongratulationTemplateDto>> GetCongratulationTemplates()
        {
            return await CongratulationTemplatesService.GetByUserIdAsync(GetUserId());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCongratulationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var congratulationTemplate = await CongratulationTemplatesService.GetAsync(id);

            return Ok(congratulationTemplate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCongratulationTemplate([FromRoute] int id, [FromBody] CongratulationTemplateDto template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await CongratulationTemplatesService.UpdateAsync(template, GetUserId());

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostCongratulationTemplate([FromBody] CongratulationTemplateDto template)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await CongratulationTemplatesService.CreateAsync(template, GetUserId());

            return CreatedAtAction("PostCongratulationTemplate", new { id = template.Id }, template);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCongratulationTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await CongratulationTemplatesService.DeleteAsync(id);

            return NoContent();
        }

        [HttpGet("Find")]
        public async Task<IActionResult> Find(string text, int? maxItems = 5)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var congratulationTemplatesService = await CongratulationTemplatesService.FindAsync(text, GetUserId(), maxItems);

            return Ok(congratulationTemplatesService);
        }

        [HttpGet("GetRandomCongratulationTemplates")]
        public async Task<IActionResult> GetRandomCongratulationTemplatesAsync(int? count = 5)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var templates = await CongratulationTemplatesService.GetRandomCongratulationTemplatesAsync(GetUserId(), count);

            return Ok(templates);
        }
    }
}