//#region User Pagination
let paginationModelForUserPage = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "name",
  sortOrder: "asc",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  userStatus: "",
  dateRange: "",
};
function updatePaginationInfoForUserPage(pagination) {
  paginationModelForUserPage.totalRecords = pagination.totalRecords;
  $("#totalRecordsForCustomerPage").val(pagination.totalRecords);
  $("#paginationInfoForUserPage").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchUsers() {
  $.ajax({
    url: "/User/UserList",
    type: "GET",
    contentType: "application/json",
    data: paginationModelForUserPage,
    success: function (response) {
      $("#userListContainer").html(response.partialView);
      updatePaginationInfoForUserPage(response.paginationDetails);
    },
    error: function (error) {
      console.log("Error:", error);
    },
  });
}
// signalr listner
document.addEventListener("UserDataChanged", (e) => {
  const { action, userData } = e.detail;
  // Handle user data updates
  fetchUsers()  // Call a function to update the user table
  }
);
// 
// Sorting
$(document).on("click", ".sortForUserList", function () {
    console.log("hello from sort");
  paginationModelForUserPage.sortColumn = $(this).data("sortby");
  paginationModelForUserPage.sortOrder =
    paginationModelForUserPage.sortOrder === "asc" ? "desc" : "asc";
  fetchUsers();
});
$(document).on("change", "#sortAccordion", function () {
  console.log("hello from sort");
paginationModelForUserPage.sortColumn = $(this).val();
paginationModelForUserPage.sortOrder = $("input[name='sortOrder']:checked").val();
fetchUsers();
});
$(document).on("click", ".btn-checkForUserOrder", function () {
paginationModelForUserPage.sortOrder = $(this).val();
paginationModelForUserPage.sortColumn = $("#sortAccordion").val();
fetchUsers();
});

// Search Functionality
$(document).on("input", "#searchUser", function () {
  paginationModelForUserPage.searchQuery = $(this).val();
  paginationModelForUserPage.pageNumber = 1;
  fetchUsers();
});

// Change Items Per Page
$(document).on("change", "#UserPerPage", function () {
  paginationModelForUserPage.pageSize = $(this).val();
  paginationModelForUserPage.pageNumber = 1;
  fetchUsers();
});

// Pagination
$(document).on("click", "#prevPageForUsersPage", function () {
    console.log("hello from prev");
  if (paginationModelForUserPage.pageNumber > 1) {
    paginationModelForUserPage.pageNumber--;
    fetchUsers();
  }
});
$(document).on("click", "#nextPageForUsersPage", function () {
  if (
    paginationModelForUserPage.pageNumber * paginationModelForUserPage.pageSize <
    paginationModelForUserPage.totalRecords
  ) {
    paginationModelForUserPage.pageNumber++;
    fetchUsers();
  }
});

//#endregion

$(document).ready(function () {
    paginationModelForUserPage.totalRecords = $(
      "#totalRecordsForUserPage"
    ).val();
    updatePaginationInfoForUserPage(paginationModelForUserPage);
  });