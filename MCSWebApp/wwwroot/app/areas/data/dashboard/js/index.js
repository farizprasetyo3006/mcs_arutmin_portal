/*
document.addEventListener('DOMContentLoaded', function () {
   
    const slider = document.getElementById('dashboardSlider');
    const cards = document.querySelectorAll('.dashboard-card');
    const prevBtn = document.getElementById('sliderPrev');
    const nextBtn = document.getElementById('sliderNext');

    let cardWidth = cards[0].offsetWidth;
    let cardsPerView = Math.floor(slider.offsetWidth / cardWidth);
    let currentIndex = 0;

    function showCards() {
        slider.style.transform = `translateX(-${currentIndex * 25}%)`;
    }
    
    function getVisibleCards() {
        if (window.innerWidth > 1200) return 4;
        if (window.innerWidth > 992) return 3;
        if (window.innerWidth > 768) return 2;
        return 1;
    }
    function updateSlider() {
        cardWidth = cards[0].offsetWidth;
        cardsPerView = Math.floor(slider.offsetWidth / cardWidth);
        slider.style.transform = `translateX(-${currentIndex * cardWidth}px)`;

        // Enable/disable buttons
        prevBtn.style.display = currentIndex > 0 ? 'flex' : 'none';
        nextBtn.style.display = currentIndex < cards.length - cardsPerView ? 'flex' : 'none';
    }

    window.addEventListener('resize', updateSlider);

    function showCards() {
        const visibleCards = getVisibleCards();
        const slidePercentage = (100 / visibleCards) * currentIndex;
        slider.style.transform = `translateX(-${slidePercentage}%)`;
    }

    function updateNavigation() {
        const visibleCards = getVisibleCards();
        prevBtn.style.display = currentIndex > 0 ? 'flex' : 'none';
        nextBtn.style.display = currentIndex < cards.length - visibleCards ? 'flex' : 'none';
    }

    prevBtn.addEventListener('click', () => {
        if (currentIndex > 0) {
            currentIndex--;
            updateSlider();
        }
    });

    nextBtn.addEventListener('click', () => {
        if (currentIndex < cards.length - cardsPerView) {
            currentIndex++;
            updateSlider();
        }
    });

    // Initial setup
    updateSlider();
    // Collapse functionality
    const collapseBtn = document.querySelector('.btn-panel');
    collapseBtn.addEventListener('click', function () {
        const icon = this.querySelector('i');
        if (icon.classList.contains('fa-minus')) {
            icon.classList.remove('fa-minus');
            icon.classList.add('fa-plus');
        } else {
            icon.classList.remove('fa-plus');
            icon.classList.add('fa-minus');
        }
    });
}); */