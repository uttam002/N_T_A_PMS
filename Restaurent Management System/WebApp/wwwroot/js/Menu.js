//#region  View Models
let categoryDetails = {
  id: 0,
  categoryName: "",
  description: "",
  editorId: 0,
};

const editorId = $("#editorIdAtMenuPage").val();
//#endregion
//#region  Validations
/* -------------------------- Validation for modals ------------------------- */
function validateCategoryDetails(categoryDetails) {
  if (categoryDetails.categoryName.trim() === "") {
    console.log("Category Details:", categoryDetails);
    $("#validationTextForCategoryNameForAddCategory").text(
      "Please Enter Category Name"
    );
    $("#validationTextForCategoryNameForAddCategory").removeClass("d-none");
    $("#categoryNameForAddCategory").focus();
    return false;
  }
  return true;
}
function validateCategoryDetailsForEdit(categoryDetails) {
  if (categoryDetails.categoryName.trim() === "") {
    console.log("Hello Edit Category Details:", categoryDetails);
    $("#validationTextForCategoryNameForEditCategory").text(
      "Please Enter Category Name"
    );
    $("#validationTextForCategoryNameForEditCategory").removeClass("d-none");
    $("#categoryNameForAddCategory").focus();
    return false;
  }
  return true;
}
function hideValidationTextForCategoryNameAtAddCategory() {
  $("#validationTextForCategoryNameForAddCategory").addClass("d-none");
  $("#validationTextForCategoryNameForAddCategory").text("");
}
function hideValidationTextForCategoryNameAtEditCategory() {
  $("#validationTextForCategoryNameForEditCategory").addClass("d-none");
  $("#validationTextForCategoryNameForEditCategory").text("");
}



//#endregion
//#region  Category Section
function highlightCategory(categoryId) {
  // Remove active class from all category items
  $(".category-item").removeClass("active-category");

  // Hide all category action buttons
  $(".showBtns").addClass("d-none");

  // Add active class to the selected category
  $(`#category-${categoryId}`).addClass("active-category");

  // Show action buttons for the selected category
  $(`#categoryBtns-${categoryId}`).removeClass("d-none");
}
// ===============================
// Category Management Functions
// ===============================
function AssignValueForCategory(categoryId) {
  $("#categoryIdForDelete").val(categoryId);
}

/* ------------------------------ Add Category ------------------------------ */
$(document).on("click", "#addCategoryBtn", function () {
  categoryDetails.categoryName = $("#categoryNameForAddCategory").val();
  categoryDetails.description = $(
    "#categoryDescriptionForAddCategory"
  ).val();
  categoryDetails.editorId = $("#editorIdForAddCategory").val();
  if (validateCategoryDetails(categoryDetails)) {
    $.ajax({
      url: "/Menu/AddCategory",
      type: "POST",
      data: { newCategory: categoryDetails },
      success: function (response) {
        showToaster(response.message, response.status);
        $("#addCategoryModal").modal("hide");
        $("#categorySection").html(response.data);
      },
      error: function () {
        showToaster("Error Occur On Ajax Call", "Error");
      },
    });
  }
});

/* ------------------------------ Edit Category ----------------------------- */
function FillFormForEditCategory(
  categoryId,
  categoryName,
  categoryDescription
) {
  $("#categoryIdForEditCategory").val(categoryId);
  $("#categoryNameForEditCategory").val(categoryName);
  $("#categoryDescriptionForEditCategory").val(categoryDescription);
  categoryDetails.id = categoryId;
  categoryDetails.categoryName = categoryName;
  categoryDetails.description = categoryDescription;
}
function clearEditCategoryForm() {
  FillFormForEditCategory(categoryDetails.id, categoryDetails.categoryName, categoryDetails.description);
}
$(document).on("click", "#editCategoryBtn", function () {
  categoryDetails.id = $("#categoryIdForEditCategory").val();
  categoryDetails.categoryName = $("#categoryNameForEditCategory").val();
  categoryDetails.description = $(
    "#categoryDescriptionForEditCategory"
  ).val();
  categoryDetails.editorId = $("#editorIdForEditCategory").val();
  console.log("Category Details:", categoryDetails);
  if (validateCategoryDetailsForEdit(categoryDetails)) {
    $.ajax({
      url: "/Menu/EditCategory",
      type: "POST",
      data: { updateCategory: categoryDetails },
      success: function (response) {
        showToaster(response.message, response.status);
        $("#EditCategoryModal").modal("hide");
        $("#categorySection").html(response.partialView);
      },
      error: function () {
        showToaster("Error Occur On Ajax Call", "Error");
      },
    });
  }
});

