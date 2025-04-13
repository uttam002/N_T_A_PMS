//#region Order Pagination
let paginationModel = {
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
function updatePaginationInfo(pagination) {
  paginationModel.totalRecords = pagination.totalRecords;
  $("#paginationInfo").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchOrders() {
  $.ajax({
    url: "/Orders/Order",
    type: "GET",
    contentType: "application/json",
    data: paginationModel,
    success: function (response) {
      console.log("Hello here " + response.partialView);
      $("#orderList").html(response.partialView);
      updatePaginationInfo(response.paginationDetails);
      // showToaster(response.message, response.status);
    },
    error: function (error) {
      console.log("Error:", error);
    },
  });
}
// Sorting
$(document).on("click", ".sort", function () {
  paginationModel.sortColumn = $(this).data("sortby");
  paginationModel.sortOrder =
    paginationModel.sortOrder === "asc" ? "desc" : "asc";
  fetchOrders();
});

// Search Functionality
$(document).on("input", "#SearchUser", function () {
  paginationModel.searchQuery = $(this).val();
  paginationModel.pageNumber = 1;
  fetchOrders();
});

// Date Filter
$(document).on("click", "#searchDateRange", function () {
  paginationModel.fromDate = $("#fromDate").val();
  paginationModel.toDate = $("#toDate").val();
  paginationModel.pageNumber = 1; // Reset to first page
  fetchOrders();
});

$(document).on("change", "#orderStatus", function () {
  paginationModel.orderStatus = $(this).val();
  paginationModel.pageNumber = 1; // Reset to first page
  fetchOrders();
});
$(document).on("change", "#dateRange", function () {
  paginationModel.dateRange = $(this).val();
  paginationModel.pageNumber = 1; // Reset to first page
  fetchOrders();
});
// Clear Date Filter
$(document).on("click", "#clearDateRange", function () {
  $("#fromDate").val("");
  $("#toDate").val("");
  paginationModel.fromDate = "";
  paginationModel.toDate = "";
  paginationModel.pageNumber = 1; // Reset to first page
  fetchOrders();
});
// Change Items Per Page
$(document).on("change", "#OrderPerPage", function () {
  paginationModel.pageSize = $(this).val();
  paginationModel.pageNumber = 1;
  fetchOrders();
});

// Pagination
$(document).on("click", "#prevPage", function () {
  if (paginationModel.pageNumber > 1) {
    paginationModel.pageNumber--;
    fetchOrders();
  }
});
$(document).on("click", "#nextPage", function () {
  if (
    paginationModel.pageNumber * paginationModel.pageSize <
    paginationModel.totalRecords
  ) {
    paginationModel.pageNumber++;
    fetchOrders();
  }
});

$(document).ready(function () {
  paginationModel.totalRecords = $("#totalRecordsForOrderPage").val();
  updatePaginationInfo(paginationModel);
});

//#endregion

//#region View-order-details

$(document).on("click",".viewOrderDetails", function () {
  console.log("Hello From OrdesDetails")
  let orderId = $(this).data("order-id");

  $.ajax({
    url: '/Orders/OrderDetails',
    type: "GET",
    data: { orderId: orderId },
    success: function () {
        showToaster("Order Details Show successfully!!!","Success");
    },
    error: function () {
        showToaster("Error during fetch Order Details!!!","Error");
    },
  });
});

//#endregion
