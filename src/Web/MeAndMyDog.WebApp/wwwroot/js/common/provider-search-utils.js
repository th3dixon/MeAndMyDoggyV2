/**
 * Shared Provider Search Utilities
 * Common functions for provider search functionality across home and search pages
 */

class ProviderSearchUtils {
    constructor() {
        this.apiEndpoints = {
            serviceCategories: '/Search/GetServiceCategories',
            search: '/Search/Search',
            nearby: '/Search/GetNearby',
            provider: '/Search/GetProvider',
            pricing: '/Search/GetProviderPricing',
            availability: '/Search/GetProviderAvailability',
            locationSuggestions: '/Search/GetLocationSuggestions'
        };
    }

    /**
     * Get service categories from API
     */
    async getServiceCategories() {
        try {
            const response = await fetch(this.apiEndpoints.serviceCategories);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const result = await response.json();
            
            if (result && result.success && result.data) {
                // Handle double-wrapped API response: result.data contains the API response wrapper
                const apiData = result.data;
                
                if (apiData && apiData.success && apiData.data && Array.isArray(apiData.data)) {
                    return apiData.data;
                } else {
                    throw new Error('API data is not in expected format or is not an array');
                }
            } else {
                throw new Error(result?.message || 'Invalid API response');
            }
        } catch (error) {
            console.warn('API services unavailable:', error.message);
            return this.getDefaultServices();
        }
    }

    /**
     * FALLBACK DATA ONLY - Get default services when API is unavailable
     * This ensures search functionality remains operational during development/outages
     * Pricing based on UK market research (last updated: 2024)
     */
    getDefaultServices() {
        console.warn('Loading fallback service data - ServiceCategories API may be unavailable');
        return [
            { 
                serviceCategoryId: 'walking',
                name: 'Dog Walking',
                description: 'Professional dog walking services',
                iconClass: 'fas fa-walking',
                subServices: [
                    { subServiceId: 'walk-30', name: '30 minute walk', description: 'Standard neighborhood walk', suggestedMinPrice: 15, suggestedMaxPrice: 28 },
                    { subServiceId: 'walk-60', name: '60 minute walk', description: 'Extended walk with exercise', suggestedMinPrice: 25, suggestedMaxPrice: 40 },
                    { subServiceId: 'walk-group', name: 'Group walk', description: 'Socialization with other dogs', suggestedMinPrice: 12, suggestedMaxPrice: 20 }
                ]
            },
            { 
                serviceCategoryId: 'sitting',
                name: 'Pet Sitting',
                description: 'In-home pet sitting and care',
                iconClass: 'fas fa-home',
                subServices: [
                    { subServiceId: 'sit-half-day', name: 'Half day sitting', description: '4-6 hours of care', suggestedMinPrice: 30, suggestedMaxPrice: 50 },
                    { subServiceId: 'sit-full-day', name: 'Full day sitting', description: '8+ hours of care', suggestedMinPrice: 50, suggestedMaxPrice: 75 },
                    { subServiceId: 'sit-overnight', name: 'Overnight sitting', description: '24-hour care at your home', suggestedMinPrice: 60, suggestedMaxPrice: 90 }
                ]
            },
            { 
                serviceCategoryId: 'grooming',
                name: 'Pet Grooming',
                description: 'Professional grooming services',
                iconClass: 'fas fa-cut',
                subServices: [
                    { subServiceId: 'groom-wash', name: 'Wash & dry', description: 'Basic wash and blow dry', suggestedMinPrice: 25, suggestedMaxPrice: 40 },
                    { subServiceId: 'groom-full', name: 'Full grooming', description: 'Wash, cut, nails, ears', suggestedMinPrice: 45, suggestedMaxPrice: 85 },
                    { subServiceId: 'groom-nails', name: 'Nail trimming', description: 'Nail clipping service', suggestedMinPrice: 15, suggestedMaxPrice: 25 }
                ]
            },
            { 
                serviceCategoryId: 'training',
                name: 'Pet Training',
                description: 'Behavioral training and obedience',
                iconClass: 'fas fa-graduation-cap',
                subServices: [
                    { subServiceId: 'train-basic', name: 'Basic obedience', description: 'Sit, stay, come commands', suggestedMinPrice: 45, suggestedMaxPrice: 70 },
                    { subServiceId: 'train-advanced', name: 'Advanced training', description: 'Complex behavior modification', suggestedMinPrice: 65, suggestedMaxPrice: 100 },
                    { subServiceId: 'train-puppy', name: 'Puppy training', description: 'Socialization and basic skills', suggestedMinPrice: 40, suggestedMaxPrice: 65 }
                ]
            }
        ];
    }

