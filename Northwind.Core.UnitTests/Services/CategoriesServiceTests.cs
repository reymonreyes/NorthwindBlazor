using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using Northwind.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Core.UnitTests.Services
{
    public class CategoriesServiceTests
    {
        [Fact]
        public async Task Create_ShouldThrowExceptionIfInputIsNull()
        {
            var mock = AutoMock.GetLoose();
            ICategoriesService categoriesService = mock.Create<CategoriesService>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => categoriesService.Create(null));
        }

        [Fact]
        public void Create_ShouldUseValidatorForInputValidation()
        {
            var mock = AutoMock.GetLoose();
            var validatorMock = mock.Mock<ICategoryValidator>();
            validatorMock.Setup(x => x.Validate(It.IsAny<CategoryDto>())).Returns(new List<ServiceMessageResult>
            {
                new ServiceMessageResult{
                    MessageType = Enums.ServiceMessageType.Error,
                    Message = new KeyValuePair<string, string>("Name", "Required")
                }
            }).Verifiable();
            var categoryDto = new CategoryDto { Name = "Category A" };
            ICategoriesService categoriesService = mock.Create<CategoriesService>();
            categoriesService.Create(categoryDto);
            validatorMock.Verify(x => x.Validate(categoryDto), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldFailIfInvalidInputs()
        {
            var mock = AutoMock.GetLoose();
            mock.Mock<ICategoryValidator>().Setup(x => x.Validate(It.IsAny<CategoryDto>())).Returns(new List<ServiceMessageResult>
            {
                new ServiceMessageResult{
                    MessageType = Enums.ServiceMessageType.Error,
                    Message = new KeyValuePair<string, string>("Name", "Required")
                }
            });
            ICategoriesService categoriesService = mock.Create<CategoriesService>();
            var result = await categoriesService.Create(new CategoryDto());
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        [Trait("CRUD", "Edit")]
        public async Task Edit_ShouldThrowExceptionIfInputIsNull()
        {           
            ICategoriesService categoriesService = GetCategoriesServiceMock();
            await Assert.ThrowsAsync<ArgumentNullException>(() => categoriesService.Edit(1, null));
        }

        [Fact]
        [Trait("CRUD", "Edit")]
        public async Task Edit_ShouldThrowExceptionIfInvalidId()
        {
            ICategoriesService categoriesService = GetCategoriesServiceMock();
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => categoriesService.Edit(0, null));
            Assert.True(exception.ParamName == "categoryId");
        }

        [Fact]
        [Trait("CRUD", "Edit")]
        public async Task Edit_ShouldThrowExceptionCategoryNotFound()
        {
            var mock = AutoMock.GetLoose();
            var categoriesRepo = mock.Mock<ICategoriesRepository>();
            categoriesRepo.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync((Category?)null);
            mock.Mock<IUnitOfWork>().Setup(x => x.CategoriesRepository).Returns(categoriesRepo.Object);
            ICategoriesService categoriesService = mock.Create<CategoriesService>();
            var exception = await Assert.ThrowsAsync<Exception>(() => categoriesService.Edit(1, new CategoryDto { Name = "Beverage", Description = "Test" }));
            Assert.True(exception.Message == "not found");
        }

        private ICategoriesService GetCategoriesServiceMock()
        {
            var mock = AutoMock.GetLoose();
            return mock.Create<CategoriesService>();
        }
    }
}
