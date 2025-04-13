document.addEventListener("DOMContentLoaded", () => {
  document.body.style.visibility = "visible";
});
let loaderStartTime = null;
const MIN_LOADER_DURATION = 300;

// Show the loader
function showLoader() {
  loaderStartTime = Date.now();
  document.body.classList.add("loading");
}

// Hide the loader after MIN_LOADER_DURATION
function hideLoader() {
  const elapsed = Date.now() - loaderStartTime;
  const delay = Math.max(MIN_LOADER_DURATION - elapsed, 0);

  setTimeout(() => {
    document.body.classList.remove("loading");
  }, delay);
}

// Show loader as soon as possible (even before DOM is ready)
showLoader();

// Wait until window fully loads, including RenderBody and images/scripts
window.addEventListener("load", () => {
  hideLoader();
});


// Also show loader on sidebar navigation
$(document).ready(() => {
  $(document).ajaxStart(function () {
    showLoader();  // Show loader
  });

  // Hide the loader when the AJAX request completes (success or error)
  $(document).ajaxStop(function () {
    hideLoader(); // Hide loader
  });

  // Optional: Hide the loader if there's an AJAX error
  $(document).ajaxError(function () {
    hideLoader();  // Hide loader on error
  });
});

