// Dashboard utility functions with Fetch API + Promise (async/await)

// ==================== Notification Helper ====================
class NotificationHelper {
    constructor(checkUrl) {
        this.checkUrl = checkUrl;
        this.badgeElement = document.getElementById('notificationBadge');
    }

    async check() {
        try {
            const request = new Request(this.checkUrl, { method: 'GET' });
            const response = await fetch(request);
            const result = await response.json();

            if (result.success && result.count > 0) {
                this.updateBadge(result.count);
            } else {
                this.hideBadge();
            }
        } catch (error) {
            console.error('Error checking notifications:', error);
        }
    }

    updateBadge(count) {
        if (this.badgeElement) {
            this.badgeElement.textContent = count;
            this.badgeElement.classList.remove('d-none');
        }
    }

    hideBadge() {
        if (this.badgeElement) {
            this.badgeElement.classList.add('d-none');
        }
    }

    startPolling(interval = 5000) {
        this.check(); // Initial check
        setInterval(() => this.check(), interval);
    }
}

// ==================== Loading Overlay ====================
class LoadingOverlay {
    constructor() {
        this.overlay = null;
        this.createOverlay();
    }

    createOverlay() {
        this.overlay = document.createElement('div');
        this.overlay.className = 'loading-overlay d-none';
        this.overlay.innerHTML = `
            <div class="spinner-border text-light spinner-border-lg" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        `;
        document.body.appendChild(this.overlay);
    }

    show() {
        this.overlay.classList.remove('d-none');
    }

    hide() {
        this.overlay.classList.add('d-none');
    }
}

// ==================== Toast Notification ====================
function showToast(message, type = 'success') {
    const toastContainer = document.getElementById('toastContainer') || createToastContainer();

    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;

    toastContainer.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();

    setTimeout(() => toast.remove(), 5000);
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toastContainer';
    container.className = 'toast-container position-fixed top-0 end-0 p-3';
    container.style.zIndex = '9999';
    document.body.appendChild(container);
    return container;
}

// ==================== Confirm Dialog ====================
async function confirmAction(message) {
    return new Promise((resolve) => {
        const result = confirm(message);
        resolve(result);
    });
}

// ==================== Export for use ====================
window.NotificationHelper = NotificationHelper;
window.LoadingOverlay = LoadingOverlay;
window.showToast = showToast;
window.confirmAction = confirmAction;