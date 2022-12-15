using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingManagementSystemApi.Models;

namespace HousingManagementSystemApi.UseCases.Interfaces
{
    public interface IVerifyPropertyEligibilityUseCase
    {
        public Task<PropertyEligibilityResult> Execute(string propertyId);
    }
}
