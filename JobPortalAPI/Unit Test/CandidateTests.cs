using JobPortalAPI.Controllers;
using JobPortalAPI.Data;
using JobPortalAPI.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
namespace JobPortalAPI.Unit_Test
{
    [TestFixture]
    public class CandidateServiceTests
    {
        private Mock<DbSet<Candidate>> _mockSet;
        private Mock<DbContext> _mockContext;
        private Mock<ILogger<CandidateController>> _mockLogger;
        private CandidateController _service;

        [SetUp]
        public void SetUp()
        {
            _mockSet = new Mock<DbSet<Candidate>>();
            _mockContext = new Mock<DbContext>();
            _mockLogger = new Mock<ILogger<CandidateController>>();

            _mockContext.Setup(m => m.Set<Candidate>()).Returns(_mockSet.Object);
        }

        [Test]
        public async Task AddOrUpdateCandidateInformation_NewCandidate_AddsToDatabase()
        {
            // Arrange
            var candidate = new Candidate { Email = "new@example.com" };
            _mockContext.Setup(m => m.Add(It.IsAny<Candidate>())).Verifiable();
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _service.AddOrUpdateCandidateInformation(candidate);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            _mockContext.Verify(m => m.Add(It.IsAny<Candidate>()), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        [Test]
        public async Task AddOrUpdateCandidateInformation_ExistingCandidate_UpdatesDatabase()
        {
            // Arrange
            var candidate = new Candidate { Email = "existing@example.com" };
            _mockContext.Setup(m => m.Update(It.IsAny<Candidate>())).Verifiable();
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);
            _service.GetType().GetMethod("candidateExists", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, new object[] { candidate.Email });

            // Act
            var result = await _service.AddOrUpdateCandidateInformation(candidate);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            _mockContext.Verify(m => m.Update(It.IsAny<Candidate>()), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
        }

        [Test]
        public async Task AddOrUpdateCandidateInformation_DbUpdateException_ReturnsInternalServerError()
        {
            // Arrange
            var candidate = new Candidate { Email = "error@example.com" };
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _service.AddOrUpdateCandidateInformation(candidate);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var statusCodeResult = (ObjectResult)result.Result;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}
