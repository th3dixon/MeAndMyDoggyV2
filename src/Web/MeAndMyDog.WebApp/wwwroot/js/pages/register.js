/**
 * Register Page JavaScript
 * Handles user registration form with multi-step process
 */

// Address Lookup Component with Google Places API
function addressLookup() {
    return {
        searchTerm: '',
        results: [],
        selectedAddress: null,
        searching: false,
        showResults: false,
        searchTimeout: null,
        
        init() {
            // Watch for changes to searchTerm and debounce the search
            this.$watch('searchTerm', (newValue) => {
                this.debouncedSearch(newValue);
            });
        },
        
        debouncedSearch(query) {
            // Don't search if an address is already selected
            if (this.selectedAddress) {
                return;
            }
            
            // Clear existing timeout
            if (this.searchTimeout) {
                clearTimeout(this.searchTimeout);
            }
            
            // Hide results immediately if query is too short
            if (query.length < 3) {
                this.results = [];
                this.showResults = false;
                this.searching = false;
                return;
            }
            
            // Set loading state
            this.searching = true;
            
            // Debounce the actual search call
            this.searchTimeout = setTimeout(() => {
                this.performSearch(query);
            }, 300); // 300ms delay
        },
        
        async performSearch(query) {
            try {
                
                const response = await fetch(`/Search/GetAddressSuggestions?query=${encodeURIComponent(query)}&maxResults=8`);
                
                if (response.ok) {
                    const result = await response.json();
                    if (result.success && result.data) {
                        this.results = result.data;
                        this.showResults = this.results.length > 0;
                    } else {
                        this.results = [];
                        this.showResults = false;
                        console.warn('No address suggestions found');
                    }
                } else {
                    console.error('Address search failed:', response.status);
                    this.results = [];
                    this.showResults = false;
                }
            } catch (error) {
                console.error('Address search error:', error);
                this.results = [];
                this.showResults = false;
            } finally {
                this.searching = false;
            }
        },
        
        // Legacy search method for manual calls (kept for compatibility)
        async search() {
            this.debouncedSearch(this.searchTerm);
        },
        
        selectAddress(address) {
            this.selectedAddress = address;
            
            // Clear any pending search timeout FIRST to prevent reopening
            if (this.searchTimeout) {
                clearTimeout(this.searchTimeout);
                this.searchTimeout = null;
            }
            
            // Close results immediately
            this.showResults = false;
            this.results = [];
            
            // Set the search term (this will trigger watcher, but we've cleared timeout)
            this.searchTerm = address.displayText;
            
            
            // Prepare address data with extensive logging
            const addressData = {
                addressLine1: address.addressLine1 || '',
                addressLine2: address.addressLine2 || '',
                city: address.city || '',
                county: address.county || '',
                postCode: address.postcodeFormatted || '',
                latitude: address.latitude || null,
                longitude: address.longitude || null
            };
            
            
            // Use $dispatch to communicate with parent registerForm component
            this.$dispatch('address-selected', addressData);
        },
        
        clearSelection() {
            this.selectedAddress = null;
            this.searchTerm = '';
            this.results = [];
            this.showResults = false;
            
            // Clear any pending search timeout
            if (this.searchTimeout) {
                clearTimeout(this.searchTimeout);
                this.searchTimeout = null;
            }
            
            // Dispatch clear event to parent component
            this.$dispatch('address-cleared');
            
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
            services: {},
            agreeTerms: false
        },
        
        serviceCategories: [],
        loadingServices: true,
        
        errors: {},
        
        async init() {
            await this.loadServiceCategories();
            
            // Listen for address selection events from child addressLookup component
            this.$el.addEventListener('address-selected', (event) => {
                
                // Update form data with selected address
                this.formData.addressLine1 = event.detail.addressLine1;
                this.formData.addressLine2 = event.detail.addressLine2;
                this.formData.city = event.detail.city;
                this.formData.county = event.detail.county;
                this.formData.postCode = event.detail.postCode;
                this.formData.latitude = event.detail.latitude;
                this.formData.longitude = event.detail.longitude;
                this.formData.addressId = null; // Not applicable for Google Places
                
                // Trigger validation for populated fields
                if (this.formData.addressLine1) {
                    this.validateField('addressLine1');
                }
                if (this.formData.city) {
                    this.validateField('city');
                }
                if (this.formData.postCode) {
                    this.validateField('postCode');
                }
                
            });
            
            // Listen for address clear events
            this.$el.addEventListener('address-cleared', () => {
                
                // Clear form data
                this.formData.addressLine1 = '';
                this.formData.addressLine2 = '';
                this.formData.city = '';
                this.formData.county = '';
                this.formData.postCode = '';
                this.formData.addressId = null;
                this.formData.latitude = null;
                this.formData.longitude = null;
                
            });
        },
        
        async loadServiceCategories() {
            try {
                const response = await fetch('/Search/GetServiceCategories');
                const result = await response.json();
                
                if (result && result.success && result.data) {
                    // Handle double-wrapped API response: result.data contains the API response wrapper
                    const apiData = result.data;
                    
                    if (apiData && apiData.success && apiData.data && Array.isArray(apiData.data)) {
                        this.serviceCategories = apiData.data;
                    } else {
                        console.error('Failed to load service categories - API data format invalid:', apiData);
                        this.serviceCategories = this.getFallbackServiceCategories();
                    }
                } else {
                    console.error('Failed to load service categories - invalid response:', result);
                    this.serviceCategories = this.getFallbackServiceCategories();
                }
            } catch (error) {
                console.error('Error loading service categories:', error);
                // Use fallback service categories for offline development
                this.serviceCategories = this.getFallbackServiceCategories();
            }
            
            // Initialize services object structure (works for both API and fallback data)
            // Ensure serviceCategories is always an array
            if (!Array.isArray(this.serviceCategories)) {
                console.warn('serviceCategories is not an array, using fallback data');
                this.serviceCategories = this.getFallbackServiceCategories();
            }
            
            if (Array.isArray(this.serviceCategories) && this.serviceCategories.length > 0) {
                this.serviceCategories.forEach(category => {
                    this.formData.services[category.serviceCategoryId] = {
                        selected: false,
                        name: category.name,
                        subServices: {}
                    };
                    if (category.subServices && Array.isArray(category.subServices)) {
                        category.subServices.forEach(subService => {
                            this.formData.services[category.serviceCategoryId].subServices[subService.subServiceId] = {
                                selected: false,
                                name: subService.name,
                                rate: '',
                                suggestedMinPrice: subService.suggestedMinPrice,
                                suggestedMaxPrice: subService.suggestedMaxPrice
                            };
                        });
                    }
                });
            }
            
            this.loadingServices = false;
        },
        
        getFallbackServiceCategories() {
            // Fallback service categories for offline development
            return [
                {
                    serviceCategoryId: "1",
                    name: "Dog Walking",
                    subServices: [
                        { subServiceId: "1", name: "30 Minute Walk", suggestedMinPrice: 15, suggestedMaxPrice: 25 },
                        { subServiceId: "2", name: "60 Minute Walk", suggestedMinPrice: 25, suggestedMaxPrice: 40 }
                    ]
                },
                {
                    serviceCategoryId: "2", 
                    name: "Pet Sitting",
                    subServices: [
                        { subServiceId: "3", name: "House Sitting", suggestedMinPrice: 30, suggestedMaxPrice: 60 },
                        { subServiceId: "4", name: "Overnight Care", suggestedMinPrice: 50, suggestedMaxPrice: 100 }
                    ]
                },
                {
                    serviceCategoryId: "3",
                    name: "Dog Grooming", 
                    subServices: [
                        { subServiceId: "5", name: "Basic Wash & Dry", suggestedMinPrice: 20, suggestedMaxPrice: 40 },
                        { subServiceId: "6", name: "Full Grooming", suggestedMinPrice: 40, suggestedMaxPrice: 80 }
                    ]
                },
                {
                    serviceCategoryId: "4",
                    name: "Pet Training",
                    subServices: [
                        { subServiceId: "7", name: "Basic Obedience", suggestedMinPrice: 30, suggestedMaxPrice: 60 },
                        { subServiceId: "8", name: "Behavioral Training", suggestedMinPrice: 50, suggestedMaxPrice: 100 }
                    ]
                }
            ];
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
            if (this.formData.userType === '1') {
                return this.formData.postCode && !this.errors.postCode;
            }
            
            // Service providers need full address and business info
            if (this.formData.userType === '2' || this.formData.userType === '3') {
                return this.formData.postCode && !this.errors.postCode &&
                       this.formData.addressLine1 && !this.errors.addressLine1 &&
                       this.formData.city && !this.errors.city &&
                       this.formData.businessName && !this.errors.businessName &&
                       this.formData.businessType && !this.errors.businessType;
            }
            
            return false;
        },
        
        get isStep4Valid() {
            if (this.formData.userType === '1') return true;
            
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
                    if ((this.formData.userType === '2' || this.formData.userType === '3') && !this.formData.businessName.trim()) {
                        this.errors.businessName = 'Business name is required';
                    }
                    break;
                    
                case 'businessType':
                    if ((this.formData.userType === '2' || this.formData.userType === '3') && !this.formData.businessType) {
                        this.errors.businessType = 'Business type is required';
                    }
                    break;
                    
                case 'addressLine1':
                    if ((this.formData.userType === '2' || this.formData.userType === '3') && !this.formData.addressLine1.trim()) {
                        this.errors.addressLine1 = 'Address Line 1 is required for service providers';
                    }
                    break;
                    
                case 'city':
                    if ((this.formData.userType === '2' || this.formData.userType === '3') && !this.formData.city.trim()) {
                        this.errors.city = 'City is required for service providers';
                    }
                    break;
            }
        },
        
        validateServices() {
            this.errors.services = '';
            
            if (this.formData.userType === '1') return;
            
            const hasSelectedService = Object.values(this.formData.services).some(service => service.selected);
            if (!hasSelectedService) {
                this.errors.services = 'You must select at least one service category';
                return;
            }
            
            
            for (const serviceKey in this.formData.services) {
                const service = this.formData.services[serviceKey];
                if (service.selected) {
                    
                    const validSubServices = Object.values(service.subServices || {}).filter(subService => {
                        const rate = parseFloat(subService.rate);
                        const isValid = subService.selected && subService.rate && !isNaN(rate) && rate >= 1;
                        return isValid;
                    });
                    
                    
                    if (validSubServices.length === 0) {
                        this.errors.services = `Service "${service.name}" must have at least one sub-service with a rate of £1 or more`;
                        return;
                    }
                }
            }
            
        },
        
        nextStep() {
            if (this.currentStep === 3 && this.formData.userType === '1') {
                this.currentStep = 5;
            } else if (this.currentStep < 5) {
                this.currentStep++;
            }
        },
        
        prevStep() {
            if (this.currentStep === 5 && this.formData.userType === '1') {
                this.currentStep = 3;
            } else if (this.currentStep > 1) {
                this.currentStep--;
            }
        },
        
        getUserTypeText() {
            switch (this.formData.userType) {
                case '1': return 'Pet Owner';
                case '2': return 'Service Provider';
                case '3': return 'Both Pet Owner and Service Provider';
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
        
        async geocodePostcode(postcode) {
            // For pet owners who only provide postcode, geocode it to get coordinates
            try {
                // Use the existing address lookup API to get coordinates for postcode
                const response = await fetch(`/api/v1/addresslookup/search?q=${encodeURIComponent(postcode)}&max=1`);
                if (response.ok) {
                    const results = await response.json();
                    if (results && results.length > 0 && results[0].latitude && results[0].longitude) {
                        return {
                            latitude: results[0].latitude,
                            longitude: results[0].longitude
                        };
                    }
                }
            } catch (error) {
                console.warn('Failed to geocode postcode:', error);
            }
            return { latitude: null, longitude: null };
        },

        async submitForm() {
            this.isSubmitting = true;
            
            try {
                // Validate all form fields
                Object.keys(this.formData).forEach(field => {
                    if (field !== 'agreeTerms') {
                        this.validateField(field);
                    }
                });
                
                // Validate services separately (for service providers)
                this.validateServices();
                
                // Check if any validation errors exist
                if (Object.values(this.errors).some(error => error)) {
                    this.isSubmitting = false;
                    return;
                }
                
                // For pet owners who only have postcode, geocode it to get coordinates
                if (this.formData.userType === '1' && this.formData.postCode && !this.formData.latitude) {
                    const coords = await this.geocodePostcode(this.formData.postCode);
                    this.formData.latitude = coords.latitude;
                    this.formData.longitude = coords.longitude;
                }
                
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
                    latitude: this.formData.latitude || null,
                    longitude: this.formData.longitude || null,
                    userType: parseInt(this.formData.userType), // Convert to enum value
                    businessName: this.formData.businessName || null,
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

                if (response.ok && result.success && result.data && result.data.token) {
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
                                    localStorage.setItem('accessToken', result.data.token);
                                    localStorage.setItem('refreshToken', result.data.refreshToken);
                                    localStorage.setItem('user', JSON.stringify(result.data.user));
                                    
                                    // Redirect based on user type
                                    if (result.data.user.userType === 'ServiceProvider') {
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
                console.error('Error details:', {
                    name: error.name,
                    message: error.message,
                    stack: error.stack
                });
                
                // Show error modal for network or other errors
                await showModal({
                    title: 'Registration Failed',
                    message: `Unable to connect to the server: ${error.message}. Please check your internet connection and try again.`,
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
            if (this.formData.userType === '1') {
                // Pet owners don't have services
                return null;
            }

            const services = [];
            
            Object.keys(this.formData.services).forEach(categoryId => {
                const service = this.formData.services[categoryId];
                if (service.selected) {
                    // Collect sub-services with rates
                    const subServices = [];
                    if (service.subServices) {
                        Object.keys(service.subServices).forEach(subServiceId => {
                            const subService = service.subServices[subServiceId];
                            if (subService.selected && subService.rate && parseFloat(subService.rate) > 0) {
                                subServices.push({
                                    subServiceId: subServiceId,
                                    price: parseFloat(subService.rate),
                                    pricingType: 1 // PricingType.PerService = 1
                                });
                            }
                        });
                    }

                    const serviceData = {
                        serviceCategoryId: categoryId,
                        offersEmergencyService: false, // Default to false, can be updated later
                        offersWeekendService: false,   // Default to false, can be updated later
                        offersEveningService: false,   // Default to false, can be updated later
                        subServices: subServices
                    };
                    
                    services.push(serviceData);
                }
            });

            return services.length > 0 ? services : null;
        }
    };
}