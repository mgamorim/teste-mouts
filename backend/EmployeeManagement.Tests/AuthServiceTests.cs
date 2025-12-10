using EmployeeManagement.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using EmployeeManagement.Api.Repositories;
using Xunit;
using System;

namespace EmployeeManagement.Tests;

public class AuthServiceTests
{
    [Fact]
    public void IsAdult_ShouldReturnFalse_ForMinor()
    {
        var repoMock = new Mock<IEmployeeRepository>();
        var service = new AuthService(repoMock.Object, new NullLogger<AuthService>());
        var dob = DateTime.UtcNow.AddYears(-17);
        service.IsAdult(dob).Should().BeFalse();
    }

    [Fact]
    public void IsAdult_ShouldReturnTrue_ForAdult()
    {
        var repoMock = new Mock<IEmployeeRepository>();
        var service = new AuthService(repoMock.Object, new NullLogger<AuthService>());
        var dob = DateTime.UtcNow.AddYears(-20);
        service.IsAdult(dob).Should().BeTrue();
    }
}
