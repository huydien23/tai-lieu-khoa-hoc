// EmailJS Configuration
document.addEventListener('DOMContentLoaded', function () {
    if (typeof window['emailjs'] !== 'undefined') {
        window['emailjs'].init('Ig3KrQ7KfcaIKloR8');
        console.log('EmailJS initialized successfully');
    } else {
        console.error('EmailJS library not loaded');
        if (typeof toastr !== 'undefined') {
            toastr.error('Không thể tải EmailJS. Vui lòng kiểm tra kết nối hoặc thử lại sau.');
        }
    }
});

// Function gửi cảnh báo tài liệu quá hạn qua email
window.sendOverdueWarning = function (maPhieu, email, hoTen, tenTaiLieu, soNgayQuaHan) {
    if (!email || email.trim() === '') {
        if (typeof toastr !== 'undefined') {
            toastr.error('Không có email để gửi cảnh báo!');
        }
        return;
    }

    // Kiểm tra EmailJS đã sẵn sàng
    if (typeof window['emailjs'] === 'undefined') {
        if (typeof toastr !== 'undefined') {
            toastr.error('EmailJS chưa được load. Vui lòng tải lại trang!');
        }
        console.error('EmailJS is not defined');
        return;
    }

    // Hiển thị loading
    const button = event.target.closest('button');
    const originalText = button.innerHTML;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Đang gửi...';
    button.disabled = true;

    // Template email cảnh báo
    const templateParams = {
        to_email: email,
        to_name: hoTen,
        document_name: tenTaiLieu,
        overdue_days: soNgayQuaHan,
        current_date: new Date().toLocaleDateString('vi-VN'),
        library_name: 'Thư viện Đại học Nam Cần Thơ',
        contact_email: 'thuvien@nct.edu.vn',
        contact_phone: '0292 123 4567'
    };

    // Gửi email sử dụng EmailJS
    window['emailjs'].send('service_kq2fref', 'template_l8t2t4k', templateParams)
        .then(function (response) {
            console.log('Email sent successfully:', response);
            if (typeof toastr !== 'undefined') {
                toastr.success('Đã gửi cảnh báo thành công!');
            }
            
            // Ghi log gửi cảnh báo
            logWarningSent(maPhieu, email, hoTen, tenTaiLieu, soNgayQuaHan);
        })
        .catch(function (error) {
            console.error('Email sending failed:', error);
            if (typeof toastr !== 'undefined') {
                toastr.error('Gửi cảnh báo thất bại. Vui lòng thử lại!');
            }
        })
        .finally(function () {
            // Khôi phục button
            button.innerHTML = originalText;
            button.disabled = false;
        });
};

// Function ghi log gửi cảnh báo
function logWarningSent(maPhieu, email, hoTen, tenTaiLieu, soNgayQuaHan) {
    const logData = {
        maPhieu: maPhieu,
        email: email,
        hoTen: hoTen,
        tenTaiLieu: tenTaiLieu,
        soNgayQuaHan: soNgayQuaHan,
        ngayGui: new Date().toISOString()
    };

    // Có thể lưu vào localStorage hoặc gửi lên server
    let warningLogs = JSON.parse(localStorage.getItem('overdueWarningLogs') || '[]');
    warningLogs.push(logData);
    localStorage.setItem('overdueWarningLogs', JSON.stringify(warningLogs));
} 