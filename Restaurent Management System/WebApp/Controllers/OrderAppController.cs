using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;
using PMSWebApp.Extensions;

namespace PMSWebApp.Controllers;

public class OrderAppController : Controller
{
    private readonly IOrderAppService _orderAppService;

    public OrderAppController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }
    ResponseResult result = new ResponseResult();
    [HttpGet]
    public async Task<IActionResult> TableView()
    {
        // var orders = await _orderAppService.GetAllPendingOrdersAsync();
        TempData["LayoutName"] = "_OrderAppLayout";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> KOT(string status = "InProgress", int categoryId = 0)
    {
        try
        {
            result = await _orderAppService.GetKOTs(status, categoryId);
            (List<KOTVM>, List<CategoryDetails>) data = ((List<KOTVM>, List<CategoryDetails>))result.Data;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_partial_KOTsGrid", data.Item1);
            }
            TempData["LayoutName"] = "_OrderAppLayout";
            return View(data);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["LayoutName"] = "_OrderAppLayout";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateKOT(List<KOTVM.KOTItemsVM> kotItems, int OrderId)
    {
        try
        {
            if (kotItems == null || kotItems.Count == 0)
            {
                return Json(new{status = "NotFound",message = "No items to update."});
            }

            result = await _orderAppService.UpdateKOT(kotItems, OrderId);
            if (result.Status == ResponseStatus.Success)
            {
                List<KOTVM> kotList = result.Data as List<KOTVM> ?? new List<KOTVM>();
                string partialView = await this.RenderPartialViewToString("_partial_KOTsGrid",kotList);
                return Json(new {partialView = partialView ,status = "Success",message = result.Message});
            }
            else
            {
                return Json(new{status = "Error",message = result.Message});
            }
        }
        catch (Exception ex)
        {
            return Json(new
            {
                status = "Error",
                message = ex.Message
            });

        }

    }
    // [HttpPost]
    // public async Task<IActionResult> Create(OrderDto orderDto)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }

    //     await _orderAppService.CreateOrderAsync(orderDto);
    //     return RedirectToAction("Index");
    // }

}
