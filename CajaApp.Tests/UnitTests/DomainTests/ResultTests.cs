using CajaApp.Domain.Shared;
using Xunit;

namespace CajaApp.Tests.UnitTests.DomainTests;

public class ResultTests
{
    [Fact]
    public void Result_Ok_CreatesSuccessResult()
    {
        var result = Result.Ok();
        
        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Result_Fail_CreatesFailureResult()
    {
        var error = new Error("TEST.ERROR", "Test error message");
        var result = Result.Fail(error);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Result_Fail_WithParameters_CreatesFailureResult()
    {
        var result = Result.Fail("TEST.ERROR", "Test error message");
        
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("TEST.ERROR", result.Error.Code);
        Assert.Equal("Test error message", result.Error.Message);
    }

    [Fact]
    public void ResultT_Ok_CreatesSuccessResultWithValue()
    {
        var result = Result<int>.Ok(42);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void ResultT_Fail_CreatesFailureResult()
    {
        var error = new Error("TEST.ERROR", "Test error message");
        var result = Result<int>.Fail(error);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(0, result.Value);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void ResultT_ImplicitConversion_FromValue()
    {
        Result<int> result = 42;
        
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ResultT_ImplicitConversion_FromError()
    {
        Result<int> result = new Error("TEST.ERROR", "Test error");
        
        Assert.False(result.IsSuccess);
        Assert.Equal("TEST.ERROR", result.Error?.Code);
    }

    [Fact]
    public void Error_ToString_ReturnsFormattedString()
    {
        var error = new Error("TEST.ERROR", "Test error message");
        
        var result = error.ToString();
        
        Assert.Equal("[TEST.ERROR] Test error message", result);
    }
}
