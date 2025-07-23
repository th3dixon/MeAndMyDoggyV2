/**
 * Account Settings Page JavaScript
 * Handles all interactive functionality for the account settings page
 */

function accountSettings() {
    return {
        // Active section
        activeSection: 'security',
        
        // Section navigation
        sections: [
            { id: 'security', title: 'Login & Security', icon: 'fas fa-shield-alt' },
            { id: 'notifications', title: 'Notifications', icon: 'fas fa-bell' },
            { id: 'privacy', title: 'Privacy', icon: 'fas fa-lock' },
            { id: 'billing', title: 'Billing & Payments', icon: 'fas fa-credit-card' },
            { id: 'data', title: 'Data Management', icon: 'fas fa-database' }
        ],
        
        
        // Password change form
        passwordForm: {
            currentPassword: '',
            newPassword: '',
            confirmPassword: ''
        },
        
        // Password strength indicators
        passwordStrength: 0,
        passwordStrengthText: '',
        passwordStrengthColor: 'text-gray-400',
        
        // Two-factor authentication status
        twoFactorEnabled: {
            sms: false,
            app: false
        },
        
        // Notification preferences
        notifications: {
            emailNotifications: true,
            smsNotifications: false,
            pushNotifications: true,
            marketingEmails: false,
            bookingReminders: true,
            newMessages: true,
            serviceUpdates: true
        },
        
        // Privacy settings
        privacy: {
            profileVisible: true,
            showLocation: true,
            shareDataWithPartners: false,
            allowAnalytics: true
        },
        
        // Delete account form
        deleteAccountForm: {
            password: '',
            reason: ''
        },
        
        // Modal states
        showDeleteModal: false,
        show2FAModal: false,
        showPlanSelectionModal: false,
        showPaymentMethodModal: false,
        
        // 2FA data
        twoFA: {
            showQRCode: false,
            showSMSVerify: false,
            showBackupCodes: false,
            qrCodeUrl: '',
            manualKey: '',
            phoneNumber: '',
            verificationCode: '',
            backupCodes: [],
            method: ''
        },
        
        // Loading states
        changingPassword: false,
        savingNotifications: false,
        savingPrivacy: false,
        requestingExport: false,
        deletingAccount: false,
        enabling2FA: false,
        
        // Billing data
        billingInfo: {
            subscription: null,
            paymentMethods: [],
            billingHistory: [],
            taxInfo: null
        },
        
        // Loading states for billing
        loadingBilling: false,
        updatingPayment: false,
        cancellingSubscription: false,
        processingPayment: false,
        changingPlan: false,
        
        // Sessions data
        sessions: [],
        
        // Stripe instance
        stripe: null,
        cardElement: null,
        
        // Payment form data
        paymentForm: {
            cardholderName: '',
            setAsDefault: true
        },
        
        // Initialize the component
        init() {
            // Load initial data
            this.load2FAStatus();
            this.loadNotificationPreferences();
            this.loadPrivacySettings();
            
            // Filter sections based on user role
            this.filterSections();
            
            // Initialize Stripe
            this.initializeStripe();
            
            // Set active section from URL hash if present
            if (window.location.hash) {
                const section = window.location.hash.substring(1);
                if (this.sections.find(s => s.id === section)) {
                    this.activeSection = section;
                }
            }
            
            // Update URL hash when section changes
            this.$watch('activeSection', (value) => {
                window.location.hash = value;
                
                // Load billing info when billing section is accessed
                if (value === 'billing' && !this.billingInfo.subscription && !this.loadingBilling) {
                    this.loadBillingInfo();
                }
                
                // Load sessions when security section is accessed
                if (value === 'security' && this.sessions.length === 0) {
                    this.loadSessions();
                }
            });
            
            // Initialize billing info to prevent null errors
            if (!this.billingInfo.subscription) {
                this.billingInfo.subscription = null;
            }
            if (!this.billingInfo.paymentMethods) {
                this.billingInfo.paymentMethods = [];
            }
            if (!this.billingInfo.billingHistory) {
                this.billingInfo.billingHistory = [];
            }
        },
        
        // Load 2FA status
        async load2FAStatus() {
            try {
                const response = await fetch('/api/v1/user/2fa/status', {
                    credentials: 'include'
                });
                if (response.ok) {
                    const status = await response.json();
                    this.twoFactorEnabled.sms = status.smsEnabled;
                    this.twoFactorEnabled.app = status.appEnabled;
                }
            } catch (error) {
                console.error('Error loading 2FA status:', error);
            }
        },
        
        // Filter sections based on user role
        filterSections() {
            const isServiceProvider = document.querySelector('meta[name="is-service-provider"]')?.content === 'true';
            
            if (!isServiceProvider) {
                // Remove billing section for non-service providers
                this.sections = this.sections.filter(s => s.id !== 'billing');
            }
        },
        
        // Load notification preferences
        async loadNotificationPreferences() {
            try {
                const response = await fetch('/api/v1/user/notifications', {
                    credentials: 'include'
                });
                if (response.ok) {
                    const preferences = await response.json();
                    // Update local state with loaded preferences
                    this.notifications = {
                        emailNotifications: preferences.emailNotifications ?? true,
                        smsNotifications: preferences.smsNotifications ?? false,
                        pushNotifications: preferences.pushNotifications ?? true,
                        marketingEmails: preferences.marketingCommunications ?? false,
                        bookingReminders: preferences.bookingReminders ?? true,
                        newMessages: preferences.newMessages ?? true,
                        serviceUpdates: preferences.serviceUpdates ?? true
                    };
                }
            } catch (error) {
                console.error('Error loading notification preferences:', error);
            }
        },
        
        // Load privacy settings
        async loadPrivacySettings() {
            try {
                const response = await fetch('/api/v1/user/privacy', {
                    credentials: 'include'
                });
                if (response.ok) {
                    const settings = await response.json();
                    // Update local state with loaded settings
                    this.privacy = {
                        profileVisible: settings.profilePublic ?? true,
                        showLocation: settings.showLocation ?? true,
                        shareDataWithPartners: settings.allowDataSharing ?? false,
                        allowAnalytics: settings.allowAnalytics ?? true
                    };
                }
            } catch (error) {
                console.error('Error loading privacy settings:', error);
            }
        },
        
        // Enable 2FA
        async enable2FA(method) {
            this.twoFA.method = method;
            this.show2FAModal = true;
            
            if (method === 'sms') {
                this.twoFA.showSMSVerify = true;
            } else if (method === 'app') {
                this.enabling2FA = true;
                try {
                    
                    const response = await fetch('/api/v1/user/2fa/enable', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'X-Requested-With': 'XMLHttpRequest',
                            'Accept': 'application/json'
                        },
                        credentials: 'include', // Use 'include' instead of 'same-origin' for cookies
                        body: JSON.stringify({ Type: 'app' })
                    });
                    
                    
                    if (!response.ok) {
                        console.error('2FA enable response error:', response.status, response.statusText);
                        if (response.status === 401) {
                            this.showNotification('You are not authenticated. Please log in again.', 'error');
                            window.location.href = '/Account/Login';
                            return;
                        }
                        if (response.status === 400) {
                            const errorData = await response.json();
                            console.error('Bad request error:', errorData);
                            this.showNotification(errorData.error || 'Invalid request', 'error');
                            return;
                        }
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    
                    const result = await response.json();
                    
                    if (result.success) {
                        // Handle both camelCase and PascalCase property names
                        this.twoFA.qrCodeUrl = result.qrCodeUrl || result.QrCodeUrl;
                        this.twoFA.manualKey = result.manualKey || result.ManualKey;
                        this.twoFA.showQRCode = true;
                        
                        // Generate QR code
                        this.$nextTick(() => {
                            this.generateQRCode();
                        });
                    } else {
                        this.showNotification(result.message || result.Message || 'Failed to enable 2FA', 'error');
                    }
                } catch (error) {
                    console.error('Error enabling 2FA:', error);
                    this.showNotification('An error occurred while enabling 2FA', 'error');
                } finally {
                    this.enabling2FA = false;
                }
            }
        },
        
        // Send SMS verification code
        async sendSMSCode() {
            if (!this.twoFA.phoneNumber) {
                this.showNotification('Please enter your phone number', 'error');
                return;
            }
            
            try {
                const response = await fetch('/api/v1/user/2fa/enable', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include',
                    body: JSON.stringify({ 
                        Type: 'sms',
                        PhoneNumber: this.twoFA.phoneNumber
                    })
                });
                
                const result = await response.json();
                
                if (result.success) {
                    this.showNotification('Verification code sent to your phone', 'success');
                } else {
                    this.showNotification(result.message || 'Failed to send verification code', 'error');
                }
            } catch (error) {
                console.error('Error sending SMS code:', error);
                this.showNotification('An error occurred while sending verification code', 'error');
            }
        },
        
        // Verify 2FA code
        async verify2FA() {
            if (!this.twoFA.verificationCode) {
                this.showNotification('Please enter the verification code', 'error');
                return;
            }
            
            try {
                const response = await fetch('/api/v1/user/2fa/verify', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include',
                    body: JSON.stringify({ 
                        Type: this.twoFA.method,
                        Code: this.twoFA.verificationCode
                    })
                });
                
                const result = await response.json();
                
                if (result.success) {
                    this.twoFA.backupCodes = result.backupCodes || [];
                    this.twoFA.showQRCode = false;
                    this.twoFA.showSMSVerify = false;
                    this.twoFA.showBackupCodes = true;
                    this.twoFactorEnabled[this.twoFA.method] = true;
                } else {
                    this.showNotification(result.error || 'Invalid verification code', 'error');
                }
            } catch (error) {
                console.error('Error verifying 2FA:', error);
                this.showNotification('An error occurred while verifying code', 'error');
            }
        },
        
        // Download backup codes
        downloadBackupCodes() {
            const content = 'MeAndMyDoggy Two-Factor Authentication Backup Codes\n\n' +
                          'Save these codes in a secure place. Each code can only be used once.\n\n' +
                          this.twoFA.backupCodes.join('\n');
            
            const blob = new Blob([content], { type: 'text/plain' });
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'meandmydoggy-backup-codes.txt';
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
        },
        
        // Reset 2FA modal
        resetTwoFAModal() {
            this.twoFA = {
                showQRCode: false,
                showSMSVerify: false,
                showBackupCodes: false,
                qrCodeUrl: '',
                manualKey: '',
                phoneNumber: '',
                verificationCode: '',
                backupCodes: [],
                method: ''
            };
        },
        
        // Generate QR code
        generateQRCode() {
            const canvas = document.getElementById('qrcode');
            if (canvas && this.twoFA.qrCodeUrl) {
                // In a real implementation, you would use a QR code library
                // For now, we'll just show a placeholder
                const ctx = canvas.getContext('2d');
                canvas.width = 200;
                canvas.height = 200;
                ctx.fillStyle = '#333';
                ctx.fillRect(0, 0, 200, 200);
                ctx.fillStyle = '#fff';
                ctx.font = '14px Arial';
                ctx.textAlign = 'center';
                ctx.fillText('QR Code', 100, 100);
                ctx.font = '10px Arial';
                ctx.fillText('(Use QR library)', 100, 120);
            }
        },
        
        // Change password
        async changePassword() {
            // Validate passwords match
            if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
                this.showNotification('Passwords do not match', 'error');
                return;
            }
            
            // Check password strength
            if (this.passwordStrength < 60) {
                this.showNotification('Please choose a stronger password', 'error');
                return;
            }
            
            this.changingPassword = true;
            
            try {
                const response = await fetch('/api/v1/user/change-password', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include',
                    body: JSON.stringify(this.passwordForm)
                });
                
                const result = await response.json();
                
                if (result.success) {
                    this.showNotification('Password changed successfully', 'success');
                    // Clear form
                    this.passwordForm = {
                        currentPassword: '',
                        newPassword: '',
                        confirmPassword: ''
                    };
                    this.passwordStrength = 0;
                } else {
                    this.showNotification(result.message || 'Failed to change password', 'error');
                }
            } catch (error) {
                console.error('Error changing password:', error);
                this.showNotification('An error occurred while changing your password', 'error');
            } finally {
                this.changingPassword = false;
            }
        },
        
        // Check password strength
        checkPasswordStrength() {
            const password = this.passwordForm.newPassword;
            let strength = 0;
            
            // Length check
            if (password.length >= 8) strength += 20;
            if (password.length >= 12) strength += 20;
            
            // Character variety checks
            if (/[a-z]/.test(password)) strength += 15;
            if (/[A-Z]/.test(password)) strength += 15;
            if (/[0-9]/.test(password)) strength += 15;
            if (/[^A-Za-z0-9]/.test(password)) strength += 15;
            
            this.passwordStrength = strength;
            
            // Set strength text and color
            if (strength < 40) {
                this.passwordStrengthText = 'Weak';
                this.passwordStrengthColor = 'text-red-500';
            } else if (strength < 60) {
                this.passwordStrengthText = 'Fair';
                this.passwordStrengthColor = 'text-yellow-500';
            } else if (strength < 80) {
                this.passwordStrengthText = 'Good';
                this.passwordStrengthColor = 'text-blue-500';
            } else {
                this.passwordStrengthText = 'Strong';
                this.passwordStrengthColor = 'text-green-500';
            }
        },
        
        // Update notification preferences
        async updateNotifications() {
            this.savingNotifications = true;
            
            try {
                const response = await fetch('/api/v1/user/notifications', {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include',
                    body: JSON.stringify({
                        EmailNotifications: this.notifications.emailNotifications,
                        SmsNotifications: this.notifications.smsNotifications,
                        PushNotifications: this.notifications.pushNotifications,
                        BookingReminders: this.notifications.bookingReminders,
                        NewMessages: this.notifications.newMessages,
                        ServiceUpdates: this.notifications.serviceUpdates,
                        MarketingCommunications: this.notifications.marketingEmails,
                        QuietHoursStart: "22:00", // Default quiet hours
                        QuietHoursEnd: "08:00"
                    })
                });
                
                if (response.ok) {
                    const result = await response.json();
                    this.showNotification(result.message || 'Notification preferences updated', 'success');
                } else {
                    const error = await response.json();
                    this.showNotification(error.message || error.error || 'Failed to update preferences', 'error');
                }
            } catch (error) {
                console.error('Error updating notifications:', error);
                this.showNotification('An error occurred while updating notification preferences', 'error');
            } finally {
                this.savingNotifications = false;
            }
        },
        
        // Update privacy settings
        async updatePrivacy() {
            this.savingPrivacy = true;
            
            try {
                const response = await fetch('/api/v1/user/privacy', {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include',
                    body: JSON.stringify({
                        ProfilePublic: this.privacy.profileVisible,
                        ShowLocation: this.privacy.showLocation,
                        AllowDataSharing: this.privacy.shareDataWithPartners,
                        AllowAnalytics: this.privacy.allowAnalytics,
                        ShowOnlineStatus: true // Default value
                    })
                });
                
                if (response.ok) {
                    const result = await response.json();
                    this.showNotification(result.message || 'Privacy settings updated', 'success');
                } else {
                    const error = await response.json();
                    this.showNotification(error.message || error.error || 'Failed to update privacy settings', 'error');
                }
            } catch (error) {
                console.error('Error updating privacy:', error);
                this.showNotification('An error occurred while updating privacy settings', 'error');
            } finally {
                this.savingPrivacy = false;
            }
        },
        
        // Request data export
        async requestDataExport() {
            this.requestingExport = true;
            
            try {
                const response = await fetch('/api/v1/user/data-export', {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include'
                });
                
                const result = await response.json();
                
                if (result.success) {
                    this.showNotification('Data export requested. You will receive an email when it\'s ready.', 'success');
                } else {
                    this.showNotification(result.message || 'Failed to request data export', 'error');
                }
            } catch (error) {
                console.error('Error requesting data export:', error);
                this.showNotification('An error occurred while requesting data export', 'error');
            } finally {
                this.requestingExport = false;
            }
        },
        
        // Delete account
        async deleteAccount() {
            if (!this.deleteAccountForm.password) {
                this.showNotification('Please enter your password to confirm', 'error');
                return;
            }
            
            this.deletingAccount = true;
            
            try {
                const response = await fetch('/api/v1/user/delete-account', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'include',
                    body: JSON.stringify(this.deleteAccountForm)
                });
                
                const result = await response.json();
                
                if (result.success) {
                    this.showNotification('Account deletion requested', 'success');
                    if (result.redirect) {
                        setTimeout(() => {
                            window.location.href = result.redirect;
                        }, 2000);
                    }
                } else {
                    this.showNotification(result.message || 'Failed to delete account', 'error');
                }
            } catch (error) {
                console.error('Error deleting account:', error);
                this.showNotification('An error occurred while deleting your account', 'error');
            } finally {
                this.deletingAccount = false;
            }
        },
        
        // Billing Management Methods
        async loadBillingInfo() {
            this.loadingBilling = true;
            try {
                const [subscriptionResponse, methodsResponse, historyResponse] = await Promise.all([
                    fetch('/api/v1/user/billing/subscription', { credentials: 'include' }),
                    fetch('/api/v1/user/billing/payment-methods', { credentials: 'include' }),
                    fetch('/api/v1/user/billing/history', { credentials: 'include' })
                ]);
                
                if (subscriptionResponse.ok) {
                    this.billingInfo.subscription = await subscriptionResponse.json();
                } else {
                    console.warn('Failed to load subscription info');
                    this.billingInfo.subscription = null;
                }
                
                if (methodsResponse.ok) {
                    this.billingInfo.paymentMethods = await methodsResponse.json();
                } else {
                    console.warn('Failed to load payment methods');
                    this.billingInfo.paymentMethods = [];
                }
                
                if (historyResponse.ok) {
                    this.billingInfo.billingHistory = await historyResponse.json();
                } else {
                    console.warn('Failed to load billing history');
                    this.billingInfo.billingHistory = [];
                }
            } catch (error) {
                console.error('Error loading billing info:', error);
                this.showNotification('Failed to load billing information', 'error');
                // Ensure we have safe defaults
                this.billingInfo.subscription = null;
                this.billingInfo.paymentMethods = [];
                this.billingInfo.billingHistory = [];
            } finally {
                this.loadingBilling = false;
            }
        },
        
        // Initialize Stripe
        async initializeStripe() {
            try {
                // Get Stripe public key from API
                const response = await fetch('/api/v1/user/billing/stripe-config', {
                    credentials: 'include'
                });
                if (response.ok) {
                    const config = await response.json();
                    this.stripe = Stripe(config.publicKey, {
                        locale: 'en-GB' // Set UK locale for Stripe
                    });
                    
                    // Initialize card element when modal is opened
                    this.$watch('showPaymentMethodModal', (value) => {
                        if (value && !this.cardElement) {
                            this.setupCardElement();
                        }
                    });
                } else {
                    console.warn('Could not load Stripe configuration');
                    // Use test key as fallback for development
                    this.stripe = Stripe('pk_test_51NLc5eSJKGqV9NLHuRgF8iZQnPzPGJvDZqJjCvUzWZBh5QqOG0CcfKQUXKGNKWxZRYIxgXhFVOcOHXKlKGBG6Ej600eROaZBUV', {
                        locale: 'en-GB'
                    });
                }
            } catch (error) {
                console.error('Error initializing Stripe:', error);
                // Use test key as fallback
                this.stripe = Stripe('pk_test_51NLc5eSJKGqV9NLHuRgF8iZQnPzPGJvDZqJjCvUzWZBh5QqOG0CcfKQUXKGNKWxZRYIxgXhFVOcOHXKlKGBG6Ej600eROaZBUV', {
                    locale: 'en-GB'
                });
            }
        },
        
        // Setup Stripe Card Element
        setupCardElement() {
            if (!this.stripe) return;
            
            const elements = this.stripe.elements({
                locale: 'en-GB' // Set locale to UK English
            });
            
            const style = {
                base: {
                    color: '#32325d',
                    fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
                    fontSmoothing: 'antialiased',
                    fontSize: '16px',
                    '::placeholder': {
                        color: '#aab7c4'
                    }
                },
                invalid: {
                    color: '#fa755a',
                    iconColor: '#fa755a'
                }
            };
            
            // Create card element with UK-specific options
            this.cardElement = elements.create('card', { 
                style: style,
                hidePostalCode: false, // Show postal code field (will display as "Postcode" for UK cards)
                placeholderCountry: 'GB' // Default to UK
            });
            this.cardElement.mount('#card-element');
            
            // Handle real-time validation errors
            this.cardElement.addEventListener('change', (event) => {
                const displayError = document.getElementById('card-errors');
                if (event.error) {
                    displayError.textContent = event.error.message;
                } else {
                    displayError.textContent = '';
                }
            });
        },
        
        async addPaymentMethod() {
            this.showPaymentMethodModal = true;
            // Reset form
            this.paymentForm = {
                cardholderName: '',
                setAsDefault: true
            };
        },
        
        async processPaymentMethod() {
            if (!this.paymentForm.cardholderName) {
                this.showNotification('Please enter the cardholder name', 'error');
                return;
            }
            
            this.processingPayment = true;
            
            try {
                // Create payment method with Stripe
                const { paymentMethod, error } = await this.stripe.createPaymentMethod({
                    type: 'card',
                    card: this.cardElement,
                    billing_details: {
                        name: this.paymentForm.cardholderName
                    }
                });
                
                if (error) {
                    this.showNotification(error.message, 'error');
                    return;
                }
                
                // Send payment method to backend
                const response = await fetch('/api/v1/user/billing/payment-methods', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: JSON.stringify({
                        paymentMethodId: paymentMethod.id,
                        setAsDefault: this.paymentForm.setAsDefault
                    })
                });
                
                const result = await response.json();
                
                if (response.ok && result.success) {
                    this.showNotification('Payment method added successfully', 'success');
                    this.showPaymentMethodModal = false;
                    await this.loadBillingInfo(); // Reload payment methods
                    
                    // Clear card element
                    if (this.cardElement) {
                        this.cardElement.clear();
                    }
                } else {
                    this.showNotification(result.message || 'Failed to add payment method', 'error');
                }
            } catch (error) {
                console.error('Error adding payment method:', error);
                this.showNotification('An error occurred while adding payment method', 'error');
            } finally {
                this.processingPayment = false;
            }
        },
        
        async removePaymentMethod(methodId) {
            if (!confirm('Are you sure you want to remove this payment method?')) {
                return;
            }
            
            try {
                const response = await fetch(`/api/v1/user/billing/payment-methods/${methodId}`, {
                    method: 'DELETE',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                
                if (response.ok) {
                    this.billingInfo.paymentMethods = this.billingInfo.paymentMethods.filter(m => m.id !== methodId);
                    this.showNotification('Payment method removed successfully', 'success');
                } else {
                    this.showNotification('Failed to remove payment method', 'error');
                }
            } catch (error) {
                console.error('Error removing payment method:', error);
                this.showNotification('An error occurred while removing payment method', 'error');
            }
        },
        
        async setDefaultPaymentMethod(methodId) {
            try {
                const response = await fetch(`/api/v1/user/billing/payment-methods/${methodId}/default`, {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                
                if (response.ok) {
                    // Update local state
                    this.billingInfo.paymentMethods.forEach(method => {
                        method.isDefault = method.id === methodId;
                    });
                    this.showNotification('Default payment method updated', 'success');
                } else {
                    this.showNotification('Failed to update default payment method', 'error');
                }
            } catch (error) {
                console.error('Error setting default payment method:', error);
                this.showNotification('An error occurred while updating payment method', 'error');
            }
        },
        
        async selectPlan(planId) {
            // Check if user has payment method for paid plans
            if (planId !== 'free' && this.billingInfo.paymentMethods.length === 0) {
                this.showPlanSelectionModal = false;
                this.showNotification('Please add a payment method before selecting a paid plan', 'warning');
                this.showPaymentMethodModal = true;
                return;
            }
            
            const planName = planId === 'free' ? 'Free' : 'Premium';
            if (confirm(`Are you sure you want to change to the ${planName} plan?`)) {
                this.showPlanSelectionModal = false;
                await this.changePlan(planId);
            }
        },
        
        async changePlan(planId) {
            this.changingPlan = true;
            try {
                const response = await fetch('/api/v1/user/billing/subscription/change', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: JSON.stringify({ planId })
                });
                
                const result = await response.json();
                
                if (result.success) {
                    this.showNotification('Subscription plan updated successfully', 'success');
                    // Reload billing info
                    await this.loadBillingInfo();
                } else {
                    this.showNotification(result.message || 'Failed to change plan', 'error');
                }
            } catch (error) {
                console.error('Error changing plan:', error);
                this.showNotification('An error occurred while changing your plan', 'error');
            } finally {
                this.changingPlan = false;
            }
        },
        
        async cancelSubscription() {
            if (!confirm('Are you sure you want to cancel your subscription? You will lose access to premium features at the end of your billing period.')) {
                return;
            }
            
            this.cancellingSubscription = true;
            try {
                const response = await fetch('/api/v1/user/billing/subscription/cancel', {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                
                const result = await response.json();
                
                if (result.success) {
                    this.showNotification('Subscription cancelled. You will retain access until ' + result.accessUntil, 'success');
                    await this.loadBillingInfo();
                } else {
                    this.showNotification(result.message || 'Failed to cancel subscription', 'error');
                }
            } catch (error) {
                console.error('Error cancelling subscription:', error);
                this.showNotification('An error occurred while cancelling your subscription', 'error');
            } finally {
                this.cancellingSubscription = false;
            }
        },
        
        async downloadInvoice(invoiceId) {
            try {
                const response = await fetch(`/api/v1/user/billing/invoices/${invoiceId}/download`);
                if (response.ok) {
                    const blob = await response.blob();
                    const url = URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = `invoice-${invoiceId}.pdf`;
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                    URL.revokeObjectURL(url);
                } else {
                    this.showNotification('Failed to download invoice', 'error');
                }
            } catch (error) {
                console.error('Error downloading invoice:', error);
                this.showNotification('An error occurred while downloading invoice', 'error');
            }
        },
        
        async updateTaxInfo(taxInfo) {
            try {
                const response = await fetch('/api/v1/user/billing/tax-info', {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: JSON.stringify(taxInfo)
                });
                
                if (response.ok) {
                    this.showNotification('Tax information updated successfully', 'success');
                    this.billingInfo.taxInfo = taxInfo;
                } else {
                    this.showNotification('Failed to update tax information', 'error');
                }
            } catch (error) {
                console.error('Error updating tax info:', error);
                this.showNotification('An error occurred while updating tax information', 'error');
            }
        },
        
        formatPrice(amount, currency = 'GBP') {
            return new Intl.NumberFormat('en-GB', {
                style: 'currency',
                currency: currency
            }).format(amount);
        },
        
        formatDate(dateString) {
            return new Date(dateString).toLocaleDateString('en-GB', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });
        },
        
        getCardIcon(brand) {
            const icons = {
                'visa': 'fab fa-cc-visa text-blue-600',
                'mastercard': 'fab fa-cc-mastercard text-red-600',
                'amex': 'fab fa-cc-amex text-blue-500',
                'discover': 'fab fa-cc-discover text-orange-500',
                'diners': 'fab fa-cc-diners-club text-gray-700',
                'jcb': 'fab fa-cc-jcb text-green-600',
                'default': 'fas fa-credit-card text-gray-600'
            };
            return icons[brand.toLowerCase()] || icons.default;
        },
        
        // Session Management
        async loadSessions() {
            try {
                const response = await fetch('/api/v1/user/sessions', {
                    credentials: 'include'
                });
                if (response.ok) {
                    this.sessions = await response.json();
                } else {
                    console.error('Failed to load sessions');
                    this.sessions = [];
                }
            } catch (error) {
                console.error('Error loading sessions:', error);
                this.sessions = [];
            }
        },
        
        getDeviceIcon(deviceType) {
            const icons = {
                'desktop': 'fas fa-desktop',
                'mobile': 'fas fa-mobile-alt',
                'tablet': 'fas fa-tablet-alt',
                'iphone': 'fab fa-apple',
                'unknown': 'fas fa-question-circle'
            };
            return icons[deviceType.toLowerCase()] || icons.unknown;
        },
        
        formatLastActive(dateString) {
            if (!dateString) return 'Unknown';
            
            const date = new Date(dateString);
            const now = new Date();
            const diffMs = now - date;
            const diffMins = Math.floor(diffMs / 60000);
            
            if (diffMins < 1) return 'Just now';
            if (diffMins < 60) return `${diffMins} minutes ago`;
            
            const diffHours = Math.floor(diffMins / 60);
            if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
            
            const diffDays = Math.floor(diffHours / 24);
            if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
            
            return date.toLocaleDateString('en-GB');
        },
        
        async terminateSession(sessionId) {
            if (!confirm('Are you sure you want to sign out this device?')) {
                return;
            }
            
            try {
                const response = await fetch(`/api/v1/user/sessions/${sessionId}`, {
                    method: 'DELETE',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                
                if (response.ok) {
                    this.showNotification('Session terminated successfully', 'success');
                    await this.loadSessions();
                } else {
                    this.showNotification('Failed to terminate session', 'error');
                }
            } catch (error) {
                console.error('Error terminating session:', error);
                this.showNotification('An error occurred while terminating session', 'error');
            }
        },
        
        async terminateAllOtherSessions() {
            if (!confirm('Are you sure you want to sign out all other devices?')) {
                return;
            }
            
            try {
                const response = await fetch('/api/v1/user/sessions/terminate-all', {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                
                if (response.ok) {
                    this.showNotification('All other sessions terminated successfully', 'success');
                    await this.loadSessions();
                } else {
                    this.showNotification('Failed to terminate sessions', 'error');
                }
            } catch (error) {
                console.error('Error terminating sessions:', error);
                this.showNotification('An error occurred while terminating sessions', 'error');
            }
        },
        
        // Show notification
        showNotification(message, type = 'info') {
            // Create notification element
            const notification = document.createElement('div');
            notification.className = `fixed top-4 right-4 z-50 p-4 rounded-lg shadow-lg transition-all duration-300 transform translate-x-full`;
            
            // Set color based on type
            switch (type) {
                case 'success':
                    notification.classList.add('bg-green-500', 'text-white');
                    break;
                case 'error':
                    notification.classList.add('bg-red-500', 'text-white');
                    break;
                case 'warning':
                    notification.classList.add('bg-yellow-500', 'text-white');
                    break;
                default:
                    notification.classList.add('bg-blue-500', 'text-white');
            }
            
            // Set content
            notification.innerHTML = `
                <div class="flex items-center">
                    <i class="fas ${type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-exclamation-circle' : 'fa-info-circle'} mr-3"></i>
                    <span>${message}</span>
                </div>
            `;
            
            // Add to DOM
            document.body.appendChild(notification);
            
            // Animate in
            setTimeout(() => {
                notification.classList.remove('translate-x-full');
                notification.classList.add('translate-x-0');
            }, 100);
            
            // Remove after 5 seconds
            setTimeout(() => {
                notification.classList.add('translate-x-full');
                setTimeout(() => {
                    notification.remove();
                }, 300);
            }, 5000);
        }
    };
}