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
        public async Task<ServiceResult> Create(CategoryDto categoryDto)
        {
            if(categoryDto == null)
                throw new ArgumentNullException("category");

            Validate(categoryDto);
            
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            await _unitOfWork.Start();
            await _unitOfWork.CategoriesRepository.Create(category);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", category.Id.ToString()) });
            
            return result;
        }

        private void Validate(CategoryDto categoryDto)
        {
            var validationResult = _categoryValidator.Validate(categoryDto);
            if (validationResult?.Count > 0)
                throw new ValidationFailedException(validationResult);
        }

        public async Task Delete(int categoryId)
        {
            await _unitOfWork.Start();
            await _unitOfWork.CategoriesRepository.Delete(categoryId);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task<ServiceResult> Update(int categoryId, CategoryDto categoryDto)
        {
            if(categoryId <= 0)
                throw new ArgumentOutOfRangeException("categoryId");
            if(categoryDto is null)
                throw new ArgumentNullException("category");
            Validate(categoryDto);

            await _unitOfWork.Start();
            var category = await _unitOfWork.CategoriesRepository.Get(categoryId);
            if (category is null)
                throw new DataNotFoundException("Category not found.");

            Validate(categoryDto);
            
            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            await _unitOfWork.CategoriesRepository.Update(category);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { Messages = new List<ServiceMessageResult>() };
            result!.Messages?.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Edit", "Category updated successfully.") });

            return result;
        }

        public async Task<CategoryDto?> Get(int categoryId)
        {
            await _unitOfWork.Start();
            var category = await _unitOfWork.CategoriesRepository.Get(categoryId);
            await _unitOfWork.Stop();
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
            await _unitOfWork.Start();
            var categories = await _unitOfWork.CategoriesRepository.GetAll();
            await _unitOfWork.Stop();

            return categories.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToList();
        }
    }
}