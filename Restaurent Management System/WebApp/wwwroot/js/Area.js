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
const editorId = $("#editorIdAtTableAndSections").val();
$(document).on("click",".deleteTable", function(){
  console.log("hello from here")
  let tableId = $(this).data("table-id");
  console.log(tableId);
  $("#tableIdForDelete").val(tableId);
  
});
const sectionDetails = {
  sectionId: 0,
  sectionName: "",
  Description: "",
  editorId: 0,

}
// const tableDetails = {
//   tableId: 0,
//   tableName: "",
//   capacity: "",
//   sectionId: 0,
//   status: "",
//   editorId: 0,
// }
//#region Section 

$(document).on("click", "#addSectionBtn", function (event) {
  event.preventDefault();
  let form = $("#AddSectionForm");
  if(!form.valid()){
    return;
  }
  sectionDetails.sectionName = $("#sectionName").val()?.trim();
  sectionDetails.Description = $("#sectionDescription").val()?.trim() || null;
  sectionDetails.sectionId = 0;
  sectionDetails.editorId = editorId; 

  $.ajax({
    url: "/SectionAndTables/AddSection",
    type: "POST",
    data: {section:sectionDetails},
    success: function (response) {
      showToaster(response.message, response.status);
      $("#sectionList").html(response.partialView);
      $("#addSectionModal").modal("hide");
    },
    error: function () {
      showToaster("An error occurred while updating section.", "Error");
      $("#addSectionModal").modal("hide");
    },
  });
});


$(document).on("click", "#updateSectionBtn", function (event) {
  event.preventDefault();
  let form = $("#EditSectionForm");
  if(!form.valid()){
    return;
  }
  sectionDetails.sectionId = $("#sectionIdForEdit").val();
  sectionDetails.sectionName = $("#EditSectionName").val()?.trim();
  sectionDetails.Description = $("#EditSectionDescription").val()?.trim() || null;
  sectionDetails.editorId = editorId; 

  $.ajax({
    url: "/SectionAndTables/EditSection",
    type: "POST",
    data: {section:sectionDetails},
    success: function (response) {
      showToaster(response.message, response.status);
      $("#sectionList").html(response.partialView);
      $("#EditSectionModal").modal("hide");
    },
    error: function () {
      showToaster("An error occurred while updating section.", "Error");
      $("#EditSectionModal").modal("hide");
    },
  });
});

$(document).on("click", "#deleteSectionBtn", function (event) {
  event.preventDefault();
  sectionDetails.sectionId = $("#sectionIdForDelete").val();
  sectionDetails.editorId = editorId; 
  $.ajax({
    url: "/SectionAndTables/DeleteSection",
    type: "POST",
    data: {section:sectionDetails},
    success: function (response) {
      showToaster(response.message, response.status);
      $("#sectionList").html(response.partialView);
      highlightFirstSection();
      $("#deleteSectionModal").modal("hide");
    },
    error: function () {
      showToaster("An error occurred while deleting section.", "Error");
      $("#deleteSectionModal").modal("hide");
    },
  });
}
);
//#endregion
//#region Table Pagination
const paginationModelForAreaPage = {
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

function updatePaginationInfoForAreaPage(pagination) {
  paginationModelForAreaPage.totalRecords =pagination.totalRecords;

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
  highlightSection(sectionId);
  $.ajax({
    url: "/SectionAndTables/GetTables",
    type: "POST",
    data: { sectionId: sectionId, paginationDetails: paginationModelForAreaPage }, // Serialize the data
    success: function (response) {
      $("#AreaTables").html(response.partialView);
      updatePaginationInfoForAreaPage(response.pagination);
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster(
        "An error occurred while fetching tables. Please try again.",
        "Error"
      );
    },
  });
}

// Search Functionality
$(document).on("keyup", "#searchforTablesList", function () {
  paginationModelForAreaPage.searchQuery = $(this).val();
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
                 showToater(response.message, response.status);

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

function AssignValueForSection(id) {
  console.log("section id: " + id);
  $("#sectionIdForDelete").val(id);
}

function FillFormForEditSection(id) {
  console.log("section id: " + id);
  $("#sectionIdForEdit").val(id);
  $("#EditSectionName").val(name);
  $("#EditSectionDescription").val(desc);
}

function highlightSection(id) {
  // Remove highlight from all categories
  $('.section-area').removeClass('active-category');
  $('.showBtnsAtSections').addClass('d-none');

  // Add highlight to the selected section
  $(`#section-${id}`).addClass('active-category');
  $(`#sectionBtns-${id}`).removeClass('d-none');
}
function highlightFirstSection() {
  let firstSection = $('#sectionList .section-area:first');
  if (firstSection.length) {
    let sectionId = firstSection.data('section-id');
    highlightSection(sectionId);
  }
}
$(document).ready(function () {
  paginationModelForAreaPage.totalRecords = $("#AreaTables .selectable-row").length;
  updatePaginationInfoForAreaPage (paginationModelForAreaPage);
  highlightFirstSection();
});