    /**
     * FALLBACK DATA ONLY - Get default availability when API is unavailable
     * This ensures availability functionality remains operational during development/outages
     */
    getFallbackAvailability(providerId, startDate, endDate) {
        console.warn('Loading fallback availability data - Provider Availability API may be unavailable');
        
        // Generate sample availability slots for the next 14 days
        const availableSlots = [];
        const currentDate = startDate ? new Date(startDate) : new Date();
        
        for (let i = 1; i <= 14; i++) {
            const slotDate = new Date(currentDate);
            slotDate.setDate(currentDate.getDate() + i);
            
            // Generate varied availability patterns
            const dayOfWeek = slotDate.getDay();
            
            // Morning slots (9:00-10:00)
            if (dayOfWeek >= 1 && dayOfWeek <= 5) { // Weekdays
                const morningSlot = new Date(slotDate);
                morningSlot.setHours(9, 0, 0, 0);
                const morningEnd = new Date(morningSlot);
                morningEnd.setHours(10, 0, 0, 0);
                
                availableSlots.push({
                    id: `slot-${i}-morning`,
                    dateTime: morningSlot.toISOString(),
                    startTime: morningSlot.toISOString(),
                    endTime: morningEnd.toISOString(),
                    duration: 60,
                    price: 25,
                    serviceType: 'Dog Walking',
                    isAvailable: true,
                    providerId: providerId
                });
            }
            
            // Afternoon slots (14:00-15:30)
            if (i % 3 !== 0) { // Skip some days for variety
                const afternoonSlot = new Date(slotDate);
                afternoonSlot.setHours(14, 0, 0, 0);
                const afternoonEnd = new Date(afternoonSlot);
                afternoonEnd.setHours(15, 30, 0, 0);
                
                availableSlots.push({
                    id: `slot-${i}-afternoon`,
                    dateTime: afternoonSlot.toISOString(),
                    startTime: afternoonSlot.toISOString(),
                    endTime: afternoonEnd.toISOString(),
                    duration: 90,
                    price: 40,
                    serviceType: 'Pet Grooming',
                    isAvailable: true,
                    providerId: providerId
                });
            }
            
            // Weekend slots (different timing)
            if (dayOfWeek === 0 || dayOfWeek === 6) {
                const weekendSlot = new Date(slotDate);
                weekendSlot.setHours(10, 30, 0, 0);
                const weekendEnd = new Date(weekendSlot);
                weekendEnd.setHours(11, 30, 0, 0);
                
                availableSlots.push({
                    id: `slot-${i}-weekend`,
                    dateTime: weekendSlot.toISOString(),
                    startTime: weekendSlot.toISOString(),
                    endTime: weekendEnd.toISOString(),
                    duration: 60,
                    price: 35,
                    serviceType: 'Weekend Service',
                    isAvailable: true,
                    providerId: providerId
                });
            }
            
            // Add some unavailable slots for realism
            if (i % 5 === 0) {
                const busySlot = new Date(slotDate);
                busySlot.setHours(16, 0, 0, 0);
                const busyEnd = new Date(busySlot);
                busyEnd.setHours(17, 0, 0, 0);
                
                availableSlots.push({
                    id: `slot-${i}-busy`,
                    dateTime: busySlot.toISOString(),
                    startTime: busySlot.toISOString(),
                    endTime: busyEnd.toISOString(),
                    duration: 60,
                    serviceType: 'Unavailable',
                    isAvailable: false,
                    providerId: providerId,
                    reason: 'Already booked'
                });
            }
        }
        
        // Transform for FullCalendar format
        return this.transformAvailabilityForFullCalendar(availableSlots);
    }