/* ------------------------------ Delete Category ----------------------------- */
$(document).on("click", "#deleteCategoryBtn", function () {
  let categoryId = $("#categoryIdForDelete").val();
  let editorId = $("#editorIdForDeleteCategory").val();
  $.ajax({
    url: "/Menu/DeleteCategory",
    type: "POST",
    data: { categoryId: categoryId, editorId: editorId },
    success: function (response) {
      showToaster(response.message, response.status);
      $("#deleteCategoryModal").modal("hide");
      $("#categorySection").html(response.partialView);
    },
    error: function () {
      showToaster("Error Occur On Ajax Call", "Error");
    },
  });
});

//#endregion

//#region  Pagination For Item Table

let paginationForItemTable = {
  pageSize: 3,
  pageNumber: 1,
  sortColumn: "",
  sortOrder: "",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
};

function updatePaginationInfoForItemPage() {
  let paginationDetailsElement = $("#paginationDetailsForItemsPage");

  paginationForItemTable.pageSize = paginationDetailsElement.data("page-size");
  paginationForItemTable.pageNumber =
    paginationDetailsElement.data("page-number");
  paginationForItemTable.sortColumn =
    paginationDetailsElement.data("sort-column");
  paginationForItemTable.sortOrder =
    paginationDetailsElement.data("sort-order");
  paginationForItemTable.searchQuery =
    paginationDetailsElement.data("search-query");
  paginationForItemTable.fromDate = paginationDetailsElement.data("from-date");
  paginationForItemTable.toDate = paginationDetailsElement.data("to-date");
  paginationForItemTable.totalRecords =
    paginationDetailsElement.data("total-records");
  console.log(paginationForItemTable);

  $("#paginationInfoForItemTable").text(
    `Showing ${(paginationForItemTable.pageNumber - 1) *
    paginationForItemTable.pageSize +
    1
    } - ${Math.min(
      paginationForItemTable.pageNumber * paginationForItemTable.pageSize,
      paginationForItemTable.totalRecords
    )} of ${paginationForItemTable.totalRecords}`
  );
}

function getItems() {
  console.log("Here in getItems............");
  let categoryId =
    $(".category-item.active-category").data("category-id") || null;
  console.log(categoryId);
  getItemsbyCategoryId(categoryId);
}

function getItemsbyCategoryId(categoryId) {
  console.log("From getItemsbyCategoryId");
  console.log(paginationForItemTable);
  $.ajax({
    url: "/Menu/GetItems",
    type: "POST",
    data: { id: categoryId, paginationDetails: paginationForItemTable },
    success: function (response) {
      $("#MenuItems").html(response);
      highlightCategory(categoryId);
      updatePaginationInfoForItemPage();
    },
    error: function () {
      alert("No Items Found in this Category");
    },
  });
}

// Search Functionality
$(document).on("input", "#searchItem", function () {
  paginationForItemTable.searchQuery = $(this).val();
  paginationForItemTable.pageNumber = 1;
  getItems();
});

// Change Items Per Page
$(document).on("change", "#itemsPerPage", function () {
  console.log("Changed items per page...");
  paginationForItemTable.pageSize = parseInt($(this).val(), 10);
  console.log(paginationForItemTable);
  paginationForItemTable.pageNumber = 1;
  getItems();
});

// Pagination - Previous Button
$(document).on("click", "#prevPageForItem", function () {
  console.log("Changed items per page...");
  if (paginationForItemTable.pageNumber > 1) {
    paginationForItemTable.pageNumber--;
    getItems();
  }
});

// Pagination - Next Button
$(document).on("click", "#nextPageForItem", function () {
  if (
    paginationForItemTable.pageNumber * paginationForItemTable.pageSize <
    paginationForItemTable.totalRecords
  ) {
    paginationForItemTable.pageNumber++;
    getItems();
  }
});
//#endregion



// Event listener for individual row checkboxes
$(document).on("click", ".row-checkbox", function () {
  let table = $(this).closest(".selectable-table");
  let checkbox = $(this);
  checkbox.prop("checked", !checkbox.prop("checked")).trigger("change");
  updateSelectAllState(table);
});

// Click on row to toggle checkbox selection (excluding direct checkbox clicks)
$(document).on("click", ".selectable-row", function () {
  let checkbox = $(this).find(".row-checkbox");
  checkbox.prop("checked", !checkbox.prop("checked")).trigger("change");

  let table = $(this).closest(".selectable-table");
  updateSelectAllState(table);
});

