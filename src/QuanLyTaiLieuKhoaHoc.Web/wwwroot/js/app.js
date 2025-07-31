// ===================================
// HỆ THỐNG QUẢN LÝ TÀI LIỆU KHOA HỌC
// Custom JavaScript
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

  // Loading overlay for forms
  $("form").on("submit", function () {
    showLoadingOverlay();
  });

  // Search suggestions (placeholder - to be implemented with backend)
  $("#quickSearch").on("input", function () {
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

// Enhanced Document Management System JavaScript

$(document).ready(function() {
    // Initialize all enhanced features
    initializeViewSwitcher();
    initializeSmoothScrolling();
    initializeFloatingActionButton();
    initializeSearchEnhancements();
    initializeCardAnimations();
    initializeLazyLoading();
    initializeInfiniteScroll();
    initializeEnhancedSearchForm();
});

// View Switcher for Grid/List/Masonry
function initializeViewSwitcher() {
    const gridView = $('#gridView');
    const listView = $('#listView');
    const masonryView = $('#masonryView');
    const documentsContainer = $('#documentsContainer');

    gridView.on('click', function() {
        switchView('grid');
        updateActiveButton(this);
    });

    listView.on('click', function() {
        switchView('list');
        updateActiveButton(this);
    });

    masonryView.on('click', function() {
        switchView('masonry');
        updateActiveButton(this);
    });

    function switchView(viewType) {
        documentsContainer.removeClass('documents-grid-modern documents-list-view documents-grid-masonry');
        
        switch(viewType) {
            case 'grid':
                documentsContainer.addClass('documents-grid-modern');
                break;
            case 'list':
                documentsContainer.addClass('documents-list-view');
                break;
            case 'masonry':
                documentsContainer.addClass('documents-grid-masonry');
                initializeMasonry();
                break;
        }
    }

    function updateActiveButton(activeButton) {
        $('.btn-view').removeClass('active');
        $(activeButton).addClass('active');
    }
}

// Masonry Layout Initialization
function initializeMasonry() {
    if (typeof Masonry !== 'undefined') {
        const grid = document.querySelector('.documents-grid-masonry');
        if (grid) {
            new Masonry(grid, {
                itemSelector: '.document-card-master',
                columnWidth: '.document-card-master',
                percentPosition: true,
                gutter: 24
            });
        }
    }
}

// Smooth Scrolling
function initializeSmoothScrolling() {
    $('a[href^="#"]').on('click', function(e) {
        e.preventDefault();
        const target = $(this.getAttribute('href'));
        if (target.length) {
            $('html, body').animate({
                scrollTop: target.offset().top - 100
            }, 800, 'easeInOutQuart');
        }
    });
}

// Floating Action Button
function initializeFloatingActionButton() {
    const fabMain = $('#fabMain');
    const fabOptions = $('.fab-options');

    // Show/hide FAB on scroll
    let lastScrollTop = 0;
    $(window).scroll(function() {
        const scrollTop = $(this).scrollTop();
        if (scrollTop > lastScrollTop && scrollTop > 200) {
            fabMain.addClass('fab-hidden');
        } else {
            fabMain.removeClass('fab-hidden');
        }
        lastScrollTop = scrollTop;
    });

    // FAB option click handlers
    $('.fab-option').on('click', function() {
        const action = $(this).attr('title');
        handleFabAction(action);
    });
}

function handleFabAction(action) {
    switch(action) {
        case 'Tìm kiếm nâng cao':
            $('#search-section').get(0).scrollIntoView({ behavior: 'smooth' });
            $('#timKiem').focus();
            break;
        case 'Lọc nhanh':
            showQuickFilterModal();
            break;
        case 'Sắp xếp':
            showSortOptions();
            break;
    }
}

// Search Enhancements
function initializeSearchEnhancements() {
    const searchInput = $('#timKiem');
    let searchTimeout;

    // Auto-complete suggestions
    searchInput.on('input', function() {
        clearTimeout(searchTimeout);
        const query = $(this).val();
        
        if (query.length >= 2) {
            searchTimeout = setTimeout(() => {
                fetchSearchSuggestions(query);
            }, 300);
        } else {
            hideSearchSuggestions();
        }
    });

    // Search form enhancement
    $('#searchForm').on('submit', function(e) {
        e.preventDefault();
        performEnhancedSearch();
    });
}

function fetchSearchSuggestions(query) {
    // Simulate API call for search suggestions
    const suggestions = [
        'Bài báo khoa học',
        'Giáo trình',
        'Tài liệu tham khảo',
        'Đề tài nghiên cứu',
        'Báo cáo đồ án'
    ].filter(item => item.toLowerCase().includes(query.toLowerCase()));

    showSearchSuggestions(suggestions);
}

function showSearchSuggestions(suggestions) {
    let suggestionsHtml = '';
    suggestions.forEach(suggestion => {
        suggestionsHtml += `<div class="suggestion-item" data-suggestion="${suggestion}">${suggestion}</div>`;
    });

    if (suggestions.length > 0) {
        $('#searchSuggestions').html(suggestionsHtml).show();
    }
}

function hideSearchSuggestions() {
    $('#searchSuggestions').hide();
}

function performEnhancedSearch() {
    // Show loading state
    showSearchLoading();
    
    // Simulate search delay
    setTimeout(() => {
        hideSearchLoading();
        // Form will submit normally
        $('#searchForm')[0].submit();
    }, 500);
}

function showSearchLoading() {
    $('#searchButton').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Đang tìm...');
}

function hideSearchLoading() {
    $('#searchButton').prop('disabled', false).html('<i class="fas fa-search me-2"></i>Tìm');
}

// Card Animations
function initializeCardAnimations() {
    // Intersection Observer for card animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('card-animate-in');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    // Observe all document cards
    document.querySelectorAll('.document-card-master').forEach(card => {
        observer.observe(card);
    });

    // Hover effects
    $('.document-card-master').on('mouseenter', function() {
        $(this).addClass('card-hover');
    }).on('mouseleave', function() {
        $(this).removeClass('card-hover');
    });
}

// Lazy Loading
function initializeLazyLoading() {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    document.querySelectorAll('img[data-src]').forEach(img => {
        imageObserver.observe(img);
    });
}

// Infinite Scroll
function initializeInfiniteScroll() {
    let isLoading = false;
    let currentPage = 1;

    $(window).scroll(function() {
        if ($(window).scrollTop() + $(window).height() >= $(document).height() - 100) {
            if (!isLoading) {
                loadMoreDocuments();
            }
        }
    });
}

function loadMoreDocuments() {
    isLoading = true;
    currentPage++;

    // Show loading indicator
    const loadingHtml = `
        <div class="col-12 text-center py-4">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Đang tải...</span>
            </div>
        </div>
    `;
    $('#documentsContainer .row').append(loadingHtml);

    // Simulate API call
    setTimeout(() => {
        // Remove loading indicator
        $('#documentsContainer .spinner-border').parent().remove();
        
        // Add new documents (this would be replaced with actual API call)
        // loadNewDocuments(currentPage);
        
        isLoading = false;
    }, 1000);
}

// Quick Filter Modal
function showQuickFilterModal() {
    const modalHtml = `
        <div class="modal fade" id="quickFilterModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Lọc nhanh</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-6">
                                <h6>Chuyên ngành</h6>
                                <div class="filter-chips">
                                    <span class="filter-chip" data-filter="cntt">Công nghệ thông tin</span>
                                    <span class="filter-chip" data-filter="kt">Kinh tế</span>
                                    <span class="filter-chip" data-filter="nn">Nông nghiệp</span>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <h6>Loại tài liệu</h6>
                                <div class="filter-chips">
                                    <span class="filter-chip" data-filter="bb">Bài báo</span>
                                    <span class="filter-chip" data-filter="gt">Giáo trình</span>
                                    <span class="filter-chip" data-filter="tk">Tài liệu tham khảo</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                        <button type="button" class="btn btn-primary" onclick="applyQuickFilters()">Áp dụng</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    if ($('#quickFilterModal').length === 0) {
        $('body').append(modalHtml);
    }
    
    new bootstrap.Modal(document.getElementById('quickFilterModal')).show();
}

// Sort Options
function showSortOptions() {
    const sortOptions = [
        { value: 'newest', label: 'Mới nhất', icon: 'fas fa-clock' },
        { value: 'popular', label: 'Phổ biến nhất', icon: 'fas fa-fire' },
        { value: 'rating', label: 'Đánh giá cao nhất', icon: 'fas fa-star' },
        { value: 'title', label: 'Theo tên A-Z', icon: 'fas fa-sort-alpha-down' }
    ];

    let sortHtml = '<div class="sort-dropdown">';
    sortOptions.forEach(option => {
        sortHtml += `
            <div class="sort-option" data-sort="${option.value}">
                <i class="${option.icon} me-2"></i>
                ${option.label}
            </div>
        `;
    });
    sortHtml += '</div>';

    // Show sort dropdown
    showDropdown(sortHtml, $('#fabMain'));
}

function showDropdown(content, target) {
    const dropdown = $(`<div class="custom-dropdown">${content}</div>`);
    $('body').append(dropdown);
    
    const targetPos = target.offset();
    dropdown.css({
        position: 'absolute',
        top: targetPos.top - dropdown.outerHeight() - 10,
        right: targetPos.left + target.outerWidth() - dropdown.outerWidth(),
        zIndex: 1001
    });

    // Close dropdown when clicking outside
    $(document).on('click.dropdown', function(e) {
        if (!dropdown.is(e.target) && dropdown.has(e.target).length === 0) {
            dropdown.remove();
            $(document).off('click.dropdown');
        }
    });
}

// Enhanced Search Form Initialization
function initializeEnhancedSearchForm() {
    const searchInput = $('#timKiem');
    const clearButton = $('#clearSearch');
    const toggleFiltersBtn = $('#toggleFilters');
    const additionalFilters = $('#additionalFilters');
    const activeFilters = $('#activeFilters');
    const clearAllFiltersBtn = $('#clearAllFilters');

    // Clear search input
    clearButton.on('click', function() {
        searchInput.val('').focus();
        updateClearButton();
        hideSearchSuggestions();
    });

    // Show/hide clear button based on input
    searchInput.on('input', function() {
        updateClearButton();
    });

    // Toggle additional filters
    toggleFiltersBtn.on('click', function() {
        const isVisible = additionalFilters.is(':visible');
        if (isVisible) {
            additionalFilters.slideUp(300);
            toggleFiltersBtn.removeClass('active');
        } else {
            additionalFilters.slideDown(300);
            toggleFiltersBtn.addClass('active');
        }
    });

    // Handle filter changes
    $('.filter-select, .filter-input').on('change input', function() {
        updateActiveFilters();
    });

    // Clear all filters
    clearAllFiltersBtn.on('click', function() {
        $('.filter-select').val('');
        $('.filter-input').val('');
        updateActiveFilters();
    });

    // Initialize
    updateClearButton();
    updateActiveFilters();
}

function updateClearButton() {
    const searchInput = $('#timKiem');
    const clearButton = $('#clearSearch');
    
    if (searchInput.val().length > 0) {
        clearButton.addClass('show');
    } else {
        clearButton.removeClass('show');
    }
}

function updateActiveFilters() {
    const activeFilters = $('#activeFilters');
    const activeFilterTags = $('#activeFilterTags');
    const filters = [];
    
    // Collect active filters
    $('.filter-select').each(function() {
        const select = $(this);
        const value = select.val();
        const label = select.find('option:selected').text();
        
        if (value && value !== '') {
            filters.push({
                name: select.attr('name'),
                value: value,
                label: label,
                type: 'select'
            });
        }
    });
    
    $('.filter-input').each(function() {
        const input = $(this);
        const value = input.val().trim();
        
        if (value) {
            filters.push({
                name: input.attr('name'),
                value: value,
                label: value,
                type: 'input'
            });
        }
    });
    
    // Update active filters display
    if (filters.length > 0) {
        let tagsHtml = '';
        filters.forEach(filter => {
            tagsHtml += `
                <span class="active-filter-tag">
                    ${filter.label}
                    <button type="button" class="remove-tag" data-filter="${filter.name}">
                        <i class="fas fa-times"></i>
                    </button>
                </span>
            `;
        });
        
        activeFilterTags.html(tagsHtml);
        activeFilters.show();
        
        // Handle remove tag clicks
        $('.remove-tag').on('click', function() {
            const filterName = $(this).data('filter');
            $(`[name="${filterName}"]`).val('').trigger('change');
        });
    } else {
        activeFilters.hide();
    }
}

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

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Enhanced search with debouncing
const debouncedSearch = debounce(function(query) {
    // Perform search logic here
    console.log('Searching for:', query);
}, 300);

// Add CSS for new components
const additionalCSS = `
    .card-animate-in {
        animation: slideInUp 0.6s ease-out;
    }
    
    .card-hover {
        transform: translateY(-8px) scale(1.02);
    }
    
    .fab-hidden {
        transform: translateY(100px);
        opacity: 0;
    }
    
    .suggestion-item {
        padding: 8px 16px;
        cursor: pointer;
        border-bottom: 1px solid #eee;
    }
    
    .suggestion-item:hover {
        background: #f8f9fa;
    }
    
    .filter-chip {
        display: inline-block;
        padding: 6px 12px;
        margin: 4px;
        background: #e9ecef;
        border-radius: 20px;
        cursor: pointer;
        transition: all 0.2s;
    }
    
    .filter-chip:hover {
        background: #007bff;
        color: white;
    }
    
    .sort-dropdown {
        background: white;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        overflow: hidden;
    }
    
    .sort-option {
        padding: 12px 16px;
        cursor: pointer;
        transition: background 0.2s;
    }
    
    .sort-option:hover {
        background: #f8f9fa;
    }
    
    @keyframes slideInUp {
        from {
            opacity: 0;
            transform: translateY(30px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
`;

// Inject additional CSS
const style = document.createElement('style');
style.textContent = additionalCSS;
document.head.appendChild(style);

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
