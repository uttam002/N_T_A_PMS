using System.Diagnostics.CodeAnalysis;

namespace PMSCore.ViewModel;

public class PaginationDetails
{
    public int PageSize { get; set; } =5;
    public int PageNumber { get; set; }=1;
    public string SortColumn { get; set; }= "";
    public string SortOrder { get; set; }="asc";
    public string SearchQuery { get; set; } ="";
    public int totalRecords { get; set; } = 0;

    public DateOnly FromDate { get; set; } = DateOnly.MinValue;
    public DateOnly ToDate { get; set;} = DateOnly.MaxValue;

    public orderStatus OrderStatus{ get; set; } = orderStatus.All;
    public timePeriod DateRange{ get; set; } = timePeriod.All;
}

public enum orderStatus{
    All,
    Pending,
    InProgress,
    Served,
    Completed,
    Cancelled,
    OnHold,
    Failed
}

public enum timePeriod{
    All,
    LastSevenDays, 
    LastThirtyDays,
    CurrentMonth,
    CustomRange
}