// Function to update "Select All" checkbox state for the current table
function updateSelectAllState(table) {
  let totalCheckboxes = table.find(".row-checkbox").length;
  let checkedCheckboxes = table.find(".row-checkbox:checked").length;
  let selectAll = table.find(".select-all-checkbox");

  if (checkedCheckboxes === 0) {
    selectAll.prop("checked", false).prop("indeterminate", false);
  } else if (checkedCheckboxes === totalCheckboxes) {
    selectAll.prop("checked", true).prop("indeterminate", false);
  } else {
    selectAll.prop("checked", false).prop("indeterminate", true);
  }
}

//#region  Item Section


function AssignValue(itemId) {
  $("#itemIdForDelete").val(itemId);
}
/* ---------------------------------Mass Items Delete ----------------------------*/
$(document).on("click", "#deleteSelectedItems", function () {
  let selectedIds = [];
  $(".row-checkbox:checked").each(function () {
    selectedIds.push($(this).data("itemid"));
  });
  if (selectedIds.length > 0) {
    $.ajax({
      url: "/Menu/DeleteMultipleItems",
      type: "POST",
      data: { ids: selectedIds, editorId: editorId },
      success: function (response) {
        getItems();
        showToaster(response.message, response.status);
        $("#deleteItemsModal").modal("hide");
      },
      error: function (xhr, status, error) {
        showToaster(error, "Error");
        $("#deleteItemsModal").modal("hide");
      },
    });
  } else {
    showToaster("Please select at least one item to delete.", "Not Found");
    $("#deleteItemsModal").modal("hide");
  }
});

function openEditModal(itemId) {
  console.log(itemId);
  $.ajax({
    url: "/Menu/GetItemById",
    type: "GET",
    data: { itemId: itemId },
    success: function (response) {
      if (response.success) {
        console.log(response.data);
        let editModal = $("#EditItemModal");
        console.log(editModal);

        if (editModal.length) {
          $("#EditItemModal").modal("show");
        }

        $("#UpdateCategoryForEdit").val(response.data.categoryId);
        $("#ItemNameForEdit").val(response.data.name);
        $("#Item-TypeForEdit").val(response.data.itemType);
        $("#PriceRateForEdit").val(response.data.unitPrice);
        $("#ItemQuantityForEdit").val(response.data.quantity);
        $("#UpdateUnitForEdit").val(response.data.unitType);
        $("#AvailableForEdit").prop("checked", response.data.isAvailable);
        $("#DefaultTaxForEdit").prop("checked", response.data.defaultTax);
        console.log(response.data.taxPercentage);
        $("#ItemTaxPercentageForEdit").val(response.data.taxPercentage);
        $("#ShortCdeForEdit").val(response.data.shortCode);
        $("#descriptionForEdit").val(response.data.description);
        $("#ItemIdForEdit").val(itemId);
        console.log("Hello " + itemId);

        // Directly pass imDetails to the function
        let itemModifierRelation = [].concat(response.data.imDetails || []);
        itemModifierRelation.forEach((modifier) => {
          console.log("Modifiers:", modifier); // Debugging

          addModifierGroup({
            IMGid: modifier.imGid,
            ItemId: modifier.itemId,
            MgId: modifier.mgId,
            MinModifiers: modifier.minModifiers,
            MaxModifiers: modifier.maxModifiers,
          });
        });
      } else {
        alert("Error fetching item data.");
      }
    },
    error: function () {
      alert("An error occurred while fetching data.");
    },
  });
}

