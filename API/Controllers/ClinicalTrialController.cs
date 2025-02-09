using Application.Commands;
using Application.Exceptions;
using Application.Queries;
using API.Validation.RequestModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API.Controllers
{
    /// <summary>
    /// API for managing clinical trials.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Clinical trials")]
    public class ClinicalTrialController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ClinicalTrialController(IMediator mediator, ILogger<ClinicalTrialController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new clinical trial.
        /// </summary>
        /// <param name="request">JSON file containing clinical trial metadata.</param>
        /// <returns>Returns the inserted clinical trial object along with its details. The response header contains the URL of the created resource.</returns>
        /// <response code="200">Return inserted clinical trial object</response>
        /// <response code="400">
        /// If any of the following conditions are met:
        /// - The JSON schema is invalid.
        /// - The object properties do not meet validation constraints.
        /// - The specified trialId exists in the database.
        /// </response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost(nameof(AddMedicalTrial))]
        public async Task<IActionResult> AddMedicalTrial([FromForm] UploadJsonFileRequestModel request)
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

        /// <summary>
        /// Updates a clinical trial
        /// </summary>
        /// <param name="request">JSON file containing clinical trial metadata.</param>
        /// <returns>Returns the updated clinical trial object along with its details.</returns>
        /// <response code="200">Returns the updated clinical trial object.</response>
        /// <response code="400">
        /// If any of the following conditions are met:
        /// - The JSON schema is invalid.
        /// - The object properties do not meet validation constraints.
        /// - The specified trialId does not exist in the database.
        /// </response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPut(nameof(UpdateMedicalTrial))]
        public async Task<IActionResult> UpdateMedicalTrial([FromForm] UploadJsonFileRequestModel request)
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

        /// <summary>
        /// Retrieves all clinical trials.
        /// </summary>
        /// <returns>Returns the list of clinical trials.</returns>
        /// <param name="Page">Optional parameter. Represents the current page you want to display.</param>
        /// <param name="Size">Optional parameter. Represents the number of records per page.</param>
        /// <remarks>
        /// **Pagination (`Page` and `Size`):**
        /// If both `Page` and `Size` parameters are provided, pagination will be applied; otherwise, all records will be returned.
        /// - **`Page`**: Must be greater than 0 if provided.
        /// - **`Size`**: Must be between 1 and 100 if provided. 
        /// </remarks>
        /// <response code="200">Returns the clinical trial list along with the total count, which helps in calculating total pages.</response>
        /// <response code="400">If input parameters value is invalid</response>
        /// <response code="500">Returned if an internal server error occurs.</response>
        [HttpGet(nameof(GetAllTrials))]
        public async Task<IActionResult> GetAllTrials([FromQuery] PaginationQueryRequestModel paginationQueryDTO)
        {
            try
            {
                return Ok(await _mediator.Send(new GetTrialsQuery(paginationQueryDTO.Page, paginationQueryDTO.Size)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves the clinical trial by Id
        /// </summary>
        /// <param name="trialId">string parameter</param>
        /// <returns>Returns specific clinical trial detail</returns>
        /// <response code="200">Returns specific clinical trial detail</response>
        /// <response code="404">If no clinical trial is found</response>
        /// <response code="500">If an internal server error occurs.</response>
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves a list of clinical trials filtered by status.
        /// </summary>
        /// <returns>Returns a paginated list of clinical trials matching the specified status.</returns>
        /// <param name="Status">Mandatory parameter. Represents the status of trials you want to retrieve.</param>
        /// <param name="Page">Optional parameter. Represents the current page number to display.</param>
        /// <param name="Size">Optional parameter. Represents the number of records per page.</param>
        /// <remarks>
        /// **`Status` available values:**
        /// - **Not Started**: Trials that have not yet begun.
        /// - **Ongoing**: Trials that are currently active.
        /// - **Completed**: Trials that have finished.
        ///
        /// **Pagination (`Page` and `Size`):**
        /// If both `Page` and `Size` parameters are provided, pagination will be applied; otherwise, all records with requested status will be returned.
        /// - **`Page`**: Must be greater than 0 if provided.
        /// - **`Size`**: Must be between 1 and 100 if provided.   
        /// </remarks>
        /// <response code="200">Returns the list of clinical trials filtered by the requested status, along with the total count of records matching that status. This helps in calculating the total number of pages.</response>
        /// <response code="400">Returned if any input parameter value is invalid.</response>
        /// <response code="500">Returned if an internal server error occurs.</response>
        [HttpGet(nameof(GetTrials))]
        public async Task<IActionResult> GetTrials([FromQuery] GetTrialByStatusRequestModel getTrialByStatusDTO)
        {
            try
            {
                var response = await _mediator.Send(new GetTrialByStatusQuery(getTrialByStatusDTO.Status, getTrialByStatusDTO.Page, getTrialByStatusDTO.Size));

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        private async Task<T> ReadAndDeserializeJson<T>(UploadJsonFileRequestModel request) where T : class
        {
            try
            {
                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                string fileContent = await reader.ReadToEndAsync();

                return JsonConvert.DeserializeObject<T>(fileContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occured in method: {nameof(ReadAndDeserializeJson)}");
                throw;
            }
        }
    }
}
