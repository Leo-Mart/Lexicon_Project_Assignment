using System.ComponentModel.DataAnnotations;
using LMS.Api.Constants;

namespace LMS.Api.DTOs.Common;

public class QueryParametersDto
{
    public string? Search { get; set; }

    public string SortBy { get; set; } = QueryConstants.DefaultSortBy;

    public string Direction { get; set; } = QueryConstants.DefaultDirection;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = QueryConstants.DefaultPage;

    [Range(1, QueryConstants.MaxPageSize)]
    public int PageSize { get; set; } = QueryConstants.DefaultPageSize;
}
