using JobPortalAPI.Controllers;
using JobPortalAPI.Data;
using JobPortalAPI.Entity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.Unit_Test
{
    [TestFixture]
    public class CandidateServiceTests(
        Mock<ApplicationDbContext> _mockDbContext, 
        CandidateController _controller)
    {
        [Test]
        public void CandidateEntity_ShouldPassValidation_WhenValidDataProvided()
        {
            // Arrange
            var candidate = new Candidate
            {
                Email = "validemail@test.com",
                FirstName = "mahesh",
                LastName = "madai",
                Comment = "This is a test comment for test",
                AppointmentTime = DateTime.Now
            };

            // Act
            var context = new ValidationContext(candidate, null, null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(candidate, context, validationResults, true);

            // Assert
            Assert.That(isValid, Is.True);
            Assert.Equals(0, validationResults.Count); 
        }

        [Test]
        public void CandidateEntity_ShouldFailValidation_WhenRequiredFieldsMissing()
        {
            // Arrange
            var candidate = new Candidate
            {
                // Missing required fields like Email, FirstName, LastName, and Comment
                AppointmentTime = DateTime.Now
            };

            // Act
            var context = new ValidationContext(candidate, null, null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(candidate, context, validationResults, true);

            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults.Count > 0, Is.True); 
        }

        [Test]
        public void CandidateEntity_ShouldFailValidation_WhenInvalidEmailProvided()
        {
            // Arrange
            var candidate = new Candidate
            {
                Email = "testemail",  // Invalid email format
                FirstName = "asd",
                LastName = "sds",
                Comment = "This is a test comment",
                AppointmentTime = DateTime.Now
            };

            // Act
            var context = new ValidationContext(candidate, null, null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(candidate, context, validationResults, true);

            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(validationResults.Exists(v => v.ErrorMessage.Contains("not a valid e-mail address")), Is.True);
        }

        [Test]
        public async Task AddOrUpdateCandidateInformation_ShouldAddCandidate_WhenCandidateDoesNotExist()
        {
            // Arrange
            var newCandidate = new Candidate
            {
                Email = "newcandidate@example.com",
                FirstName = "John",
                LastName = "Doe",
                Comment = "This is a comment",
                AppointmentTime = DateTime.Now
            };

            _mockDbContext.Setup(db => db.Candidates.FindAsync(newCandidate.Email)).ReturnsAsync((Candidate)null); // Candidate doesn't exist

            // Act
            var result = await _controller.AddOrUpdateCandidateInformation(newCandidate);

            // Assert
            Assert.That(result, Is.Not.Null);
            var actionResult = result.Result as OkObjectResult;

            Assert.That(actionResult, Is.Not.Null);
            var returnedCandidate = actionResult.Value as Candidate;
            Assert.Equals(newCandidate.Email, returnedCandidate.Email);
        }

        [Test]
        public async Task AddOrUpdateCandidateInformation_ShouldUpdateCandidate_WhenCandidateExists()
        {
            // Arrange
            var existingCandidate = new Candidate
            {
                Email = "test@test.com",
                FirstName = "asdads",
                LastName = "adsddf",
                Comment = "Existing test comment",
                AppointmentTime = DateTime.Now
            };

            _mockDbContext.Setup(db => db.Candidates.FindAsync(existingCandidate.Email)).ReturnsAsync(existingCandidate); // Candidate exists

            // Act
            var result = await _controller.AddOrUpdateCandidateInformation(existingCandidate);

            // Assert
            Assert.That(result, Is.Not.Null);
            var actionResult = result.Result as OkObjectResult;

            Assert.That(actionResult, Is.Not.Null);
            var returnedCandidate = actionResult.Value as Candidate;
            Assert.Equals(existingCandidate.Email, returnedCandidate.Email);
        }
    }
}
