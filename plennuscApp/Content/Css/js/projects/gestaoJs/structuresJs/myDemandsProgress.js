document.addEventListener('DOMContentLoaded', function () {
    colorStatusBadges();
});

function colorStatusBadges() {
    const badges = document.querySelectorAll('.status-badge');
    badges.forEach(badge => {
        const statusText = badge.textContent.trim().toLowerCase();
        badge.style.backgroundColor = '';
        badge.style.color = '';

        if (statusText.includes('aberta')) {
            badge.style.backgroundColor = '#e6f4ea';
            badge.style.color = '#137333';
        } else if (statusText.includes('andamento')) {
            badge.style.backgroundColor = '#e8f0fe';
            badge.style.color = '#1a73e8';
        } else if (statusText.includes('concluída') || statusText.includes('concluida')) {
            badge.style.backgroundColor = '#fef7e0';
            badge.style.color = '#f9ab00';
        } else if (statusText.includes('fechada')) {
            badge.style.backgroundColor = '#fce8e6';
            badge.style.color = '#c5221f';
        } else {
            badge.style.backgroundColor = '#f1f3f4';
            badge.style.color = '#5f6368';
        }
    });
}

const observer = new MutationObserver(function (mutations) {
    mutations.forEach(function (mutation) {
        if (mutation.addedNodes.length) {
            colorStatusBadges();
        }
    });
});

window.addEventListener('load', function () {
    const gridContainer = document.querySelector('.grid-container');
    if (gridContainer) {
        observer.observe(gridContainer, { childList: true, subtree: true });
    }
});