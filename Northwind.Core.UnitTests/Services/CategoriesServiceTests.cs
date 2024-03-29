﻿using Autofac;
using Autofac.Extras.Moq;
using Moq;
using Northwind.Core.Validators;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
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
            var categoryValidator = new CategoryValidator();
            var mock = AutoMock.GetLoose(config =>
            {
                config.RegisterInstance(categoryValidator).As<ICategoryValidator>();
            });
            ICategoriesService categoriesService = mock.Create<CategoriesService>();
            var exception = await Assert.ThrowsAsync<ValidationFailedException>(() => categoriesService.Create(new CategoryDto()));
        }

        [Fact]
        [Trait("CRUD", "Edit")]
        public async Task Edit_ShouldThrowExceptionIfInputIsNull()
        {           
            ICategoriesService categoriesService = GetCategoriesServiceMock();
            await Assert.ThrowsAsync<ArgumentNullException>(() => categoriesService.Update(1, null!));
        }

        [Fact]
        [Trait("CRUD", "Edit")]
        public async Task Edit_ShouldThrowExceptionIfInvalidId()
        {
            ICategoriesService categoriesService = GetCategoriesServiceMock();
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => categoriesService.Update(0, new CategoryDto { Name = "Beverage", Description = "Test" }));
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
            await Assert.ThrowsAsync<DataNotFoundException>(() => categoriesService.Update(1, new CategoryDto { Name = "Beverage", Description = "Test" }));
        }

        [Fact]
        [Trait("CRUD", "Delete")]
        public async Task Delete_ShouldThrowDataNotFoundException()
        {
            var mock = AutoMock.GetLoose();
            var categoryRepo = mock.Mock<ICategoriesRepository>();
            categoryRepo.Setup(x => x.Delete(It.IsAny<int>())).Throws<DataNotFoundException>();
            var uOW = mock.Mock<IUnitOfWork>();
            uOW.Setup(x => x.CategoriesRepository).Returns(categoryRepo.Object);
            ICategoriesService categoriesService = mock.Create<CategoriesService>();
            await Assert.ThrowsAsync<DataNotFoundException>(() => categoriesService.Delete(1));
        }

        [Fact]
        [Trait("CRUD", "Delete")]
        public async Task Delete_ShouldThrowDataStoreException()
        {
            var mock = AutoMock.GetLoose();
            var categoryRepo = mock.Mock<ICategoriesRepository>();
            categoryRepo.Setup(x => x.Delete(It.IsAny<int>())).Returns(Task.CompletedTask);
            var uOW = mock.Mock<IUnitOfWork>();
            uOW.Setup(x => x.CategoriesRepository).Returns(categoryRepo.Object);
            uOW.Setup(x => x.Commit()).Throws<DataStoreException>();
            ICategoriesService categoriesService = mock.Create<CategoriesService>();
            await Assert.ThrowsAsync<DataStoreException>(() => categoriesService.Delete(1));
        }

        private ICategoriesService GetCategoriesServiceMock()
        {
            var mock = AutoMock.GetLoose();
            return mock.Create<CategoriesService>();
        }
    }
}
