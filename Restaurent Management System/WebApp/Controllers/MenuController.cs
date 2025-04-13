using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;

namespace PMSWebApp.Controllers;

public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    ResponseResult result = new ResponseResult();


    public async Task<IActionResult> Menu(PaginationDetails paginationDetails)
    {

        try
        {
            result = await _menuService.GetDefaultMenu(paginationDetails);
            ViewBag.paginationDetails = paginationDetails;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        MenuDetails menu = (MenuDetails)result.Data;
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        @TempData["LayoutName"] = "_Layout";
        return View(menu);
    }

    #region Category
    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryDetails newCategory)
    {
        try
        {
            result = await _menuService.AddCategory(newCategory);
            string partialView = await this.RenderPartialViewToString("_partial_CategoryListGrid", result.Data as List<CategoryDetails> ?? new List<CategoryDetails>());
            return Json(new { partialView = partialView, message = result.Message, status = result.Status });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }

    }

    //  [AuthorizePermission("Menu","can_createandedit")]
    [HttpPost]
    public async Task<IActionResult> EditCategory(CategoryDetails updateCategory)
    {
        try
        {
            result = await _menuService.EditCategory(updateCategory);
            string partialView = await this.RenderPartialViewToString("_partial_CategoryListGrid", result.Data as List<CategoryDetails> ?? new List<CategoryDetails>());
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
    public async Task<IActionResult> DeleteCategory(int categoryId, int editorId)
    {
        try
        {
            result = await _menuService.DeleteCategory(categoryId, editorId);
            string partialView = await this.RenderPartialViewToString("_partial_CategoryListGrid", result.Data as List<CategoryDetails> ?? new List<CategoryDetails>());
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


    #region Item

    [HttpPost]
    public async Task<IActionResult> GetItems(int id, PaginationDetails paginationDetails)
    {
        try
        {

            result = await _menuService.GetItems(id, paginationDetails);
            ViewBag.paginationDetails = paginationDetails;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        List<ItemDetails> itemList = (List<ItemDetails>)result.Data as List<ItemDetails> ?? new List<ItemDetails>();
        return PartialView("_partial_ItemListGrid", itemList);
        // string renderedView = await this.RenderPartialViewToString("_partial_ItemListGrid", itemList);
        // return Json(new { partialView = renderedView, paginationDetails = paginationDetails});
    }
    [HttpPost]
    public async Task<IActionResult> AddItem([FromForm] AddItem newItem)
    {
        try
        {
            if (!string.IsNullOrEmpty(Request.Form["newItem.IMDetails"]))
            {
                newItem.IMDetails = JsonConvert.DeserializeObject<List<ItemModifierGroupRelation>>(Request.Form["newItem.IMDetails"]);
            }
            result = await _menuService.AddItem(newItem);
            (List<ItemDetails> itemList, PaginationDetails paginationDetails) = ((List<ItemDetails>, PaginationDetails))result.Data;
            string partialView = await this.RenderPartialViewToString("_partial_ItemListGrid", itemList);
            return Json(new { partiview = partialView, message = result.Message, status = result.Status, paginaiton = paginationDetails });
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { message = result.Message, status = result.Status });
        }

    }
    [HttpPost]
    public async Task<IActionResult> UpdateItem([FromForm] AddItem editItem)
    {

        try
        {
            if (!string.IsNullOrEmpty(Request.Form["editItem.IMDetails"]))
            {
                editItem.IMDetails = JsonConvert.DeserializeObject<List<ItemModifierGroupRelation>>(Request.Form["editItem.IMDetails"]);
            }
            result = await _menuService.UpdateItem(editItem);
            List<ItemDetails> itemList = result.Data as List<ItemDetails> ?? new List<ItemDetails>();
            string partialView = await this.RenderPartialViewToString("_partial_ItemListGrid", itemList);
            return Json(new { data = partialView, message = result.Message, status = result.Status });

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
            return Json(new { status = result.Status, message = result.Message });
        }
    }
    [HttpGet]
    public async Task<IActionResult> DeleteItemById(int itemId, int editorId)
    {
        try
        {
            result = await _menuService.DeleteItem(itemId, editorId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return Json(new { message = result.Message, status = result.Status });
    }
    [HttpPost]
    public async Task<IActionResult> DeleteMultipleItems(int[] ids, int editorId)
    {
        try
        {
            result = await _menuService.DeleteMultipleItems(ids, editorId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return Json(new { message = result.Message, status = result.Status });
    }
    #endregion


    #region ModifierGroup

    #endregion

    #region Modifier

    #endregion
    [HttpPost]
    public async Task<IActionResult> GetModifiersList(int modifierGroupId, PaginationDetails paginationDetails)
    {
        try
        {
            result = await _menuService.GetModifiers(modifierGroupId, paginationDetails);
            ViewBag.paginationDetails = paginationDetails;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        List<ModifierDetails> modifierList = result.Data as List<ModifierDetails> ?? new List<ModifierDetails>();
        return PartialView("_partial_ModifiersListGrid", modifierList);
        // return Json(modifierList);
    }

    public async Task<IActionResult> GetModifierForEdit(int modifierId)
    {
        try
        {
            result = await _menuService.GetModifierByModifierId(modifierId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        ModifierDetails modifier = (ModifierDetails)result.Data;
        return Json(modifier);
    }

    public async Task<IActionResult> DeleteModifierById(int modifierId)
    {
        try
        {
            result = await _menuService.DeleteModifier(modifierId);

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return RedirectToAction("Menu", "Menu");
    }
    public async Task<IActionResult> DeleteModifierGroupById(int modifierGroupId,int editorId)
    {
        try
        {
            result = await _menuService.DeleteModifierGroup(modifierGroupId,editorId);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        List<ModifierGropDetails> updatedGroupList = result.Data as List<ModifierGropDetails> ?? new List<ModifierGropDetails>();
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        return PartialView("_partial_ModifiersGroupListGrid", updatedGroupList);
    }




    [HttpPost]
    public async Task<IActionResult> AddModifier(AddModifier newModifier)
    {
        try
        {
            result = await _menuService.AddNewModifier(newModifier);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        List<ModifierDetails> modifierList = (List<ModifierDetails>)result.Data;
        return RedirectToAction("Menu", "Menu");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteMultipleModifiers(int[] ids)
    {
        try
        {
            result = await _menuService.DeleteMultipleModifiers(ids);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return Json(new { message = result.Message, status = result.Status });
    }
    [HttpGet]
    public async Task<IActionResult> GetItemById(int itemId)
    {
        try
        {
            AddItem item = await _menuService.GetItemById(itemId);
            return Json(new { success = true, data = item });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, data = ex.Message });
        }

    }
    [HttpPost]
    public async Task<IActionResult> UpdateModifier(AddModifier editModifier)
    {
        try
        {
            result = await _menuService.EditModifier(editModifier);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        TempData["ToastMessage"] = result.Message;
        TempData["ToastStatus"] = result.Status.ToString(); // Convert Enum to String
        List<ModifierDetails> modifierList = (List<ModifierDetails>)result.Data;
        return RedirectToAction("Menu", "Menu");
    }
    [HttpGet]
    public async Task<IActionResult> GetAllExistingModifers(PaginationDetails paginationDetails)
    {
        try
        {
            List<ModifierDetails> modifiers = await _menuService.GetAllModifiers(paginationDetails);
            // return Json(new { success = true, data = modifiers });
            ViewBag.paginationDetails = paginationDetails;
            return PartialView("_partial_AllReadyExistingModifiers", modifiers);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, data = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddModifierGroup(string groupName, string description, List<int> modifierIdList)
    {

        // if (!string.IsNullOrEmpty(modifierIdList.ToString()))
        // {
        //     modifierIdList = JsonConvert.DeserializeObject<List<int>>(Request.Form["modifiersForAddNewGroup"]);
        // }

        ModifierGroupVM newGroup = new ModifierGroupVM();
        newGroup.groupName = groupName;
        newGroup.description = description;
        newGroup.modifierIds = modifierIdList;
        try
        {
            result = await _menuService.AddModifierGroup(newGroup);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return Json(new { success = true, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateModifierGroup([FromBody] ModifierGroupVM updateGroup)
    {

        // if (!string.IsNullOrEmpty(modifierIdList.ToString()))
        // {
        //     modifierIdList = JsonConvert.DeserializeObject<List<int>>(Request.Form["modifiersForAddNewGroup"]);
        // }
        try
        {
            result = await _menuService.UpdateModifierGroup(updateGroup);
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }

        return Json(new { data = result.Data, message = result.Message, status = result.Status });
    }

    public async Task<IActionResult> GetModifiersByGroupId(int groupId)
    {
        try
        {
            List<ModifierDetails> modifiers = await _menuService.GetModifiersByGroupId(groupId);
            result.Data = modifiers;
            if (modifiers != null)
            {
                result.Message = "Data Get Successfully!!!";
                result.Status = ResponseStatus.Success;
            }
            else
            {
                result.Message = "Data Not Found!!!";
                result.Status = ResponseStatus.NotFound;
            }

        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.Status = ResponseStatus.Error;
        }
        return Json(new { modifiers = result.Data as List<ModifierDetails>, message = result.Message, status = result.Status });
    }

    [HttpPost]
    public async Task<IActionResult> ShowModifierGroup(List<ModifierGropDetails> groupList)
    {
        return PartialView("_partial_ModifiersGroupListGrid", groupList);
    }
}
