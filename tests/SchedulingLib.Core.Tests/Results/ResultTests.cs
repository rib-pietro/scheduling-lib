using SchedulingLib.Core.Results;

namespace SchedulingLib.Core.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Ok_NonGeneric_IsSuccess()
    {
        var result = Result.Ok();
        Assert.True(result.IsSuccess);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Fail_NonGeneric_IsNotSuccess()
    {
        var result = Result.Fail("something went wrong");
        Assert.False(result.IsSuccess);
        Assert.Equal("something went wrong", result.ErrorMessage);
    }

    [Fact]
    public void Ok_Generic_CarriesValue()
    {
        var result = Result.Ok(42);
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Fail_Generic_HasNullValue()
    {
        var result = Result.Fail<int>("error");
        Assert.False(result.IsSuccess);
        Assert.Equal(default, result.Value);
        Assert.Equal("error", result.ErrorMessage);
    }

    [Fact]
    public void Ok_ViaStaticHelper_Generic_Works()
    {
        var result = Result.Ok("hello");
        Assert.True(result.IsSuccess);
        Assert.Equal("hello", result.Value);
    }
}
