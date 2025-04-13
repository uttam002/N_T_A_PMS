using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using SelectPdf;
using PMSData.Interfaces;
using PMSData.Reposetories;
using PMSServices.Interfaces;

namespace PMSServices.Services;

public class OrdersService : IOrdersService
{
    private readonly IOrderRepo _orderRepo;
    private readonly IInvoiceItemMappingRepo _invoiceItemMapping;
    private readonly IInvoiceRepo _invoiceRepo;
    private readonly IInvoiceTaxesMappingRepo _invoiceTaxesMapping;
    private readonly ITableRepo _tableRepo;
    private readonly ISectionRepo _sectionRepo;
    private readonly ITaxesRepo _taxRepo;
    private readonly IItemRepo _itemRepo;
    private readonly IModifierRepo _modifierRepo;

    public OrdersService(IOrderRepo orderRepo, ITableRepo tableRepo, ISectionRepo sectionRepo, ITaxesRepo taxRepo, IItemRepo itemRepo, IModifierRepo modifierRepo, IInvoiceItemMappingRepo invoiceItemMapping, IInvoiceRepo invoiceRepo, IInvoiceTaxesMappingRepo invoiceTaxesMapping)
    {
        _orderRepo = orderRepo;
        _invoiceRepo = invoiceRepo;
        _tableRepo = tableRepo;
        _sectionRepo = sectionRepo;
        _taxRepo = taxRepo;
        _itemRepo = itemRepo;
        _modifierRepo = modifierRepo;
        _invoiceItemMapping = invoiceItemMapping;
        _invoiceTaxesMapping = invoiceTaxesMapping;
    }

