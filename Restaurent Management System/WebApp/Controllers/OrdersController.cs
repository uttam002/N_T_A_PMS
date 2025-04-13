using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;

namespace PMSWebApp.Controllers;

public class OrdersController : Controller
{
    private readonly IOrdersService _ordersService;
    public OrdersController(IOrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    ResponseResult result = new ResponseResult();
    public async Task<IActionResult> Order(PaginationDetails paginationDetails)
    {

        try
        {
            result = await _ordersService.GetOrderList(paginationDetails);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        IEnumerable<OrderDetailsVM> orderList = (IEnumerable<OrderDetailsVM>)result.Data;
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            string partialView = await this.RenderPartialViewToString("_partial_OrderTable", orderList);
            return Json(new
            {
                message = result.Message,
                status = result.Status,
                partialView = partialView,
                paginationDetails = paginationDetails
            });
        }
        @TempData["LayoutName"] = "_Layout";
        return View((orderList, paginationDetails));
    }

    // [HttpPost]
    // public async Task<IActionResult> ShowOrderList(List<OrderDetailsVM> orderList)
    // {
    //     return PartialView("_partial_OrderTable", orderList);
    // }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int orderId)
    {
        try
        {
            result = await _ordersService.GetOrderDetailsAsync(orderId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString();
        TempData["LayoutName"] = "_Layout";
        return View(result.Data as OrderExportDetails);
    }
    [HttpGet]
    public async Task<IActionResult> ExportData(string orderSearch, string OrderStatus, string dateRange)
    {
        try
        {
            result = await _ordersService.ExportOrderList(orderSearch, OrderStatus, dateRange);
            byte[] fileContent = result.Data as byte[];
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetsml.sheet";
            string currentDate = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            string fileName = $"OrderSalesData_{currentDate}.xlsx";

            // Return the file as a response
            TempData["ToastMessage"] = result.Message;
            TempData["ToastStatus"] = result.Status.ToString();
            return File(fileContent, contentType, fileName);
        }
        catch (Exception ex)
        {
            TempData["ToastMessage"] = ex.Message;
            TempData["ToastStatus"] = "Error";
            return RedirectToAction("Order", "Orders");
        }
    }

    public async Task<IActionResult> DownloadInvoice(int orderId){
        try{
            result = await _ordersService.GetOrderDetailsAsync(orderId);
            OrderExportDetails invoice = result.Data as OrderExportDetails;

            
            string partialView  = await this.RenderPartialViewToString("Invoice", invoice);
            (byte[] pdfArray, string fileName) = await _ordersService.CreatePdfAsync(orderId,partialView);


            return File(pdfArray, "application/pdf", fileName);
        }catch(Exception ex){
            TempData["ToastMessage"] = ex.Message;
            TempData["ToastStatus"] = "Error";
            return RedirectToAction("Order","Orders");
        }
    }

}
