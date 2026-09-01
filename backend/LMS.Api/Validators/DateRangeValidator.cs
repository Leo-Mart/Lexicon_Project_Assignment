using LMS.Api.Exceptions;

namespace LMS.Api.Validators;

public static class DateRangeValidator
{
    public static void ValidateRange<T>(T start, T end, string entityName)
        where T : IComparable<T>
    {
        if (start.CompareTo(end) >= 0)
        {
            throw new InvalidDateException(
                $"{entityName} end date/time must be after start date/time.",
                400
            );
        }
    }

    public static void ValidateWithinParent<T>(
        T start,
        T end,
        T parentStart,
        T parentEnd,
        string entityName,
        string parentName
    )
        where T : IComparable<T>
    {
        if (start.CompareTo(parentStart) < 0 || end.CompareTo(parentEnd) > 0)
        {
            throw new InvalidDateException(
                $"{entityName} start or end date/time is outside the timeframe of {parentName}.",
                400
            );
        }
    }

    public static bool Overlaps<T>(T start, T end, T existingStart, T existingEnd)
        where T : IComparable<T>
    {
        return start.CompareTo(existingEnd) < 0 && end.CompareTo(existingStart) > 0;
    }

    public static void ValidateNotBefore<T>(T start, T minimum, string entityName)
        where T : IComparable<T>
    {
        if (start.CompareTo(minimum) < 0)
        {
            throw new InvalidDateException(
                $"{entityName} start date/time cannot be in the past.",
                400
            );
        }
    }
}
