/**
 * Home Page JavaScript
 * Handles search functionality and dynamic service loading
 */

// Premium Search Widget Component
function premiumSearchWidget() {
    return {
        smartSearch: '',
        userPostcode: '',
        showSuggestions: false,
        showAdvanced: false,
        showInstantResults: false,
        selectedServices: [],
        petCount: 1,
        location: '',
        dateRange: '',
        datePickerInstance: null,
        selectedServiceIcon: '',
        selectedServiceName: '',
        availableProviders: 0,
        priceRange: '',
        availability: '',
        topProvider: null,
        loadingServices: true,
        
        // Dynamic services from API
        services: [],
        serviceCategories: [],
        
        // Smart suggestions will be generated from actual service data
        smartSuggestions: [],

        async init() {
            // Load services from API
            await this.loadServiceCategories();
            
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
                        }
                    });
                }
            });
        },

        async loadServiceCategories() {
            this.loadingServices = true;
            try {
                const response = await fetch('/Search/GetServiceCategories');
                const result = await response.json();
                
                if (result.success && result.data) {
                    this.serviceCategories = result.data;
                    
                    // Transform categories to flat services array for the UI
                    this.services = this.serviceCategories.map(category => ({
                        id: category.serviceCategoryId,
                        name: category.name,
                        icon: this.getCategoryIcon(category.name),
                        price: this.getCategoryPriceRange(category),
                        description: category.description,
                        subServices: category.subServices
                    }));
                    
                    // Generate smart suggestions based on actual services
                    await this.generateSmartSuggestions();
                } else {
                    console.error('Failed to load service categories:', result.message);
                    // Fallback to default services if API fails
                    this.loadDefaultServices();
                }
            } catch (error) {
                console.error('Error loading service categories:', error);
                // Fallback to default services if API fails
                this.loadDefaultServices();
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

        async generateSmartSuggestions() {
            try {
                // Try to load real provider data for suggestions
                await this.loadRealProviderSuggestions();
            } catch (error) {
                console.warn('Failed to load real provider suggestions, using default:', error);
                // Fallback to service-based suggestions
                this.smartSuggestions = this.services.slice(0, 4).map((service, index) => ({
                    id: service.id,
                    icon: service.icon,
                    title: `${service.name} in your area`,
                    description: `Multiple providers available`,
                    price: service.price
                }));
            }
        },

        async loadRealProviderSuggestions() {
            // Load trending providers from different areas for diverse suggestions
            const locations = [
                { name: 'London', lat: 51.5074, lng: -0.1278 },
                { name: 'Manchester', lat: 53.4808, lng: -2.2426 },
                { name: 'Birmingham', lat: 52.4862, lng: -1.8904 },
                { name: 'Bristol', lat: 51.4545, lng: -2.5879 }
            ];

            const suggestions = [];
            
            for (const location of locations.slice(0, 2)) { // Only check first 2 locations to avoid too many requests
                try {
                    const response = await fetch(`/Search/GetNearby?latitude=${location.lat}&longitude=${location.lng}&radiusMiles=15`);
                    if (response.ok) {
                        const result = await response.json();
                        if (result.success && result.data && result.data.length > 0) {
                            const providers = result.data;
                            const topProvider = providers[0];
                            const service = topProvider.services?.[0];
                            
                            suggestions.push({
                                id: service?.serviceCategory || 'general',
                                icon: this.getCategoryIcon(service?.categoryName || 'General'),
                                title: `${service?.categoryName || 'Pet Care'} in ${location.name}`,
                                description: `${providers.length} providers near you`,
                                price: topProvider.priceRange?.minPrice ? `£${topProvider.priceRange.minPrice}+` : 'Custom pricing'
                            });
                        }
                    }
                } catch (error) {
                    console.warn(`Failed to load providers for ${location.name}:`, error);
                }
            }

            // Fill remaining suggestions with service categories if we don't have enough
            while (suggestions.length < 4 && suggestions.length < this.services.length) {
                const service = this.services[suggestions.length];
                if (service && !suggestions.find(s => s.id === service.id)) {
                    suggestions.push({
                        id: service.id,
                        icon: service.icon,
                        title: `${service.name} services`,
                        description: 'Professional providers available',
                        price: service.price
                    });
                }
            }

            this.smartSuggestions = suggestions.slice(0, 4);
        },

        loadDefaultServices() {
            // Fallback services if API is unavailable
            this.services = [
                { id: 'walking', name: 'Dog Walking', icon: '🚶‍♂️', price: '£15-25' },
                { id: 'sitting', name: 'Pet Sitting', icon: '🏠', price: '£25-45' },
                { id: 'grooming', name: 'Grooming', icon: '✂️', price: '£30-80' },
                { id: 'training', name: 'Training', icon: '🎓', price: '£40-70' },
                { id: 'emergency', name: 'Emergency', icon: '🚨', price: '£50+' },
                { id: 'vet', name: 'Vet Visits', icon: '🏥', price: '£20+' },
                { id: 'boarding', name: 'Boarding', icon: '🏨', price: '£35+' },
                { id: 'taxi', name: 'Pet Taxi', icon: '🚗', price: '£15+' }
            ];
            
            this.generateSmartSuggestions();
        },

        processSmartSearch() {
            if (this.smartSearch.length > 2) {
                this.showSuggestions = true;
                // Filter suggestions based on search input
                const searchLower = this.smartSearch.toLowerCase();
                this.smartSuggestions = this.smartSuggestions.filter(suggestion => 
                    suggestion.title.toLowerCase().includes(searchLower) ||
                    suggestion.description.toLowerCase().includes(searchLower)
                );
            } else {
                this.showSuggestions = false;
            }
        },

        selectSuggestion(suggestion) {
            this.smartSearch = suggestion.title;
            this.showSuggestions = false;
            this.performSearch();
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
                
                // Build search parameters
                const searchParams = {
                    location: this.userPostcode || this.location || '',
                    serviceCategories: this.selectedServices,
                    petCount: this.petCount,
                    includeAvailability: true,
                    maxResults: 20,
                    radiusMiles: 25
                };

                console.log('Performing search with:', searchParams);
                
                // Try to perform real search
                if ((this.userPostcode || this.location) && this.selectedServices.length > 0) {
                    // Redirect to search page with parameters
                    this.redirectToSearchPage(searchParams);
                } else {
                    // Show search configuration modal
                    let configMessage = 'Please complete your search:';
                    if (!this.userPostcode && !this.location) {
                        configMessage += '\n• Enter your postcode to find providers near you';
                    }
                    if (this.selectedServices.length === 0) {
                        configMessage += '\n• Select at least one service type';
                    }
                    
                    await showModal({
                        title: 'Complete Your Search',
                        message: configMessage,
                        type: 'info',
                        actions: [
                            { text: 'OK', primary: true }
                        ]
                    });
                }
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

                // Search for providers
                const serviceCategories = searchParams.serviceCategories.join(',');
                const response = await fetch(`/Search/GetNearby?latitude=${searchLat}&longitude=${searchLng}&radiusMiles=${searchParams.radiusMiles}&serviceCategories=${serviceCategories}`);
                
                if (response.ok) {
                    const result = await response.json();
                    if (result.success && result.data) {
                        // Display search results
                        await this.displaySearchResults(result.data, searchParams);
                    } else {
                        throw new Error('Failed to search providers');
                    }
                } else {
                    throw new Error('Failed to search providers');
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
                resultMessage += `\nRating: ${topProvider.rating}/5 (${topProvider.reviewCount} reviews)`;
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
            let searchText = `Searching for "${this.smartSearch}"`;
            if (this.location) searchText += ` in ${this.location}`;
            if (this.selectedServices.length > 0) {
                const serviceNames = this.selectedServices.map(id => 
                    this.services.find(s => s.id === id)?.name
                ).filter(Boolean).join(', ');
                searchText += ` for ${serviceNames}`;
            }
            if (this.petCount > 1) searchText += ` for ${this.petCount} pets`;
            if (this.dateRange) searchText += ` from ${this.dateRange}`;
            
            return searchText;
        },

        performAdvancedSearch() {
            this.performSearch();
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
                
                // Search for real providers near user's location
                const selectedService = this.services.find(s => s.id === this.selectedServices[0]);
                const serviceCategories = selectedService ? selectedService.id : '';
                
                const response = await fetch(`/Search/GetNearby?latitude=${searchLat}&longitude=${searchLng}&radiusMiles=25&serviceCategories=${serviceCategories}`);
                
                if (response.ok) {
                    const result = await response.json();
                    if (result.success && result.data) {
                        const providers = result.data;
                    
                        this.availableProviders = providers.length;
                        this.availability = providers.length > 0 ? 'Available in your area' : 'No providers available in your area';
                    
                        if (providers.length > 0) {
                        // Use the first real provider
                        const topProvider = providers[0];
                        this.topProvider = {
                            name: topProvider.businessName || topProvider.providerName,
                            initials: this.getInitials(topProvider.businessName || topProvider.providerName),
                            rating: topProvider.rating ? topProvider.rating.toFixed(1) : '0.0',
                            distance: topProvider.distanceMiles ? `${topProvider.distanceMiles.toFixed(1)} miles away` : 'Distance not available',
                            description: topProvider.description || `${topProvider.businessName || 'Professional provider'} • ${topProvider.reviewCount || 0} reviews`,
                            price: topProvider.priceRange?.minPrice ? `From £${topProvider.priceRange.minPrice}` : 'Contact for pricing',
                            isVerified: topProvider.isVerified || false,
                            reviewCount: topProvider.reviewCount || 0
                        };
                        } else {
                            // No providers found
                            this.topProvider = null;
                        }
                    } else {
                        throw new Error('Failed to load providers');
                    }
                } else {
                    throw new Error('Failed to load providers');
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

        async viewAllProviders() {
            // Navigate to search page with current search parameters
            this.redirectToSearchPage();
        },

        refineSearch() {
            this.showInstantResults = false;
            this.showAdvanced = true;
            this.$nextTick(() => {
                document.querySelector('.advanced-search-panel').scrollIntoView({ 
                    behavior: 'smooth' 
                });
            });
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

                navigator.geolocation.getCurrentPosition(
                    async (position) => {
                        try {
                            // Reverse geocode to get postcode
                            const response = await fetch(`/Search/GetLocationSuggestions?query=${position.coords.latitude},${position.coords.longitude}&maxResults=1`);
                            if (response.ok) {
                                const result = await response.json();
                                if (result.success && result.data && result.data.length > 0) {
                                    const locations = result.data;
                                    this.userPostcode = locations[0].postcode || locations[0].displayName || '';
                                }
                            }
                        } catch (error) {
                            console.warn('Could not get postcode from coordinates:', error);
                        }
                    },
                    async (error) => {
                        await showModal({
                            title: 'Location Access Denied',
                            message: 'Please enter your postcode manually or allow location access.',
                            type: 'warning',
                            actions: [{ text: 'OK', primary: true }]
                        });
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
        }
    }
}

// Initialize modals if not already loaded
if (typeof showModal === 'undefined') {
    console.warn('Modal system not loaded. Please ensure modal.js is included before home.js');
}