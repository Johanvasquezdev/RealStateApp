// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function() {
    const favoriteForms = document.querySelectorAll('.favorite-form');
    
    favoriteForms.forEach(form => {
        form.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const btn = this.querySelector('button');
            const icon = btn.querySelector('i');
            const originalAction = this.action;
            
            // Add bounce animation class
            icon.classList.add('heart-bounce');
            setTimeout(() => icon.classList.remove('heart-bounce'), 400);

            // Toggle visual state immediately for better UX
            if (icon.classList.contains('bi-heart')) {
                icon.classList.remove('bi-heart');
                icon.classList.add('bi-heart-fill');
            } else {
                icon.classList.remove('bi-heart-fill');
                icon.classList.add('bi-heart');
            }

            try {
                const response = await fetch(originalAction, {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                
                if (response.redirected && response.url.includes('Login')) {
                    // Not logged in, redirect manually
                    window.location.href = response.url;
                }
            } catch (error) {
                console.error('Error toggling favorite:', error);
                // Revert visual state on error
                if (icon.classList.contains('bi-heart')) {
                    icon.classList.remove('bi-heart');
                    icon.classList.add('bi-heart-fill');
                } else {
                    icon.classList.remove('bi-heart-fill');
                    icon.classList.add('bi-heart');
                }
            }
        });
    });
});
