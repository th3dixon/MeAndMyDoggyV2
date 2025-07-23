/**
 * Administration Dashboard JavaScript
 * Handles all admin interface interactions and state management
 */

function adminDashboard() {
    return {
        // State Management
        activeSection: 'dashboard',
        sidebarCollapsed: false,
        mobileMenuOpen: false,
        showUserModal: false,
        
        // Configuration
        activeConfigCategory: 'general',
        
        // Filters and Pagination
        userFilters: {
            search: '',
            type: '',
            status: ''
        },
        currentPage: 1,
        usersPerPage: 10,
        
        // Dashboard Statistics
        dashboardStats: {
            totalUsers: 12847,
            activeProviders: 1284,
            pendingReviews: 47,
            monthlyRevenue: 28459
        },
        
        // Alert Counters
        pendingUserActions: 12,
        securityAlerts: 3,
        moderationQueue: 15,
        
        // Sample Data
        recentActivity: [
            {
                id: 1,
                title: 'New User Registration',
                description: 'Sarah Johnson joined as a pet owner',
                time: '2 minutes ago',
                icon: 'fas fa-user-plus text-green-600',
                iconBg: 'bg-green-100'
            },
            {
                id: 2,
                title: 'Security Alert',
                description: 'Multiple failed login attempts detected',
                time: '15 minutes ago',
                icon: 'fas fa-shield-alt text-red-600',
                iconBg: 'bg-red-100'
            },
            {
                id: 3,
                title: 'Content Reported',
                description: 'User profile flagged for review',
                time: '1 hour ago',
                icon: 'fas fa-flag text-yellow-600',
                iconBg: 'bg-yellow-100'
            },
            {
                id: 4,
                title: 'Payment Processed',
                description: '£45.99 booking payment completed',
                time: '2 hours ago',
                icon: 'fas fa-credit-card text-blue-600',
                iconBg: 'bg-blue-100'
            }
        ],
        
        users: [
            {
                id: 1,
                name: 'John Smith',
                email: 'john.smith@example.com',
                type: 'Pet Owner',
                status: 'active',
                joinedDate: '2024-01-15',
                lastActive: '2 hours ago',
                avatar: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHJlY3Qgd2lkdGg9IjQwIiBoZWlnaHQ9IjQwIiByeD0iMjAiIGZpbGw9IiMzQjgyRjYiLz4KPHN2ZyB4PSI4IiB5PSI4IiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSI+CjxwYXRoIGQ9Ik0xMiAxMkM5Ljc5IDEyIDggMTAuMjEgOCA4UzkuNzkgNDEyIDRTMTQgNi4yMSAxNCA4IDEyLjIxIDEyIDEyIDEyWk0xMiAxNEM3LjU4IDE0IDQgMTUuNzkgNCAyMFYyMkgyMFYyMEMxOS0 MTUuNzkgMTYuNDIgMTQgMTIgMTRaIiBmaWxsPSJ3aGl0ZSIvPgo8L3N2Zz4KPC9zdmc+'
            },
            {
                id: 2,
                name: 'Emma Watson',
                email: 'emma.watson@example.com',
                type: 'Service Provider',
                status: 'active',
                joinedDate: '2024-01-10',
                lastActive: '1 day ago',
                avatar: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHJlY3Qgd2lkdGg9IjQwIiBoZWlnaHQ9IjQwIiByeD0iMjAiIGZpbGw9IiNFRjQ0NDQiLz4KPHN2ZyB4PSI4IiB5PSI4IiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSI+CjxwYXRoIGQ9Ik0xMiAxMkM5Ljc5IDEyIDggMTAuMjEgOCA4UzkuNzkgNDEyIDRTMTQgNi4yMSAxNCA4IDEyLjIxIDEyIDEyIDEyWk0xMiAxNEM3LjU4IDE0IDQgMTUuNzkgNCAyMFYyMkgyMFYyMEMxOS0 MTUuNzkgMTYuNDIgMTQgMTIgMTRaIiBmaWxsPSJ3aGl0ZSIvPgo8L3N2Zz4KPC9zdmc+'
            },
            {
                id: 3,
                name: 'Michael Brown',
                email: 'michael.brown@example.com',
                type: 'Pet Owner',
                status: 'suspended',
                joinedDate: '2024-01-05',
                lastActive: '1 week ago',
                avatar: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHJlY3Qgd2lkdGg9IjQwIiBoZWlnaHQ9IjQwIiByeD0iMjAiIGZpbGw9IiM5Q0E4QjAiLz4KPHN2ZyB4PSI4IiB5PSI4IiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSI+CjxwYXRoIGQ9Ik0xMiAxMkM5Ljc5IDEyIDggMTAuMjEgOCA4UzkuNzkgNDEyIDRTMTQgNi4yMSAxNCA4IDEyLjIxIDEyIDEyIDEyWk0xMiAxNEM3LjU4IDE0IDQgMTUuNzkgNCAyMFYyMkgyMFYyMEMxOS0 MTUuNzkgMTYuNDIgMTQgMTIgMTRaIiBmaWxsPSJ3aGl0ZSIvPgo8L3N2Zz4KPC9zdmc+'
            },
            {
                id: 4,
                name: 'Lisa Davis',
                email: 'lisa.davis@example.com',
                type: 'Service Provider',
                status: 'pending',
                joinedDate: '2024-01-18',
                lastActive: '3 hours ago',
                avatar: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHJlY3Qgd2lkdGg9IjQwIiBoZWlnaHQ9IjQwIiByeD0iMjAiIGZpbGw9IiNGNTlFMEIiLz4KPHN2ZyB4PSI4IiB5PSI4IiB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSI+CjxwYXRoIGQ9Ik0xMiAxMkM5Ljc5IDEyIDggMTAuMjEgOCA4UzkuNzkgNDEyIDRTMTQgNi4yMSAxNCA4IDEyLjIxIDEyIDEyIDEyWk0xMiAxNEM3LjU4IDE0IDQgMTUuNzkgNCAyMFYyMkgyMFYyMEMxOS0 MTUuNzkgMTYuNDIgMTQgMTIgMTRaIiBmaWxsPSJ3aGl0ZSIvPgo8L3N2Zz4KPC9zdmc+'
            }
        ],
        
        configCategories: [
            { id: 'general', name: 'General Settings', icon: 'fas fa-cog' },
            { id: 'features', name: 'Feature Flags', icon: 'fas fa-flag', hasChanges: true },
            { id: 'payments', name: 'Payments', icon: 'fas fa-credit-card' },
            { id: 'email', name: 'Email Settings', icon: 'fas fa-envelope' },
            { id: 'security', name: 'Security', icon: 'fas fa-shield-alt' }
        ],
        
        featureFlags: [
            { id: 1, name: 'Premium Subscriptions', description: 'Enable premium service provider subscriptions', enabled: true },
            { id: 2, name: 'Video Calling', description: 'Allow video calls between users and providers', enabled: false },
            { id: 3, name: 'AI Matching', description: 'Use AI to match pets with suitable providers', enabled: true },
            { id: 4, name: 'Instant Booking', description: 'Allow instant booking without approval', enabled: false }
        ],
        
        securityEvents: [
            {
                id: 1,
                timestamp: '2024-01-20 14:30:15',
                type: 'Failed Login',
                severity: 'medium',
                source: '192.168.1.100',
                description: 'Multiple failed login attempts',
                status: 'investigating'
            },
            {
                id: 2,
                timestamp: '2024-01-20 13:45:22',
                type: 'Suspicious Activity',
                severity: 'high',
                source: 'user@example.com',
                description: 'Account access from unusual location',
                status: 'open'
            },
            {
                id: 3,
                timestamp: '2024-01-20 12:15:45',
                type: 'Permission Change',
                severity: 'low',
                source: 'admin@system.com',
                description: 'User role updated from owner to provider',
                status: 'resolved'
            }
        ],
        
        moderationQueueItems: [
            {
                id: 1,
                type: 'User Profile',
                priority: 'high',
                title: 'Inappropriate Profile Content',
                description: 'User profile contains potentially offensive images and inappropriate content in bio section.',
                reportedBy: 'user123@example.com',
                reason: 'Inappropriate Content',
                contentAuthor: 'flagged_user@example.com',
                reportedAt: '2 hours ago'
            },
            {
                id: 2,
                type: 'Service Listing',
                priority: 'medium',
                title: 'Misleading Service Description',
                description: 'Service provider advertising services outside of their verified qualifications.',
                reportedBy: 'concerned_user@example.com',
                reason: 'Misleading Information',
                contentAuthor: 'provider@example.com',
                reportedAt: '4 hours ago'
            },
            {
                id: 3,
                type: 'Review',
                priority: 'low',
                title: 'Fake Review Suspected',
                description: 'Review appears to be artificially generated or incentivized.',
                reportedBy: 'system_algorithm',
                reason: 'Spam/Fake Content',
                contentAuthor: 'reviewer@example.com',
                reportedAt: '1 day ago'
            }
        ],
        
        // Computed Properties
        get totalAlerts() {
            return this.pendingUserActions + this.securityAlerts + this.moderationQueue;
        },
        
        get filteredUsers() {
            return this.users.filter(user => {
                const matchesSearch = !this.userFilters.search || 
                    user.name.toLowerCase().includes(this.userFilters.search.toLowerCase()) ||
                    user.email.toLowerCase().includes(this.userFilters.search.toLowerCase());
                const matchesType = !this.userFilters.type || user.type.toLowerCase().replace(' ', '_') === this.userFilters.type;
                const matchesStatus = !this.userFilters.status || user.status === this.userFilters.status;
                
                return matchesSearch && matchesType && matchesStatus;
            });
        },
        
        get paginatedUsers() {
            const start = (this.currentPage - 1) * this.usersPerPage;
            const end = start + this.usersPerPage;
            return this.filteredUsers.slice(start, end);
        },
        
        get totalPages() {
            return Math.ceil(this.filteredUsers.length / this.usersPerPage);
        },
        
        // Navigation Methods
        setActiveSection(section) {
            this.activeSection = section;
            if (window.innerWidth < 1024) {
                this.mobileMenuOpen = false;
            }
        },
        
        getSectionTitle(section) {
            const titles = {
                'dashboard': 'Dashboard',
                'users': 'User Management',
                'system': 'System Configuration',
                'security': 'Security Monitoring',
                'moderation': 'Content Moderation',
                'analytics': 'Analytics & Reporting',
                'financial': 'Financial Oversight',
                'integrations': 'Integration Management'
            };
            return titles[section] || 'Dashboard';
        },
        
        getConfigCategoryTitle(category) {
            const category_obj = this.configCategories.find(c => c.id === category);
            return category_obj ? category_obj.name : 'General Settings';
        },
        
        // User Management Methods
        getUserTypeBadge(type) {
            const badges = {
                'Pet Owner': 'bg-blue-100 text-blue-800',
                'Service Provider': 'bg-green-100 text-green-800',
                'Administrator': 'bg-purple-100 text-purple-800'
            };
            return badges[type] || 'bg-gray-100 text-gray-800';
        },
        
        getUserStatusClass(status) {
            return `status-${status}`;
        },
        
        getUserStatusDot(status) {
            const dots = {
                'active': 'bg-green-600',
                'suspended': 'bg-red-600',
                'pending': 'bg-yellow-600'
            };
            return dots[status] || 'bg-gray-600';
        },
        
        viewUser(user) {
            console.log('View user:', user);
            this.showUserModal = true;
        },
        
        editUser(user) {
            console.log('Edit user:', user);
        },
        
        suspendUser(user) {
            if (confirm(`Are you sure you want to suspend ${user.name}?`)) {
                user.status = 'suspended';
                console.log('User suspended:', user);
            }
        },
        
        reactivateUser(user) {
            if (confirm(`Are you sure you want to reactivate ${user.name}?`)) {
                user.status = 'active';
                console.log('User reactivated:', user);
            }
        },
        
        // Security Management Methods
        getEventTypeBadge(type) {
            const badges = {
                'Failed Login': 'bg-red-100 text-red-800',
                'Suspicious Activity': 'bg-yellow-100 text-yellow-800',
                'Permission Change': 'bg-blue-100 text-blue-800'
            };
            return badges[type] || 'bg-gray-100 text-gray-800';
        },
        
        getSeverityClass(severity) {
            return `status-${severity === 'high' ? 'suspended' : severity === 'medium' ? 'pending' : 'active'}`;
        },
        
        getSeverityDot(severity) {
            const dots = {
                'high': 'bg-red-600',
                'medium': 'bg-yellow-600',
                'low': 'bg-green-600'
            };
            return dots[severity] || 'bg-gray-600';
        },
        
        getEventStatusClass(status) {
            const classes = {
                'open': 'status-suspended',
                'investigating': 'status-pending',
                'resolved': 'status-active'
            };
            return classes[status] || 'status-pending';
        },
        
        viewSecurityEvent(event) {
            console.log('View security event:', event);
        },
        
        investigateEvent(event) {
            event.status = 'investigating';
            console.log('Investigating event:', event);
        },
        
        resolveEvent(event) {
            event.status = 'resolved';
            console.log('Event resolved:', event);
        },
        
        // Content Moderation Methods
        getContentTypeBadge(type) {
            const badges = {
                'User Profile': 'bg-blue-100 text-blue-800',
                'Service Listing': 'bg-green-100 text-green-800',
                'Review': 'bg-purple-100 text-purple-800',
                'Message': 'bg-gray-100 text-gray-800'
            };
            return badges[type] || 'bg-gray-100 text-gray-800';
        },
        
        getPriorityClass(priority) {
            const classes = {
                'high': 'status-suspended',
                'medium': 'status-pending',
                'low': 'status-active'
            };
            return classes[priority] || 'status-pending';
        },
        
        viewModerationItem(item) {
            console.log('View moderation item:', item);
        },
        
        approveModerationItem(item) {
            if (confirm('Are you sure you want to approve this content?')) {
                console.log('Content approved:', item);
                // Remove from queue
                const index = this.moderationQueueItems.findIndex(i => i.id === item.id);
                if (index > -1) {
                    this.moderationQueueItems.splice(index, 1);
                    this.moderationQueue--;
                }
            }
        },
        
        rejectModerationItem(item) {
            if (confirm('Are you sure you want to remove this content?')) {
                console.log('Content removed:', item);
                // Remove from queue
                const index = this.moderationQueueItems.findIndex(i => i.id === item.id);
                if (index > -1) {
                    this.moderationQueueItems.splice(index, 1);
                    this.moderationQueue--;
                }
            }
        },
        
        // Initialization
        init() {
            console.log('Admin Dashboard initialized');
            
            // Handle responsive sidebar
            this.$watch('$store.screenWidth', (width) => {
                if (width >= 1024) {
                    this.mobileMenuOpen = false;
                }
            });
        }
    }
}