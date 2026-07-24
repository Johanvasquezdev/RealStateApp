document.addEventListener("DOMContentLoaded", function () {
    // Scroll reveal animation
    const observerOptions = {
        root: null,
        rootMargin: '0px',
        threshold: 0.1
    };

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-reveal');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    // Apply animation to cards and main sections
    const elementsToAnimate = document.querySelectorAll('.card, .hero-section, .property-header, .chat-bubble');
    elementsToAnimate.forEach(el => {
        el.classList.add('reveal-hidden');
        observer.observe(el);
    });

    // Add smooth scrolling to all links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector(this.getAttribute('href')).scrollIntoView({
                behavior: 'smooth'
            });
        });
    });
});