function addModifierGroup(modifier) {
  let itemId = modifier.ItemId;

  let itemModifierRelationId = modifier.IMGid;
  let modifierGroupId = modifier.MgId;
  let minModifiers = modifier.MinModifiers;
  let maxModifiers = modifier.MaxModifiers;

  if ($("#modifierGroupSection-" + modifierGroupId).length === 0) {
    $.ajax({
      url: "/Menu/GetModifiersByGroupId",
      type: "GET",
      data: { groupId: modifierGroupId },
      success: function (data) {
        if (!data || data.length === 0) return;
        console.log(modifierGroupId);
        let groupName =
          $("#attachModifierGroupforEditItem")
            .find(`option[value="${modifierGroupId}"]`)
            .text() || "Unknown Group";
        let tableRows = data.modifiers
          .map(
            (mod) =>
              `<tr><td><li>${mod.modifierName}</li></td><td class="text-end">${mod.unitPrice}</td></tr>`
          )
          .join("");
        console.log("Hello form here" + tableRows);

        let newModifiersHtml = `
          <div class="row px-4 modifier-group" id="modifierGroupSection-${modifierGroupId}">
              <div class="col-12 my-1 d-flex justify-content-between">
                  <h4 class="mx-4">${groupName}</h4>
                  <button type="button" class="btn-close remove-modifier-group" data-group-id="${modifierGroupId}"></button>
              </div>
              <div class="col-12 d-flex justify-content-center">
                  <input type="number" class="form-control rounded-pill mx-2 min-modifier" name="IMDetails[${modifierGroupId}].MinModifiers" value="${minModifiers}" placeholder="Min Modifiers" min="0">
                  <input type="number" class="form-control rounded-pill mx-2 max-modifier" name="IMDetails[${modifierGroupId}].MaxModifiers" value="${maxModifiers}" placeholder="Max Modifiers" min="0">
              </div>
              <input type="hidden" name="IMDetails[${modifierGroupId}].IMGid" class="IMGid" value="${itemModifierRelationId}">
              <input type="hidden" name="IMDetails[${modifierGroupId}].ItemId" class="ItemId" value="${itemId}">
              @* <input type="hidden" name="IMDetails[${modifierGroupId}].MgId" class="MgId" value="${modifierGroupId}"> *@
              <div class="col-12 d-flex justify-content-center">
                  <table class="mx-2 w-100">
                      <tbody>${tableRows}</tbody>
                  </table>
              </div>
          </div>`;
        if (itemId > 0) {
          $("#SelectedModifierGroups").append(newModifiersHtml);
        } else {
          $("#SelectedModifierGroupsForAddItem").append(newModifiersHtml);
        }
      },
      error: function (xhr, status, error) {
        toastr.error("Failed to load modifier details. ", "Error", {
          positionClass: "toast-top-right",
          timeOut: 3000,
        });
      },
    });
  } else {
    // Show a toastr notification if the element already exists
    toastr.warning("This modifier group already exists!", "Warning", {
      positionClass: "toast-top-right",
      timeOut: 3000, // Auto close in 3 seconds
    });
  }
}

$(document).on("change", "#attachModifierGroupforEditItem", function () {
  let modifierGroupId = parseInt($(this).val(), 10);
  let itemId = parseInt($("#ItemIdForEdit").val(), 10);
  console.log(itemId);
  if (!isNaN(modifierGroupId)) {
    $(this).prop("disabled", true);
    // @* console.log(modifierGroupId); *@
    let modifier = {
      IMGid: 0,
      ItemId: itemId,
      MgId: modifierGroupId,
      MinModifiers: 0,
      MaxModifiers: 0,
    };

    addModifierGroup(modifier);
    // @* console.log(modifier); *@
    $(this).prop("disabled", false);
  }
});

$("#SelectedModifierGroups").on("click", ".remove-modifier-group", function () {
  $("#modifierGroupSection-" + $(this).data("group-id")).remove();
});

$(document).on("click", "#updateItemBtn", function (event) {
  event.preventDefault();
  let modifierGroups = [];
  $("#SelectedModifierGroups > .row").each(function () {
    let groupId = $(this).attr("id").split("-")[1];
    let imGid = $(this).find(".IMGid").val() || 0;
    let itemId = $(this).find(".ItemId").val() || 0;
    let minModifier = $(this).find(".min-modifier").val() || 0;
    let maxModifier = $(this).find(".max-modifier").val() || 0;

    modifierGroups.push({
      IMGid: imGid,
      ItemId: itemId,
      MgId: groupId,
      MinModifiers: minModifier,
      MaxModifiers: maxModifier,
    });
  });
  $("#ItemModifierRelationForEdit").val(JSON.stringify(modifierGroups));
  let formData1 = new FormData($("#editItemForm")[0]);
  let form = $('#editItemForm');
  if(!form.valid()){
    console.log("Form is not valid");
    return;
  }
  $.ajax({
    url: "/Menu/UpdateItem",
    type: "POST",
    data: formData1,
    dataType: "json",
    contentType: false, // Necessary for FormData
    processData: false, // Necessary for FormData
    success: function (response) {
      showToaster(response.message, response.status);
      $("#MenuItems").html(response.data);
      $("#EditItemModal").modal("hide");
    },
    error: function () {
      showToaster("An error occurred while updating item.", "Error");
      $("#EditItemModal").modal("hide");
    },
  });

});

