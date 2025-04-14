using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;

namespace PMSWebApp.Controllers;

public class SectionAndTablesController : Controller
{
    private readonly ISectionAndTablesService _sectionAndTablesService;
    public SectionAndTablesController(ISectionAndTablesService sectionAndTablesService)
    {
        _sectionAndTablesService = sectionAndTablesService;
    }
    ResponseResult result = new ResponseResult();



    public async Task<IActionResult> SectionAndTables(PaginationDetails paginationDetails)
    {
        try
        {
            paginationDetails.PageSize = 2;
            result = await _sectionAndTablesService.GetDefaultAreaDeatils(paginationDetails);
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


    #region Section

    [HttpPost]
    public async Task<IActionResult> AddSection(SectionDetails section)
    {
        try
        {
            result = await _sectionAndTablesService.AddSection(section);
            List<SectionDetails> sectionList = result.Data as List<SectionDetails> ?? new List<SectionDetails>();
            string partialView = await this.RenderPartialViewToString("_partial_SectionListGrid", sectionList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSection(SectionDetails section)
    {
        try
        {
            result = await _sectionAndTablesService.DeleteSection(section.SectionId, section.editorId);
            List<SectionDetails> sectionList = result.Data as List<SectionDetails> ?? new List<SectionDetails>();
            string partialView = await this.RenderPartialViewToString("_partial_SectionListGrid", sectionList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status });

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }

    }
    [HttpPost]
    public async Task<IActionResult> EditSection(SectionDetails section)
    {

        try
        {
            result = await _sectionAndTablesService.EditSection(section);
            List<SectionDetails> sectionList = result.Data as List<SectionDetails> ?? new List<SectionDetails>();
            string partialView = await this.RenderPartialViewToString("_partial_SectionListGrid", sectionList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }
    }

    #endregion

    #region Tables
    [HttpPost]
    public async Task<IActionResult> GetTables(int sectionId, PaginationDetails paginationDetails)
    {
        try
        {
            result = await _sectionAndTablesService.GetTables(sectionId, paginationDetails);
            List<TableDetails> tableList = result.Data as List<TableDetails> ?? new List<TableDetails>();
            string partialView = await this.RenderPartialViewToString("_partial_TablesListGrid", tableList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = paginationDetails });

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddNewTable(TableDetails newTable)
    {
        try
        {

            result = await _sectionAndTablesService.AddTable(newTable);
            (List<SectionDetails> sectionList, PaginationDetails pagination) = ((List<SectionDetails>, PaginationDetails))result.Data;
            string partialView = await this.RenderPartialViewToString("_partial_SectionListGrid", sectionList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteTableById(int tableId, int editorId)
    {
        try
        {
            result = await _sectionAndTablesService.DeleteTable(tableId, editorId);
            (List<SectionDetails> sectionList, PaginationDetails pagination) = ((List<SectionDetails>, PaginationDetails))result.Data;
            string partialView = await this.RenderPartialViewToString("_partial_SectionListGrid", sectionList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTable(TableDetails updateTable)
    {
        try
        {
            result = await _sectionAndTablesService.UpdateTable(updateTable);
            (List<SectionDetails> sectionList, PaginationDetails pagination) = ((List<SectionDetails>, PaginationDetails))result.Data;
            string partialView = await this.RenderPartialViewToString("_partial_SectionListGrid", sectionList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }
    }

    [HttpPost]
    public async Task<IActionResult> MassDeleteTable(int[] tableIds, int editorId)
    {
        try
        {
            result = await _sectionAndTablesService.MassDeleteTableAsync(tableIds, editorId);
            (List<SectionDetails> sectionList, PaginationDetails pagination) = ((List<SectionDetails>, PaginationDetails))result.Data;
            string partialView = await this.RenderPartialViewToString("_partial_SectionListGrid", sectionList);
            return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }

    }
    
    #endregion

}

