using System;

namespace ECommerce.Business.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;

    public PagedResultDto(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static PagedResultDto<T> Create(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        return new PagedResultDto<T>(data, pageNumber, pageSize, totalCount);
    }
}
