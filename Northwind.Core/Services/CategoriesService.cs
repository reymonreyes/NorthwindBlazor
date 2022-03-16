using Northwind.Core.Dtos;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;

namespace Northwind.Core
{
    public class CategoriesService : ICategoriesService
    {
        private IUnitOfWork _unitOfWork;
        private ICategoryValidator _categoryValidator;
        public CategoriesService(IUnitOfWork unitOfWork, ICategoryValidator categoryValidator)
        {
            _unitOfWork = unitOfWork;
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

                return result;
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            await _unitOfWork.CategoriesRepository.Create(category);
            await _unitOfWork.Commit();

            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", category.Id.ToString()) });
            return result;
        }
    }
}