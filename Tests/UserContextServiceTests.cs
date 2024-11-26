using Business.UserContextService;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UserContextServiceTests
    {
        [Fact]
        public void UserId_ValidClaim_ReturnsUserId()
        {
            // Arrange
            var userId = 123;
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            contextAccessorMock.Setup(x => x.HttpContext.User.FindFirst("UserId"))
                .Returns(new Claim("UserId", userId.ToString()));

            var userContextService = new UserContextService(contextAccessorMock.Object);

            // Act
            var result = userContextService.UserId;

            // Assert
            Assert.Equal(userId, result);
        }

    }
}
