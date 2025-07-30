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

// Homepage Animations and Interactions
document.addEventListener('DOMContentLoaded', function() {
    // Animate elements on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);

    // Observe elements for animation
    const animateElements = document.querySelectorAll('.document-card, .stats-card, .search-container');
    animateElements.forEach(el => {
        observer.observe(el);
    });

    // Voice search functionality
    const voiceSearchBtn = document.querySelector('.voice-search-btn');
    if (voiceSearchBtn) {
        voiceSearchBtn.addEventListener('click', function() {
            if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
                const recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
                recognition.lang = 'vi-VN';
                recognition.continuous = false;
                recognition.interimResults = false;

                recognition.onstart = function() {
                    voiceSearchBtn.innerHTML = '<i class="fas fa-microphone-slash"></i>';
                    voiceSearchBtn.style.background = '#EF4444';
                    voiceSearchBtn.style.color = 'white';
                };

                recognition.onresult = function(event) {
                    const transcript = event.results[0][0].transcript;
                    document.querySelector('.search-input').value = transcript;
                };

                recognition.onend = function() {
                    voiceSearchBtn.innerHTML = '<i class="fas fa-microphone"></i>';
                    voiceSearchBtn.style.background = '';
                    voiceSearchBtn.style.color = '';
                };

                recognition.start();
            } else {
                alert('Trình duyệt của bạn không hỗ trợ tìm kiếm bằng giọng nói');
            }
        });
    }

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Parallax effect for hero section
    const heroSection = document.querySelector('.hero-section');
    if (heroSection) {
        window.addEventListener('scroll', function() {
            const scrolled = window.pageYOffset;
            const rate = scrolled * -0.5;
            heroSection.style.transform = `translateY(${rate}px)`;
        });
    }

    // Animate statistics numbers
    const statNumbers = document.querySelectorAll('.stat-number');
    statNumbers.forEach(stat => {
        const finalNumber = parseInt(stat.textContent);
        let currentNumber = 0;
        const increment = finalNumber / 50;
        
        const timer = setInterval(() => {
            currentNumber += increment;
            if (currentNumber >= finalNumber) {
                currentNumber = finalNumber;
                clearInterval(timer);
            }
            stat.textContent = Math.floor(currentNumber);
        }, 30);
    });

    // Add hover effects to keyword tags
    const keywordTags = document.querySelectorAll('.keyword-tag');
    keywordTags.forEach(tag => {
        tag.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px) scale(1.05)';
        });
        
        tag.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Document card hover effects
    const documentCards = document.querySelectorAll('.document-card');
    documentCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-8px) rotateX(5deg)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) rotateX(0)';
        });
    });

    // Search input focus effects
    const searchInput = document.querySelector('.search-input');
    if (searchInput) {
        searchInput.addEventListener('focus', function() {
            this.parentElement.style.transform = 'scale(1.02)';
        });
        
        searchInput.addEventListener('blur', function() {
            this.parentElement.style.transform = 'scale(1)';
        });
    }

    // Add loading skeleton animation
    function addLoadingSkeleton() {
        const skeletonElements = document.querySelectorAll('.loading-skeleton');
        skeletonElements.forEach(el => {
            el.classList.add('loading');
        });
    }

    // Remove loading skeleton when content is loaded
    window.addEventListener('load', function() {
        const skeletonElements = document.querySelectorAll('.loading-skeleton');
        skeletonElements.forEach(el => {
            el.classList.remove('loading');
        });
    });

    // Add smooth transitions for all interactive elements
    const interactiveElements = document.querySelectorAll('button, a, input, .document-card, .keyword-tag');
    interactiveElements.forEach(el => {
        el.style.transition = 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)';
    });

    // Add ripple effect to buttons
    function createRipple(event) {
        const button = event.currentTarget;
        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = event.clientX - rect.left - size / 2;
        const y = event.clientY - rect.top - size / 2;
        
        ripple.style.width = ripple.style.height = size + 'px';
        ripple.style.left = x + 'px';
        ripple.style.top = y + 'px';
        ripple.classList.add('ripple');
        
        button.appendChild(ripple);
        
        setTimeout(() => {
            ripple.remove();
        }, 600);
    }

    const buttons = document.querySelectorAll('.btn, .search-submit-btn, .view-summary-btn');
    buttons.forEach(button => {
        button.addEventListener('click', createRipple);
    });

    // Add CSS for ripple effect
    const style = document.createElement('style');
    style.textContent = `
        .ripple {
            position: absolute;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.6);
            transform: scale(0);
            animation: ripple-animation 0.6s linear;
            pointer-events: none;
        }
        
        @keyframes ripple-animation {
            to {
                transform: scale(4);
                opacity: 0;
            }
        }
        
        .animate-in {
            animation: fadeInUp 0.6s ease-out forwards;
        }
        
        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .loading-skeleton {
            background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
            background-size: 200px 100%;
            animation: shimmer 1.5s infinite;
        }
        
        @keyframes shimmer {
            0% { background-position: -200px 0; }
            100% { background-position: calc(200px + 100%) 0; }
        }
    `;
    document.head.appendChild(style);
});
