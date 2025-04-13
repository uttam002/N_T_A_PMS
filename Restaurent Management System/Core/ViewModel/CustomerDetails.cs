namespace PMSCore.ViewModel;

public class CustomerDetails
{
    public int CustomerId { get; set; } = 0;
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public int NoOfPerson { get; set; }
    public string CustomerEmail { get; set; }
    public int TotalOrders { get; set; } =0;
    public DateOnly LastOrder {get;set;} = new DateOnly();
}
