/**
 * Login Page JavaScript
 * Handles user authentication with form validation and token management
 */

// Login Form Component
function loginForm() {
    return {
        formData: {
            email: '',
            password: '',
            rememberMe: false
        },
        
        errors: {},
        isSubmitting: false,
        showPassword: false,
        
        init() {
            console.log('Login form initialized');
        },
        
        validateField(field) {
            this.errors[field] = '';
            
            switch (field) {
                case 'email':
                    if (!this.formData.email.trim()) {
                        this.errors.email = 'Email address is required';
                    } else if (!this.isValidEmail(this.formData.email)) {
                        this.errors.email = 'Please enter a valid email address';
                    }
                    break;
                    
                case 'password':
                    if (!this.formData.password) {
                        this.errors.password = 'Password is required';
                    }
                    break;
            }
        },
        
        isValidEmail(email) {
            const atSymbol = String.fromCharCode(64); // @ symbol
            return email.includes(atSymbol) && 
                   email.includes('.') && 
                   email.indexOf(atSymbol) > 0 && 
                   email.indexOf('.') > email.indexOf(atSymbol) + 1 &&
                   email.indexOf(atSymbol) === email.lastIndexOf(atSymbol) &&
                   !email.includes(' ');
        },
        
        async submitLogin() {
            console.log('submitLogin called');
            this.isSubmitting = true;
            
            // Validate form
            this.validateField('email');
            this.validateField('password');
            
            console.log('Form data:', this.formData);
            console.log('Validation errors:', this.errors);
            
            // Check if validation passed
            if (this.errors.email || this.errors.password) {
                this.isSubmitting = false;
                return;
            }
            
            try {
                console.log('Submitting login for:', this.formData.email);
                
                // Create form data for POST request
                const formData = new FormData();
                formData.append('email', this.formData.email);
                formData.append('password', this.formData.password);
                formData.append('rememberMe', this.formData.rememberMe);
                
                const response = await fetch('/Auth/Login', {
                    method: 'POST',
                    body: formData
                });
                
                const result = await response.json();
                console.log('Login response:', result);
                
                if (result.success && result.data) {
                    // Handle double-wrapped API response similar to registration
                    const loginData = result.data;
                    
                    if (loginData && loginData.success && loginData.data) {
                        // Login successful - store tokens and redirect
                        const userData = loginData.data;
                        
                        localStorage.setItem('accessToken', userData.token);
                        if (userData.refreshToken) {
                            localStorage.setItem('refreshToken', userData.refreshToken);
                        }
                        localStorage.setItem('user', JSON.stringify(userData.user));
                        
                        console.log('Login successful, redirecting user...');
                        
                        // Show success message and redirect
                        await showModal({
                            title: 'Welcome Back!',
                            message: 'You have been logged in successfully.',
                            type: 'success',
                            actions: [
                                { 
                                    text: 'Continue to Dashboard', 
                                    primary: true, 
                                    action: () => {
                                        // Redirect based on user type
                                        if (userData.user.userType === 'ServiceProvider' || userData.user.userType === '1') {
                                            window.location.href = '/ServiceProvider/Dashboard';
                                        } else {
                                            window.location.href = '/PetOwner/Dashboard';
                                        }
                                    }
                                }
                            ]
                        });
                    } else {
                        console.error('Invalid login response format:', loginData);
                        await showModal({
                            title: 'Login Failed',
                            message: 'Invalid response from server. Please try again.',
                            type: 'error',
                            actions: [
                                { text: 'OK', primary: true }
                            ]
                        });
                    }
                } else {
                    // Login failed - show error message
                    let errorMessage = 'Login failed. Please check your email and password.';
                    
                    if (result.message) {
                        errorMessage = result.message;
                    } else if (result.details && result.details.message) {
                        errorMessage = result.details.message;
                    }
                    
                    console.log('Login failed:', errorMessage);
                    
                    await showModal({
                        title: 'Login Failed',
                        message: errorMessage,
                        type: 'error',
                        actions: [
                            { text: 'OK', primary: true }
                        ]
                    });
                }
            } catch (error) {
                console.error('Login error:', error);
                
                await showModal({
                    title: 'Login Error',
                    message: 'Unable to connect to the server. Please check your internet connection and try again.',
                    type: 'error',
                    actions: [
                        { text: 'OK', primary: true }
                    ]
                });
            } finally {
                this.isSubmitting = false;
            }
        }
    };
}