$(document).ready(function () {

  
  function clearListFromEditItems() {
    console.log("Helo-----------------------------------")
   
    $('#EditItemModal select').each(function () {
      $(this).prop('selectedIndex', 0);
    });
  }

  $('#EditItemModal').on('hidden.bs.modal', function () {
    $("#SelectedModifierGroups").children().remove();
    let form = $('#editItemForm');

            // Clear validation messages
            form.find('.field-validation-error').each(function () {
                console.log("Clearing validation message");
                $(this).text('');
            });

            // Remove validation classes (invalid styles)
            form.find('.is-invalid').removeClass('is-invalid');
            form.find('.is-valid').removeClass('is-valid');

            // Optionally reset the form fields
            form[0].reset(); // Reset all form fields

            // Optional: Reset dropzone preview if needed
            $('#fileDetails').addClass('d-none');
            $('#imagePreview').hide();
            $('#fileInput').val('');

            // Clear the hidden IMDetails input field if needed
            $('#IMDetails').val('');
  });
  $('#EditCategoryModal').on('hidden.bs.modal', function () {
    console.log("Modal closed");
    hideValidationTextForCategoryNameAtEditCategory();
  });
  $('#AddCategoryModal').on('hidden.bs.modal', function () {
    console.log("Modal closed");
    hideValidationTextForCategoryNameAtAddCategory();
  });
});

//#endregion

//#region  Modifier Group Section

function highlightModifierGroup(groupId) {
  $(".modifier-group").removeClass("active-category");
  $(".showBtns").addClass("d-none");
  $(`#modifierGroup-${groupId}`).addClass("active-category");
  $(`#modifierGroupBtns-${groupId}`).removeClass("d-none");
}
// Event listener for "Select All" checkbox
$(document).on("change", ".select-all-checkbox", function () {
  let table = $(this).closest(".selectable-table");
  let rowCheckboxes = table.find(".row-checkbox");

  rowCheckboxes.prop("checked", this.checked);
  updateSelectAllState(table);
});


function AssignValueForDeleteModifierGroup(modifierGroupId) {
  console.log("Modifier Group ID: " + modifierGroupId);
  $("#modifierGroupIdForDelete").val(modifierGroupId);
}
function DeleteModifierGroup() {
  let modifierGroupId = $("#modifierGroupIdForDelete").val();
  $.ajax({
    url: "/Menu/DeleteModifierGroupById",
    type: "POst",
    data: { modifierGroupId: modifierGroupId,editorId: editorId },
    success: function (result) {
      $("#modiferGroupSection").html(result);
      $("body").removeClass("fade"); // Fix scrolling issue
    },
    error: function () {
      showToaster(
        "Error at Ajax handling during delete ModifierGroup",
        "Error"
      );
    },
  });
}
function FillFormForEditModifierGroup(
  modifierGroupId,
  modifierGroupName,
  modifierGroupDescription
) {
  clearListFromUpdateGroupSection();
  $("#editModifierGroupId").val(modifierGroupId);
  $("#editModifierGroupName").val(modifierGroupName);
  $("#editModifierGroupDescription").val(modifierGroupDescription);

  $.ajax({
    url: "/Menu/GetModifiersByGroupId",
    type: "GET",
    data: { groupId: modifierGroupId },
    success: function (result) {
      let modifiers = [].concat(result.data || []);
      modifiers.forEach((modifier) => {
        addModifierBadge(modifier);
      });
      showToaster(result.message, result.status);
    },
  });
}

function addModifierBadge(modifier) {
  let modifierId = modifier.id;
  let modifierName = modifier.modifierName;
  if (document.getElementById("modifierBadge-" + modifierId) === null) {
    var newBadge = `
          <span id="modifierBadge-${modifierId}" class="badge bg-primary p-2 me-2">
              ${modifierName}
              <button type="button" class="btn-close" onclick="removeModifierBadge(${modifierId})" ></button>
          </span>
      `;

    $(".modifierBadges").append(newBadge);
  }
}
function removeModifierBadge(modifierId) {
  $("#modifierBadge-" + modifierId).remove();
}
function saveModifierGroup() {
  console.log("modifierList hi");
  let modifierList = [];
  $(".modifierBadges > .badge").each(function () {
    let groupId = $(this).attr("id").split("-")[1];
    modifierList.push(groupId);
  });
  $("#modifiersForAddNewGroup").val(JSON.stringify(modifierList));
  console.log(modifierList);
  let groupName = $("#addModifierGroupName").val();
  let description = $("#addModifierGroupDescription").val();
  console.log(groupName);
  console.log(description);
  $.ajax({
    url: "/Menu/AddModifierGroup",
    type: "Post",
    data: {
      groupName: groupName,
      description: description,
      modifierIdList: modifierList,
    },
    success: function (data) {
      showToaster(data.Message, data.Status);
    },
    error: function () {
      toastr.error("Error occur On Ajax Call", "Error", {
        positionClass: "toast-top-right",
        timeOut: 3000, // Auto close in 3 seconds
      });
    },
  });
}
function clearListFromAddItems() {
  $("#SelectedModifierGroupsForAddItem").children().remove();
}
function clearListFromAddGroupSection() {
  $("#badgesForNewGroup").children().remove();
}
function clearListFromUpdateGroupSection() {
  $("#badgesForEditGroup").children().remove();
}

