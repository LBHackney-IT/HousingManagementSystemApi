using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace HousingManagementSystemApi.Controllers
{
    using System;
    using System.Linq;
    using HousingManagementSystemApi.UseCases.Interfaces;
    using Microsoft.Extensions.Logging;
    using Sentry;
    using UseCases;
    using Constants = HousingManagementSystemApi.Constants;

    [ApiController]
    [Route($"{Constants.ApiV1RoutePrefix}[controller]")]
    [ApiVersion("1.0")]
    public class AddressesController : ControllerBase
    {
        private readonly IRetrieveAddressesUseCase _retrieveAddressesUseCase;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(IRetrieveAddressesUseCase retrieveAddressesUseCase, ILogger<AddressesController> logger)
        {
            _retrieveAddressesUseCase = retrieveAddressesUseCase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Address([FromQuery] string postcode)
        {
            try
            {
                var result = await _retrieveAddressesUseCase.Execute(postcode);
                _logger.LogInformation("Number of results being returned by AddressesController.Address: {Count} - for postcode {Postcode}", result?.Count(), postcode);
                return Ok(result);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogInformation("Error {ErrorMessage} occurred in AddressesController.Address while retrieving address results for postcode {PostCode}", e.Message, postcode);
                return StatusCode(500, e.Message);
            }
        }
    }
}
