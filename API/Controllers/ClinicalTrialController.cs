using Application.Commands;
using Application.Exceptions;
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
                var addTrial = await ReadAndDeserializeJson<AddTrialCommand>(request);
                if (addTrial == null)
                {
                    return BadRequest("Invalid JSON structure.");
                }

                var result = await _mediator.Send(addTrial);

                return CreatedAtAction(nameof(GetTrial), new { trialId = result?.TrialId }, result);
            }
            catch (TrialDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpPut(nameof(UpdateMedicalTrial))]
        public async Task<IActionResult> UpdateMedicalTrial([FromForm] UploadJsonFileRequestDTO request)
        {
            try
            {
                var updateTrial = await ReadAndDeserializeJson<UpdateTrialCommand>(request);
                if (updateTrial == null)
                {
                    return BadRequest("Invalid JSON structure.");
                }

                var result = await _mediator.Send(updateTrial);

                return Ok(result);
            }
            catch (TrialDataException ex)
            {
                return BadRequest(ex.Message);
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
                if (!Enum.IsDefined(typeof(TrialStatus), status.Replace(" ", "")))
                {
                    return BadRequest("Invalid parameter");
                }
                var response = await _mediator.Send(new GetTrialByStatusQuery(status));
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

        private async Task<T> ReadAndDeserializeJson<T>(UploadJsonFileRequestDTO request) where T : class
        {
            using var stream = new MemoryStream();
            await request.File.CopyToAsync(stream);
            stream.Position = 0;

            using var reader = new StreamReader(stream, Encoding.UTF8);
            string fileContent = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<T>(fileContent);
        }
    }
}
