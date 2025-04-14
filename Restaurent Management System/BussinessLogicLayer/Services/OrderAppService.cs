using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class OrderAppService : IOrderAppService
{
    private readonly IOrderRepo _orderRepo;
    private readonly IOrdersService _ordersService;
    private readonly IInvoiceItemMappingRepo _invoiceItemMapping;
    private readonly IInvoiceRepo _invoiceRepo;
    private readonly IInvoiceTaxesMappingRepo _invoiceTaxesMapping;
    private readonly ITableRepo _tableRepo;
    private readonly ISectionRepo _sectionRepo;
    private readonly ITaxesRepo _taxRepo;
    private readonly IItemRepo _itemRepo;
    private readonly ICategoryRepo _categoryRepo;
    private readonly IModifierRepo _modifierRepo;


    public OrderAppService(IOrderRepo orderRepo, IOrdersService ordersService, ITableRepo tableRepo, ICategoryRepo categoryRepo, ISectionRepo sectionRepo, ITaxesRepo taxRepo, IItemRepo itemRepo, IModifierRepo modifierRepo, IInvoiceItemMappingRepo invoiceItemMapping, IInvoiceRepo invoiceRepo, IInvoiceTaxesMappingRepo invoiceTaxesMapping)
    {
        _orderRepo = orderRepo;
        _ordersService = ordersService;
        _invoiceRepo = invoiceRepo;
        _tableRepo = tableRepo;
        _sectionRepo = sectionRepo;
        _taxRepo = taxRepo;
        _itemRepo = itemRepo;
        _categoryRepo = categoryRepo;
        _modifierRepo = modifierRepo;
        _invoiceItemMapping = invoiceItemMapping;
        _invoiceTaxesMapping = invoiceTaxesMapping;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> GetKOTs(string status, int categoryId)
    {
        try
        {
            List<KOTVM> listOfKOTs = new();

            List<Order> filteredOrders = await _orderRepo.GetOrdersByCategoryId(status, categoryId);
            listOfKOTs = await FitKOTDataInKOTVM(status, filteredOrders);

            if (listOfKOTs == null)
            {
                result.Message = "List Of KOTs are Not Fetched!!!";
                result.Status = ResponseStatus.NotFound;
            }
            else
            {
                List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                List<CategoryDetails> categoryDetails = new List<CategoryDetails>();
                foreach (Category category in categories)
                {
                    CategoryDetails categoryDetail = new CategoryDetails();
                    categoryDetail.id = category.CategoryId;
                    categoryDetail.categoryName = category.CategoryName;
                    categoryDetail.description = category.Description;
                    categoryDetails.Add(categoryDetail);
                }
                result.Message = "List of KOTs are successfully Fetched!!!";
                result.Data = (listOfKOTs, categoryDetails);
                result.Status = ResponseStatus.Success;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
    private async Task<List<KOTVM>> FitKOTDataInKOTVM(string status, List<Order> orders)
    {
        try
        {
            List<KOTVM> listOfKOTs = new List<KOTVM>();
            foreach (Order order in orders)
            {
                KOTVM kot = new KOTVM();
                kot.OrderId = order.OrderId;
                (kot.tableName, kot.sectionName) = await _ordersService.GetTableBasedOrdersDetails(order.OrderDetails.FirstOrDefault().TableId);
                kot.orderStatus = order.Status;
                kot.orderAt = order.Createat;
                kot.kotItems = await GetMappedItems(status, order.InvoiceItemModifierMappings);
                kot.orderComments = await CreateOneGeneraleComments(kot.kotItems);
                listOfKOTs.Add(kot);
            }
            return listOfKOTs;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return null;
        }
    }
    private async Task<string> CreateOneGeneraleComments(List<KOTVM.KOTItemsVM> kotItems)
    {
        try
        {
            string orderComment = "";
            foreach (KOTVM.KOTItemsVM kotItem in kotItems)
            {
                orderComment += kotItem.itemComments + " in " + kotItem.itemName + ", ";
            }
            orderComment = orderComment.TrimEnd(new char[] { ' ', ',' });
            return orderComment;
        }
        catch
        {
            return string.Empty;
        }
    }
    private async Task<List<KOTVM.KOTItemsVM>> GetMappedItems(string status, IEnumerable<InvoiceItemModifierMapping> invoiceItemModifierMappings)
    {
        try
        {
            List<KOTVM.KOTItemsVM> listOfKOTItems = new List<KOTVM.KOTItemsVM>();

            IEnumerable<IGrouping<int, InvoiceItemModifierMapping>> groupedItems = invoiceItemModifierMappings.GroupBy(i => i.ItemId);

            foreach (IGrouping<int, InvoiceItemModifierMapping> group in groupedItems)
            {
                InvoiceItemModifierMapping firstItem = group.First(); // Get first entry to retrieve item details
                KOTVM.KOTItemsVM orderedItem = new KOTVM.KOTItemsVM();
                orderedItem.categoryId = firstItem.Item.CategoryId;
                orderedItem.categoryName = firstItem.Item.Category.CategoryName;
                orderedItem.itemId = firstItem.ItemId;
                orderedItem.itemName = firstItem.Item.ItemName;
                orderedItem.quantity = firstItem.ItemQuantity;
                orderedItem.preparedItems = firstItem.PreparedItems;
                orderedItem.itemComments = firstItem.ExtraComments ?? "";
                if (status == "Ready")
                {
                    orderedItem.isPrepared = true;
                }
                foreach (InvoiceItemModifierMapping mappedRow in group)
                {
                    KOTVM.KOTItemsVM.KOTModifiersVM newModifier = new()
                    {
                        modifierId = mappedRow.Modifier.MId,
                        modifierName = mappedRow.Modifier.MName,
                    };
                    orderedItem.modifiers.Add(newModifier);
                }
                listOfKOTItems.Add(orderedItem);
            }
            return listOfKOTItems;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return null;
        }
    }

    public async Task<ResponseResult> UpdateKOT(List<KOTVM.KOTItemsVM> kotItems, int orderId)
    {
        try
        {
            if (kotItems == null || kotItems.Count == 0)
            {
                result.Message = "No items to update.";
                result.Status = ResponseStatus.NotFound;
                return result;
            }
            List<InvoiceItemModifierMapping> itemMapping = await _invoiceItemMapping.GetItemsForKOTAsync(orderId);
            if (itemMapping == null)
            {
                result.Message = "Order not found.";
                result.Status = ResponseStatus.NotFound;
                return result;
            }
            foreach (KOTVM.KOTItemsVM item in kotItems)
            {
                InvoiceItemModifierMapping invoiceItemModifierMapping = itemMapping.FirstOrDefault(i => i.ItemId == item.itemId);
                invoiceItemModifierMapping.PreparedItems = item.preparedItems;
                invoiceItemModifierMapping.ExtraComments = item.itemComments;
                result = await _invoiceItemMapping.UpdateItemMappingAsync(invoiceItemModifierMapping);
                if (result.Status == ResponseStatus.Error)
                {
                    result.Message = "Error updating item mapping.";
                    return result;
                }
            }
            result.Message = "KOT updated successfully.";
            result.Status = ResponseStatus.Success;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return result;
    }
}
