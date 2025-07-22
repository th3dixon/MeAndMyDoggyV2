/**
 * Enhanced Navigation System
 * Handles role-based navigation, notifications, and mobile interactions
 */

class NavigationManager {
    constructor() {
        this.notificationInterval = null;
        this.unreadMessageCount = 0;
        this.unreadNotificationCount = 0;
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.setupNotifications();
        this.setupMobileNavigation();
        this.setupKeyboardNavigation();
        
        // Start polling for notifications if authenticated
        if (this.isAuthenticated()) {
            this.startNotificationPolling();
        }
    }

    isAuthenticated() {
        // Check if user is authenticated by looking for auth indicators
        return document.querySelector('.user-avatar') !== null || 
               document.querySelector('[data-authenticated="true"]') !== null;
    }

    setupEventListeners() {
        // Close dropdowns when clicking outside
        document.addEventListener('click', (e) => {
            if (!e.target.closest('.dropdown-container')) {
                this.closeAllDropdowns();
            }
        });

        // Handle active navigation state
        this.updateActiveNavigation();
        
        // Smooth scroll for anchor links
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', (e) => {
                e.preventDefault();
                const target = document.querySelector(anchor.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    setupNotifications() {
        const notificationBell = document.getElementById('notification-bell');
        const messagesIndicator = document.getElementById('messages-indicator');
        const notificationCount = document.getElementById('notification-count');
        
        if (notificationBell) {
            notificationBell.addEventListener('click', () => {
                this.loadNotifications();
            });
        }
        
        // Initialize notification badges
        this.updateNotificationBadges();
    }

    setupMobileNavigation() {
        const mobileToggle = document.querySelector('[data-mobile-toggle]');
        const mobileMenu = document.querySelector('[data-mobile-menu]');
        
        if (mobileToggle && mobileMenu) {
            // Handle mobile menu backdrop
            const backdrop = document.createElement('div');
            backdrop.className = 'fixed inset-0 bg-black bg-opacity-25 z-40 lg:hidden hidden';
            backdrop.setAttribute('data-mobile-backdrop', '');
            document.body.appendChild(backdrop);
            
            // Close menu when clicking backdrop
            backdrop.addEventListener('click', () => {
                this.closeMobileMenu();
            });
        }
        
        // Handle mobile menu scrolling
        this.handleMobileScrolling();
    }

    setupKeyboardNavigation() {
        // Escape key closes dropdowns
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                this.closeAllDropdowns();
                this.closeMobileMenu();
            }
            
            // Alt + M opens mobile menu (accessibility)
            if (e.altKey && e.key === 'm') {
                e.preventDefault();
                this.toggleMobileMenu();
            }
        });
    }

    closeAllDropdowns() {
        // Close Alpine.js dropdowns by dispatching events
        document.querySelectorAll('[x-data*="open"]').forEach(dropdown => {
            dropdown.dispatchEvent(new CustomEvent('close-dropdown'));
        });
    }

    closeMobileMenu() {
        const backdrop = document.querySelector('[data-mobile-backdrop]');
        if (backdrop) {
            backdrop.classList.add('hidden');
        }
        
        // Dispatch event to close mobile menu
        document.dispatchEvent(new CustomEvent('mobile-menu-toggle', {
            detail: { open: false }
        }));
    }

    toggleMobileMenu() {
        const backdrop = document.querySelector('[data-mobile-backdrop]');
        if (backdrop) {
            backdrop.classList.toggle('hidden');
        }
        
        // Let Alpine.js handle the toggle
        const isOpen = !backdrop?.classList.contains('hidden');
        document.dispatchEvent(new CustomEvent('mobile-menu-toggle', {
            detail: { open: isOpen }
        }));
    }

    handleMobileScrolling() {
        let lastScrollTop = 0;
        const nav = document.querySelector('nav');
        
        if (!nav) return;
        
        window.addEventListener('scroll', () => {
            const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
            
            // Hide/show navigation on mobile when scrolling
            if (window.innerWidth < 768) {
                if (scrollTop > lastScrollTop && scrollTop > 100) {
                    // Scrolling down
                    nav.style.transform = 'translateY(-100%)';
                } else {
                    // Scrolling up
                    nav.style.transform = 'translateY(0)';
                }
            } else {
                nav.style.transform = 'translateY(0)';
            }
            
            lastScrollTop = scrollTop;
        });
    }

    updateActiveNavigation() {
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.nav-link, .mobile-nav-link');
        
        navLinks.forEach(link => {
            const href = link.getAttribute('href');
            if (href && (currentPath === href || (href !== '/' && currentPath.startsWith(href)))) {
                link.classList.add('active');
            } else {
                link.classList.remove('active');
            }
        });
    }

    async startNotificationPolling() {
        // Poll every 30 seconds for new notifications
        this.notificationInterval = setInterval(() => {
            this.checkNotifications();
        }, 30000);
        
        // Initial check
        this.checkNotifications();
    }

    async checkNotifications() {
        try {
            // Check for new messages
            const messagesResponse = await fetch('/api/messages/unread-count', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
                    'Content-Type': 'application/json'
                }
            });
            
            if (messagesResponse.ok) {
                const messagesData = await messagesResponse.json();
                this.unreadMessageCount = messagesData.count || 0;
            }
            
            // Check for general notifications
            const notificationsResponse = await fetch('/api/notifications/unread-count', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
                    'Content-Type': 'application/json'
                }
            });
            
            if (notificationsResponse.ok) {
                const notificationsData = await notificationsResponse.json();
                this.unreadNotificationCount = notificationsData.count || 0;
            }
            
            this.updateNotificationBadges();
            
        } catch (error) {
            console.debug('Notification polling error:', error);
            // Gracefully handle errors - don't spam console in production
        }
    }

    updateNotificationBadges() {
        // Update message indicator
        const messagesIndicator = document.getElementById('messages-indicator');
        if (messagesIndicator) {
            if (this.unreadMessageCount > 0) {
                messagesIndicator.style.display = 'block';
                messagesIndicator.classList.add('notification-badge');
            } else {
                messagesIndicator.style.display = 'none';
                messagesIndicator.classList.remove('notification-badge');
            }
        }
        
        // Update notification count
        const notificationCount = document.getElementById('notification-count');
        if (notificationCount) {
            if (this.unreadNotificationCount > 0) {
                notificationCount.textContent = this.unreadNotificationCount > 99 ? '99+' : this.unreadNotificationCount;
                notificationCount.style.display = 'flex';
                notificationCount.classList.add('notification-badge');
            } else {
                notificationCount.style.display = 'none';
                notificationCount.classList.remove('notification-badge');
            }
        }
        
        // Update page title with notification count
        this.updatePageTitle();
    }

    updatePageTitle() {
        const baseTitle = document.title.replace(/^\(\d+\)\s*/, '');
        const totalNotifications = this.unreadMessageCount + this.unreadNotificationCount;
        
        if (totalNotifications > 0) {
            document.title = `(${totalNotifications}) ${baseTitle}`;
        } else {
            document.title = baseTitle;
        }
    }

    async loadNotifications() {
        const notificationsList = document.getElementById('notifications-list');
        if (!notificationsList) return;
        
        try {
            notificationsList.innerHTML = `
                <div class="p-4 text-center">
                    <i class="fas fa-spinner fa-spin text-gray-400 text-xl mb-2"></i>
                    <p class="text-gray-500 text-sm">Loading notifications...</p>
                </div>
            `;
            
            const response = await fetch('/api/notifications/recent', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                this.renderNotifications(data.notifications || []);
            } else {
                throw new Error('Failed to load notifications');
            }
            
        } catch (error) {
            console.error('Failed to load notifications:', error);
            notificationsList.innerHTML = `
                <div class="p-4 text-center text-gray-500">
                    <i class="fas fa-exclamation-triangle text-yellow-500 text-xl mb-2"></i>
                    <p class="text-sm">Failed to load notifications</p>
                    <button onclick="navigationManager.loadNotifications()" 
                            class="text-pet-blue text-xs mt-1 hover:underline">
                        Try again
                    </button>
                </div>
            `;
        }
    }

    renderNotifications(notifications) {
        const notificationsList = document.getElementById('notifications-list');
        if (!notificationsList) return;
        
        if (notifications.length === 0) {
            notificationsList.innerHTML = `
                <div class="p-8 text-center text-gray-500">
                    <i class="fas fa-bell-slash text-3xl mb-3 opacity-50"></i>
                    <p>No new notifications</p>
                </div>
            `;
            return;
        }
        
        notificationsList.innerHTML = notifications.map(notification => `
            <div class="notification-item ${!notification.isRead ? 'notification-unread' : ''}" 
                 onclick="navigationManager.markNotificationAsRead('${notification.id}')">
                <div class="flex items-start space-x-3">
                    <div class="notification-icon bg-${this.getNotificationColor(notification.type)}">
                        <i class="fas fa-${this.getNotificationIcon(notification.type)}"></i>
                    </div>
                    <div class="flex-1 min-w-0">
                        <p class="text-sm font-medium text-gray-900 truncate">
                            ${notification.title}
                        </p>
                        <p class="text-xs text-gray-500 mt-1 line-clamp-2">
                            ${notification.message}
                        </p>
                        <p class="text-xs text-gray-400 mt-1">
                            ${this.formatTimeAgo(notification.createdAt)}
                        </p>
                    </div>
                    ${!notification.isRead ? '<div class="w-2 h-2 bg-pet-blue rounded-full flex-shrink-0"></div>' : ''}
                </div>
            </div>
        `).join('');
    }

    getNotificationColor(type) {
        const colors = {
            'booking': 'pet-blue',
            'message': 'pet-green', 
            'payment': 'pet-purple',
            'system': 'gray-500',
            'emergency': 'red-500'
        };
        return colors[type] || 'gray-500';
    }

    getNotificationIcon(type) {
        const icons = {
            'booking': 'calendar',
            'message': 'comment',
            'payment': 'credit-card',
            'system': 'info-circle',
            'emergency': 'exclamation-triangle'
        };
        return icons[type] || 'bell';
    }

    formatTimeAgo(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffInSeconds = Math.floor((now - date) / 1000);
        
        if (diffInSeconds < 60) return 'Just now';
        if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}m ago`;
        if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}h ago`;
        if (diffInSeconds < 604800) return `${Math.floor(diffInSeconds / 86400)}d ago`;
        
        return date.toLocaleDateString();
    }

    async markNotificationAsRead(notificationId) {
        try {
            await fetch(`/api/notifications/${notificationId}/mark-read`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('accessToken')}`,
                    'Content-Type': 'application/json'
                }
            });
            
            // Refresh notifications
            this.checkNotifications();
            
        } catch (error) {
            console.error('Failed to mark notification as read:', error);
        }
    }

    destroy() {
        if (this.notificationInterval) {
            clearInterval(this.notificationInterval);
        }
        
        // Remove event listeners
        document.removeEventListener('click', this.closeAllDropdowns);
        document.removeEventListener('keydown', this.setupKeyboardNavigation);
    }
}

// Initialize navigation manager when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.navigationManager = new NavigationManager();
});

// Handle page navigation without full reload (for SPA-like behavior)
window.addEventListener('popstate', () => {
    if (window.navigationManager) {
        window.navigationManager.updateActiveNavigation();
    }
});

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = NavigationManager;
}