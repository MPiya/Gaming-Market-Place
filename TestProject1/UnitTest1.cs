using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using F2Play.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using F2Play.WebApp.Areas.Admin.Controllers;
using F2Play.Models;
using System.Linq.Expressions;

namespace YourTestProjectNamespace
{
	public class ProductControllerTests
	{
		[Fact]
		public void GetAll_ReturnsJsonResult()
		{
			// Arrange
			var unitOfWorkMock = new Mock<IUnitOfWork>();
			var hostEnvironmentMock = new Mock<IWebHostEnvironment>();
			var controller = new ProductController(unitOfWorkMock.Object, hostEnvironmentMock.Object);

			// Create a list of dummy products for testing
			var dummyProducts = new List<Product>
			{
				new Product { Id = 1 },
				new Product { Id = 2 }
               
            };

			// Mock the behavior of _unitOfWork.Product.GetAll
			unitOfWorkMock.Setup(u => u.Product.GetAll(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<string>()))
				.Returns(dummyProducts);

			// Act
			var result = controller.GetAll() as JsonResult;
			
			// Assert
			Assert.NotNull(result);
			Assert.IsType<JsonResult>(result);


		}
	}
}
