using Microsoft.EntityFrameworkCore;

namespace PMSCore.ViewModel;

public class AddModifier
{
    public int modifierId = 0;

    public int ModifierGroupId { get; set; }
    public string modifierName { get; set; } = null!;
    public string? Description { get; set; }
    [Precision(7, 2)]
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string UnitType { get; set; } = null!;
    public int EditorId { get; set;} = 0;
}
