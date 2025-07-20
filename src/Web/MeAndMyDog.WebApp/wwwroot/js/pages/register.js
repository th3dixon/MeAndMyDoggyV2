/**
 * Register Page JavaScript
 * Handles user registration form with multi-step process
 */

// Address Lookup Component
function addressLookup() {
    return {
        searchTerm: '',
        results: [],
        selectedAddress: null,
        searching: false,
        showResults: false,
        
        async search() {
            if (this.searchTerm.length < 3) {
                this.results = [];
                return;
            }
            
            this.searching = true;
            try {
                const response = await fetch(`/api/v1/addresslookup/search?q=${encodeURIComponent(this.searchTerm)}&max=10`);
                if (response.ok) {
                    this.results = await response.json();
                    this.showResults = true;
                } else {
                    this.results = [];
                }
            } catch (error) {
                console.error('Address search error:', error);
                this.results = [];
            } finally {
                this.searching = false;
            }
        },
        
        selectAddress(address) {
            this.selectedAddress = address;
            this.searchTerm = address.displayText;
            
            // Update form data in parent component
            const parentData = this.$root.formData;
            parentData.addressLine1 = address.addressLine1;
            parentData.addressLine2 = address.addressLine2 || '';
            parentData.city = address.city;
            parentData.county = address.county;
            parentData.postCode = address.postcodeFormatted;
            
            // Store additional data for potential use
            parentData.addressId = address.addressId;
            parentData.latitude = address.latitude;
            parentData.longitude = address.longitude;
            
            this.showResults = false;
            
            // Trigger validation for postcode
            this.$root.validateField('postCode');
        },
        
        clearSelection() {
            this.selectedAddress = null;
            this.searchTerm = '';
            this.results = [];
            
            // Clear form data in parent component
            const parentData = this.$root.formData;
            parentData.addressLine1 = '';
            parentData.addressLine2 = '';
            parentData.city = '';
            parentData.county = '';
            parentData.postCode = '';
            parentData.addressId = null;
            parentData.latitude = null;
            parentData.longitude = null;
        }
    };
}

