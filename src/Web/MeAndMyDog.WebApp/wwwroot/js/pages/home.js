/**
 * Home Page JavaScript
 * Handles search functionality and dynamic service loading
 */

// Premium Search Widget Component
function premiumSearchWidget() {
    return {
        userPostcode: '',
        showInstantResults: false,
        selectedServices: [],
        loadingTopProviders: false,
        topProviders: [],
        petCount: 1,
        location: '',
        dateRange: '',
        datePickerInstance: null,
        selectedServiceIcon: '',
        selectedServiceName: '',
        availableProviders: 0,
        
        // Carousel properties
        activeProviderIndex: 0,
        carouselContainer: null,
        touchStartX: 0,
        touchEndX: 0,
        priceRange: '',
        availability: '',
        topProvider: null,
        loadingServices: true,
        postcodeTimeout: null,
        searchTimeout: null,
        isSignedIn: false, // Authentication status - TODO: Update from server-side data
        
        // Dynamic services from API
        services: [],
        serviceCategories: [],
        

        async init() {
            // Load default services immediately so user sees something
            this.loadDefaultServices();
            
            // Try to load services from API in background
            try {
                await this.loadServiceCategories();
            } catch (error) {
                console.warn('Failed to load API services, using defaults:', error);
                // Default services are already loaded
            }
            
            // Set up periodic check for autocompleted postcodes
            setInterval(() => {
                this.checkButtonState();
            }, 500);
            
            // Initialize date picker when the component loads
            this.$nextTick(() => {
                if (this.$refs.dateRange) {
                    this.datePickerInstance = flatpickr(this.$refs.dateRange, {
                        mode: 'range',
                        dateFormat: 'd M Y',
                        minDate: 'today',
                        allowInput: true,
                        theme: 'light',
                        locale: {
                            rangeSeparator: ' to '
                        },
                        onChange: (selectedDates, dateStr) => {
                            this.dateRange = dateStr;
                            this.onDateRangeChange();
                        }
                    });
                }

                // Check if postcode is already populated on page load and trigger search if needed
                this.checkInitialPostcode();
            });
        },

        checkInitialPostcode() {
            // Check if postcode field already has a value (from browser autofill, etc.)
            if (this.userPostcode && this.userPostcode.length >= 3 && this.selectedServices.length > 0) {
                console.log('Initial postcode detected:', this.userPostcode);
                setTimeout(() => {
                    this.performQuickSearch();
                }, 100); // Small delay to ensure everything is initialized
            }
        },

        async loadServiceCategories() {
            try {
                const response = await fetch('/Search/GetServiceCategories');
                
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                
                const result = await response.json();
                
                if (result && result.success && result.data) {
                    // Handle double-wrapped API response: result.data contains the API response wrapper
                    const apiData = result.data;
                    
                    if (apiData && apiData.success && apiData.data && Array.isArray(apiData.data)) {
                        this.serviceCategories = apiData.data;
                        
                        // Transform categories to flat services array for the UI
                        this.services = this.serviceCategories.map(category => ({
                            id: category.serviceCategoryId,
                            name: category.name,
                            icon: this.getCategoryIcon(category.name),
                            price: this.getCategoryPriceRange(category),
                            description: category.description,
                            subServices: category.subServices
                        }));
                        
                        console.log('Loaded API services successfully:', this.services.length);
                    } else {
                        throw new Error('API data is not in expected format or is not an array');
                    }
                } else {
                    throw new Error(result?.message || 'Invalid API response');
                }
            } catch (error) {
                console.warn('API services unavailable, using defaults:', error.message);
                // Ensure serviceCategories is always an array
                if (!Array.isArray(this.serviceCategories)) {
                    this.serviceCategories = [];
                }
                // Don't overwrite default services that are already loaded
            } finally {
                this.loadingServices = false;
            }
        },

        getCategoryIcon(categoryName) {
            // Map category names to appropriate icons
            const iconMap = {
                'Dog Walking': '🚶‍♂️',
                'Pet Sitting': '🏠',
                'Grooming': '✂️',
                'Training': '🎓',
                'Emergency Care': '🚨',
                'Vet Visits': '🏥',
                'Boarding': '🏨',
                'Pet Taxi': '🚗',
                'Daycare': '🌞',
                'Photography': '📷'
            };
            
            return iconMap[categoryName] || '🐾';
        },

        getCategoryPriceRange(category) {
            if (!category.subServices || category.subServices.length === 0) {
                return 'Custom pricing';
            }
            
            const prices = category.subServices
                .filter(sub => sub.suggestedMinPrice && sub.suggestedMaxPrice)
                .map(sub => ({
                    min: sub.suggestedMinPrice,
                    max: sub.suggestedMaxPrice
                }));
            
            if (prices.length === 0) {
                return 'Custom pricing';
            }
            
            const minPrice = Math.min(...prices.map(p => p.min));
            const maxPrice = Math.max(...prices.map(p => p.max));
            
            return `£${minPrice}-${maxPrice}`;
        },


        loadDefaultServices() {
            // FALLBACK DATA ONLY - Used when /Search/GetServiceCategories API fails
            // This ensures the page remains functional for offline development
            // Pricing ranges based on UK market research (last updated: 2024)
            console.warn('Loading fallback service data - API may be unavailable');
            this.services = [
                { id: 'walking', name: 'Dog Walking', icon: '🚶‍♂️', price: '£15-30' },
                { id: 'sitting', name: 'Pet Sitting', icon: '🏠', price: '£25-50' },
                { id: 'grooming', name: 'Grooming', icon: '✂️', price: '£35-85' },
                { id: 'training', name: 'Training', icon: '🎓', price: '£45-75' },
                { id: 'emergency', name: 'Emergency Care', icon: '🚨', price: '£60+' },
                { id: 'vet', name: 'Vet Visits', icon: '🏥', price: '£40+' },
                { id: 'boarding', name: 'Boarding', icon: '🏨', price: '£40+' },
                { id: 'taxi', name: 'Pet Taxi', icon: '🚗', price: '£20+' }
            ];
        },


        selectService(service) {
            // Toggle service selection
            const index = this.selectedServices.indexOf(service.id);
            if (index === -1) {
                this.selectedServices.push(service.id);
            } else {
                this.selectedServices.splice(index, 1);
            }
            
            // Update selected service info for display
            if (this.selectedServices.length > 0) {
                this.selectedServiceIcon = service.icon;
                this.selectedServiceName = service.name;
                this.priceRange = service.price;
                
                // Auto-search: If user has postcode, perform quick search for top 3 providers
                if (this.userPostcode && this.userPostcode.length >= 3) {
                    // Small delay to prevent rapid-fire searches while user is selecting multiple services
                    clearTimeout(this.searchTimeout);
                    this.searchTimeout = setTimeout(() => {
                        this.performQuickSearch();
                    }, 300);
                }
            } else {
                // Clear results when no services selected
                this.topProviders = [];
                this.showInstantResults = false;
                this.availableProviders = 0;
            }
        },

        onPostcodeChange() {
            // Debounced postcode input - trigger auto-search after user stops typing
            clearTimeout(this.postcodeTimeout);
            this.postcodeTimeout = setTimeout(() => {
                if (this.userPostcode && this.userPostcode.length >= 3 && this.selectedServices.length > 0) {
                    this.performQuickSearch();
                }
            }, 500);
        },

        onSearchOptionsChange() {
            // Trigger provider loading when search options change (auto-search)
            if (this.userPostcode && this.selectedServices.length > 0) {
                this.performQuickSearch();
            }
        },

        onPetCountChange() {
            // Auto-search when pet count changes
            if (this.userPostcode && this.selectedServices.length > 0) {
                this.performQuickSearch();
            }
        },

        onDateRangeChange() {
            // Auto-search when date range changes
            if (this.userPostcode && this.selectedServices.length > 0) {
                this.performQuickSearch();
            }
        },

        async performQuickSearch() {
            // Quick search for top 3 providers using shared utilities
            if (!this.userPostcode) {
                await showModal({
                    title: 'Enter Your Postcode',
                    message: 'Please enter your postcode so we can find pet care providers near you.',
                    type: 'info',
                    actions: [{ text: 'OK', primary: true }]
                });
                return;
            }

            if (this.selectedServices.length === 0) {
                await showModal({
                    title: 'Select a Service',
                    message: 'Please select at least one pet service to continue your search.',
                    type: 'info',
                    actions: [{ text: 'OK', primary: true }]
                });
                return;
            }

            // Always show loading state and results container
            this.loadingTopProviders = true;
            this.topProviders = [];
            this.showInstantResults = true; // Show container immediately for visual feedback
            
            try {
                // Add small delay to ensure user sees loading state
                await new Promise(resolve => setTimeout(resolve, 200));
                
                // Properly geocode the postcode using multiple methods
                let coordinates = await this.geocodePostcode(this.userPostcode);

                console.log('Geocoded coordinates for', this.userPostcode, ':', coordinates);

                // Search for top 3 providers
                const searchParams = {
                    latitude: coordinates.lat,
                    longitude: coordinates.lng,
                    radiusMiles: 25,
                    serviceCategories: this.selectedServices,
                    maxResults: 3
                };

                let providers = [];
                let searchSuccessful = false;
                
                if (window.providerSearchUtils) {
                    try {
                        // Use unified search method (same as Search page)
                        const searchFilters = {
                            latitude: coordinates.lat,
                            longitude: coordinates.lng,
                            radiusMiles: 25,
                            serviceCategoryIds: this.selectedServices,
                            pageSize: 3,
                            sortBy: 'distance'
                        };
                        
                        const searchResults = await window.providerSearchUtils.searchProviders(searchFilters);
                        providers = searchResults ? searchResults.results || [] : [];
                        searchSuccessful = true;
                    } catch (utilsError) {
                        console.warn('Search utils failed, trying fallback:', utilsError);
                    }
                }
                
                if (!searchSuccessful) {
                    // Fallback to existing method
                    const response = await fetch(`/Search/GetNearby?latitude=${coordinates.lat}&longitude=${coordinates.lng}&radiusMiles=25&serviceCategories=${this.selectedServices.join(',')}&maxResults=3`);
                    if (response.ok) {
                        const result = await response.json();
                        providers = result.success ? result.data : [];
                        searchSuccessful = true;
                    } else {
                        throw new Error(`Search API returned ${response.status}: ${response.statusText}`);
                    }
                }

                console.log('Search results:', providers);

                // Transform providers for display
                this.topProviders = providers.map(provider => ({
                    id: provider.id,
                    businessName: provider.businessName || provider.providerName || 'Professional Provider',
                    initials: this.getInitials(provider.businessName || provider.providerName),
                    rating: provider.rating ? provider.rating.toFixed(1) : null,
                    reviewCount: provider.reviewCount > 0 ? provider.reviewCount : null,
                    description: provider.description || 'Professional pet care services',
                    minPrice: provider.priceRange?.minPrice || 25,
                    distance: provider.distanceMiles ? `${provider.distanceMiles.toFixed(1)} miles` : 'Near you',
                    lastJobText: this.formatLastJobDate(provider.lastJobCompletedDate),
                    isVerified: provider.isVerified || false,
                    rawData: provider
                }));

                this.availableProviders = this.topProviders.length;
                
                // Set loading to false BEFORE logging (fix timing issue)
                this.loadingTopProviders = false;
                
                // Log search completion for debugging
                console.log(`Quick search completed: ${this.topProviders.length} providers found`);
                console.log('Debug state:', {
                    loadingTopProviders: this.loadingTopProviders,
                    showInstantResults: this.showInstantResults, 
                    topProvidersLength: this.topProviders.length,
                    shouldShowNoResults: !this.loadingTopProviders && this.topProviders.length === 0 && this.showInstantResults
                });

            } catch (error) {
                console.error('Quick search failed:', error);
                
                // Show user-friendly error modal if search completely fails
                if (error.message.includes('fetch')) {
                    await showModal({
                        title: 'Connection Error',
                        message: 'Unable to search for providers due to a connection issue. Please check your internet connection and try again.',
                        type: 'warning',
                        actions: [{ text: 'OK', primary: true }]
                    });
                } else if (error.message.includes('geocod')) {
                    await showModal({
                        title: 'Location Error',
                        message: 'Unable to find your location. Please check your postcode spelling or try a nearby area.',
                        type: 'warning',
                        actions: [{ text: 'OK', primary: true }]
                    });
                }
                
                this.topProviders = [];
                this.availableProviders = 0;
                this.loadingTopProviders = false; // Set loading to false in error case too
                // Keep showInstantResults true so "no results" message appears
                
                console.log('Error state debug:', {
                    loadingTopProviders: this.loadingTopProviders,
                    showInstantResults: this.showInstantResults, 
                    topProvidersLength: this.topProviders.length,
                    shouldShowNoResults: !this.loadingTopProviders && this.topProviders.length === 0 && this.showInstantResults
                });
            } finally {
                // Ensure loading is false (redundant now, but safe)
                this.loadingTopProviders = false;
            }
        },

        async geocodePostcode(postcode) {
            let searchLat = null;
            let searchLng = null;
            
            // Method 1: Try internal API first
            try {
                const locationResponse = await fetch(`/Search/GetLocationSuggestions?query=${encodeURIComponent(postcode)}&maxResults=1`);
                if (locationResponse.ok) {
                    const result = await locationResponse.json();
                    if (result.success && result.data && result.data.length > 0) {
                        const location = result.data[0];
                        if (location.latitude && location.longitude) {
                            searchLat = location.latitude;
                            searchLng = location.longitude;
                            console.log('Geocoded using internal API:', searchLat, searchLng);
                            return { lat: searchLat, lng: searchLng };
                        }
                    }
                }
            } catch (error) {
                console.warn('Internal geocoding API failed:', error);
            }
            
            // Method 2: Try Google Geocoding if available
            if (typeof google !== 'undefined' && google.maps && google.maps.Geocoder) {
                try {
                    const geocoder = new google.maps.Geocoder();
                    
                    const result = await new Promise((resolve, reject) => {
                        geocoder.geocode({ address: postcode + ', UK' }, (results, status) => {
                            if (status === 'OK' && results[0]) {
                                searchLat = results[0].geometry.location.lat();
                                searchLng = results[0].geometry.location.lng();
                                console.log('Geocoded using Google Maps API:', searchLat, searchLng);
                                resolve({ lat: searchLat, lng: searchLng });
                            } else {
                                console.warn('Google Geocoding failed:', status);
                                reject(new Error('Google Geocoding failed'));
                            }
                        });
                    });
                    return result;
                } catch (error) {
                    console.warn('Google geocoding failed:', error);
                }
            }
            
            // Method 3: Fallback to approximate coordinates
            const coordinates = this.getApproximateUKCoordinates(postcode);
            console.log('Using approximate UK coordinates:', coordinates.lat, coordinates.lng);
            return coordinates;
        },

        async performFullSearch() {
            // Validate required fields before redirecting
            if (this.selectedServices.length === 0) {
                await showModal({
                    title: 'Select a Service',
                    message: 'Please select at least one pet service to continue your search.',
                    type: 'info',
                    actions: [{ text: 'OK', primary: true }]
                });
                return;
            }
            
            if (!this.userPostcode) {
                await showModal({
                    title: 'Enter Your Postcode',
                    message: 'Please enter your postcode so we can find pet care providers near you.',
                    type: 'info',
                    actions: [{ text: 'OK', primary: true }]
                });
                return;
            }

            // Build search parameters and redirect to /Search page
            const searchParams = {
                location: this.userPostcode,
                serviceCategories: this.selectedServices,
                petCount: this.petCount,
                includeAvailability: true,
                radiusMiles: 25
            };

            this.redirectToSearchPage(searchParams);
        },


        async contactProvider(provider) {
            if (!provider) {
                console.error('Invalid provider for contact');
                return;
            }

            try {
                // Use shared utilities if available
                if (window.providerSearchUtils && typeof window.providerSearchUtils.openContactModal === 'function') {
                    await window.providerSearchUtils.openContactModal(provider.rawData || provider);
                } else {
                    // Fallback contact modal
                    await showModal({
                        title: 'Contact Provider',
                        content: `
                            <div class="text-center mb-6">
                                <div class="w-16 h-16 bg-pet-orange rounded-full flex items-center justify-center text-white font-bold text-xl mx-auto mb-4">
                                    ${provider.initials}
                                </div>
                                <h3 class="text-xl font-semibold text-gray-900">${provider.businessName}</h3>
                                <p class="text-gray-600">${provider.description}</p>
                            </div>
                            
                            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                                <button class="bg-pet-orange text-white px-6 py-3 rounded-lg font-medium hover:bg-orange-600 transition-colors flex items-center justify-center">
                                    <i class="fas fa-phone mr-2"></i>
                                    Call Provider
                                </button>
                                <button class="bg-pet-blue text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-600 transition-colors flex items-center justify-center">
                                    <i class="fas fa-envelope mr-2"></i>
                                    Send Message
                                </button>
                            </div>
                            
                            <div class="bg-gray-50 rounded-lg p-4">
                                <h4 class="font-semibold text-gray-900 mb-2">What would you like to know?</h4>
                                <textarea class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-pet-orange focus:border-transparent resize-none" 
                                         rows="3" 
                                         placeholder="Hi ${provider.businessName}, I'm interested in your pet care services..."></textarea>
                                <div class="flex justify-end mt-4">
                                    <button class="bg-pet-orange text-white px-6 py-2 rounded-lg font-medium hover:bg-orange-600 transition-colors">
                                        Send Enquiry
                                    </button>
                                </div>
                            </div>
                        `,
                        type: 'custom',
                        actions: [
                            { text: 'Close', action: () => {} }
                        ]
                    });
                }
            } catch (error) {
                console.error('Error opening contact modal:', error);
                await showModal({
                    title: 'Contact Error',
                    message: 'Unable to open contact form at this time. Please try again later.',
                    type: 'error',
                    actions: [{ text: 'OK', primary: true }]
                });
            }
        },

        toggleService(service) {
            const index = this.selectedServices.indexOf(service.id);
            if (index === -1) {
                this.selectedServices.push(service.id);
            } else {
                this.selectedServices.splice(index, 1);
            }
        },

        quickSelectService(service) {
            this.selectedServices = [service.id];
            this.selectedServiceIcon = service.icon;
            this.selectedServiceName = service.name;
            this.priceRange = service.price;
            this.showInstantResults = true;
            
            // Use setTimeout to avoid blocking UI
            setTimeout(() => {
                this.simulateInstantSearch();
            }, 100);
        },

        async performSearch() {
            try {
                // Show loading state
                this.showInstantResults = false;
                
                // Validate required fields
                if (this.selectedServices.length === 0) {
                    await showModal({
                        title: 'Select a Service',
                        message: 'Please select at least one pet service to continue your search.',
                        type: 'info',
                        actions: [{ text: 'OK', primary: true }]
                    });
                    return;
                }
                
                if (!this.userPostcode) {
                    await showModal({
                        title: 'Enter Your Postcode',
                        message: 'Please enter your postcode so we can find pet care providers near you.',
                        type: 'info',
                        actions: [{ text: 'OK', primary: true }]
                    });
                    return;
                }
                
                // Build search parameters
                const searchParams = {
                    location: this.userPostcode,
                    serviceCategories: this.selectedServices,
                    petCount: this.petCount,
                    includeAvailability: true,
                    maxResults: 20,
                    radiusMiles: 25
                };

                console.log('Performing search with:', searchParams);
                
                // Redirect to search page with parameters
                this.redirectToSearchPage(searchParams);
            } catch (error) {
                console.error('Search error:', error);
                await showModal({
                    title: 'Search Error',
                    message: 'Unable to perform search at this time. Please try again later.',
                    type: 'error',
                    actions: [{ text: 'OK', primary: true }]
                });
            }
        },

        async performRealSearch(searchParams) {
            try {
                // First, geocode the location if needed
                let searchLat = 51.5074; // Default London
                let searchLng = -0.1278;
                
                if (searchParams.location !== 'London, UK') {
                    // Try to get location suggestions first
                    const locationResponse = await fetch(`/Search/GetLocationSuggestions?query=${encodeURIComponent(searchParams.location)}&maxResults=1`);
                    if (locationResponse.ok) {
                        const result = await locationResponse.json();
                        if (result.success && result.data && result.data.length > 0) {
                            const locations = result.data;
                            searchLat = locations[0].latitude || searchLat;
                            searchLng = locations[0].longitude || searchLng;
                        }
                    }
                }

                // Search for providers using shared search utils (same as Search page)
                if (window.providerSearchUtils) {
                    const searchFilters = {
                        location: searchParams.location,
                        latitude: searchLat,
                        longitude: searchLng,
                        radiusMiles: searchParams.radiusMiles,
                        serviceCategoryIds: searchParams.serviceCategories,
                        pageSize: 20,
                        sortBy: 'distance'
                    };
                    
                    const searchResults = await window.providerSearchUtils.searchProviders(searchFilters);
                    if (searchResults && searchResults.results) {
                        // Display search results using the same data format as Search page
                        await this.displaySearchResults(searchResults.results, searchParams);
                    } else {
                        throw new Error('No search results returned');
                    }
                } else {
                    throw new Error('Provider search utilities not available');
                }
            } catch (error) {
                console.error('Real search failed:', error);
                // Fallback to mock search
                await showModal({
                    title: 'Search Results',
                    message: this.buildSearchMessage() + '\n\nSearch completed! Real provider integration will show actual results.',
                    type: 'success',
                    actions: [
                        { text: 'View Results', primary: true },
                        { text: 'Refine Search' }
                    ]
                });
            }
        },

        async displaySearchResults(providers, searchParams) {
            const providerCount = providers.length;
            const serviceNames = searchParams.serviceCategories
                .map(id => this.services.find(s => s.id === id)?.name)
                .filter(Boolean)
                .join(', ');
            
            let resultMessage = `Found ${providerCount} providers for ${serviceNames}`;
            if (searchParams.location !== 'London, UK') {
                resultMessage += ` near ${searchParams.location}`;
            }
            
            if (providerCount > 0) {
                const topProvider = providers[0];
                resultMessage += `\n\nTop result: ${topProvider.businessName || topProvider.providerName}`;
                if (topProvider.rating) {
                    resultMessage += `\nRating: ${topProvider.rating}/5`;
                    if (topProvider.reviewCount > 0) {
                        resultMessage += ` (${topProvider.reviewCount} reviews)`;
                    }
                }
                if (topProvider.distanceMiles) {
                    resultMessage += `\nDistance: ${topProvider.distanceMiles.toFixed(1)} miles`;
                }
            }
            
            await showModal({
                title: 'Search Results',
                message: resultMessage,
                type: 'success',
                actions: [
                    { text: 'View All Results', primary: true },
                    { text: 'Book Now' },
                    { text: 'Refine Search' }
                ]
            });
        },

        buildSearchMessage() {
            let searchText = 'Searching for ';
            if (this.selectedServices.length > 0) {
                const serviceNames = this.selectedServices.map(id => 
                    this.services.find(s => s.id === id)?.name
                ).filter(Boolean).join(', ');
                searchText += serviceNames;
            } else {
                searchText += 'pet services';
            }
            
            if (this.userPostcode) searchText += ` near ${this.userPostcode}`;
            if (this.petCount > 1) searchText += ` for ${this.petCount} pets`;
            if (this.dateRange) searchText += ` from ${this.dateRange}`;
            
            return searchText;
        },


        async simulateInstantSearch() {
            try {
                // Use user's postcode coordinates or prompt for postcode
                let searchLat, searchLng;
                
                if (this.userPostcode) {
                    // Get coordinates from postcode
                    const locationResponse = await fetch(`/Search/GetLocationSuggestions?query=${encodeURIComponent(this.userPostcode)}&maxResults=1`);
                    if (locationResponse.ok) {
                        const result = await locationResponse.json();
                        if (result.success && result.data && result.data.length > 0) {
                            const locations = result.data;
                            searchLat = locations[0].latitude;
                            searchLng = locations[0].longitude;
                        }
                    }
                }
                
                if (!searchLat || !searchLng) {
                    // No postcode provided - show message
                    this.showInstantResults = false;
                    await showModal({
                        title: 'Postcode Required',
                        message: 'Please enter your postcode to find providers near you.',
                        type: 'info',
                        actions: [{ text: 'OK', primary: true }]
                    });
                    return;
                }
                
                // Search for real providers using shared search utils (same as Search page)
                if (window.providerSearchUtils) {
                    const searchFilters = {
                        latitude: searchLat,
                        longitude: searchLng,
                        radiusMiles: 25,
                        serviceCategoryIds: this.selectedServices.length > 0 ? this.selectedServices : [],
                        pageSize: 5,
                        sortBy: 'distance'
                    };
                    
                    const searchResults = await window.providerSearchUtils.searchProviders(searchFilters);
                    if (searchResults && searchResults.results) {
                        const providers = searchResults.results;
                        
                        this.availableProviders = providers.length;
                        this.availability = providers.length > 0 ? 'Available in your area' : 'No providers available in your area';
                    
                        if (providers.length > 0) {
                            // Use the first real provider
                            const topProvider = providers[0];
                            this.topProvider = {
                                id: topProvider.id,
                                name: topProvider.businessName || topProvider.providerName,
                                initials: this.getInitials(topProvider.businessName || topProvider.providerName),
                                rating: topProvider.rating ? topProvider.rating.toFixed(1) : null,
                                distance: topProvider.distance ? `${topProvider.distance.toFixed(1)} miles away` : 'Distance not available',
                                description: topProvider.description || `${topProvider.businessName || 'Professional provider'}${topProvider.reviewCount > 0 ? ' • ' + topProvider.reviewCount + ' reviews' : ''}`,
                                price: topProvider.priceRange?.min ? `From £${topProvider.priceRange.min}` : 'Contact for pricing',
                                isVerified: topProvider.verified || false,
                                reviewCount: topProvider.reviewCount || 0,
                                businessName: topProvider.businessName || topProvider.providerName
                            };
                        } else {
                            // No providers found
                            this.topProvider = null;
                        }
                    } else {
                        throw new Error('No search results returned');
                    }
                } else {
                    throw new Error('Provider search utilities not available');
                }
            } catch (error) {
                console.error('Error loading providers:', error);
                this.availableProviders = 0;
                this.availability = 'Unable to load providers';
                this.topProvider = null;
                
                await showModal({
                    title: 'Search Error',
                    message: 'Unable to search for providers at this time. Please try again later.',
                    type: 'error',
                    actions: [{ text: 'OK', primary: true }]
                });
            }
        },

        getInitials(name) {
            if (!name) return 'PP';
            return name.split(' ')
                .map(word => word.charAt(0))
                .join('')
                .toUpperCase()
                .substring(0, 2);
        },

        formatLastJobDate(lastJobDate) {
            if (!lastJobDate) {
                return 'New to platform';
            }
            
            const date = new Date(lastJobDate);
            const now = new Date();
            const diffTime = Math.abs(now - date);
            const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
            
            if (diffDays === 0) {
                return 'Last job today';
            } else if (diffDays === 1) {
                return 'Last job yesterday';
            } else if (diffDays < 7) {
                return `Last job ${diffDays} days ago`;
            } else if (diffDays < 30) {
                const weeks = Math.floor(diffDays / 7);
                return `Last job ${weeks} week${weeks > 1 ? 's' : ''} ago`;
            } else if (diffDays < 365) {
                const months = Math.floor(diffDays / 30);
                return `Last job ${months} month${months > 1 ? 's' : ''} ago`;
            } else {
                return 'Last job over a year ago';
            }
        },

        extractPostcodeFromDisplayName(displayName) {
            if (!displayName) return null;
            // UK postcode regex pattern
            const postcodePattern = /\b[A-Z]{1,2}[0-9][A-Z0-9]?\s?[0-9][A-Z]{2}\b/i;
            const match = displayName.match(postcodePattern);
            return match ? match[0] : null;
        },

        async viewAllProviders() {
            // Navigate to search page with current search parameters
            this.redirectToSearchPage();
        },


        async detectLocation() {
            try {
                if (!navigator.geolocation) {
                    await showModal({
                        title: 'Location Not Available',
                        message: 'Geolocation is not supported by this browser.',
                        type: 'error',
                        actions: [{ text: 'OK', primary: true }]
                    });
                    return;
                }

                // Show loading state
                const loadingButton = document.querySelector('button[onclick*="detectLocation"]');
                if (loadingButton) {
                    loadingButton.innerHTML = '<i class="fas fa-spinner fa-spin mr-1"></i>Detecting...';
                    loadingButton.disabled = true;
                }

                navigator.geolocation.getCurrentPosition(
                    async (position) => {
                        try {
                            console.log('Location detected:', position.coords.latitude, position.coords.longitude);
                            
                            // Try to reverse geocode using Google's geocoding API
                            try {
                                // Check if Google Maps API is available
                                if (typeof google !== 'undefined' && google.maps && google.maps.Geocoder) {
                                    const geocoder = new google.maps.Geocoder();
                                    const latlng = {
                                        lat: position.coords.latitude,
                                        lng: position.coords.longitude
                                    };
                                    
                                    geocoder.geocode({ location: latlng }, (results, status) => {
                                        if (status === 'OK' && results[0]) {
                                            console.log('Google Geocode result:', results[0]);
                                            
                                            // Extract postcode or city from address components
                                            let postcode = '';
                                            let city = '';
                                            let area = '';
                                            
                                            for (const component of results[0].address_components) {
                                                if (component.types.includes('postal_code')) {
                                                    postcode = component.long_name;
                                                } else if (component.types.includes('locality')) {
                                                    city = component.long_name;
                                                } else if (component.types.includes('administrative_area_level_1')) {
                                                    area = component.long_name;
                                                }
                                            }
                                            
                                            if (postcode) {
                                                this.userPostcode = postcode;
                                                console.log('Postcode found:', postcode);
                                            } else if (city) {
                                                this.userPostcode = city + (area ? `, ${area}` : '');
                                                console.log('Using city as location:', this.userPostcode);
                                            } else {
                                                // Use formatted address as fallback
                                                this.userPostcode = results[0].formatted_address.split(',')[0];
                                            }
                                           
                                            // Load top providers if service is selected
                                            if (this.selectedServices.length > 0) {
                                                this.loadTopProviders();
                                            }
                                        } else {
                                            console.warn('Google Geocoding failed:', status);
                                            throw new Error('Geocoding failed');
                                        }
                                    });
                                    return;
                                } else {
                                    // Fallback to server-side geocoding via our API
                                    const response = await fetch(`/api/location/reverse-geocode?lat=${position.coords.latitude}&lng=${position.coords.longitude}`);
                                    if (response.ok) {
                                        const locationData = await response.json();
                                        console.log('Server reverse geocode result:', locationData);
                                        
                                        if (locationData.postcode) {
                                            this.userPostcode = locationData.postcode;
                                        } else if (locationData.city) {
                                            this.userPostcode = locationData.city;
                                        } else {
                                            throw new Error('No location data available');
                                        }
                                       
                                        // Load top providers if service is selected
                                        if (this.selectedServices.length > 0) {
                                            this.loadTopProviders();
                                        }
                                        return;
                                    }
                                }
                            } catch (apiError) {
                                console.warn('Reverse geocoding failed:', apiError);
                            }
                            
                            // Fallback: Use coordinates to estimate area and set a reasonable default
                            const lat = position.coords.latitude;
                            const lng = position.coords.longitude;
                            
                            let approximateLocation = '';
                            if (lat >= 51.3 && lat <= 51.7 && lng >= -0.5 && lng <= 0.3) {
                                approximateLocation = 'London, UK';
                            } else if (lat >= 53.4 && lat <= 53.5 && lng >= -2.3 && lng <= -2.2) {
                                approximateLocation = 'Manchester, UK';
                            } else if (lat >= 52.4 && lat <= 52.5 && lng >= -1.9 && lng <= -1.8) {
                                approximateLocation = 'Birmingham, UK';
                            } else if (lat >= 55.8 && lat <= 55.9 && lng >= -3.3 && lng <= -3.1) {
                                approximateLocation = 'Edinburgh, UK';
                            } else if (lat >= 51.4 && lat <= 51.5 && lng >= -2.7 && lng <= -2.5) {
                                approximateLocation = 'Bristol, UK';
                            } else {
                                // Default to a major UK city
                                approximateLocation = 'London, UK';
                            }
                            
                            this.userPostcode = approximateLocation;
                            console.log('Using approximate location:', approximateLocation);
                            
                            await showModal({
                                title: 'Location Approximated',
                                message: `We've set your location to ${approximateLocation} based on your coordinates. You can edit this if needed.`,
                                type: 'info',
                                actions: [{ text: 'OK', primary: true }]
                            });
                            
                            // Load top providers if service is selected
                            if (this.selectedServices.length > 0) {
                                this.loadTopProviders();
                            }
                            
                        } catch (error) {
                            console.error('Could not process location:', error);
                            await showModal({
                                title: 'Location Error',
                                message: 'Unable to determine your location automatically. Please enter your postcode manually.',
                                type: 'warning',
                                actions: [{ text: 'OK', primary: true }]
                            });
                        } finally {
                            // Reset button state
                            if (loadingButton) {
                                loadingButton.innerHTML = '<i class="fas fa-location-crosshairs mr-1"></i>Detect';
                                loadingButton.disabled = false;
                            }
                        }
                    },
                    async (error) => {
                        console.error('Geolocation error:', error);
                        // Reset button state
                        if (loadingButton) {
                            loadingButton.innerHTML = '<i class="fas fa-location-crosshairs mr-1"></i>Detect';
                            loadingButton.disabled = false;
                        }
                        
                        await showModal({
                            title: 'Location Access Denied',
                            message: 'Please allow location access or enter your postcode manually.',
                            type: 'warning',
                            actions: [{ text: 'OK', primary: true }]
                        });
                    },
                    {
                        enableHighAccuracy: true,
                        timeout: 10000,
                        maximumAge: 300000
                    }
                );
            } catch (error) {
                console.error('Error detecting location:', error);
            }
        },

        redirectToSearchPage(searchParams = null) {
            // Build URL parameters from current search state
            const params = new URLSearchParams();
            
            // Use provided searchParams or build from current state
            const location = searchParams?.location || this.userPostcode || this.location;
            const services = searchParams?.serviceCategories || this.selectedServices;
            const petCount = searchParams?.petCount || this.petCount;
            
            if (location) {
                params.set('location', location);
            }
            
            if (services && services.length > 0) {
                params.set('services', services.join(','));
            }
            
            if (petCount && petCount > 1) {
                params.set('petCount', petCount.toString());
            }
            
            if (this.dateRange) {
                params.set('dateRange', this.dateRange);
            }
            
            if (this.smartSearch && this.smartSearch.trim()) {
                params.set('query', this.smartSearch.trim());
            }
            
            // Build the URL
            const searchUrl = `/Search${params.toString() ? '?' + params.toString() : ''}`;
            
            // Navigate to search page
            console.log('Redirecting to search page:', searchUrl);
            window.location.href = searchUrl;
        },

        async loadTopProviders() {
            if (!this.userPostcode || this.selectedServices.length === 0) {
                this.topProviders = [];
                return;
            }

            this.loadingTopProviders = true;
            try {
                // Use the new geocoding method for consistency
                const coordinates = await this.geocodePostcode(this.userPostcode);

                // Use unified search method (same as Search page and quick search)
                if (window.providerSearchUtils) {
                    const searchFilters = {
                        latitude: coordinates.lat,
                        longitude: coordinates.lng,
                        radiusMiles: 25,
                        serviceCategoryIds: this.selectedServices,
                        pageSize: 3,
                        sortBy: 'distance'
                    };
                    
                    const searchResults = await window.providerSearchUtils.searchProviders(searchFilters);
                    if (searchResults && searchResults.results) {
                        // Transform the provider data for display
                        this.topProviders = searchResults.results.map(provider => ({
                            id: provider.id,
                            businessName: provider.businessName || provider.providerName || 'Professional Provider',
                            initials: this.getInitials(provider.businessName || provider.providerName),
                            rating: provider.rating ? provider.rating.toFixed(1) : null,
                            reviewCount: provider.reviewCount > 0 ? provider.reviewCount : null,
                            description: provider.description || 'Professional pet care services',
                            minPrice: provider.priceRange?.min || provider.priceRange?.minPrice || 25,
                            distance: provider.distance ? `${provider.distance.toFixed(1)} miles` : 'Near you',
                            lastJobText: this.formatLastJobDate(provider.lastJobCompletedDate),
                            rawData: provider
                        }));
                        
                        console.log('Loaded top providers:', this.topProviders);
                        
                        // Reset carousel to first provider when new results load
                        this.activeProviderIndex = 0;
                        
                        // Smooth scroll to results after they're displayed
                        if (this.topProviders.length > 0 && this.showInstantResults) {
                            this.scrollToResults();
                        }
                    } else {
                        this.topProviders = [];
                        console.warn('No providers found in API response');
                    }
                } else {
                    throw new Error('Provider search utilities not available');
                }
            } catch (error) {
                console.error('Error loading top providers:', error);
                this.topProviders = [];
            } finally {
                this.loadingTopProviders = false;
            }
        },

        getApproximateUKCoordinates(postcode) {
            // Extract first part of UK postcode for approximate coordinates
            const postcodePrefix = postcode.toUpperCase().replace(/\s+/g, '').substring(0, 2);
            
            const coordinateMap = {
                // London areas
                'SW': { lat: 51.4897, lng: -0.1436 }, // South West London
                'SE': { lat: 51.4626, lng: -0.0478 }, // South East London  
                'NW': { lat: 51.5547, lng: -0.1849 }, // North West London
                'NE': { lat: 51.5390, lng: -0.0322 }, // North East London
                'E1': { lat: 51.5118, lng: -0.0591 }, // East London
                'W1': { lat: 51.5154, lng: -0.1447 }, // West London
                'N1': { lat: 51.5430, lng: -0.1043 }, // North London
                'EC': { lat: 51.5155, lng: -0.0922 }, // City of London
                'WC': { lat: 51.5142, lng: -0.1306 }, // West Central London
                
                // Major UK cities
                'M1': { lat: 53.4808, lng: -2.2426 }, // Manchester
                'B1': { lat: 52.4862, lng: -1.8904 }, // Birmingham  
                'LS': { lat: 53.8008, lng: -1.5491 }, // Leeds
                'S1': { lat: 53.3811, lng: -1.4701 }, // Sheffield
                'L1': { lat: 53.4084, lng: -2.9916 }, // Liverpool
                'NG': { lat: 52.9548, lng: -1.1581 }, // Nottingham
                'BS': { lat: 51.4545, lng: -2.5879 }, // Bristol
                'EH': { lat: 55.9533, lng: -3.1883 }, // Edinburgh
                'G1': { lat: 55.8642, lng: -4.2518 }, // Glasgow
                'CF': { lat: 51.4816, lng: -3.1791 }, // Cardiff
                'BH': { lat: 50.7192, lng: -1.8808 }, // Bournemouth
                'PL': { lat: 50.3755, lng: -4.1427 }, // Plymouth
                'TQ': { lat: 50.4619, lng: -3.5253 }, // Torquay
                'EX': { lat: 50.7184, lng: -3.5339 }, // Exeter
                'BA': { lat: 51.3758, lng: -2.3599 }, // Bath
                'OX': { lat: 51.7520, lng: -1.2577 }, // Oxford
                'CB': { lat: 52.2053, lng: 0.1218 },  // Cambridge
                'BR': { lat: 51.4063, lng: 0.0156 },  // Bromley
                'CR': { lat: 51.3762, lng: -0.0982 }, // Croydon
                'KT': { lat: 51.3968, lng: -0.2957 }, // Kingston upon Thames
                'SM': { lat: 51.3693, lng: -0.1652 }, // Sutton
                'TW': { lat: 51.4607, lng: -0.3571 }, // Twickenham
                'HA': { lat: 51.5898, lng: -0.3346 }, // Harrow
                'UB': { lat: 51.5541, lng: -0.4543 }, // Southall
                'IG': { lat: 51.5981, lng: 0.0831 },  // Ilford
                'RM': { lat: 51.5782, lng: 0.2040 },  // Romford
                'EN': { lat: 51.6529, lng: -0.0803 }, // Enfield
                'AL': { lat: 51.7562, lng: -0.3376 }, // St Albans
                'WD': { lat: 51.6562, lng: -0.3985 }, // Watford
                'HP': { lat: 51.6306, lng: -0.7553 }, // High Wycombe
                'SL': { lat: 51.5095, lng: -0.5951 }, // Slough
                'RG': { lat: 51.4543, lng: -0.9781 }, // Reading
                'GU': { lat: 51.2362, lng: -0.5704 }, // Guildford
                'KT': { lat: 51.3968, lng: -0.2957 }, // Kingston
                'BN': { lat: 50.8429, lng: -0.1313 }, // Brighton
                'ME': { lat: 51.3889, lng: 0.5053 },  // Medway
                'CT': { lat: 51.2802, lng: 1.0789 },  // Canterbury
                'TN': { lat: 51.1859, lng: 0.2767 },  // Tonbridge
                'DA': { lat: 51.4669, lng: 0.2072 },  // Dartford
            };
            
            // Try exact match first
            if (coordinateMap[postcodePrefix]) {
                return coordinateMap[postcodePrefix];
            }
            
            // Try first character for broader area match
            const firstChar = postcodePrefix.charAt(0);
            const broadAreaMap = {
                'M': { lat: 53.4808, lng: -2.2426 }, // Manchester area
                'B': { lat: 52.4862, lng: -1.8904 }, // Birmingham area
                'L': { lat: 53.4084, lng: -2.9916 }, // Liverpool/Lancashire
                'S': { lat: 53.3811, lng: -1.4701 }, // Sheffield/South Yorkshire
                'G': { lat: 55.8642, lng: -4.2518 }, // Glasgow area
                'E': { lat: 51.5118, lng: -0.0591 }, // East London
                'N': { lat: 51.5430, lng: -0.1043 }, // North London/Midlands
                'W': { lat: 51.5154, lng: -0.1447 }, // West London/Wales
            };
            
            if (broadAreaMap[firstChar]) {
                return broadAreaMap[firstChar];
            }
            
            // Default to London if no match found
            return { lat: 51.5074, lng: -0.1278 };
        },

        checkButtonState() {
            // Force Alpine to re-evaluate button state
            // This helps when autocomplete fills the postcode but doesn't trigger events
            if (this.userPostcode && this.userPostcode.trim()) {
                this.$nextTick(() => {
                    // Force reactivity check
                    Alpine.nextTick();
                });
            }
        },


        async showAuthModal() {
            try {
                await showModal({
                    title: 'Sign In Required',
                    content: `
                        <div class="text-center mb-6">
                            <div class="w-16 h-16 bg-pet-orange rounded-full flex items-center justify-center text-white mx-auto mb-4">
                                <i class="fas fa-user text-2xl"></i>
                            </div>
                            <h3 class="text-xl font-semibold text-gray-900 mb-2">Contact Provider</h3>
                            <p class="text-gray-600">Sign in to contact providers directly and manage your bookings.</p>
                        </div>
                        
                        <div class="space-y-4">
                            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <button onclick="window.location.href='/Account/Login'" 
                                        class="bg-pet-orange text-white px-6 py-3 rounded-lg font-medium hover:bg-orange-600 transition-colors flex items-center justify-center">
                                    <i class="fas fa-sign-in-alt mr-2"></i>
                                    Sign In
                                </button>
                                <button onclick="window.location.href='/Account/Register'" 
                                        class="bg-pet-blue text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-600 transition-colors flex items-center justify-center">
                                    <i class="fas fa-user-plus mr-2"></i>
                                    Register
                                </button>
                            </div>
                            
                            <div class="bg-gray-50 rounded-lg p-4">
                                <h4 class="font-semibold text-gray-900 mb-2">Benefits of signing in:</h4>
                                <ul class="text-sm text-gray-700 space-y-1">
                                    <li><i class="fas fa-check text-green-500 mr-2"></i>Direct contact with providers</li>
                                    <li><i class="fas fa-check text-green-500 mr-2"></i>Manage bookings and messages</li>
                                    <li><i class="fas fa-check text-green-500 mr-2"></i>Save favorite providers</li>
                                    <li><i class="fas fa-check text-green-500 mr-2"></i>Booking history and reviews</li>
                                </ul>
                            </div>
                        </div>
                    `,
                    type: 'custom',
                    actions: [
                        { text: 'Close', action: () => {} }
                    ]
                });
            } catch (error) {
                console.error('Error opening auth modal:', error);
            }
        },

        async viewProviderAvailability(provider) {
            if (!provider) {
                console.error('Invalid provider for availability check');
                return;
            }

            try {
                // Use shared utilities to get availability data
                if (window.providerSearchUtils) {
                    const startDate = new Date();
                    const endDate = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000); // 30 days ahead
                    const availabilityData = await window.providerSearchUtils.getProviderAvailability(provider.id, startDate, endDate);
                    
                    if (availabilityData && availabilityData.events) {
                        
                        await showModal({
                            title: `${provider.businessName} - Availability`,
                            content: `
                                <div class="text-center mb-6">
                                    <div class="w-16 h-16 bg-pet-orange rounded-full flex items-center justify-center text-white font-bold text-xl mx-auto mb-4">
                                        ${provider.initials}
                                    </div>
                                    <p class="text-gray-600">Click on an available time slot to book</p>
                                </div>
                                
                                <div id="home-availability-calendar" class="mb-4"></div>
                                
                                <div class="flex items-center justify-center gap-6 text-sm text-gray-600 mb-4">
                                    <div class="flex items-center gap-2">
                                        <div class="w-4 h-4 bg-green-500 rounded"></div>
                                        <span>Available</span>
                                    </div>
                                    <div class="flex items-center gap-2">
                                        <div class="w-4 h-4 bg-red-500 rounded"></div>
                                        <span>Booked</span>
                                    </div>
                                </div>
                                
                                <div class="bg-blue-50 border border-blue-200 rounded-lg p-3 text-sm text-blue-800">
                                    <i class="fas fa-info-circle mr-2"></i>
                                    Found ${availabilityData.events.length} time slots in the next 30 days
                                </div>
                            `,
                            type: 'custom',
                            actions: [
                                { 
                                    text: 'Book Appointment', 
                                    primary: true, 
                                    action: () => {
                                        // Redirect to search page with pre-filled provider
                                        window.location.href = `/Search?providerId=${provider.id}`;
                                    } 
                                },
                                { text: 'Contact Provider', action: () => this.contactProvider(provider) },
                                { text: 'Close', action: () => {} }
                            ],
                            onShow: () => {
                                // Initialize calendar after modal is shown
                                setTimeout(() => {
                                    window.providerSearchUtils.initFullCalendar(
                                        'home-availability-calendar',
                                        availabilityData,
                                        (info) => {
                                            // Handle slot selection - show booking details
                                            const event = info.event;
                                            const props = event.extendedProps;
                                            
                                            if (props.type === 'available') {
                                                alert(`Selected: ${event.title}\nTime: ${event.start.toLocaleString('en-GB')}\nPrice: £${props.price}`);
                                            }
                                        }
                                    );
                                }, 100);
                            }
                        });
                    } else {
                        throw new Error('No availability data received');
                    }
                } else {
                    throw new Error('Provider search utilities not loaded');
                }
            } catch (error) {
                console.error('Error loading availability:', error);
                await showModal({
                    title: 'Availability Error',
                    message: 'Unable to load availability at this time. Please try again later or contact the provider directly.',
                    type: 'error',
                    actions: [
                        { text: 'Contact Provider', primary: true, action: () => this.contactProvider(provider) },
                        { text: 'OK', action: () => {} }
                    ]
                });
            }
        },

        async viewProviderPricing(provider) {
            if (!provider) {
                console.error('Invalid provider for pricing');
                return;
            }

            try {
                // Show loading modal first
                await showModal({
                    title: 'Loading Pricing...',
                    content: `
                        <div class="text-center py-8">
                            <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-pet-orange"></div>
                            <p class="text-gray-600 mt-4">Fetching pricing for ${provider.businessName}...</p>
                        </div>
                    `,
                    type: 'info',
                    actions: []
                });

                // Use shared utilities to get pricing data
                if (window.providerSearchUtils) {
                    const pricingData = await window.providerSearchUtils.getProviderPricing(provider.id);
                    
                    if (pricingData && pricingData.services) {
                        
                        await showModal({
                            title: `${provider.businessName} - Pricing`,
                            content: `
                                <div class="text-center mb-6">
                                    <div class="w-16 h-16 bg-pet-orange rounded-full flex items-center justify-center text-white font-bold text-xl mx-auto mb-4">
                                        ${provider.initials}
                                    </div>
                                    <p class="text-gray-600">All prices in British Pounds (£)</p>
                                </div>
                                
                                <div class="space-y-4 max-h-96 overflow-y-auto">
                                    ${pricingData.services.map(service => `
                                        <div class="border border-gray-200 rounded-lg overflow-hidden">
                                            <div class="bg-gray-50 px-4 py-3 border-b border-gray-200">
                                                <h3 class="font-semibold text-lg text-gray-900">${service.serviceName}</h3>
                                            </div>
                                            <div class="p-4 space-y-3">
                                                ${service.subServices.map(subService => `
                                                    <div class="flex justify-between items-start">
                                                        <div class="flex-1">
                                                            <p class="font-medium text-gray-900">${subService.subServiceName}</p>
                                                            <p class="text-sm text-gray-600">${subService.description}</p>
                                                        </div>
                                                        <span class="text-lg font-bold text-pet-orange ml-4">£${subService.price}</span>
                                                    </div>
                                                `).join('')}
                                            </div>
                                        </div>
                                    `).join('')}
                                    
                                    ${pricingData.additionalServices && pricingData.additionalServices.length > 0 ? `
                                        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
                                            <h3 class="font-semibold text-blue-900 mb-3">Additional Services & Fees</h3>
                                            <div class="space-y-2">
                                                ${pricingData.additionalServices.map(additional => `
                                                    <div class="flex justify-between items-center text-sm">
                                                        <span class="text-blue-900">${additional.name}</span>
                                                        <span class="font-bold text-blue-900">
                                                            ${additional.surcharge ? (additional.surcharge > 0 ? '+' + additional.surcharge + '%' : additional.surcharge + '%') : '£' + additional.price}
                                                        </span>
                                                    </div>
                                                `).join('')}
                                            </div>
                                        </div>
                                    ` : ''}
                            `,
                            type: 'custom',
                            actions: [
                                { text: 'Book Service', primary: true, action: () => window.location.href = `/Search?providerId=${provider.id}` },
                                { text: 'Contact Provider', action: () => this.contactProvider(provider) },
                                { text: 'Close', action: () => {} }
                            ]
                        });
                    } else {
                        throw new Error('No pricing data received');
                    }
                } else {
                    throw new Error('Provider search utilities not loaded');
                }
            } catch (error) {
                console.error('Error loading pricing:', error);
                await showModal({
                    title: 'Pricing Error',
                    message: 'Unable to load pricing information at this time. Please try again later or contact the provider directly.',
                    type: 'error',
                    actions: [
                        { text: 'Contact Provider', primary: true, action: () => this.contactProvider(provider) },
                        { text: 'OK', action: () => {} }
                    ]
                });
            }
        },

        /**
         * Smoothly scrolls to the Top 3 Providers results section
         * Called after search results are successfully loaded and displayed
         */
        scrollToResults() {
            // Use setTimeout to ensure the DOM has updated with the results
            setTimeout(() => {
                // Find the Top 3 Providers section using Alpine.js template
                const resultsSection = document.querySelector('[x-if="showInstantResults && topProviders.length > 0"]');
                
                if (resultsSection && resultsSection.style.display !== 'none') {
                    // Get the actual rendered element (Alpine.js template creates the content dynamically)
                    const parentElement = resultsSection.parentElement;
                    const actualResultsContainer = parentElement?.querySelector('.bg-white.rounded-2xl.shadow-xl');
                    
                    if (actualResultsContainer) {
                        // Calculate scroll position - scroll to show the results with some padding above
                        const elementTop = actualResultsContainer.getBoundingClientRect().top + window.pageYOffset;
                        const scrollOffset = 100; // 100px padding from top of viewport
                        const scrollPosition = Math.max(0, elementTop - scrollOffset);
                        
                        // Smooth scroll to the results
                        window.scrollTo({
                            top: scrollPosition,
                            behavior: 'smooth'
                        });
                        
                        console.log('Smoothly scrolled to search results');
                    } else {
                        console.log('Results container not yet rendered, trying alternative scroll method');
                        // Fallback: scroll to the general area where results appear
                        const searchContainer = document.querySelector('.search-container');
                        if (searchContainer) {
                            const containerTop = searchContainer.getBoundingClientRect().top + window.pageYOffset;
                            window.scrollTo({
                                top: containerTop + 200, // Approximate position of results
                                behavior: 'smooth'
                            });
                        }
                    }
                } else {
                    console.log('Results section not found or not visible');
                }
            }, 300); // 300ms delay to ensure Alpine.js has rendered the template
        },

        /**
         * Initialize carousel functionality
         * Called when the carousel container is rendered
         */
        initializeCarousel() {
            // Initialize carousel only if there are multiple providers
            if (this.topProviders.length <= 1) return;
            
            // Add touch/swipe support for mobile devices
            this.$nextTick(() => {
                const carouselContainer = this.$el.querySelector('.relative.overflow-hidden');
                if (carouselContainer) {
                    this.carouselContainer = carouselContainer;
                    this.setupTouchEvents();
                }
            });
        },

        /**
         * Setup touch events for swipe gesture support
         */
        setupTouchEvents() {
            if (!this.carouselContainer) return;

            // Touch start
            this.carouselContainer.addEventListener('touchstart', (e) => {
                this.touchStartX = e.changedTouches[0].screenX;
            });

            // Touch move (optional: could show preview of next slide)
            this.carouselContainer.addEventListener('touchmove', (e) => {
                e.preventDefault(); // Prevent default scroll behavior during swipe
            });

            // Touch end
            this.carouselContainer.addEventListener('touchend', (e) => {
                this.touchEndX = e.changedTouches[0].screenX;
                this.handleSwipeGesture();
            });

            // Keyboard support
            document.addEventListener('keydown', (e) => {
                if (!this.showInstantResults || this.topProviders.length <= 1) return;
                
                if (e.key === 'ArrowLeft') {
                    e.preventDefault();
                    this.previousProvider();
                } else if (e.key === 'ArrowRight') {
                    e.preventDefault();
                    this.nextProvider();
                }
            });
        },

        /**
         * Handle swipe gesture detection
         */
        handleSwipeGesture() {
            const swipeThreshold = 50; // Minimum distance for a swipe
            const swipeDistance = this.touchEndX - this.touchStartX;

            if (Math.abs(swipeDistance) < swipeThreshold) return;

            if (swipeDistance > 0) {
                // Swipe right - go to previous
                this.previousProvider();
            } else {
                // Swipe left - go to next
                this.nextProvider();
            }
        },

        /**
         * Navigate to the next provider in the carousel
         */
        nextProvider() {
            if (this.activeProviderIndex < this.topProviders.length - 1) {
                this.activeProviderIndex++;
            }
        },

        /**
         * Navigate to the previous provider in the carousel
         */
        previousProvider() {
            if (this.activeProviderIndex > 0) {
                this.activeProviderIndex--;
            }
        },

        /**
         * Set active provider by index (used by dot indicators)
         */
        setActiveProvider(index) {
            if (index >= 0 && index < this.topProviders.length) {
                this.activeProviderIndex = index;
            }
        }
    }
}

// Initialize modals if not already loaded
if (typeof showModal === 'undefined') {
    console.warn('Modal system not loaded. Please ensure modal.js is included before home.js');
}