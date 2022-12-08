using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace HousingManagementSystemApi.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Sentry;
    using UseCases;
    using Constants = HousingManagementSystemApi.Constants;

    [ApiController]
    [Route($"{Constants.ApiV1RoutePrefix}[controller]")]
    [ApiVersion("1.0")]
    public class AddressesController : ControllerBase
    {
        private readonly IRetrieveAddressesUseCase retrieveAddressesUseCase;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(IRetrieveAddressesUseCase retrieveAddressesUseCase, ILogger<AddressesController> logger)
        {
            this.retrieveAddressesUseCase = retrieveAddressesUseCase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Address([FromQuery] string postcode)
        {
            try
            {
                var result = await retrieveAddressesUseCase.Execute(postcode);
                _logger.LogInformation($"Number of results being returned by AddressesController.Address: {result.Count()} - for postcode ${postcode}");
                return Ok(result);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _logger.LogInformation($"Error {e.Message} occurred in AddressesController.Address while retrieving address results for postcode {postcode}. Exception {e}");
                return StatusCode(500, e.Message);
            }
        }
    }
}
