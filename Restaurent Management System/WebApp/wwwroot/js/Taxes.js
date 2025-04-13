function AssignValueForDeleteTax(id) {
  document.getElementById("taxIdForDelete").value = id;
}

function AssignValueForEditTax(taxId, name, type, isEnabled, isDefault, rate,editorId) {
  console.log(taxId + name + type + rate + isEnabled + isDefault);
  $("#taxIdForEdit").val(taxId);
  $("#UpdateTaxNameForEdit").val(name);
  $("#UpdateTaxTypeForEdit").val(type);
  $("#RateForEdit").val(rate);
  $("#checkBoxForIsEnabledTaxForEdit").prop(
    "checked",
    isEnabled == "True" ? true : false
  ); // Convert to boolean
  $("#checkBoxForDefaultTaxForEdit").prop(
    "checked",
    isDefault == "True" ? true : false
  );
  $("#editorIdForEdit").val(editorId);
  $("#EditTaxModal").modal("show"); // Show the modal
}

// Handle form submission using AJAX
$(document).on("click", "#updateTaxes", function () {
  // Get form data
  let formData = {
    TaxId: $("#taxIdForEdit").val(),
    TaxName: $("#UpdateTaxNameForEdit").val(),
    TaxType: $("#UpdateTaxTypeForEdit").val(),
    TaxValue: $("#RateForEdit").val(),
    Isenabled: $("#checkBoxForIsEnabledTaxForEdit").is(":checked"),
    Isdefault: $("#checkBoxForDefaultTaxForEdit").is(":checked"),
    EditorId: $("#editorIdForEdit").val(),
  };
  console.log(formData);
  $.ajax({
    url: "/Taxes/UpdateTax",
    type: "POST",
    data: { taxDetails: formData },
    success: function (response) {
      showToaster(response.message, response.status);
      $("#EditTaxModal").modal("hide"); // Hide the modal
    },
    error: function (error) {
      console.log(error);
    },
  });
});

$(document).on("click", "#saveNewTaxes", function (e) {
  e.preventDefault(); // Prevent the default form submission

  // Get form data
  let formData = {
    TaxName: $("#newTaxName").val(),
    TaxType: $("#newTaxType").val(),
    TaxValue: $("#newTaxRate").val(),
    Isenabled: $("#checkBoxForIsEnabledForNewTax").is(":checked"),
    Isdefault: $("#checkBoxForDefaultNewTax").is(":checked"),
    EditorId: $("#editorIdForNewTax").val(),
  };
  console.log(formData);
  $.ajax({
    url: "/Taxes/AddNewTax",
    type: "POST",
    data: { taxDetails: formData },
    success: function (response) {
      console.log(response.partialView);
      $("#taxesList").html(response.partialView);
      updatePaginationInfoForTaxes();
      showToaster(response.message, response.status);
      $("#AddTaxModal").modal("hide");
    },
    error: function (error) {
      console.log(error);
    },
  });
});

let paginationModelForTaxes = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "id",
  sortOrder: "asc",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  orderStatus: "",
  dateRange: "",
};
let paginationDetailsElement = $("#paginationDetailsForTaxesPage");
function updatePaginationInfoForTaxes(pagination) {
  paginationModelForTaxes.totalRecords = pagination.totalRecords;
  console.log(paginationModelForTaxes);
  $("#paginationInfoForTaxesPage").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchTax() {
  $.ajax({
    url: "/Taxes/Taxes",
    type: "POST",
    data: paginationModelForTaxes,
    success: function (response) {
      console.log("Hello here " + response);
      $("#taxesList").html(response.partialView);
      updatePaginationInfoForTaxes(response.paginationDetails);
      // showToaster(response.message, response.status);
    },
    error: function (error) {
      console.log("Error:", error);
    },
  });
}

// Search Functionality
$(document).on("input", "#SearchTax", function () {
  paginationModelForTaxes.searchQuery = $(this).val();
  paginationModelForTaxes.pageNumber = 1;
  fetchTax();
});

// Change Items Per Page
$(document).on("change", "#TaxesPerPage", function () {
  paginationModelForTaxes.pageSize = $(this).val();
  paginationModelForTaxes.pageNumber = 1;
  fetchTax();
});

// Pagination
$(document).on("click", "#prevPageForTaxes", function () {
  if (paginationModelForTaxes.pageNumber > 1) {
    paginationModelForTaxes.pageNumber--;
    fetchTax();
  }
});
$(document).on("click", "#nextPageForTaxes", function () {
  if (
    paginationModelForTaxes.pageNumber * paginationModelForTaxes.pageSize <
    paginationModelForTaxes.totalRecords
  ) {
    paginationModelForTaxes.pageNumber++;
    fetchTax();
  }
});

$(document).ready(function () {
  paginationModelForTaxes.totalRecords = $("#totalRecordsForTaxPage").val();
  updatePaginationInfoForTaxes(paginationModelForTaxes);
});
