using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace HousingManagementSystemApi.Controllers
{
    using System;
    using HousingManagementSystemApi.UseCases.Interfaces;
    using Microsoft.Extensions.Logging;
    using Sentry;
    using UseCases;
    using Constants = HousingManagementSystemApi.Constants;

    [ApiController]
    [Route($"{Constants.ApiV1RoutePrefix}[controller]")]
    [ApiVersion("1.0")]
    public class PropertyEligibleController : ControllerBase
    {
        private readonly IVerifyPropertyEligibilityUseCase _verifyPropertyEligibilityUseCase;
        private readonly ILogger<AddressesController> _logger;

        public PropertyEligibleController(IVerifyPropertyEligibilityUseCase verifyPropertyEligibilityUseCase, ILogger<AddressesController> logger)
        {
            _verifyPropertyEligibilityUseCase = verifyPropertyEligibilityUseCase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPropertyEligibility([FromQuery] string propertyId)
        {
            _logger.LogInformation("Verifying property eligibility for property {PropertyId}",  propertyId);

            try
            {
                var result = await _verifyPropertyEligibilityUseCase.Execute(propertyId);
                return Ok(result);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return StatusCode(500, e.Message);
            }
        }
    }
}
