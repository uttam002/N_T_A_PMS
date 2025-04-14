/* -------------------------------------------------------------------------- */
/*                                 Sidebar Js                                 */
/* -------------------------------------------------------------------------- */
const defaultIcons = {

  dashboard: 'dashboard.png',
  users: 'users.png',
  roles_permissions: 'roles-permissions.png',
  menu: 'menu.png',
  section_table: 'dinner-table.png',
  taxes: 'money-bag.png',
  orders: 'clipboard.png',
  customers: 'client.png',

}
const activeIcons = {

  dashboard: 'dashboard_active.png',
  users: 'users_active.png',
  roles_permissions: 'roles-permissions_active.png',
  menu: 'menu_active.png',
  section_table: 'dinner-table_active.png',
  taxes: 'money-bag_active.png',
  orders: 'clipboard_active.png',
  customers: 'client_active.png',

};
/* ------------------------------ Sidebar hover ----------------------------- */
$(document).ready(function () {
  const currentUrl = window.location.pathname.toLowerCase();

  $('#sidebar a').each(function () {
    let linkUrl = $(this).attr('href').toLowerCase();
    let img = $(this).find('img');
    let src = img.attr('src');
    let currentPage = $(this).data('page-name');

    if (currentUrl.includes(linkUrl.split('/')[1])) {
      $(this).addClass('activepage');
      img.attr('src', src.replace(defaultIcons[currentPage], activeIcons[currentPage]));
    }
  });

});

/* ----------------------------- Sidebar Toggle ----------------------------- */

function func() {
  $("#sidebarColumn").addClass("d-none");
  $("#navbarLogo").removeClass("invisible");
}

function myfuncRev() {
  $("#sidebarColumn").removeClass("d-none");
  $("#navbarLogo").addClass("invisible");
}

/* -------------------------------------------------------------------------- */
/*                           For Password Eye Button                          */
/* -------------------------------------------------------------------------- */

function showPassword(sectionId, imageId) {
  var id1 = document.getElementById(sectionId);
  var id2 = document.getElementById(imageId);

  if (id1.type === "password") {
    id1.type = "text";
    id2.src = "../images/icons/shared-vision.png"
  } else {
    id1.type = "password";
    id2.src = "../images/icons/eye.png"
  }
}

/* -------------------------------------------------------------------------- */
/*                   Email Auto Populate To the Forgot Page                   */
/* -------------------------------------------------------------------------- */

function populateForgotPasswordLink() {
  var email = document.getElementById("loginEmail").value.trim();
  window.location.href = "/Login/ForgotPassword?forgotAction=" + btoa(email);
}


/* --------------------------------------------------- */
/*                  For Toast Message                  */
/* --------------------------------------------------- */
$(document).ready(function () {
  if (toastMessage.trim() !== "") { // Ensure there's a message
    showToast(toastMessage, toastStatus);
  }
});

function showToast(message, status) {
  var toastElement = $('#globalToast');
  var toastMessage = $('#toastMessage');

  toastMessage.text(message); // Set the message

  // Remove existing background classes before applying a new one
  toastElement.removeClass("bg-success bg-danger bg-warning bg-secondary");

  // Change color based on ResponseStatus enum
  switch (status) {
    case 'Success':
      toastElement.addClass("bg-success");
      break;
    case 'Error':
      toastElement.addClass("bg-danger");
      break;
    case 'NotFound':
      toastElement.addClass("bg-warning");
      break;
    default:
      toastElement.addClass("bg-secondary");
      break;
  }

  // Show the toast
  var toast = new bootstrap.Toast(toastElement[0]);
  toast.show();
}

/* ------------------------------- Auto Toster ------------------------------ */

function showToaster(message, status) {

  // 1 stands for error in ENUM
  if (status == 1 || status == "Error") {
    toastr.error(message, "Error", {
      positionClass: "toast-top-right",
      timeOut: 3000, // Auto close in 3 seconds
    });
  }
  // 2 stands for Success in ENUM
  else if (status == 2 || status == "Success") {
    toastr.success(message, "Success", {
      positionClass: "toast-top-right",
      timeOut: 3000,
    });
  }
  // 3 stands for Warning or NOT Found in ENUM
  else {
    toastr.warning(message, "Warning", {
      positionClass: "toast-top-right",
      timeOut: 3000,
    });
  }
}

// document.getElementById('dropzone').addEventListener('click', function () {
//     document.getElementById('fileInput').click();
// });

// document.getElementById('fileInput').addEventListener('change', function () {
//     const fileInput = this;
//     const fileDetails = document.getElementById('fileDetails');
//     const fileName = document.getElementById('fileName');
//     const imagePreview = document.getElementById('imagePreview');
//     const removeImageButton = document.getElementById('removeImageButton');

//     if (fileInput.files && fileInput.files[0]) {
//         const file = fileInput.files[0];
//         if (file.type.startsWith('image/')) {
//             fileName.textContent = file.name;
//             const reader = new FileReader();
//             reader.onload = function (e) {
//                 imagePreview.src = e.target.result;
//                 imagePreview.style.display = 'block';
//             };
//             reader.readAsDataURL(file);

//             fileDetails.classList.remove('d-none');
//         } else {
//             alert('Please select an image file.');
//             fileInput.value = '';
//             fileDetails.classList.add('d-none');
//         }
//     }
// });

// document.getElementById('removeImageButton').addEventListener('click', function () {
//     const fileInput = document.getElementById('fileInput');
//     const fileDetails = document.getElementById('fileDetails');
//     const imagePreview = document.getElementById('imagePreview');

//     fileInput.value = '';
//     fileDetails.classList.add('d-none');
//     imagePreview.style.display = 'none';
// });

$(document).on("change", ".select-all-checkbox", function () {
  let table = $(this).closest(".selectable-table");
  let rowCheckboxes = table.find(".row-checkbox");

  rowCheckboxes.prop("checked", this.checked);
  updateSelectAllState(table);
});

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
/* ======================== */
/*         Signal R         */
/* ======================== */
// // SignalR connection setup
// const connection = new signalR.HubConnectionBuilder()
//   .withUrl("/signalrhub")
//   .build();

// // Start the SignalR connection
// connection.start()
//   .then(() => {
//     console.log("SignalR connected");

//     // Dynamically join the group when the user is on the "UserPage" section
//     if (window.location.pathname.includes("/User/UserList")) {
//       connection.invoke("JoinGroup", "UserList")
//         .then(() => console.log("Joined group: UserList"))
//         .catch(err => console.error("Error joining group:", err));
//     }

//   })
//   .catch(err => console.error("SignalR connection failed:", err));


// connection.on("UserDataChanged", (action, userData) => {
//   // Publish a custom event for user data changes
//   const event = new CustomEvent("UserDataChanged", {
//     detail: { action, userData },
//   });
//   document.dispatchEvent(event);
// });

// // Leave the group when navigating away from the section
// window.addEventListener("beforeunload", () => {
//   if (window.location.pathname.includes("/User/UserList")) {
//     connection.invoke("LeaveGroup", "UserList")
//       .then(() => console.log("Left group: UserList"))
//       .catch(err => console.error("Error leaving group:", err));
//   }

// });