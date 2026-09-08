using System.ComponentModel.DataAnnotations;
using LMS.Api.Constants;

namespace LMS.Api.DTOs.Common;

public class UserQueryParametersDto
{
    public string? Search { get; set; }

    public string SortBy { get; set; } = UserQueryConstants.DefaultSortBy;

    public string Direction { get; set; } = UserQueryConstants.DefaultDirection;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = UserQueryConstants.DefaultPage;

    [Range(1, UserQueryConstants.MaxPageSize)]
    public int PageSize { get; set; } = UserQueryConstants.DefaultPageSize;
}
