using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;

namespace PMSWebApp.Controllers;


public class TaxesController : Controller
{
    private readonly ITaxesAndFeesService _taxesAndFeesService;

    public TaxesController(ITaxesAndFeesService taxesAndFeesService)
    {
        _taxesAndFeesService = taxesAndFeesService;
    }

    ResponseResult result = new ResponseResult();
    public async Task<IActionResult> Taxes(PaginationDetails pagination)
    {
        try
        {
            result = await _taxesAndFeesService.GetTaxes(pagination);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string partialView = await this.RenderPartialViewToString("_partial_taxList", (IEnumerable<TaxDetails>)result.Data);
                return Json(new
                {
                    message = result.Message,
                    status = result.Status,
                    partialView = partialView,
                    paginationDetails = pagination
                });
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        @TempData["LayoutName"] = "_Layout";
        return View(((IEnumerable<TaxDetails>)result.Data, pagination));
    }

    public async Task<IActionResult> AddNewTax(TaxDetails taxDetails)
    {
        try
        {
            result = await _taxesAndFeesService.AddNewTax(taxDetails);
            string partialView = await this.RenderPartialViewToString("_partial_taxList", (IEnumerable<TaxDetails>)result.Data);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status.ToString() });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status.ToString() });
        }
    }
    public async Task<IActionResult> UpdateTax(TaxDetails taxDetails)
    {
        try
        {
            result = await _taxesAndFeesService.UpdateTax(taxDetails);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return Json(new { message = result.Message, status = result.Status.ToString() });

    }
    public async Task<IActionResult> DeleteTax(int taxId,int editorId)
    {
        try
        {
            result = await _taxesAndFeesService.DeleteTax(taxId,editorId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction("Taxes", "Taxes");
    }
}