// Main Registration Form Component
function registerForm() {
    return {
        currentStep: 1,
        isSubmitting: false,
        showPassword: false,
        showConfirmPassword: false,
        
        formData: {
            userType: '',
            firstName: '',
            lastName: '',
            email: '',
            phoneNumber: '',
            password: '',
            confirmPassword: '',
            postCode: '',
            city: '',
            addressLine1: '',
            addressLine2: '',
            county: '',
            addressId: null,
            latitude: null,
            longitude: null,
            businessName: '',
            businessType: '',
            companyNumber: '',
            vatNumber: '',
            services: {},
            agreeTerms: false
        },
        
        serviceCategories: [],
        loadingServices: true,
        
        errors: {},
        
        async init() {
            await this.loadServiceCategories();
        },
        
        async loadServiceCategories() {
            try {
                const response = await fetch('/api/v1/servicecatalog/public/categories');
                const result = await response.json();
                
                if (result.isSuccess) {
                    this.serviceCategories = result.data;
                    // Initialize services object structure
                    this.serviceCategories.forEach(category => {
                        this.formData.services[category.serviceCategoryId] = {
                            selected: false,
                            name: category.name,
                            subServices: {}
                        };
                        category.subServices.forEach(subService => {
                            this.formData.services[category.serviceCategoryId].subServices[subService.subServiceId] = {
                                selected: false,
                                name: subService.name,
                                rate: '',
                                suggestedMinPrice: subService.suggestedMinPrice,
                                suggestedMaxPrice: subService.suggestedMaxPrice
                            };
                        });
                    });
                } else {
                    console.error('Failed to load service categories:', result.message);
                }
            } catch (error) {
                console.error('Error loading service categories:', error);
            } finally {
                this.loadingServices = false;
            }
        },
        
        get passwordChecks() {
            return {
                minLength: this.formData.password.length >= 8,
                hasUppercase: this.hasUppercase(this.formData.password),
                hasLowercase: this.hasLowercase(this.formData.password),
                hasDigit: this.hasDigit(this.formData.password)
            };
        },
        
        get passwordStrength() {
            const checks = this.passwordChecks;
            const score = Object.values(checks).filter(Boolean).length;
            
            if (score === 0) return { class: 'password-strength-weak', textClass: 'text-red-500', text: 'Very Weak' };
            if (score === 1) return { class: 'password-strength-weak', textClass: 'text-red-500', text: 'Weak' };
            if (score === 2) return { class: 'password-strength-fair', textClass: 'text-yellow-500', text: 'Fair' };
            if (score === 3) return { class: 'password-strength-good', textClass: 'text-green-500', text: 'Good' };
            if (score === 4) return { class: 'password-strength-strong', textClass: 'text-green-600', text: 'Strong' };
        },
        
        get isStep2Valid() {
            return this.formData.firstName && 
                   this.formData.lastName && 
                   this.formData.email && 
                   this.formData.password && 
                   this.formData.confirmPassword &&
                   !this.errors.firstName &&
                   !this.errors.lastName &&
                   !this.errors.email &&
                   !this.errors.password &&
                   !this.errors.confirmPassword;
        },
        
        get isStep3Valid() {
            // Pet owners only need postcode
            if (this.formData.userType === '0') {
                return this.formData.postCode && !this.errors.postCode;
            }
            
            // Service providers need full address and business info
            if (this.formData.userType === '1' || this.formData.userType === '2') {
                return this.formData.postCode && !this.errors.postCode &&
                       this.formData.addressLine1 && !this.errors.addressLine1 &&
                       this.formData.city && !this.errors.city &&
                       this.formData.businessName && !this.errors.businessName &&
                       this.formData.businessType && !this.errors.businessType;
            }
            
            return false;
        },
        
        get isStep4Valid() {
            if (this.formData.userType === '0') return true;
            
            const hasSelectedService = Object.values(this.formData.services).some(service => service.selected);
            if (!hasSelectedService) return false;
            
            for (const serviceKey in this.formData.services) {
                const service = this.formData.services[serviceKey];
                if (service.selected) {
                    const hasValidSubService = Object.values(service.subServices || {}).some(subService => {
                        const rate = parseFloat(subService.rate);
                        return subService.selected && subService.rate && !isNaN(rate) && rate >= 1;
                    });
                    if (!hasValidSubService) return false;
                }
            }
            
            return !this.errors.services;
        },
        
        validateField(field) {
            this.errors[field] = '';
            
            switch (field) {
                case 'firstName':
                    if (!this.formData.firstName.trim()) {
                        this.errors.firstName = 'First name is required';
                    } else if (this.formData.firstName.length < 2) {
                        this.errors.firstName = 'First name must be at least 2 characters';
                    }
                    break;
                    
                case 'lastName':
                    if (!this.formData.lastName.trim()) {
                        this.errors.lastName = 'Last name is required';
                    } else if (this.formData.lastName.length < 2) {
                        this.errors.lastName = 'Last name must be at least 2 characters';
                    }
                    break;
                    
                case 'email':
                    if (!this.formData.email.trim()) {
                        this.errors.email = 'Email is required';
                    } else if (!this.isValidEmail(this.formData.email)) {
                        this.errors.email = 'Please enter a valid email address';
                    }
                    break;
                    
                case 'phoneNumber':
                    if (this.formData.phoneNumber && this.formData.phoneNumber.length < 10) {
                        this.errors.phoneNumber = 'Please enter a valid phone number';
                    }
                    break;
                    
                case 'password':
                    if (!this.formData.password) {
                        this.errors.password = 'Password is required';
                    } else if (this.formData.password.length < 8) {
                        this.errors.password = 'Password must be at least 8 characters';
                    } else if (!Object.values(this.passwordChecks).every(check => check)) {
                        this.errors.password = 'Password must meet all requirements';
                    }
                    break;
                    
                case 'confirmPassword':
                    if (!this.formData.confirmPassword) {
                        this.errors.confirmPassword = 'Please confirm your password';
                    } else if (this.formData.password !== this.formData.confirmPassword) {
                        this.errors.confirmPassword = 'Passwords do not match';
                    }
                    break;
                    
                case 'postCode':
                    if (!this.formData.postCode.trim()) {
                        this.errors.postCode = 'Post code is required';
                    } else if (!this.isValidPostCode(this.formData.postCode.trim())) {
                        this.errors.postCode = 'Please enter a valid UK post code';
                    }
                    break;
                    
                case 'businessName':
                    if ((this.formData.userType === '1' || this.formData.userType === '2') && !this.formData.businessName.trim()) {
                        this.errors.businessName = 'Business name is required';
                    }
                    break;
                    
                case 'businessType':
                    if ((this.formData.userType === '1' || this.formData.userType === '2') && !this.formData.businessType) {
                        this.errors.businessType = 'Business type is required';
                    }
                    break;
                    
                case 'addressLine1':
                    if ((this.formData.userType === '1' || this.formData.userType === '2') && !this.formData.addressLine1.trim()) {
                        this.errors.addressLine1 = 'Address Line 1 is required for service providers';
                    }
                    break;
                    
                case 'city':
                    if ((this.formData.userType === '1' || this.formData.userType === '2') && !this.formData.city.trim()) {
                        this.errors.city = 'City is required for service providers';
                    }
                    break;
            }
        },
        
        validateServices() {
            this.errors.services = '';
            
            if (this.formData.userType === '0') return;
            
            const hasSelectedService = Object.values(this.formData.services).some(service => service.selected);
            if (!hasSelectedService) {
                this.errors.services = 'You must select at least one service category';
                return;
            }
            
            for (const serviceKey in this.formData.services) {
                const service = this.formData.services[serviceKey];
                if (service.selected) {
                    const hasValidSubService = Object.values(service.subServices || {}).some(subService => {
                        const rate = parseFloat(subService.rate);
                        return subService.selected && subService.rate && !isNaN(rate) && rate >= 1;
                    });
                    if (!hasValidSubService) {
                        this.errors.services = `Each selected service must have at least one sub-service with a rate of £1 or more`;
                        return;
                    }
                }
            }
        },
        
        nextStep() {
            if (this.currentStep === 3 && this.formData.userType === '0') {
                this.currentStep = 5;
            } else if (this.currentStep < 5) {
                this.currentStep++;
            }
        },
        
        prevStep() {
            if (this.currentStep === 5 && this.formData.userType === '0') {
                this.currentStep = 3;
            } else if (this.currentStep > 1) {
                this.currentStep--;
            }
        },
        
        getUserTypeText() {
            switch (this.formData.userType) {
                case '0': return 'Pet Owner';
                case '1': return 'Service Provider';
                case '2': return 'Both Pet Owner and Service Provider';
                default: return '';
            }
        },
        
        // Helper functions for validation
        isValidEmail(email) {
            const atSymbol = String.fromCharCode(64); // at symbol
            return email.includes(atSymbol) && 
                   email.includes('.') && 
                   email.indexOf(atSymbol) > 0 && 
                   email.indexOf('.') > email.indexOf(atSymbol) + 1 &&
                   email.indexOf(atSymbol) === email.lastIndexOf(atSymbol) &&
                   !email.includes(' ');
        },
        
        hasUppercase(password) {
            return password !== password.toLowerCase();
        },
        
        hasLowercase(password) {
            return password !== password.toUpperCase();
        },
        
        hasDigit(password) {
            return '0123456789'.split('').some(digit => password.includes(digit));
        },
        
        isValidPostCode(postCode) {
            // Basic UK postcode validation - simplified approach
            const cleaned = postCode.toUpperCase().replace(' ', '');
            if (cleaned.length < 5 || cleaned.length > 7) return false;
            
            // Check first character is letter
            const firstChar = cleaned.charAt(0);
            if (firstChar < 'A' || firstChar > 'Z') return false;
            
            // Check has digits
            const hasDigit = '0123456789'.split('').some(digit => cleaned.includes(digit));
            if (!hasDigit) return false;
            
            // Check last two characters are letters
            const lastTwo = cleaned.slice(-2);
            return lastTwo.length === 2 && 
                   lastTwo.charAt(0) >= 'A' && lastTwo.charAt(0) <= 'Z' &&
                   lastTwo.charAt(1) >= 'A' && lastTwo.charAt(1) <= 'Z';
        },
        
        async submitForm() {
            this.isSubmitting = true;
            
            Object.keys(this.formData).forEach(field => {
                if (field !== 'agreeTerms') {
                    this.validateField(field);
                }
            });
            
            if (Object.values(this.errors).some(error => error)) {
                this.isSubmitting = false;
                return;
            }
            
            try {
                // Prepare registration data for API
                const registrationData = {
                    email: this.formData.email,
                    password: this.formData.password,
                    firstName: this.formData.firstName,
                    lastName: this.formData.lastName,
                    phoneNumber: this.formData.phoneNumber || null,
                    postCode: this.formData.postCode,
                    city: this.formData.city || null,
                    addressLine1: this.formData.addressLine1 || null,
                    addressLine2: this.formData.addressLine2 || null,
                    county: this.formData.county || null,
                    userType: parseInt(this.formData.userType), // Convert to enum value
                    businessName: this.formData.businessName || null,
                    companyNumber: this.formData.companyNumber || null,
                    vatNumber: this.formData.vatNumber || null,
                    services: this.prepareServicesData()
                };

                // Call API registration endpoint
                const response = await fetch('/api/v1/auth/register', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(registrationData)
                });

                const result = await response.json();

                if (response.ok && result.token) {
                    // Registration successful
                    await showModal({
                        title: 'Registration Successful!',
                        message: 'Your account has been created successfully. You are now logged in.',
                        type: 'success',
                        actions: [
                            { 
                                text: 'Continue to Dashboard', 
                                primary: true, 
                                action: () => {
                                    // Store tokens and redirect
                                    localStorage.setItem('accessToken', result.token);
                                    localStorage.setItem('refreshToken', result.refreshToken);
                                    localStorage.setItem('user', JSON.stringify(result.user));
                                    
                                    // Redirect based on user type
                                    if (result.user.userType === 'ServiceProvider') {
                                        window.location.href = '/ServiceProvider/Dashboard';
                                    } else {
                                        window.location.href = '/PetOwner/Dashboard';
                                    }
                                }
                            }
                        ]
                    });
                } else {
                    // Registration failed - show specific errors
                    let errorMessage = 'There was an error processing your registration.';
                    
                    if (result.errors && Array.isArray(result.errors)) {
                        errorMessage = result.errors.join('\n');
                    } else if (result.message) {
                        errorMessage = result.message;
                    }

                    await showModal({
                        title: 'Registration Failed',
                        message: errorMessage,
                        type: 'error',
                        actions: [
                            { text: 'OK', primary: true }
                        ]
                    });
                }
            } catch (error) {
                console.error('Registration error:', error);
                
                // Show error modal for network or other errors
                await showModal({
                    title: 'Registration Failed',
                    message: 'Unable to connect to the server. Please check your internet connection and try again.',
                    type: 'error',
                    actions: [
                        { text: 'OK', primary: true }
                    ]
                });
            } finally {
                this.isSubmitting = false;
            }
        },

        prepareServicesData() {
            if (this.formData.userType === '0') {
                // Pet owners don't have services
                return null;
            }

            const services = [];
            
            Object.keys(this.formData.services).forEach(categoryId => {
                const service = this.formData.services[categoryId];
                if (service.selected) {
                    const serviceData = {
                        serviceCategoryId: categoryId,
                        offersEmergencyService: false, // TODO: Add UI for these options
                        offersWeekendService: false,
                        offersEveningService: false
                    };
                    
                    services.push(serviceData);
                }
            });

            return services.length > 0 ? services : null;
        }
    };
}