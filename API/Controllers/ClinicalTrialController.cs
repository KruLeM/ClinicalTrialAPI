using Application.Commands;
using Application.Queries;
using Domain.Entities;
using Infrastructure.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicalTrialController : ControllerBase
    {
        //TODO -> Add validaitons for GET endpoints
        private readonly IMediator _mediator;

        public ClinicalTrialController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost(nameof(AddMedicalTrial))]
        public async Task<IActionResult> AddMedicalTrial([FromForm] UploadJsonFileRequestDTO request)
        {
            try
            {
                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                string fileContent = await reader.ReadToEndAsync();

                var command = JsonConvert.DeserializeObject<AddTrialCommand>(fileContent);
                if (command == null)
                {
                    return BadRequest("Invalid JSON structure.");
                }

                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetTrial) + $"?trialId={result?.TrialId}", result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpPost(nameof(UpdateMedicalTrial))]
        public async Task<IActionResult> UpdateMedicalTrial([FromForm] UploadJsonFileRequestDTO request)
        {
            try
            {
                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                string fileContent = await reader.ReadToEndAsync();

                var command = JsonConvert.DeserializeObject<UpdateTrialCommand>(fileContent);
                if (command == null)
                {
                    return BadRequest("Invalid JSON structure.");
                }

                var result = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetTrial) + $"?trialId={result?.TrialId}", result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpGet(nameof(GetAllTrials))]
        public async Task<IActionResult> GetAllTrials()
        {
            try
            {
                return Ok(await _mediator.Send(new GetTrialsQuery()));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpGet(nameof(GetTrial))]
        public async Task<IActionResult> GetTrial(string trialId)
        {
            try
            {
                var response = await _mediator.Send(new GetTrialByTrialIdQuery(trialId));
                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpGet(nameof(GetTrials))]
        public async Task<IActionResult> GetTrials(string status)
        {
            try
            {
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

    }
}
