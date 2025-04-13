using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSServices.Interfaces;

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
    public async Task<IActionResult> KOT(string status="InProgress",int categoryId=0)
    {
        try
        {
            result = await _orderAppService.GetKOTs(status,categoryId);
            (List<KOTVM> , List<CategoryDetails> ) data = ((List<KOTVM> , List<CategoryDetails> ))result.Data;
            if(Request.Headers["X-Requested-With"] == "XMLHttpRequest"){
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
