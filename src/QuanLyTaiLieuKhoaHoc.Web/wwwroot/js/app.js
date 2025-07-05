// ===================================
// HỆ THỐNG QUẢN LÝ TÀI LIỆU KHOA HỌC
// Enhanced Custom JavaScript
// ===================================

$(document).ready(function () {
  // Initialize tooltips
  var tooltipTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
  );
  var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
  });

  // Initialize popovers
  var popoverTriggerList = [].slice.call(
    document.querySelectorAll('[data-bs-toggle="popover"]')
  );
  var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl);
  });

  // Enhanced Navbar Scroll Effect
  let lastScrollTop = 0;
  $(window).scroll(function () {
    const currentScroll = $(this).scrollTop();
    const navbar = $(".navbar-custom");

    if (currentScroll > 100) {
      navbar.addClass("scrolled");
    } else {
      navbar.removeClass("scrolled");
    }

    // Hide/show navbar on scroll
    if (currentScroll > lastScrollTop && currentScroll > 200) {
      navbar.css("transform", "translateY(-100%)");
    } else {
      navbar.css("transform", "translateY(0)");
    }
    lastScrollTop = currentScroll;
  });

  // Card Hover Effects with 3D Transform
  $(".floating-card").hover(
    function () {
      $(this).css("transform", "translateY(-12px) rotateX(5deg) rotateY(5deg)");
    },
    function () {
      $(this).css("transform", "translateY(0) rotateX(0) rotateY(0)");
    }
  );

  // Enhanced Loading Animation
  $(".btn").on("click", function () {
    const btn = $(this);
    const originalText = btn.html();

    if (!btn.hasClass("no-loading")) {
      btn.html('<i class="fas fa-spinner fa-spin me-2"></i>Đang xử lý...');
      btn.prop("disabled", true);

      setTimeout(() => {
        btn.html(originalText);
        btn.prop("disabled", false);
      }, 2000);
    }
  });

  // Auto-hide alerts after 5 seconds
  setTimeout(function () {
    $(".alert").fadeOut("slow");
  }, 5000);

  // Smooth scrolling for anchor links
  $('a[href^="#"]').on("click", function (event) {
    var target = $(this.getAttribute("href"));
    if (target.length) {
      event.preventDefault();
      $("html, body")
        .stop()
        .animate(
          {
            scrollTop: target.offset().top - 100,
          },
          1000
        );
    }
  });

  // Enhanced Form Validation
  $("form").on("submit", function (e) {
    const form = $(this);
    let isValid = true;

    // Check required fields
    form.find("[required]").each(function () {
      const field = $(this);
      if (!field.val().trim()) {
        field.addClass("is-invalid");
        isValid = false;
      } else {
        field.removeClass("is-invalid");
      }
    });

    if (!isValid) {
      e.preventDefault();
      showToast("Vui lòng điền đầy đủ thông tin bắt buộc!", "error");
    }
  });

  // Real-time Search Suggestions
  let searchTimeout;
  $('input[name="query"]').on("input", function () {
    var query = $(this).val();
    if (query.length >= 2) {
      // TODO: Implement search suggestions
      console.log("Searching for: " + query);
    }
  });

  // Table row hover effects
  $(".table-hover tbody tr").hover(
    function () {
      $(this).addClass("table-active");
    },
    function () {
      $(this).removeClass("table-active");
    }
  );

  // Confirm delete actions
  $(".btn-delete, .delete-confirm").on("click", function (e) {
    e.preventDefault();
    var title = $(this).data("title") || "Xác nhận xóa";
    var message =
      $(this).data("message") || "Bạn có chắc chắn muốn xóa mục này không?";
    var url = $(this).attr("href") || $(this).data("url");

    showConfirmDialog(title, message, function () {
      if (url) {
        window.location.href = url;
      }
    });
  });

  // Number formatting
  $(".format-number").each(function () {
    var number = parseInt($(this).text());
    $(this).text(number.toLocaleString("vi-VN"));
  });

  // Date formatting
  $(".format-date").each(function () {
    var dateStr = $(this).text();
    if (dateStr) {
      var date = new Date(dateStr);
      $(this).text(date.toLocaleDateString("vi-VN"));
    }
  });

  // Copy to clipboard functionality
  $(".copy-to-clipboard").on("click", function () {
    var text = $(this).data("text") || $(this).text();
    navigator.clipboard.writeText(text).then(function () {
      showToast("Đã sao chép vào clipboard!", "success");
    });
  });

  // Auto-resize textareas
  $("textarea.auto-resize")
    .each(function () {
      this.style.height = "auto";
      this.style.height = this.scrollHeight + "px";
    })
    .on("input", function () {
      this.style.height = "auto";
      this.style.height = this.scrollHeight + "px";
    });

  // Form validation styling
  $("form").on("submit", function () {
    $(this).find(".is-invalid").removeClass("is-invalid");
    $(this).find(".is-valid").removeClass("is-valid");
  });

  // Real-time form validation (basic)
  $("input[required], select[required], textarea[required]").on(
    "blur",
    function () {
      if ($(this).val()) {
        $(this).removeClass("is-invalid").addClass("is-valid");
      } else {
        $(this).removeClass("is-valid").addClass("is-invalid");
      }
    }
  );

  // File upload preview
  $('input[type="file"]').on("change", function () {
    var input = this;
    var preview = $(input).closest(".form-group").find(".file-preview");

    if (input.files && input.files[0]) {
      var reader = new FileReader();
      var fileName = input.files[0].name;
      var fileSize = (input.files[0].size / 1024 / 1024).toFixed(2);

      reader.onload = function (e) {
        if (preview.length) {
          preview.html(`
                        <div class="alert alert-info">
                            <i class="fas fa-file me-2"></i>
                            <strong>${fileName}</strong> (${fileSize} MB)
                        </div>
                    `);
        }
      };

      reader.readAsDataURL(input.files[0]);
    }
  });

  // Search filters toggle
  $(".search-filters-toggle").on("click", function () {
    $(".search-filters").slideToggle();
    var icon = $(this).find("i");
    icon.toggleClass("fa-chevron-down fa-chevron-up");
  });

  // Table sorting (basic)
  $(".sortable th").on("click", function () {
    var table = $(this).closest("table");
    var column = $(this).index();
    var rows = table.find("tbody tr").toArray();
    var isAsc = $(this).hasClass("sort-asc");

    // Remove all sort classes
    $(this).siblings().removeClass("sort-asc sort-desc");

    // Add appropriate class
    $(this).toggleClass("sort-asc sort-desc");

    rows.sort(function (a, b) {
      var aVal = $(a).children().eq(column).text();
      var bVal = $(b).children().eq(column).text();

      if (isNumeric(aVal) && isNumeric(bVal)) {
        return isAsc
          ? parseFloat(aVal) - parseFloat(bVal)
          : parseFloat(bVal) - parseFloat(aVal);
      } else {
        return isAsc ? aVal.localeCompare(bVal) : bVal.localeCompare(aVal);
      }
    });

    table.find("tbody").empty().append(rows);
  });
});

