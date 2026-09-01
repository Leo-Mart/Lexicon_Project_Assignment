using LMS.Api.Validators;

namespace LMS.Api.Tests.Validators;

public class DateRangeValidatorTests
{
    [Fact]
    public void ValidateRange_EndAfterStart_ShouldNotThrow()
    {
        DateOnly start = new(2026, 9, 1);
        DateOnly end = new(2026, 9, 10);

        Exception? exception = Record.Exception(() => DateRangeValidator.ValidateRange(start, end, "Module"));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateRange_EndBeforeStart_ShouldThrowArgumentException()
    {
        DateOnly start = new(2026, 9, 10);
        DateOnly end = new(2026, 9, 1);

        ArgumentException exception = Assert.Throws<ArgumentException>(() => DateRangeValidator.ValidateRange(start, end, "Module"));

        Assert.Contains("Module", exception.Message);
    }

    [Fact]
    public void ValidateRange_StartEqualsEnd_ShouldThrowArgumentException()
    {
        DateOnly date = new(2026, 9, 1);

        Assert.Throws<ArgumentException>(() => DateRangeValidator.ValidateRange(date, date, "Module"));
    }

    [Fact]
    public void ValidateWithinParent_RangeInsideParent_ShouldNotThrow()
    {
        DateOnly parentStart = new(2026, 9, 1);
        DateOnly parentEnd = new(2026, 12, 31);
        DateOnly start = new(2026, 10, 1);
        DateOnly end = new(2026, 10, 31);

        Exception? exception = Record.Exception(
            () => DateRangeValidator.ValidateWithinParent(
                start,
                end,
                parentStart,
                parentEnd,
                "Module",
                "Course"
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateWithinParent_SameAsParentBoundaries_ShouldNotThrow()
    {
        DateOnly parentStart = new(2026, 9, 1);
        DateOnly parentEnd = new(2026, 12, 31);

        Exception? exception = Record.Exception(
            () => DateRangeValidator.ValidateWithinParent(
                parentStart,
                parentEnd,
                parentStart,
                parentEnd,
                "Module",
                "Course"
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateWithinParent_StartBeforeParent_ShouldThrowArgumentException()
    {
        DateOnly parentStart = new(2026, 9, 1);
        DateOnly parentEnd = new(2026, 12, 31);
        DateOnly start = new(2026, 8, 31);
        DateOnly end = new(2026, 10, 1);

        Assert.Throws<ArgumentException>(
            () => DateRangeValidator.ValidateWithinParent(
                start,
                end,
                parentStart,
                parentEnd,
                "Module",
                "Course"
            )
        );
    }

    [Fact]
    public void ValidateWithinParent_EndAfterParent_ShouldThrowArgumentException()
    {
        DateOnly parentStart = new(2026, 9, 1);
        DateOnly parentEnd = new(2026, 12, 31);
        DateOnly start = new(2026, 12, 1);
        DateOnly end = new(2027, 1, 1);

        Assert.Throws<ArgumentException>(
            () => DateRangeValidator.ValidateWithinParent(
                start,
                end,
                parentStart,
                parentEnd,
                "Module",
                "Course"
            )
        );
    }

    [Fact]
    public void Overlaps_OverlappingRanges_ShouldReturnTrue()
    {
        DateTime existingStart = new(2026, 9, 1, 10, 0, 0);
        DateTime existingEnd = new(2026, 9, 1, 12, 0, 0);

        DateTime start = new(2026, 9, 1, 11, 0, 0);
        DateTime end = new(2026, 9, 1, 13, 0, 0);

        bool result = DateRangeValidator.Overlaps(
            start,
            end,
            existingStart,
            existingEnd
        );

        Assert.True(result);
    }

    [Fact]
    public void Overlaps_NonOverlappingRanges_ShouldReturnFalse()
    {
        DateTime existingStart = new(2026, 9, 1, 10, 0, 0);
        DateTime existingEnd = new(2026, 9, 1, 12, 0, 0);

        DateTime start = new(2026, 9, 1, 13, 0, 0);
        DateTime end = new(2026, 9, 1, 14, 0, 0);

        bool result = DateRangeValidator.Overlaps(
            start,
            end,
            existingStart,
            existingEnd
        );

        Assert.False(result);
    }

    [Fact]
    public void Overlaps_StartsWhenExistingEnds_ShouldReturnFalse()
    {
        DateTime existingStart = new(2026, 9, 1, 10, 0, 0);
        DateTime existingEnd = new(2026, 9, 1, 11, 0, 0);

        DateTime start = existingEnd;
        DateTime end = new(2026, 9, 1, 12, 0, 0);

        bool result = DateRangeValidator.Overlaps(
            start,
            end,
            existingStart,
            existingEnd
        );

        Assert.False(result);
    }

    [Fact]
    public void ValidateNotBefore_StartAfterMinimum_ShouldNotThrow()
    {
        DateOnly minimum = new(2026, 9, 1);
        DateOnly start = new(2026, 9, 2);

        Exception? exception = Record.Exception(() => DateRangeValidator.ValidateNotBefore(start, minimum, "Module"));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateNotBefore_StartEqualsMinimum_ShouldNotThrow()
    {
        DateOnly minimum = new(2026, 9, 1);

        Exception? exception = Record.Exception(() => DateRangeValidator.ValidateNotBefore(minimum, minimum, "Module"));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateNotBefore_StartBeforeMinimum_ShouldThrowArgumentException()
    {
        DateOnly minimum = new(2026, 9, 1);
        DateOnly start = new(2026, 8, 31);

        ArgumentException exception = Assert.Throws<ArgumentException>(() => DateRangeValidator.ValidateNotBefore(start, minimum, "Module"));

        Assert.Contains("Module", exception.Message);
    }
}