$(document).on("click", ".remove-modifier-group", function () {
  var groupId = $(this).data("group-id");
  $("#modifierGroupSection-" + groupId).remove();
});
/**
 * Highlights the selected modifier group.
 * @param {number} modifierGroupId - The ID of the modifier group.
 */
function highlightModifiers(modifierGroupId) {
  $(".category-item").removeClass("active-category");
  $(".showBtns").addClass("d-none");

  $(`#modifierGroup-${modifierGroupId}`).addClass("active-category");
  $(`#modifierGroupBtns-${modifierGroupId}`).removeClass("d-none");
}

//#endregion

function AssignValueForModifier(modifierId) {
  document.getElementById("modifierIdForDelete").value = modifierId;
}

let paginationForModifierTable = {
  pageSize: 3,
  pageNumber: 1,
  sortColumn: "",
  sortOrder: "",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
};

function updatePaginationInfoForModifierPage() {
  let paginationDetailsElement = $("#paginationDetailsForModifiersPage");

  paginationForModifierTable.pageSize =
    paginationDetailsElement.data("page-size");
  paginationForModifierTable.pageNumber =
    paginationDetailsElement.data("page-number");
  paginationForModifierTable.sortColumn =
    paginationDetailsElement.data("sort-column");
  paginationForModifierTable.sortOrder =
    paginationDetailsElement.data("sort-order");
  paginationForModifierTable.searchQuery =
    paginationDetailsElement.data("search-query");
  paginationForModifierTable.fromDate =
    paginationDetailsElement.data("from-date");
  paginationForModifierTable.toDate = paginationDetailsElement.data("to-date");
  paginationForModifierTable.totalRecords =
    paginationDetailsElement.data("total-records");
  $("#paginationInfoForModifierTable").text(
    `Showing ${(paginationForModifierTable.pageNumber - 1) *
    paginationForModifierTable.pageSize +
    1
    } - ${Math.min(
      paginationForModifierTable.pageNumber *
      paginationForModifierTable.pageSize,
      paginationForModifierTable.totalRecords
    )} of ${paginationForModifierTable.totalRecords}`
  );
}

function getModifiersList() {
  console.log("Here in getModifiers............");
  let modiferGroupId =
    $(".modifier-group.active-category").data("group-id") || null;
  console.log(modiferGroupId);
  getModifiersByGroupId(modiferGroupId);
}

function getModifiersByGroupId(modiferGroupId) {
  console.log("From getModifiersByCategoryId");
  console.log(paginationForModifierTable);
  $.ajax({
    url: "/Menu/GetModifiersList",
    type: "POST",
    data: {
      modifierGroupId: modiferGroupId,
      paginationDetails: paginationForModifierTable,
    },
    success: function (response) {
      $("#MenuModifiers").html(response);
      highlightModifierGroup(modiferGroupId);
      updatePaginationInfoForModifierPage();
    },
    error: function () {
      alert("No Modifiers Found in this Category");
    },
  });
}
$(document).on("click", "#deleteSelectedModifiers", function () {
  let selectedIds = [];
  $(".row-checkbox:checked").each(function () {
    selectedIds.push($(this).data("moidierid"));
  });

  if (selectedIds.length > 0) {
    // Send the IDs to the server via AJAX
    console.log(JSON.stringify(selectedIds));
    $.ajax({
      url: "/Menu/DeleteMultipleModifiers",
      type: "POST",
      data: { ids: selectedIds },
      success: function (response) {
        getItems();
        showToaster(response.message, response.status);
        $("#deleteMultipleModifiersModal").modal("hide");
      },
      error: function (xhr, status, error) {
        // Handle any errors
        alert("Error deleting items: " + error);
      },
    });
  } else {
    alert("Please select at least one item to delete.");
  }
});

// Search Functionality
$(document).on("input", "#searchModifier", function () {
  paginationForModifierTable.searchQuery = $(this).val();
  paginationForModifierTable.pageNumber = 1;
  getModifiersList();
});