// Utility Functions
function showLoadingOverlay() {
  if ($("#loadingOverlay").length === 0) {
    $("body").append(`
            <div id="loadingOverlay" class="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center" style="background: rgba(0,0,0,0.5); z-index: 9999;">
                <div class="text-center text-white">
                    <div class="spinner-custom mx-auto mb-3"></div>
                    <div>Đang xử lý...</div>
                </div>
            </div>
        `);
  }
}

function hideLoadingOverlay() {
  $("#loadingOverlay").remove();
}

function showToast(message, type = "info", duration = 3000) {
  var toastClass = "bg-primary";
  var icon = "fas fa-info-circle";

  switch (type) {
    case "success":
      toastClass = "bg-success";
      icon = "fas fa-check-circle";
      break;
    case "error":
    case "danger":
      toastClass = "bg-danger";
      icon = "fas fa-exclamation-triangle";
      break;
    case "warning":
      toastClass = "bg-warning";
      icon = "fas fa-exclamation-circle";
      break;
  }

  var toast = $(`
        <div class="toast align-items-center text-white ${toastClass} border-0 position-fixed" style="top: 20px; right: 20px; z-index: 10000;" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="${icon} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `);

  $("body").append(toast);
  var toastBootstrap = new bootstrap.Toast(toast[0], { delay: duration });
  toastBootstrap.show();

  toast.on("hidden.bs.toast", function () {
    $(this).remove();
  });
}

function showConfirmDialog(title, message, onConfirm, onCancel) {
  var modal = $(`
        <div class="modal fade" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">${title}</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <p>${message}</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                        <button type="button" class="btn btn-danger" id="confirmBtn">Xác nhận</button>
                    </div>
                </div>
            </div>
        </div>
    `);

  $("body").append(modal);
  var modalBootstrap = new bootstrap.Modal(modal[0]);
  modalBootstrap.show();

  modal.find("#confirmBtn").on("click", function () {
    modalBootstrap.hide();
    if (onConfirm) onConfirm();
  });

  modal.on("hidden.bs.modal", function () {
    $(this).remove();
    if (onCancel) onCancel();
  });
}

function isNumeric(str) {
  return !isNaN(str) && !isNaN(parseFloat(str));
}

function formatNumber(num) {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function formatDate(dateStr, format = "vi-VN") {
  var date = new Date(dateStr);
  return date.toLocaleDateString(format);
}

function formatDateTime(dateStr, format = "vi-VN") {
  var date = new Date(dateStr);
  return date.toLocaleString(format);
}

function debounce(func, wait, immediate) {
  var timeout;
  return function () {
    var context = this,
      args = arguments;
    var later = function () {
      timeout = null;
      if (!immediate) func.apply(context, args);
    };
    var callNow = immediate && !timeout;
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
    if (callNow) func.apply(context, args);
  };
}

// Export for global use
window.AppUtils = {
  showLoadingOverlay,
  hideLoadingOverlay,
  showToast,
  showConfirmDialog,
  formatNumber,
  formatDate,
  formatDateTime,
  debounce,
};
