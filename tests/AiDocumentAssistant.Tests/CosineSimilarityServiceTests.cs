using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class CosineSimilarityServiceTests
{
    private readonly CosineSimilarityService _service = new();

    [Fact]
    public void Calculate_ReturnsOne_WhenVectorsAreIdentical()
    {
        var first = new float[] { 1, 2, 3 };

        var result = _service.Calculate(first, first);

        Assert.Equal(1.0, result, 6);
    }

    [Fact]
    public void Calculate_ReturnsZero_WhenVectorsArePerpendicular()
    {
        var first = new float[] { 1, 0 };

        var second = new float[] { 0, 1 };

        var result = _service.Calculate(first, second);

        Assert.Equal(0.0, result, 6);
    }

    [Fact]
    public void Calculate_ReturnsNegativeOne_WhenVectorsPointInOppositeDirections()
    {
        var first = new float[] { 1, 0 };

        var second = new float[] { -1, 0 };

        var result = _service.Calculate(first, second);

        Assert.Equal(-1.0, result, 6);
    }

    [Fact]
    public void Calculate_ReturnsOne_WhenVectorsHaveSameDirectionButDifferentMagnitude()
    {
        var first = new float[] { 1, 2, 3 };

        var second = new float[] { 2, 4, 6 };

        var result = _service.Calculate(first, second);

        Assert.Equal(1.0, result, 6);
    }

    [Fact]
    public void Calculate_ThrowsException_WhenVectorsHaveDifferentLengths()
    {
        var first = new float[] { 1, 2 };

        var second = new float[] { 1, 2, 3 };

        Assert.Throws<ArgumentException>(
            () => _service.Calculate(first, second));
    }

    [Fact]
    public void Calculate_ThrowsException_WhenVectorHasZeroMagnitude()
    {
        var first = new float[] { 0, 0 };

        var second = new float[] { 1, 2 };

        Assert.Throws<ArgumentException>(
            () => _service.Calculate(first, second));
    }

    [Fact]
    public void Calculate_ThrowsException_WhenVectorIsEmpty()
    {
        Assert.Throws<ArgumentException>(
            () => _service.Calculate(
                Array.Empty<float>(),
                Array.Empty<float>()));
    }
}