//#region Customer Pagination
let paginationModelForCustomerPage = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "name",
  sortOrder: "asc",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  customerStatus: "",
  dateRange: "",
};
function updatePaginationInfo(pagination) {
  paginationModelForCustomerPage.totalRecords = pagination.totalRecords;
  $("#totalRecordsForCustomerPage").val(pagination.totalRecords);
  $("#fromDateForCustomerPageHiddenValue").val(pagination.fromDate);
  $("#toDateForCustomerPageHiddenValue").val(pagination.toDate);
  $("#paginationInfoForCustomerPage").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchCustomers() {
  $.ajax({
    url: "/Customers/Customer",
    type: "GET",
    contentType: "application/json",
    data: paginationModelForCustomerPage,
    beforeSend: function () {
      $("#customerList").css("opacity", "0.5"); // Show fade effect
  },
    success: function (response) {
      $("#customerList").html(response.partialView);
      $("#customerList").css("opacity", "1"); // Show fade effect
      updatePaginationInfo(response.paginationDetails);
    },
    error: function (error) {
      showToaster(error, "Error");
    },
  });
}

// Sorting
$(document).on("click", ".sortForCustomerList", function () {
  paginationModelForCustomerPage.sortColumn = $(this).data("sortby");
  paginationModelForCustomerPage.sortOrder =
    paginationModelForCustomerPage.sortOrder === "asc" ? "desc" : "asc";
  fetchCustomers();
});

// Search Functionality
$(document).on("input", "#SearchCustomer", function () {
  paginationModelForCustomerPage.searchQuery = $(this).val();
  paginationModelForCustomerPage.pageNumber = 1;
  fetchCustomers();
});

// Date Filter
$(document).on("click", "#searchDateRangeForCustomerPage", function () {
  paginationModelForCustomerPage.fromDate = $("#fromDateForCustomerPage").val();
  paginationModelForCustomerPage.toDate = $("#toDateForCustomerPage").val();
  paginationModelForCustomerPage.pageNumber = 1; // Reset to first page
  fetchCustomers();
  $("#customDateRangeModal").modal("hide");
});

$(document).on("change", "#dateRangeForCustomerPage", function () {
  if ($(this).val() === "CustomRange") {
    $("#customDateRangeModal").modal("show");
    paginationModelForCustomerPage.dateRange = $(this).val();
  } else {
    paginationModelForCustomerPage.fromDate = "";
    paginationModelForCustomerPage.toDate = "";
    paginationModelForCustomerPage.dateRange = $(this).val();
    paginationModelForCustomerPage.pageNumber = 1; // Reset to first page
    fetchCustomers();
  }
});
// Clear Date Filter
$(document).on("click", "#clearDateRangeForCustomerPage", function () {
  $("#fromDateForCustomerPage").val("");
  $("#toDateForCustomerPage").val("");
  paginationModelForCustomerPage.fromDate = "";
  paginationModelForCustomerPage.toDate = "";
  paginationModelForCustomerPage.pageNumber = 1; // Reset to first page
  fetchCustomers();
});
// Change Items Per Page
$(document).on("change", "#CustomerPerPage", function () {
  paginationModelForCustomerPage.pageSize = $(this).val();
  paginationModelForCustomerPage.pageNumber = 1;
  fetchCustomers();
});

// Pagination
$(document).on("click", "#prevPageForCustomerPage", function () {
  if (paginationModelForCustomerPage.pageNumber > 1) {
    paginationModelForCustomerPage.pageNumber--;
    fetchCustomers();
  }
});
$(document).on("click", "#nextPageForCustomerPage", function () {
  if (
    paginationModelForCustomerPage.pageNumber *
      paginationModelForCustomerPage.pageSize <
    paginationModelForCustomerPage.totalRecords
  ) {
    paginationModelForCustomerPage.pageNumber++;
    fetchCustomers();
  }
});
$(document).on("click", "#nextPageForCustomerPage", function () {
  if (
    paginationModelForCustomerPage.pageNumber *
      paginationModelForCustomerPage.pageSize <
    paginationModelForCustomerPage.totalRecords
  ) {
    paginationModelForCustomerPage.pageNumber++;
    fetchCustomers();
  }
});


$(document).on("click", ".customerDetailsRow", function () {
  console.log($(this).data("customer-id"));
  let customerId = $(this).data("customer-id");
  $.ajax({
    url: `/Customers/GetCustomerHistory/`,
    type: "GET",
    data: {
      customerId: customerId,
    },
    success: function (response) {
      $("#showCustomerHistory").html(response);
      $("#CustomerHistoryModal").modal("show");
    },
    error: function (error) {
      showToaster(error, "Error");
    },
  });
});
$(document).ready(function () {
  paginationModelForCustomerPage.totalRecords = $(
    "#totalRecordsForCustomerPage"
  ).val();
  updatePaginationInfo(paginationModelForCustomerPage);
});
//#endregion