    ResponseResult result = new ResponseResult();
    public async Task<ResponseResult> GetOrderList(PaginationDetails paginationDetails)
    {
        try
        {
            result.Data = await _orderRepo.GetOrderListAsync(paginationDetails);

            if (result.Data == null)
            {
                result.Message = "No Records Found in OrderSection!!!";
                result.Status = ResponseStatus.NotFound;
            }
            else
            {
                result.Message = "Records Found Successfully in OrderSection!!!";
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

    public async Task<(byte[], string)> CreatePdfAsync(int orderId,string partialView)
    {
        try
        {
            // Convert the HTML string to a PDF
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument pdf = converter.ConvertHtmlString(partialView);

            using (var stream = new MemoryStream())
            {
                pdf.Save(stream);
                pdf.Close();

                // Generate a file name for the PDF
                string fileName = $"Invoice_{orderId}.pdf";

                // Return the PDF as a byte array and the file name
                return (stream.ToArray(), fileName);
            }
        }
        catch
        {
            throw new Exception("Failed to generate PDF.");
        }
    }
    public async Task<ResponseResult> ExportOrderList(string orderSearch, string status, string dateRange)
    {
        try
        {
            PaginationDetails paginationDetails = new()
            {
                PageSize = 0,
                SearchQuery = orderSearch,
                OrderStatus = Enum.Parse<orderStatus>(status),
                DateRange = Enum.Parse<timePeriod>(dateRange)
            };
            List<OrderDetailsVM> orders = await _orderRepo.GetOrderListAsync(paginationDetails);
            result.Data = await GenerateExcelFile(orderSearch, status, dateRange, paginationDetails.totalRecords, orders);
            if (result.Data == null)
            {
                result.Message = "Data Can Not Export in File!!!";
                result.Status = ResponseStatus.NotFound;
            }
            else
            {
                result.Message = "File Successfully Created!!!";
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

    private async Task<byte[]> GenerateExcelFile(string orderSearch, string status, string dateRange, int noOfRecords, List<OrderDetailsVM> orders)
    {
        try
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "Order_Sales_Data_Format.xlsx");
            FileInfo fileInfo = new FileInfo(templatePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Template Not Found");
            }
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                worksheet.Cells["C2:F3"].Value = status;
                worksheet.Cells["J2:M3"].Value = orderSearch;
                worksheet.Cells["C5:F6"].Value = dateRange;
                worksheet.Cells["J5:M6"].Value = noOfRecords;
                worksheet.Cells[1, 1, 8, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1, 8, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int startRow = 9;

                foreach (OrderDetailsVM order in orders)
                {
                    worksheet.Cells[startRow, 1].Value = order.OrderId;
                    worksheet.Cells[startRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 2, startRow, 4].Merge = true;
                    worksheet.Cells[startRow, 2, startRow, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 2].Value = order.OrderDate.ToString();
                    worksheet.Cells[startRow, 5, startRow, 7].Merge = true;
                    worksheet.Cells[startRow, 5, startRow, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 5].Value = order.CustomerName;
                    worksheet.Cells[startRow, 8, startRow, 10].Merge = true;
                    worksheet.Cells[startRow, 8, startRow, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 8].Value = order.OrderStatus;
                    worksheet.Cells[startRow, 11, startRow, 12].Merge = true;
                    worksheet.Cells[startRow, 11, startRow, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 11].Value = order.PaymentType;
                    worksheet.Cells[startRow, 13, startRow, 14].Merge = true;
                    worksheet.Cells[startRow, 13, startRow, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 13].Value = order.Rating;
                    worksheet.Cells[startRow, 15, startRow, 16].Merge = true;
                    worksheet.Cells[startRow, 15, startRow, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 15].Value = order.TotalAmount;

                    worksheet.Cells[startRow, 1, startRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, 1, startRow, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    startRow++;
                }
                return package.GetAsByteArray();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<ResponseResult> GetOrderDetailsAsync(int orderId)
    {
        try
        {
            // result = await _orderRepo.GetOrderDetailsAsync(orderId);
            OrderDetail orderDetails = await _orderRepo.GetOrderDetailsByOrderIdAsync(orderId);
            List<InvoiceItemModifierMapping> listofItems = await _invoiceItemMapping.GetItemsForInvoiceAsync(orderId);
            Invoice invoiceDetails = await _invoiceRepo.GetInvoiceNumberAsync(orderId);
            List<InvoiceTaxesMapping> listOfTaxes = await _invoiceTaxesMapping.GetTaxesListForInvoiceAsync(invoiceDetails.InvoiceId);

            OrderExportDetails invoice = await CreateInvoice(orderDetails, listofItems, invoiceDetails, listOfTaxes);

            result.Data = invoice;
            if (result.Data == null)
            {
                result.Message = "No Records Found for this Order!!!";
                result.Status = ResponseStatus.NotFound;
            }
            else
            {
                result.Message = "Records Found Successfully for this Order!!!";
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


    //Helper Method s
    private async Task<OrderExportDetails> CreateInvoice(OrderDetail orderDetails, List<InvoiceItemModifierMapping> listofItems, Invoice invoiceDetails, List<InvoiceTaxesMapping> listOfTaxes)
    {
        OrderExportDetails invoice = new OrderExportDetails();

        invoice.OrderId = orderDetails.OrderId;
        invoice.InvoiceNo = invoiceDetails.InvoiceNumber;
        invoice.Status = orderDetails.Order.Status;
        invoice.PaidOn = orderDetails.Payment.Createat;
        invoice.PlacedOn = orderDetails.Createdat;
        invoice.ModifiedOn = orderDetails.Modifiedat ?? orderDetails.Createdat;
        invoice.OrderDuration = TimeOnly.FromTimeSpan(invoice.ModifiedOn - invoice.PlacedOn);
        (invoice.Tables, invoice.Section) = await GetTableBasedOrdersDetails(orderDetails.TableId);
        invoice.PaymentMethod = orderDetails.Payment.PaymentMethod;
        invoice.taxDetails = await GetTaxDetails(listOfTaxes, listofItems);
        invoice.OrderItems = await GetOrderItems(listofItems);
        invoice.SubTotal = orderDetails.Payment.ActualPrice;
        invoice.TotalAmountToPay = await CalculateTotalAmount(invoice.taxDetails, invoice.SubTotal);
        invoice.CustomerInfo = await GetCustomerInfo(orderDetails.Order.Customer,orderDetails.Order.NuOfPersons);

        return invoice;
    }

    private static async Task<CustomerDetails> GetCustomerInfo(Customer customer,int noOfPersons)
    {
        CustomerDetails customerDetails = new CustomerDetails();
        customerDetails.CustomerName = customer.CustName;
        customerDetails.NoOfPerson = noOfPersons;
        customerDetails.CustomerPhone = customer.PhoneNumber;
        customerDetails.CustomerEmail = customer.EmailId??"_";

        return customerDetails;
    }


    private async Task<decimal> CalculateTotalAmount(List<OrderExportDetails.TaxDetailsHelperModel> taxDetails, decimal SubTotal)
    {
        decimal totalTax = taxDetails.Sum(t => t.TaxValue);
        return totalTax+SubTotal;
    }


    private async Task<List<OrderExportDetails.OrderItemHelperModel>> GetOrderItems(List<InvoiceItemModifierMapping> listofItems)
    {
        List<OrderExportDetails.OrderItemHelperModel> orderItems = new List<OrderExportDetails.OrderItemHelperModel>();

        IEnumerable<IGrouping<int, InvoiceItemModifierMapping>> groupedItems = listofItems.GroupBy(i => i.ItemId); // Grouping by item_id

        foreach (IGrouping<int, InvoiceItemModifierMapping> group in groupedItems)
        {
            InvoiceItemModifierMapping firstItem = group.First(); // Get first entry to retrieve item details

            OrderExportDetails.OrderItemHelperModel orderItem = new OrderExportDetails.OrderItemHelperModel
            {
                ItemName = await _itemRepo.GetItemNameByIdAsync(firstItem.ItemId), // Fetch item name from DB 
                Quantity = firstItem.ItemQuantity,
                UnitPrice = firstItem.ItemPrice,
                TotalPrice = firstItem.ItemQuantity * firstItem.ItemPrice,
                Modifiers = new List<OrderExportDetails.OrderItemHelperModel.OrderModiferHelperModel>()
            };

            foreach (InvoiceItemModifierMapping modifier in group)
            {
                OrderExportDetails.OrderItemHelperModel.OrderModiferHelperModel modifierModel = new OrderExportDetails.OrderItemHelperModel.OrderModiferHelperModel
                {
                    ModifierName = await _modifierRepo.GetModifierNameByIdAsync(modifier.ModifierId), // Fetch modifier name
                    ModiferQuantity = modifier.ModifiersQuantity,
                    ModifierPrice = modifier.ModifierPrice,
                    ModifierTotalPrice = modifier.ModifierPrice * modifier.ModifiersQuantity
                };

                orderItem.Modifiers.Add(modifierModel);
            }

            orderItems.Add(orderItem);
        }

        return orderItems;
    }



    private async Task<List<OrderExportDetails.TaxDetailsHelperModel>> GetTaxDetails(List<InvoiceTaxesMapping> listOfTaxes, List<InvoiceItemModifierMapping> listofItems)
    {
        List<OrderExportDetails.TaxDetailsHelperModel> taxDetails = new List<OrderExportDetails.TaxDetailsHelperModel>();
        foreach (InvoiceTaxesMapping taxmapping in listOfTaxes)
        {
            OrderExportDetails.TaxDetailsHelperModel taxDetail = new OrderExportDetails.TaxDetailsHelperModel();
            Taxis taxis = await _taxRepo.GetTaxAsync(taxmapping.TaxId);
            taxDetail.TaxName = taxis.TaxName;
            taxDetail.TaxValue = taxmapping.InvoiceTaxValue;
            taxDetails.Add(taxDetail);
        }
        OrderExportDetails.TaxDetailsHelperModel taxDetailFormPerItems = await CalculateTaxForItems(listofItems);
        taxDetails.Add(taxDetailFormPerItems);
        return taxDetails;
    }

    private async Task<OrderExportDetails.TaxDetailsHelperModel> CalculateTaxForItems(List<InvoiceItemModifierMapping> listofItems)
    {
        if (listofItems == null || !listofItems.Any())
        {
            return new OrderExportDetails.TaxDetailsHelperModel
            {
                TaxName = "Other",
                TaxValue = 0m
            };
        }

        decimal totalTax = listofItems.Sum(item =>
        {
            decimal itemTotalPrice = item.ItemPrice * item.ItemQuantity;
            decimal taxAmount = item.ItemTaxPercentage.HasValue
                ? (itemTotalPrice * item.ItemTaxPercentage.Value / 100)
                : 0m;
            return taxAmount;
        });

        return new OrderExportDetails.TaxDetailsHelperModel
        {
            TaxName = "Other",
            TaxValue = totalTax
        };
    }


    public async Task<(string, string)> GetTableBasedOrdersDetails(int[] tableIds)
    {
        string TablesName = "";
        int sectionId = 0;
        foreach (int tableId in tableIds)
        {
            Table t = await _tableRepo.GetTableAsync(tableId);
            TablesName += t.TableName + " , ";
            sectionId = t.SectionId;
        }
        TablesName = TablesName.TrimEnd(new char[] { ' ', ',' });
        Section sectionDetails = await _sectionRepo.GetSectionAsync(sectionId);
        return (TablesName, sectionDetails.SectionName);
    }

    

}
