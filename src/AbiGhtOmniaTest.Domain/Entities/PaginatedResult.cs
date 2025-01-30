namespace AbiGhtOmniaTest.Domain.Entities;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
}
