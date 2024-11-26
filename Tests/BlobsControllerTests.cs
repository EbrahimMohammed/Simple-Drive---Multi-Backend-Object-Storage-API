using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using API.Controllers;
using API.Dtos;
using API.Validators;
using Services.StorageBackendsServices;
using Business.BlobTrackingsRepository;
namespace Tests
{


    public class BlobsControllerTests
    {
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly Mock<IBlobTrackingsRepository> _blobTrackingRepoMock;
        private readonly BlobsController _controller;

        public BlobsControllerTests()
        {
            _storageServiceMock = new Mock<IStorageService>();
            _blobTrackingRepoMock = new Mock<IBlobTrackingsRepository>();
            _controller = new BlobsController(_storageServiceMock.Object, _blobTrackingRepoMock.Object);
        }

        [Fact]
        public async Task UploadBlob_ValidInput_ReturnsOk()
        {
            // Arrange
            var request = new CreateBlobRequest
            {
                Id = "valid-id",
                Data = Convert.ToBase64String(new byte[] { 1, 2, 3 })
            };

            _storageServiceMock
                .Setup(service => service.StoreBlobAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UploadBlob(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                  .Which.Value.Should().Be("Blob uploaded successfully.");

            _storageServiceMock.Verify(
                service => service.StoreBlobAsync(request.Id, It.Is<byte[]>(data => data.Length == 3)),
                Times.Once);
        }

        [Fact]
        public async Task UploadBlob_InvalidBase64Data_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateBlobRequest
            {
                Id = "valid-id",
                Data = "INVALID_BASE64"
            };

            // Act
            var result = await _controller.UploadBlob(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be("Invalid Base64 string in the 'data' field.");
        }



        [Fact]
        public async Task UploadBlob_MissingId_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateBlobRequest
            {
                Id = "",
                Data = Convert.ToBase64String(new byte[] { 1, 2, 3 })
            };

            var validator = new BlobUploadRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                _controller.ModelState.Clear(); // Ensure a fresh ModelState
                foreach (var error in validationResult.Errors)
                {
                    _controller.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            // Act
            var result = await _controller.UploadBlob(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }



        [Fact]
        public async Task UploadBlob_MissingData_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateBlobRequest
            {
                Id = "valid-id",
                Data = ""
            };

            var validator = new BlobUploadRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                _controller.ModelState.Clear(); // Ensure a fresh ModelState
                foreach (var error in validationResult.Errors)
                {
                    _controller.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }

            // Act
            var result = await _controller.UploadBlob(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

    }

}
