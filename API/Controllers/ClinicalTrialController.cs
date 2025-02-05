using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicalTrialController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClinicalTrialController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("AddMedicalTrial")]
        public async Task<IActionResult> AddMedicalTrial(IFormFile file)
        {
            //TODo Validaiton for file
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (Path.GetExtension(file.FileName).ToLower() != ".json")
                return BadRequest("Only .json files are allowed.");

            try
            {

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                string fileContent = await reader.ReadToEndAsync();



                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetAllTrials")]
        public async Task<IActionResult> GetAllTrials()
        {
            try
            {
                return Ok(await _mediator.Send(new GetTrialsQuery()));
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet("GetTrial")]
        public async Task<IActionResult> GetTrial(string trialId)
        {
            try
            {
                //TODO - validation for trial id, null or empty string not allowed
                var response = await _mediator.Send(new GetTrialByTrialIdQuery(trialId));
                if(response == null)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet("GetTrials")]
        public async Task<IActionResult> GetTrials(string status)
        {
            try
            {
                //TODO - validation for status, null or empty string not allowed
                //TODO - validation if 
                if (!Enum.IsDefined(typeof(TrialStatus), status))
                {
                    return BadRequest("Invalid parameter");
                }
                Enum.TryParse<TrialStatus>(status, out var trialStatus);
                var response = await _mediator.Send(new GetTrialByStatusQuery(trialStatus));
                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
