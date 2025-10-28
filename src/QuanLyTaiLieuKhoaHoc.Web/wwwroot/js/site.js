window.openTraPhieuModal = function(maPhieu) {
    $('#traPhieuModal').remove();
    $('body').append('<div class="modal fade" id="traPhieuModal" tabindex="-1" aria-labelledby="traPhieuModalLabel" aria-hidden="true"><div class="modal-dialog"><div class="modal-content"></div></div></div>');
    $.get('/PhieuMuonTra/LapPhieuTra', { maPhieu: maPhieu }, function(html) {
        $('#traPhieuModal .modal-content').html(html);
        $('#traPhieuModal').modal('show');
    });
};

// Submit form trả tài liệu qua AJAX
$(document).on('submit', '#lapPhieuTraForm', function(e) {
    e.preventDefault();
    var form = $(this);
    var formData = form.serialize();
    var submitBtn = form.find('button[type="submit"]');
    var originalText = submitBtn.html();
    submitBtn.html('<i class="fas fa-spinner fa-spin me-1"></i>Đang xử lý...');
    submitBtn.prop('disabled', true);
    $.post('/PhieuMuonTra/LapPhieuTra', formData, function(response) {
        if (response.success) {
            toastr.success(response.message);
            $('#traPhieuModal').modal('hide');
            setTimeout(function() { location.reload(); }, 1000);
        } else {
            toastr.error(response.message);
        }
    }).fail(function() {
        toastr.error('Có lỗi xảy ra khi trả tài liệu!');
    }).always(function() {
        submitBtn.html(originalText);
        submitBtn.prop('disabled', false);
    });
});

// Reset nội dung modal khi đóng để tránh form cũ còn tồn tại
$(document).on('hidden.bs.modal', '#traPhieuModal', function () {
    $(this).find('.modal-content').html('');
});
