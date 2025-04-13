using Microsoft.AspNetCore.Mvc.Rendering;
using PMSCore.ViewModel;

namespace PMSWebApp.Extensions;

public static class EnumExtensions
{
    /* 
        * This method converts an enum type to a list of SelectListItem objects,
        * which can be used to populate dropdown lists in ASP.NET MVC views.
    */
    public static List<SelectListItem> ToSelectList<TEnum>(this TEnum enumType) where TEnum : Enum
    {
        /*
            * Get all the values of the enum type and cast them to TEnum.
        */
        IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        /*
            * Convert each enum value to a SelectListItem object.
            * The Value property is the string representation of the enum value.
            * The Text property is the display name of the enum value, which is obtained using the GetDisplayName method.
        */
        return values.Select(e => new SelectListItem
        {
            Value = e.ToString(), 
            Text = GetDisplayName(e) 
        }).ToList(); 
    }

    private static string GetDisplayName<TEnum>(TEnum enumValue) where TEnum : Enum
    {
        switch (enumValue)
        {
            case orderStatus.All:
                return "All Status";
            case orderStatus.OnHold:
                return "On Hold";
            case orderStatus.Pending:
                return "Pending";
            case orderStatus.InProgress:
                return "In Progress";
            case orderStatus.Completed:
                return "Completed";
            case orderStatus.Served:
                return "Served";
            case orderStatus.Cancelled:
                return "Cancelled";
            case orderStatus.Failed:
                return "Failed";
            case timePeriod.All:
                return "All Time";
            case timePeriod.LastSevenDays:
                return "Last 7 Days";
            case timePeriod.LastThirtyDays:
                return "Last 30 Days";
            case timePeriod.CurrentMonth:
                return "Current Month";
            default:
                return Enum.GetName(typeof(TEnum), enumValue);
        }
    }
}