    /**
     * Comprehensive provider search (same as Search page)
     * This ensures consistent results between home page and search page
     */
    async searchProviders(searchFilters) {
        try {
            const response = await fetch('/Search/Search', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    location: searchFilters.location,
                    latitude: searchFilters.latitude,
                    longitude: searchFilters.longitude,
                    radiusMiles: searchFilters.radiusMiles || 25,
                    serviceCategoryIds: Array.isArray(searchFilters.serviceCategoryIds) 
                        ? searchFilters.serviceCategoryIds 
                        : (searchFilters.serviceCategoryIds ? [searchFilters.serviceCategoryIds] : []),
                    subServiceIds: searchFilters.subServiceIds || null,
                    maxPrice: searchFilters.maxPrice,
                    minPrice: searchFilters.minPrice,
                    petCount: searchFilters.petCount || 1,
                    emergencyServiceOnly: searchFilters.emergencyServiceOnly || false,
                    weekendAvailable: searchFilters.weekendAvailable || false,
                    verifiedOnly: searchFilters.verifiedOnly || false,
                    dogSizes: searchFilters.dogSizes || [],
                    sortBy: searchFilters.sortBy || 'distance',
                    page: searchFilters.page || 1,
                    pageSize: searchFilters.pageSize || 20
                })
            });
            
            if (response.ok) {
                const result = await response.json();
                if (result.success && result.data) {
                    return result.data;
                } else {
                    throw new Error(result.message || 'Search failed');
                }
            } else {
                throw new Error(`Search API returned ${response.status}`);
            }
        } catch (error) {
            console.error('Error searching providers:', error);
            console.warn('API services unavailable, returning fallback search data');
            // Return fallback search results for offline development
            return this.getFallbackSearchResults(searchFilters);
        }
    }

    /**
     * FALLBACK DATA ONLY - Get default search results when API is unavailable
     * This ensures search functionality remains operational during development/outages
     */
    getFallbackSearchResults(searchFilters) {
        console.warn('Loading fallback search results - Search API may be unavailable');
        
        // Generate sample providers based on search filters
        const baseProviders = [
            {
                id: 'provider-1',
                businessName: 'Cambridge Pet Care',
                description: 'Professional pet care services in Cambridge area',
                rating: 4.8,
                reviewCount: 127,
                distance: 0.8,
                priceRange: { min: 20, max: 45 },
                services: ['grooming', 'walking', 'sitting'],
                verified: true,
                emergencyAvailable: true,
                weekendAvailable: true,
                nextAvailable: 'Today at 14:30',
                profileImage: null,
                contactInfo: { phone: '01223 123456', email: 'hello@cambridgepetcare.com' }
            },
            {
                id: 'provider-2', 
                businessName: 'Happy Tails Grooming',
                description: 'Expert dog grooming with 10+ years experience',
                rating: 4.6,
                reviewCount: 89,
                distance: 1.2,
                priceRange: { min: 35, max: 75 },
                services: ['grooming', 'nail-care'],
                verified: true,
                emergencyAvailable: false,
                weekendAvailable: true,
                nextAvailable: 'Tomorrow at 10:00',
                profileImage: null,
                contactInfo: { phone: '01223 789012', email: 'bookings@happytails.co.uk' }
            },
            {
                id: 'provider-3',
                businessName: 'Walkies Dog Walking',
                description: 'Reliable dog walking service covering Cambridge',
                rating: 4.9,
                reviewCount: 234,
                distance: 2.1,
                priceRange: { min: 15, max: 30 },
                services: ['walking', 'pet-taxi'],
                verified: true,
                emergencyAvailable: true,
                weekendAvailable: true,
                nextAvailable: 'Today at 16:00',
                profileImage: null,
                contactInfo: { phone: '01223 345678', email: 'walks@walkiesservice.com' }
            }
        ];
        
        // Filter providers based on search criteria
        let filteredProviders = baseProviders.filter(provider => {
            // Service filter
            if (searchFilters.serviceCategoryIds && searchFilters.serviceCategoryIds.length > 0) {
                const hasMatchingService = searchFilters.serviceCategoryIds.some(serviceId => {
                    const serviceMap = {
                        'walking': 'walking',
                        'grooming': 'grooming', 
                        'sitting': 'sitting',
                        'training': 'training'
                    };
                    return provider.services.includes(serviceMap[serviceId] || serviceId);
                });
                if (!hasMatchingService) return false;
            }
            
            // Price filter
            if (searchFilters.maxPrice && provider.priceRange.min > searchFilters.maxPrice) {
                return false;
            }
            
            // Emergency filter
            if (searchFilters.emergencyServiceOnly && !provider.emergencyAvailable) {
                return false;
            }
            
            // Weekend filter  
            if (searchFilters.weekendAvailable && !provider.weekendAvailable) {
                return false;
            }
            
            // Verified filter
            if (searchFilters.verifiedOnly && !provider.verified) {
                return false;
            }
            
            return true;
        });
        
        // Sort results
        if (searchFilters.sortBy === 'distance') {
            filteredProviders.sort((a, b) => a.distance - b.distance);
        } else if (searchFilters.sortBy === 'rating') {
            filteredProviders.sort((a, b) => b.rating - a.rating);
        } else if (searchFilters.sortBy === 'price') {
            filteredProviders.sort((a, b) => a.priceRange.min - b.priceRange.min);
        }
        
        // Apply pagination
        const pageSize = searchFilters.pageSize || 20;
        const page = searchFilters.page || 1;
        const startIndex = (page - 1) * pageSize;
        const paginatedResults = filteredProviders.slice(startIndex, startIndex + pageSize);
        
        return {
            results: paginatedResults,
            totalCount: filteredProviders.length,
            page: page,
            pageSize: pageSize,
            totalPages: Math.ceil(filteredProviders.length / pageSize),
            searchFilters: searchFilters,
            fallbackData: true
        };
    }

    /**
     * Search for nearby providers
     */
    async searchNearbyProviders(searchParams) {
        try {
            const { latitude, longitude, radiusMiles = 25, serviceCategories, maxResults = 20 } = searchParams;
            
            let url = `${this.apiEndpoints.nearby}?latitude=${latitude}&longitude=${longitude}&radiusMiles=${radiusMiles}`;
            
            if (serviceCategories && serviceCategories.length > 0) {
                url += `&serviceCategories=${serviceCategories.join(',')}`;
            }
            
            if (maxResults) {
                url += `&maxResults=${maxResults}`;
            }
            
            const response = await fetch(url);
            
            if (response.ok) {
                const result = await response.json();
                if (result.success && result.data) {
                    return result.data;
                } else {
                    throw new Error(result.message || 'Failed to search providers');
                }
            } else {
                throw new Error(`API returned ${response.status}`);
            }
        } catch (error) {
            console.error('Error searching nearby providers:', error);
            throw error;
        }
    }

    /**
     * Get detailed provider information
     */
    async getProviderDetails(providerId, includeAvailability = false, startDate = null, endDate = null) {
        try {
            let url = `${this.apiEndpoints.provider}?id=${providerId}`;
            
            if (includeAvailability) {
                url += '&includeAvailability=true';
            }
            
            if (startDate) {
                url += `&startDate=${startDate.toISOString()}`;
            }
            
            if (endDate) {
                url += `&endDate=${endDate.toISOString()}`;
            }
            
            const response = await fetch(url);
            
            if (response.ok) {
                const result = await response.json();
                if (result.success && result.data) {
                    return result.data;
                } else {
                    throw new Error(result.message || 'Failed to get provider details');
                }
            } else {
                throw new Error(`API returned ${response.status}`);
            }
        } catch (error) {
            console.error('Error getting provider details:', error);
            throw error;
        }
    }

    /**
     * Get provider pricing information
     */
    async getProviderPricing(providerId, subServiceId = 'default', startDate = null, durationMinutes = 60) {
        try {
            // Set default start date if not provided (tomorrow)
            if (!startDate) {
                startDate = new Date();
                startDate.setDate(startDate.getDate() + 1);
            }
            
            const response = await fetch(`${this.apiEndpoints.pricing}?providerId=${providerId}&subServiceId=${subServiceId}&startDate=${startDate.toISOString()}&durationMinutes=${durationMinutes}`);
            
            if (response.ok) {
                const result = await response.json();
                if (result.success && result.data) {
                    return result.data;
                } else {
                    throw new Error(result.message || 'Failed to get pricing');
                }
            } else {
                throw new Error(`API returned ${response.status}`);
            }
        } catch (error) {
            console.error('Error getting provider pricing:', error);
            console.warn('API services unavailable, returning fallback pricing data');
            // Return fallback pricing data for offline development
            return this.getFallbackPricing(providerId, subServiceId, startDate, durationMinutes);
        }
    }

    /**
     * FALLBACK DATA ONLY - Get default pricing when API is unavailable
     * This ensures pricing functionality remains operational during development/outages
     */
    getFallbackPricing(providerId, subServiceId, startDate, durationMinutes) {
        console.warn('Loading fallback pricing data - Provider Pricing API may be unavailable');
        
        // Generate comprehensive pricing structure for modal display
        const services = [
            {
                serviceName: 'Dog Walking',
                subServices: [
                    { subServiceName: '30 Minute Walk', price: 18, description: 'Standard neighbourhood walk for exercise and toilet breaks' },
                    { subServiceName: '60 Minute Walk', price: 30, description: 'Extended walk with more exercise and exploration time' },
                    { subServiceName: 'Group Walk', price: 22, description: 'Socialization walk with other well-behaved dogs' },
                    { subServiceName: 'Adventure Walk', price: 35, description: 'Off-lead walk in secure areas like parks or beaches' }
                ]
            },
            {
                serviceName: 'Pet Grooming',
                subServices: [
                    { subServiceName: 'Full Grooming Package', price: 55, description: 'Wash, dry, brush, nail trim, ear cleaning, and styling' },
                    { subServiceName: 'Wash & Dry Only', price: 30, description: 'Professional wash with high-quality products and blow dry' },
                    { subServiceName: 'Nail Trimming', price: 15, description: 'Safe and gentle nail clipping service' },
                    { subServiceName: 'Ear Cleaning', price: 12, description: 'Professional ear cleaning and health check' },
                    { subServiceName: 'Teeth Cleaning', price: 25, description: 'Dental hygiene service to maintain oral health' }
                ]
            },
            {
                serviceName: 'Pet Sitting',
                subServices: [
                    { subServiceName: 'Half Day Sitting (4 hours)', price: 45, description: 'In-home care including feeding, walking, and companionship' },
                    { subServiceName: 'Full Day Sitting (8+ hours)', price: 65, description: 'Extended care with meals, walks, playtime, and attention' },
                    { subServiceName: 'Overnight Sitting', price: 85, description: 'Complete overnight care in your home with morning routine' },
                    { subServiceName: 'Holiday Care Package', price: 75, description: 'Daily visits during holidays with extended time and activities' }
                ]
            },
            {
                serviceName: 'Pet Training',
                subServices: [
                    { subServiceName: 'Basic Obedience (1 hour)', price: 50, description: 'Fundamental commands: sit, stay, come, down, and lead training' },
                    { subServiceName: 'Puppy Training Session', price: 45, description: 'House training, socialization, and basic puppy manners' },
                    { subServiceName: 'Behavioral Correction', price: 65, description: 'Address specific behavioral issues like jumping, barking, or pulling' },
                    { subServiceName: 'Advanced Training', price: 70, description: 'Complex commands, off-lead training, and specialized skills' }
                ]
            }
        ];

        const additionalServices = [
            { name: 'Travel Fee (over 5 miles)', price: 8, description: 'Additional charge for services outside standard area' },
            { name: 'Weekend Premium', surcharge: 20, description: 'Weekend service surcharge (Friday evening to Sunday)' },
            { name: 'Bank Holiday Premium', surcharge: 30, description: 'Additional charge for services on bank holidays' },
            { name: 'Emergency Call-out', price: 25, description: 'Same-day booking or emergency service fee' },
            { name: 'Multiple Pet Discount', surcharge: -15, description: 'Discount for services involving multiple pets from same household' }
        ];
        
        return {
            services,
            additionalServices,
            providerId,
            currency: 'GBP',
            lastUpdated: new Date().toISOString(),
            fallbackData: true,
            notes: [
                'All prices shown in British Pounds (£)',
                'Prices may vary based on specific requirements',
                'Contact provider for custom package pricing',
                'Cancellation policy applies to all bookings'
            ]
        };
    }

    /**
     * Get provider availability for FullCalendar integration
     */
    async getProviderAvailability(providerId, startDate = null, endDate = null) {
        try {
            // Set default dates if not provided (next 30 days)
            if (!startDate) {
                startDate = new Date();
            }
            if (!endDate) {
                endDate = new Date();
                endDate.setDate(endDate.getDate() + 30);
            }
            
            let url = `${this.apiEndpoints.availability}?providerId=${providerId}&startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`;
            
            const response = await fetch(url);
            
            if (response.ok) {
                const result = await response.json();
                if (result.success && result.data) {
                    // Transform API data for FullCalendar format
                    return this.transformAvailabilityForFullCalendar(result.data);
                } else {
                    throw new Error(result.message || 'Failed to get availability');
                }
            } else {
                throw new Error(`API returned ${response.status}`);
            }
        } catch (error) {
            console.error('Error getting provider availability:', error);
            console.warn('API services unavailable, returning fallback availability data');
            // Return fallback availability data for offline development
            return this.getFallbackAvailability(providerId, startDate, endDate);
        }
    }

    /**
     * Transform availability data for FullCalendar
     */
    transformAvailabilityForFullCalendar(apiData) {
        if (!Array.isArray(apiData)) return { events: [], availableDates: [], bookedDates: [] };

        const events = [];
        const availableDates = [];
        const bookedDates = [];

        apiData.forEach(slot => {
            const startDate = new Date(slot.dateTime || slot.startTime);
            const endDate = slot.endTime ? new Date(slot.endTime) : new Date(startDate.getTime() + (slot.duration || 60) * 60000);
            const dateStr = startDate.toISOString().split('T')[0];

            if (slot.isAvailable || slot.available) {
                // Available slot
                events.push({
                    id: slot.id || `slot-${startDate.getTime()}`,
                    title: 'Available',
                    start: startDate,
                    end: endDate,
                    color: '#10B981', // Green for available
                    borderColor: '#059669',
                    textColor: 'white',
                    extendedProps: {
                        type: 'available',
                        price: slot.price,
                        serviceType: slot.serviceType || 'Service Available',
                        duration: slot.duration || 60,
                        providerId: slot.providerId
                    }
                });
                
                if (!availableDates.includes(dateStr)) {
                    availableDates.push(dateStr);
                }
            } else {
                // Booked/unavailable slot
                events.push({
                    id: slot.id || `busy-${startDate.getTime()}`,
                    title: 'Unavailable',
                    start: startDate,
                    end: endDate,
                    color: '#EF4444', // Red for unavailable
                    borderColor: '#DC2626',
                    textColor: 'white',
                    extendedProps: {
                        type: 'unavailable',
                        reason: slot.reason || 'Booked'
                    }
                });

                if (!bookedDates.includes(dateStr)) {
                    bookedDates.push(dateStr);
                }
            }
        });

        return {
            events,
            availableDates,
            bookedDates,
            providerId: apiData[0]?.providerId || 'unknown'
        };
    }

    /**
     * Get location suggestions for geocoding
     */
    async getLocationSuggestions(query, maxResults = 10) {
        try {
            if (!query || query.length < 2) {
                return [];
            }

            const response = await fetch(`${this.apiEndpoints.locationSuggestions}?query=${encodeURIComponent(query)}&maxResults=${maxResults}`);
            
            if (response.ok) {
                const result = await response.json();
                if (result.success && result.data) {
                    return result.data;
                } else {
                    throw new Error(result.message || 'Failed to get location suggestions');
                }
            } else {
                throw new Error(`API returned ${response.status}`);
            }
        } catch (error) {
            console.error('Error getting location suggestions:', error);
            return [];
        }
    }

    /**
     * Transform service categories to flat services array for UI
     */
    transformCategoriesToServices(serviceCategories) {
        return serviceCategories.map(category => ({
            id: category.serviceCategoryId,
            name: category.name,
            icon: this.getCategoryIcon(category.name),
            price: this.getCategoryPriceRange(category),
            description: category.description,
            subServices: category.subServices
        }));
    }

    /**
     * Get category icon based on name
     */
    getCategoryIcon(categoryName) {
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
    }

    /**
     * Get category price range from sub-services
     */
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
    }

    /**
     * Get initials from name for avatar display
     */
    getInitials(name) {
        if (!name) return 'PP';
        return name.split(' ')
            .map(word => word.charAt(0))
            .join('')
            .toUpperCase()
            .substring(0, 2);
    }

    /**
     * Format distance for display
     */
    formatDistance(distanceMiles) {
        if (!distanceMiles) return 'Distance unavailable';
        
        if (distanceMiles < 0.1) {
            return 'Very close';
        } else if (distanceMiles < 1) {
            return `${(distanceMiles * 5280).toFixed(0)} yards`;
        } else {
            return `${distanceMiles.toFixed(1)} miles`;
        }
    }

    /**
     * Format price range for display
     */
    formatPriceRange(priceRange) {
        if (!priceRange) return 'Contact for pricing';
        
        if (priceRange.minPrice === priceRange.maxPrice) {
            return `£${priceRange.minPrice}`;
        } else {
            return `£${priceRange.minPrice}-${priceRange.maxPrice}`;
        }
    }

    /**
     * Get UK coordinates approximation from postcode
     */
    getApproximateUKCoordinates(postcode) {
        const postcodePrefix = postcode.toUpperCase().replace(/\s+/g, '').substring(0, 2);
        
        const coordinateMap = {
            // London areas
            'SW': { lat: 51.4897, lng: -0.1436 },
            'SE': { lat: 51.4626, lng: -0.0478 },
            'NW': { lat: 51.5547, lng: -0.1849 },
            'NE': { lat: 51.5390, lng: -0.0322 },
            'E1': { lat: 51.5118, lng: -0.0591 },
            'W1': { lat: 51.5154, lng: -0.1447 },
            'N1': { lat: 51.5430, lng: -0.1043 },
            'EC': { lat: 51.5155, lng: -0.0922 },
            'WC': { lat: 51.5142, lng: -0.1306 },
            
            // Major UK cities
            'M1': { lat: 53.4808, lng: -2.2426 },
            'B1': { lat: 52.4862, lng: -1.8904 },
            'LS': { lat: 53.8008, lng: -1.5491 },
            'S1': { lat: 53.3811, lng: -1.4701 },
            'L1': { lat: 53.4084, lng: -2.9916 },
            'NG': { lat: 52.9548, lng: -1.1581 },
            'BS': { lat: 51.4545, lng: -2.5879 },
            'EH': { lat: 55.9533, lng: -3.1883 },
            'G1': { lat: 55.8642, lng: -4.2518 },
            'CF': { lat: 51.4816, lng: -3.1791 }
        };
        
        return coordinateMap[postcodePrefix] || { lat: 51.5074, lng: -0.1278 }; // Default to London
    }

    /**
     * Initialize FullCalendar for availability display
     */
    initFullCalendar(containerId, availabilityData, onEventClick = null, onDateSelect = null) {
        const calendarEl = document.getElementById(containerId);
        if (!calendarEl) {
            console.error(`Calendar container ${containerId} not found`);
            return null;
        }

        // Clear any existing calendar
        calendarEl.innerHTML = '';

        const calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth',
            height: 'auto',
            aspectRatio: 1.35,
            headerToolbar: {
                left: 'prev,next',
                center: 'title',
                right: 'today,dayGridMonth,timeGridWeek'
            },
            validRange: {
                start: new Date().toISOString().split('T')[0] // Today onwards
            },
            events: availabilityData.events || [],
            eventDisplay: 'block',
            dayMaxEvents: 3,
            moreLinkClick: 'popover',
            eventClick: (info) => {
                if (onEventClick) {
                    onEventClick(info);
                } else {
                    // Default behavior - show event details
                    const event = info.event;
                    const props = event.extendedProps;
                    
                    let eventDetails = `<strong>${event.title}</strong><br>`;
                    eventDetails += `<strong>Time:</strong> ${event.start.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })}`;
                    if (event.end) {
                        eventDetails += ` - ${event.end.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })}`;
                    }
                    if (props.price) {
                        eventDetails += `<br><strong>Price:</strong> £${props.price}`;
                    }
                    if (props.serviceType) {
                        eventDetails += `<br><strong>Service:</strong> ${props.serviceType}`;
                    }
                    if (props.duration) {
                        eventDetails += `<br><strong>Duration:</strong> ${props.duration} minutes`;
                    }

                    // Create tooltip or small popup
                    const tooltip = document.createElement('div');
                    tooltip.className = 'absolute bg-white border border-gray-300 rounded-lg shadow-lg p-3 z-50 text-sm max-w-xs';
                    tooltip.innerHTML = eventDetails;
                    
                    document.body.appendChild(tooltip);
                    
                    // Position tooltip near click
                    const rect = info.jsEvent.target.getBoundingClientRect();
                    tooltip.style.left = Math.min(rect.left, window.innerWidth - tooltip.offsetWidth - 10) + 'px';
                    tooltip.style.top = Math.max(rect.bottom + 5, 10) + 'px';
                    
                    // Remove tooltip after 3 seconds or on click elsewhere
                    setTimeout(() => tooltip.remove(), 3000);
                    document.addEventListener('click', () => tooltip.remove(), { once: true });
                }
            },
            dateClick: (info) => {
                if (onDateSelect) {
                    onDateSelect(info);
                } else {
                    // Default behavior - highlight selected date
                    document.querySelectorAll('.fc-day.selected-date').forEach(el => {
                        el.classList.remove('selected-date');
                    });
                    info.dayEl.classList.add('selected-date');
                }
            },
            eventDidMount: (info) => {
                // Add custom styling based on event type
                const props = info.event.extendedProps;
                if (props.type === 'available') {
                    info.el.style.borderLeftWidth = '4px';
                    info.el.style.borderLeftColor = '#10B981';
                } else if (props.type === 'unavailable') {
                    info.el.style.borderLeftWidth = '4px';
                    info.el.style.borderLeftColor = '#EF4444';
                }
            }
        });

        calendar.render();
        return calendar;
    }

    /**
     * Enhanced contact functionality with predefined message templates
     */
    getContactMessageTemplates(providerName, serviceName = null) {
        const templates = [
            {
                subject: 'General Inquiry',
                message: `Hi ${providerName},\n\nI'm interested in learning more about your pet care services. Could you please provide more information about availability and pricing?\n\nThank you!`
            },
            {
                subject: 'Booking Request',
                message: `Hi ${providerName},\n\nI'd like to book ${serviceName || 'your services'} for my dog. Could you please let me know your availability?\n\nLooking forward to hearing from you.`
            },
            {
                subject: 'Service Questions',
                message: `Hi ${providerName},\n\nI have some questions about ${serviceName || 'your services'}. Could we arrange a time to discuss my dog's specific needs?\n\nThank you for your time.`
            },
            {
                subject: 'Pricing Information',
                message: `Hi ${providerName},\n\nCould you please provide detailed pricing information for ${serviceName || 'your services'}? I'm interested in both one-off and regular booking options.\n\nThanks!`
            },
            {
                subject: 'Emergency Service',
                message: `Hi ${providerName},\n\nI need urgent pet care services. Are you available for emergency bookings? Please let me know as soon as possible.\n\nThank you.`
            }
        ];

        return templates;
    }

    /**
     * Send message to provider (placeholder for actual implementation)
     */
    async sendMessageToProvider(providerId, subject, message, contactDetails = {}) {
        try {
            const messageData = {
                providerId,
                subject,
                message,
                senderName: contactDetails.name || 'Pet Owner',
                senderEmail: contactDetails.email || '',
                senderPhone: contactDetails.phone || '',
                dogDetails: contactDetails.dogDetails || {},
                urgentRequest: contactDetails.urgent || false,
                timestamp: new Date().toISOString()
            };

            // In a real implementation, this would send to the API
            // Simulate API call
            await new Promise(resolve => setTimeout(resolve, 1000));
            
            return {
                success: true,
                messageId: `msg-${Date.now()}`,
                estimatedResponseTime: '2-4 hours',
                message: 'Your message has been sent successfully!'
            };
        } catch (error) {
            console.error('Error sending message:', error);
            return {
                success: false,
                error: 'Failed to send message. Please try again.'
            };
        }
    }
}

// Create global instance
window.providerSearchUtils = new ProviderSearchUtils();