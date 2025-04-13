using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;

namespace PMSWebApp.Controllers;

public class SectionAndTablesController : Controller
{
    private readonly ISectionAndTablesService _sectionAndTablesService;
    public SectionAndTablesController(ISectionAndTablesService sectionAndTablesService)
    {
        _sectionAndTablesService = sectionAndTablesService;
    }
    ResponseResult result = new ResponseResult();
    public async Task<IActionResult> SectionAndTables()
    {
        try
        {
            PaginationDetails paginationDetails = new PaginationDetails();
            paginationDetails.PageSize = 2;
            result = await _sectionAndTablesService.GetDefaultAreaDeatils(paginationDetails);
            ViewBag.paginationDetails = paginationDetails;

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        AreaDetails areaDetails = (AreaDetails)result.Data;
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        @TempData["LayoutName"] = "_Layout";
        return View(areaDetails);
    }
    [HttpPost]
    public async Task<IActionResult> GetTables(int sectionId,PaginationDetails paginationDetails)
    {
        try
        {
            result = await _sectionAndTablesService.GetTables(sectionId,paginationDetails);
            ViewBag.paginationDetails = paginationDetails;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        List<TableDetails> tableList = result.Data as List<TableDetails> ?? new List<TableDetails>();
        return PartialView("_partial_TablesListGrid", tableList);
    }

    public async Task<IActionResult> AddSection([FromForm]string SectionName, [FromForm]string Description)
    {
        try
        {
            result = await _sectionAndTablesService.AddSection(SectionName, Description);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction("SectionAndTables", "SectionAndTables");
    }

    public async Task<IActionResult> DeleteSection(int sectionId,int editorId)
    {
        try
        {
            result = await _sectionAndTablesService.DeleteSection(sectionId,editorId);

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction("SectionAndTables", "SectionAndTables");
    }
    [HttpPost]
    public async Task<IActionResult> EditSection(int sectionId, string SectionName, string Description, int editorId)
    {

        try
        {
            result = await _sectionAndTablesService.EditSection(sectionId, SectionName, Description, editorId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
                                                            // return RedirectToAction("GetTables", "SectionAndTables", new{id = sectionId});
        return RedirectToAction("SectionAndTables", "SectionAndTables");
    }
    [HttpPost]
    public async Task<IActionResult> AddNewTable(TableDetails newTable)
    {
        try
        {
            if (ModelState.IsValid)
            {
                result = await _sectionAndTablesService.AddTable(newTable);
            }
            else
            {
                result.Message = "Model State is not valid!!!";
                result.Status = ResponseStatus.Error;
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString();
        // return Json(new { status = result.Status, message = result.Message });
        return RedirectToAction("SectionAndTables","SectionAndTables");
    }
    public async Task<IActionResult> DeleteTableById(int tableId){
        try{
            result = await _sectionAndTablesService.DeleteTable(tableId);
        }catch(Exception ex){
            result.Message=ex.Message;
            result.Status=ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction("SectionAndTables", "SectionAndTables");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTable(TableDetails updateTable){
        try{
            
                result = await _sectionAndTablesService.UpdateTable(updateTable);
            
        }catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString();
        return RedirectToAction("SectionAndTables","SectionAndTables");
    }




}

