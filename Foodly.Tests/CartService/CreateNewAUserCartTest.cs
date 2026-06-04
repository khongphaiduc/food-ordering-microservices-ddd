
using cart_service.CartService.Application.DTOs;
using cart_service.CartService.Domain.Aggregate;
using cart_service.CartService.Domain.Interface;
using cart_service.CartService.Infrastructure.ImplementServices;
using Microsoft.Extensions.Logging;
using Moq;


namespace Foodly.Tests.CartService
{
    public class CreateNewAUserCartTest
    {
        private readonly Mock<ICartRepository> _mockCartRepo;
        private readonly Mock<ILogger<CreateNewCart>> _mockLogger;
        private readonly CreateNewCart _sut;
        public CreateNewAUserCartTest()
        {
            _mockCartRepo = new Mock<ICartRepository>();
            _mockLogger = new Mock<ILogger<CreateNewCart>>();
            _sut = new CreateNewCart(_mockCartRepo.Object, _mockLogger.Object);

        }

        [Fact]
        public async Task Execute_ValidRequest_ShouldReturnCartIdAndCallRepoOnce()
        {
          
            var userId = Guid.NewGuid();
            var request = new RequestCreateNewCartUser { UserId = userId };
            var expectedCartId = Guid.NewGuid();

         
            _mockCartRepo
                .Setup(repo => repo.CreateCartAsync(It.IsAny<CartAggregate>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCartId);

            
            var result = await _sut.Execute(request, CancellationToken.None);

            
            Assert.Equal(expectedCartId, result);

            
            _mockCartRepo.Verify(
                repo => repo.CreateCartAsync(
                    It.Is<CartAggregate>(cart => cart.UserId == userId),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

    }
}