// Change Modifiers Per Page
$(document).on("change", "#modifiersListPerPage", function () {
  console.log("Changed modifiers per page...");
  paginationForModifierTable.pageSize = parseInt($(this).val(), 10);
  console.log(paginationForModifierTable);
  paginationForModifierTable.pageNumber = 1;
  getModifiersList();
});

// Pagination - Previous Button
$(document).on("click", "#prevPageForModifier", function () {
  console.log("Previous page for modifiers...");
  if (paginationForModifierTable.pageNumber > 1) {
    paginationForModifierTable.pageNumber--;
    getModifiersList();
  }
});

// Pagination - Next Button
$(document).on("click", "#nextPageForModifier", function () {
  if (
    paginationForModifierTable.pageNumber *
    paginationForModifierTable.pageSize <
    paginationForModifierTable.totalRecords
  ) {
    paginationForModifierTable.pageNumber++;
    getModifiersList();
  }
});
//#endregion

function updateModifierGroup() {
  console.log("modifierList hi");
  clearListFromAddGroupSection();
  var modifierList = [];
  $(".modifierBadges > .badge").each(function () {
    var groupId = $(this).attr("id").split("-")[1];
    modifierList.push(groupId);
  });
  let groupId = $("#editModifierGroupId").val();
  let groupName = $("#editModifierGroupName").val();
  let description = $("#editModifierGroupDescription").val();
  let updateGroup = {
    groupId: groupId,
    groupName: groupName,
    description: description,
    modifierIds: modifierList,
  };

  $.ajax({
    url: "/Menu/UpdateModifierGroup",
    type: "POST",
    contentType: "application/json",
    data: JSON.stringify(updateGroup),
    success: function (respose) {
      getModifiers(groupId);
      showModifierGroupSectionPartially(respose.data);
      $("#editModifierGroupModal").modal("hide");
      showToaster(respose.message, respose.status);
    },
  });

  function showModifierGroupSectionPartially(groupList) {
    console.log(groupList);
    $.ajax({
      url: "/Menu/ShowModifierGroup",
      type: "POST",
      data: { groupList: groupList },
      success: function (respose) {
        console.log("Hiiiiiiiiiiiii" + respose);
        $("#modiferGroupSection").html(respose);
      },
      error: function () {
        showToaster("Failed To load Updated Group List", 1);
      },
    });
  }
}

//#region All Ready Existing Modifier's Modal
let paginationModelforEM = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "",
  sortOrder: "",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
};

// Extract pagination details from the hidden element
function updatePaginationInfo() {
  let paginationDetailsElement = $("#paginationDetailsForExistingModifiers");

  paginationModelforEM.pageSize = paginationDetailsElement.data("page-size");
  paginationModelforEM.pageNumber =
    paginationDetailsElement.data("page-number");
  paginationModelforEM.sortColumn =
    paginationDetailsElement.data("sort-column");
  paginationModelforEM.sortOrder = paginationDetailsElement.data("sort-order");
  paginationModelforEM.searchQuery =
    paginationDetailsElement.data("search-query");
  paginationModelforEM.fromDate = paginationDetailsElement.data("from-date");
  paginationModelforEM.toDate = paginationDetailsElement.data("to-date");
  paginationModelforEM.totalRecords =
    paginationDetailsElement.data("total-records");
  console.log("Hello" + paginationModelforEM);
  $("#paginationInfoAtExistingModifierModal").text(
    `Showing ${(paginationModelforEM.pageNumber - 1) * paginationModelforEM.pageSize + 1
    } - ${Math.min(
      paginationModelforEM.pageNumber * paginationModelforEM.pageSize,
      paginationModelforEM.totalRecords
    )} of ${paginationModelforEM.totalRecords}`
  );
}

function GetAllExistingModifers() {
  $.ajax({
    url: "/Menu/GetAllExistingModifers",
    type: "GET",
    data: paginationModelforEM,
    success: function (response) {
      console.log("Received Data:", response);

      let existingModal = document.getElementById("AllModifiersModal");
      if (existingModal) {
        console.log(existingModal);
        $(
          "#ExsitingModifiersModal .modal-content .modal-body #modifiersTableBody"
        ).html(response);
        updatePaginationInfo();
        // updatePaginationInfo(paginationModel);
        // Show modal
        if (!existingModal.classList.contains("show")) {
          var myModal = new bootstrap.Modal(existingModal, {
            backdrop: "static",
            keyboard: false,
          });
          myModal.show();
        }
      } else {
        console.error("Modal with ID 'AllModifiersModal' not found!");
      }
    },
    error: function (status, error) {
      console.error("AJAX Error:", error);
      alert("Failed to fetch existing modifiers. Please try again.");
    },
  });
}

