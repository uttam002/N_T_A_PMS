/* ----------------------- To Get Tables Via AJAX Call ----------------------- */

// function getTables(sectionId) {
//   $.ajax({
//     url: "/SectionAndTables/GetTables",
//     type: "GET",
//     data: { id: sectionId },
//     success: function (data) {
//       $("#AreaTables").html(data);
//       initialize(); // Reinitialize pagination after AJAX call
//     },
//     error: function () {
//       alert("No Tables Found in this Section");
//     },
//   });
// }

$(document).on("click",".deleteTable", function(){
  console.log("hello from here")
  let tableId = $(this).data("table-id");
  console.log(tableId);
  $("#tableIdForDelete").val(tableId);
  
});


//#region Table Pagination
let paginationModelForAreaPage = {
  pageSize: 2,
  pageNumber: 1,
  sortColumn: "id",
  sortOrder: "asc", // Changed from sortTable to sortOrder
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  tableStatus: "",
  dateRange: "",
};

function updatePaginationInfoForAreaPage() {
  let paginationDetailsElement = $("#paginationDetailsForAreaPage");

  paginationModelForAreaPage.pageSize = paginationDetailsElement.data("page-size");
  paginationModelForAreaPage.pageNumber =
    paginationDetailsElement.data("page-number");
  paginationModelForAreaPage.sortColumn =
    paginationDetailsElement.data("sort-column");
  paginationModelForAreaPage.sortOrder =
    paginationDetailsElement.data("sort-order");
  paginationModelForAreaPage.searchQuery =
    paginationDetailsElement.data("search-query");
  paginationModelForAreaPage.fromDate =
    paginationDetailsElement.data("from-date");
  paginationModelForAreaPage.toDate = paginationDetailsElement.data("to-date");
  paginationModelForAreaPage.totalRecords =
    paginationDetailsElement.data("total-records");
  console.log(paginationModelForAreaPage);
  $("#paginationInfoForAreaPage").text(
    `Showing ${
      (paginationModelForAreaPage.pageNumber - 1) * paginationModelForAreaPage.pageSize + 1
    } - ${Math.min(
      paginationModelForAreaPage.pageNumber * paginationModelForAreaPage.pageSize,
      paginationModelForAreaPage.totalRecords
    )} of ${paginationModelForAreaPage.totalRecords}`
  );
}

function fetchTables() {
  console.log("Here in fetchTables............");
  let sectionId =
    $("#sectionList .section-area.active-category").data("section-id") || null;
  console.log(sectionId);
  getTablesbySectionId(sectionId);
}

function getTablesbySectionId(sectionId) {
  console.log(sectionId);
  $.ajax({
    url: "/SectionAndTables/GetTables",
    type: "POST",
    data: { sectionId: sectionId, paginationDetails: paginationModelForAreaPage }, // Serialize the data
    success: function (response) {
      console.log("Hello here " + response);
      $("#AreaTables").html(response);
      updatePaginationInfoForAreaPage();
      // showToaster(response.message, response.status);
    },
    error: function (error) {
      console.log("Error:", error);
    },
  });
}

// Search Functionality
$(document).on("keyup", "#searchforTablesList", function () {
  paginationModelForAreaPage.searchQuery = $(this).val();
  console.log(paginationModelForAreaPage.searchQuery);
  paginationModelForAreaPage.pageNumber = 1;
  fetchTables();
});

// Change Items Per Page
$(document).on("change", "#TablePerPage", function () {
  paginationModelForAreaPage.pageSize = $(this).val();
  paginationModelForAreaPage.pageNumber = 1;
  fetchTables();
});

// Pagination
$(document).on("click", "#prevPageforTablesList", function () {
  if (paginationModelForAreaPage.pageNumber > 1) {
    paginationModelForAreaPage.pageNumber--;
    fetchTables();
  }
});
$(document).on("click", "#nextPageforTablesList", function () {
  if (
    paginationModelForAreaPage.pageNumber * paginationModelForAreaPage.pageSize <
    paginationModelForAreaPage.totalRecords
  ) {
    paginationModelForAreaPage.pageNumber++;
    fetchTables();
  }
});

//#endregion

//#region View-table-details

//#endregion

//#region  Table CRUD

function FillFormForEditTable(id, name, capacity,section,status) {
  console.log("table id: " + id);
  console.log(id + name + capacity + section + status);
  $("#tableIdForUpdate").val(id);
  $("#EditTableName").val(name);
  $("#EditTableCapacity").val(capacity);
  $("#sectionIdForUpdateTable").val(section);
  $("#tableStatusForUpdateTable").val(status);
  $("#editTableModal").modal('show'); // Show the modal
}

$(document).on("click", "#updateTableBtn", function () {
    let itemModel = {
      TableId : $("#tableIdForUpdate").val(),
      TableName : $("#EditTableName").val(),
      SectionId : $("#EditTableCapacity").val(),
      Capacity : $("#sectionIdForUpdateTable").val(),
      Status : $("#tableStatusForUpdateTable").val("Available"),
      editorId : $("#editorIdForEditSection").val(),
    } 
    $.ajax({
      url: '/SectionsAndTables/UpdateTable',
      type: "POST",
      data: {updateTable : itemModel},
      success: function (response) {
          if (response.success) {
                  $('#editTableModal').modal('hide');
                  toastr.success(response.message, 'Success', {
                      positionClass: 'toast-top-right',
                      timeOut: 3000
                  });
          } else {

              toastr.warning(response.message, 'Warning', {
                  positionClass: 'toast-top-right',
                  timeOut: 3000
              });
          }
      },
      error: function () {
          toastr.error('An error occurred while updating table.', 'Error', {
              positionClass: 'toast-top-right',
              timeOut: 3000
          });
      }
  });
});
//#endregion
$(document).ready(function () {
  let sectionIdOfFirstchild = $('#sectionList .section-area:first').data("section-id");
  updatePaginationInfoForAreaPage ();
  console.log("Hello from:-"+sectionIdOfFirstchild);
  highlightSection(sectionIdOfFirstchild);
});


