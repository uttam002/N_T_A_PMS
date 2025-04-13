using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;

namespace PMSWebApp.Controllers;

public class CustomersController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    ResponseResult result = new ResponseResult();
    public async Task<IActionResult> Customer(PaginationDetails paginationDetails){
        try{
            result = await _customerService.GetCustomerList(paginationDetails);
        }catch(Exception ex){
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        IEnumerable<CustomerDetails> cutomerList = (IEnumerable<CustomerDetails>)result.Data;
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            string partialView = await this.RenderPartialViewToString("_partial_CustomerTableGrid",cutomerList);
            return Json(new
            {
                partialView = partialView,
                message = result.Message,
                status = result.Status,
                paginationDetails = paginationDetails,
            });
        }
        @TempData["LayoutName"] = "_Layout";
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return View((cutomerList, paginationDetails));
        
    }
    public async Task<IActionResult> ExportData(PaginationDetails paginationDetails)
    {
        try
        {
            result  = await _customerService.ExportCustomerDataAsync(paginationDetails);
            
            (byte[] fileContent, string contentType, string fileName) = ((byte[], string, string))result.Data;
            TempData["ToastMessage"] = result.Message;
            TempData["ToastStatus"] = result.Status.ToString();
            return File(fileContent, contentType, fileName);
        }
        catch (Exception ex)
        {
            TempData["ToastMessage"] = ex.Message;
            TempData["ToastStatus"] = "Error";
            return RedirectToAction("Customer", "Customers");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerHistory(int customerId)
    {
        try
        {
            result = await _customerService.GetCustomerHistoryAsync(customerId);
            CustomerHistory customerHistory = result.Data as CustomerHistory?? new CustomerHistory();
            return PartialView("_partial_Customer_HistoryGrid",customerHistory);
        }
        catch (Exception ex)
        {
            TempData["ToastMessage"] = ex.Message;
            TempData["ToastStatus"] = ResponseStatus.Error.ToString();
            return RedirectToAction("Customer", "Customers");
        }
    }
}
