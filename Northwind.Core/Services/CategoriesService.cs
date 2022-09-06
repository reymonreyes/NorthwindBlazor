using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;

namespace Northwind.Core.Services
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

            var serviceResult = Validate(categoryDto);
            if(!serviceResult.IsSuccessful)
                return serviceResult;

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            await _unitOfWork.CategoriesRepository.Create(category);
            await _unitOfWork.Commit();

            serviceResult!.Messages?.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", category.Id.ToString()) });
            return serviceResult;
        }

        private ServiceResult Validate(CategoryDto categoryDto)
        {
            var result = new ServiceResult { Messages = new List<ServiceMessageResult>() };
            var validationResult = _categoryValidator.Validate(categoryDto);
            if (validationResult?.Count > 0)
            {
                result.IsSuccessful = false;
                result.Messages.AddRange(validationResult);
                return result;
            }

            result.IsSuccessful = true;
            return result;
        }

        public async Task Delete(int categoryId)
        {
            await _unitOfWork.CategoriesRepository.Delete(categoryId);
            await _unitOfWork.Commit();
        }

        public async Task<ServiceResult> Edit(int categoryId, CategoryDto? categoryDto)
        {
            if(categoryId <= 0)
                throw new ArgumentOutOfRangeException("categoryId");
            if(categoryDto is null)
                throw new ArgumentNullException("category");

            var category = await _unitOfWork.CategoriesRepository.Get(categoryId);
            if (category is null)
                throw new DataNotFoundException("Category data not found.");

            var serviceResult = Validate(categoryDto);
            if (!serviceResult.IsSuccessful)
                return serviceResult;

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            await _unitOfWork.Commit();

            serviceResult!.Messages?.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Edit", "Category updated successfully.") });

            return serviceResult;
        }

        public async Task<CategoryDto?> Get(int categoryId)
        {
            var category = await _unitOfWork.CategoriesRepository.Get(categoryId);
            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<ICollection<CategoryDto>> GetAll()
        {
            var categories = await _unitOfWork.CategoriesRepository.GetAll();

            return categories.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
        }


    }
}