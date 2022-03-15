using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;

namespace Northwind.Core
{
    public class CategoriesService : ICategoriesService
    {
        private ICategoryValidator _categoryValidator;
        public CategoriesService(ICategoryValidator categoryValidator)
        {
            _categoryValidator = categoryValidator;
        }
        public async Task<ServiceResult> Create(CategoryDto? categoryDto)
        {
            if(categoryDto == null)
                throw new ArgumentNullException("category");

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };

            var validationResult = _categoryValidator.Validate(categoryDto);            
            if(validationResult?.Count > 0)
            {
                result.IsSuccessful = false;
                validationResult.AddRange(validationResult);
            }

            if (!result.IsSuccessful)
                return result;



            return result;
        }
    }
}