$(document).on("click", ".existingModiferModalBtn", function () {
  GetAllExistingModifers();
});
// Search Functionality
$(document).on("input", "#modifierSearchBox", function () {
  console.log("Search Query:", $(this).val());
  paginationModelforEM.searchQuery = $(this).val();
  paginationModelforEM.pageNumber = 1;
  GetAllExistingModifers();
});

// Change Items Per Page
$(document).on("change", "#modifiersPerPage", function () {
  paginationModelforEM.pageSize = $(this).val();
  console.log("Page Size:", paginationModelforEM.pageSize);
  paginationModelforEM.pageNumber = 1;
  GetAllExistingModifers();
});

// Pagination
$(document).on("click", "#prevModifierPage", function () {
  if (paginationModelforEM.pageNumber > 1) {
    paginationModelforEM.pageNumber--;
    console.log("Previous Page:", paginationModelforEM.pageNumber);
    GetAllExistingModifers();
  }
});

$(document).on("click", "#nextModifierPage", function () {
  if (
    paginationModelforEM.pageNumber * paginationModelforEM.pageSize <
    paginationModelforEM.totalRecords
  ) {
    paginationModelforEM.pageNumber++;
    console.log("Next Page:", paginationModelforEM.pageNumber);
    GetAllExistingModifers();
  }
});

function populateModifiers() {
  let checkedCheckboxes = document.querySelectorAll(
    ".modifierRowCheckBox:checked"
  );
  var selectedModifiers = [];

  checkedCheckboxes.forEach((modifier) => {
    selectedModifiers.push([
      modifier.getAttribute("data-modifierId"),
      modifier.getAttribute("data-modifierName"),
    ]);
  });

  selectedModifiers.forEach((modifier) => {
    if (document.getElementById("modifierBadge-" + modifier[0]) === null) {
      var newBadge = `
         <span id="modifierBadge-${modifier[0]}" class="badge bg-primary p-2 me-2">
           ${modifier[1]}
           <button type="button" class="btn-close" onclick="removeModifierBadge(${modifier[0]})" ></button>
         </span>`;
      $(".modifierBadges").append(newBadge);
    } else {
      showToster(
        "Some of them Are allready addModifierModal into this group!!!"
      );
    }
  });
}

function removeModifierBadge(modifierId) {
  $("#modifierBadge-" + modifierId).remove();
}
//#endregion

// Call the getItems function on page load
$(document).ready(function () {
  let categoryIdOfFirstchild = $("#categoryList .category-item:first").data(
    "category-id"
  );
  highlightCategory(categoryIdOfFirstchild);
  updatePaginationInfoForItemPage();
});
function prepareModifierPage() {
  console.log("Hello from perpareModifierPage");
  let groupIdOfFirstchild = $("#modifierGroupList .modifier-group:first").data(
    "group-id"
  );
  highlightModifierGroup(groupIdOfFirstchild);
  getModifiersList();
}

$(document).ready(function () {
  $('#fileInputAtAddItem').change(function () {
      const file = this.files[0];
      if (file) {
          $('#fileDetailsAtAddItem').removeClass('d-none');
          $('#fileNameAtAddItem').text(file.name);

          if (file.type.startsWith('image/')) {
              const reader = new FileReader();
              reader.onload = function (e) {
                  $('#imagePreviewAtAddItem').attr('src', e.target.result).show();
              };
              reader.readAsDataURL(file);
          } else {
              $('#imagePreviewAtAddItem').hide();
          }
      } else {
          $('#fileDetailsAtAddItem').addClass('d-none');
      }
  });

  $('#removeImageButtonAtEditItem').click(function () {
      $('#fileInputAtEditItem').val('');
      $('#fileDetailsAtEditItem').addClass('d-none');
      $('#imagePreviewAtEditItem').hide();
  });

  $('#fileInputAtEditItem').change(function () {
    const file = this.files[0];
    if (file) {
        $('#fileDetailsAtEditItem').removeClass('d-none');
        $('#fileNameAtEditItem').text(file.name);

        if (file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = function (e) {
                $('#imagePreviewAtEditItem').attr('src', e.target.result).show();
            };
            reader.readAsDataURL(file);
        } else {
            $('#imagePreviewAtEditItem').hide();
        }
    } else {
        $('#fileDetailsAtEditItem').addClass('d-none');
    }
});

$('#removeImageButtonAtEditItem').click(function () {
    $('#fileInputAtEditItem').val('');
    $('#fileDetailsAtEditItem').addClass('d-none');
    $('#imagePreviewAtEditItem').hide();
});
});
