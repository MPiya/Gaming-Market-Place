using System;
using System.Collections.Generic;
using System.Linq;

using Moq;
using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
	public class ProductControllerTests
	{
		[Fact]
		public void GetAll_ReturnsJsonResult()
		{
			// Arrange
			var unitOfWorkMock = new Mock<UnitOfWork>();
		}

	}
}

