// Search Page JavaScript
// Alpine.js component data
document.addEventListener('alpine:init', () => {
    Alpine.data('serviceDiscovery', () => ({
        selectedService: 'grooming', 
        selectedSubService: null,
        showMobileFilters: false,
        showMobileMenu: false,
        providers: [], // Will be loaded from API
        showAvailabilityModal: false,
        showPricesModal: false,
        showContactModal: false,
        showBookingSheet: false,
        selectedProvider: null,
        selectedTimeSlot: null,
        providerPricing: null,
        providerAvailability: null,
        emergencyAvailabilityCount: 0,
        nextAvailableSlot: null,
        userLocation: 'SW1A 1AA',
        radiusMiles: 5,
        numberOfDogs: 1,
        dogSizes: [],
        emergencyAvailable: false,
        weekendAvailable: false,
        verifiedOnly: false,
        mapExpanded: false,
        priceRange: 80,
        selectedDate: null,
        calendarInstance: null,
        selectedDay: 'today',
        mobileView: 'results',
        loading: false,
        initialDateRange: null,
        smartSearchQuery: null,
        
        async init() {
            // Parse URL parameters and populate form fields
            this.parseUrlParameters();
            
            // Initialize map if Google Maps hasn't already done it
            if (!window.hasGoogleMapsKey) {
                window.initStaticMap();
            }
            // Load providers from API when component initializes
            await this.searchProviders();
            // Load emergency availability data
            await this.loadEmergencyAvailability();
        },

        parseUrlParameters() {
            const urlParams = new URLSearchParams(window.location.search);
            
            // Parse location parameter
            const location = urlParams.get('location');
            if (location) {
                this.userLocation = decodeURIComponent(location);
            }
            
            // Parse service parameter
            const service = urlParams.get('service');
            if (service) {
                const [mainService, subService] = service.split(':');
                this.selectedService = mainService;
                if (subService) {
                    this.selectedSubService = subService;
                }
            }
            
            // Parse other parameters
            const dateRange = urlParams.get('dateRange');
            if (dateRange) {
                this.initialDateRange = dateRange;
                // Initialize date range picker with the value
                setTimeout(() => {
                    const dateRangeInput = document.getElementById('dateRange');
                    if (dateRangeInput && window.flatpickr) {
                        flatpickr(dateRangeInput, {
                            mode: "range",
                            dateFormat: "Y-m-d",
                            defaultDate: dateRange.split(' to ')
                        });
                    }
                }, 100);
            }
            
            const petCount = urlParams.get('petCount');
            if (petCount) {
                this.numberOfDogs = parseInt(petCount) || 1;
            }
            
            const query = urlParams.get('q');
            if (query) {
                this.smartSearchQuery = decodeURIComponent(query);
            }
        },

        async searchProviders() {
            this.loading = true;
            try {
                // Build query parameters
                const params = new URLSearchParams({
                    location: this.userLocation,
                    service: this.selectedSubService ? `${this.selectedService}:${this.selectedSubService}` : this.selectedService,
                    radius: this.radiusMiles,
                    numberOfDogs: this.numberOfDogs,
                    emergencyAvailable: this.emergencyAvailable,
                    weekendAvailable: this.weekendAvailable,
                    verifiedOnly: this.verifiedOnly,
                    maxPrice: this.priceRange
                });

                if (this.dogSizes.length > 0) {
                    params.append('dogSizes', this.dogSizes.join(','));
                }

                const response = await fetch(`/Search/SearchProviders?${params.toString()}`);
                if (response.ok) {
                    this.providers = await response.json();
                    // Update map markers
                    if (window.updateMapMarkers) {
                        window.updateMapMarkers(this.providers);
                    }
                } else {
                    console.error('Failed to search providers');
                }
            } catch (error) {
                console.error('Error searching providers:', error);
            } finally {
                this.loading = false;
            }
        },

        async loadEmergencyAvailability() {
            try {
                const response = await fetch('/Search/GetEmergencyAvailability');
                if (response.ok) {
                    const data = await response.json();
                    this.emergencyAvailabilityCount = data.count;
                    this.nextAvailableSlot = data.nextSlot;
                }
            } catch (error) {
                console.error('Error loading emergency availability:', error);
            }
        },

        formatPrice(price) {
            return `£${price}`;
        },

        async viewProviderAvailability(provider) {
            this.selectedProvider = provider;
            this.showAvailabilityModal = true;
            
            // Load availability data
            try {
                const response = await fetch(`/Search/GetProviderAvailability?providerId=${provider.id}`);
                if (response.ok) {
                    this.providerAvailability = await response.json();
                    // Initialize calendar after modal is shown
                    this.$nextTick(() => {
                        this.initializeAvailabilityCalendar();
                    });
                }
            } catch (error) {
                console.error('Error loading availability:', error);
            }
        },

        async viewProviderPrices(provider) {
            this.selectedProvider = provider;
            this.showPricesModal = true;
            
            // Load pricing data
            try {
                const response = await fetch(`/Search/GetProviderPricing?providerId=${provider.id}`);
                if (response.ok) {
                    this.providerPricing = await response.json();
                }
            } catch (error) {
                console.error('Error loading pricing:', error);
            }
        },

        contactProvider(provider) {
            this.selectedProvider = provider;
            this.showContactModal = true;
        },

        async sendMessage() {
            // Implementation for sending message
            // Close modal
            this.showContactModal = false;
        },

        initializeAvailabilityCalendar() {
            const calendarEl = document.getElementById('availabilityCalendar');
            if (!calendarEl || !window.FullCalendar) return;

            if (this.calendarInstance) {
                this.calendarInstance.destroy();
            }

            this.calendarInstance = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek'
                },
                events: this.providerAvailability?.availableSlots || [],
                eventClick: (info) => {
                    this.selectedTimeSlot = info.event;
                    this.showBookingSheet = true;
                }
            });

            this.calendarInstance.render();
        },

        async bookTimeSlot() {
            // Implementation for booking
            this.showBookingSheet = false;
            this.showAvailabilityModal = false;
        },

        updatePriceRangeLabel(value) {
            this.priceRange = value;
        },

        toggleMobileView(view) {
            this.mobileView = view;
            if (view === 'map') {
                // Trigger map resize if needed
                setTimeout(() => {
                    if (window.google && window.map) {
                        google.maps.event.trigger(window.map, 'resize');
                    }
                }, 100);
            }
        },

        toggleFilter(filter, value) {
            if (filter === 'dogSize') {
                const index = this.dogSizes.indexOf(value);
                if (index > -1) {
                    this.dogSizes.splice(index, 1);
                } else {
                    this.dogSizes.push(value);
                }
            } else {
                this[filter] = value;
            }
            this.searchProviders();
        },

        // Ensure Alpine data is available globally for provider-search-utils.js
        getAlpineData() {
            return this;
        }
    }));
});

// Map initialization functions
function initMap() {
    if (!window.google || !document.getElementById('map')) {
        window.initStaticMap();
        return;
    }
    
    const mapElement = document.getElementById('map');
    const map = new google.maps.Map(mapElement, {
        center: { lat: 51.5074, lng: -0.1278 }, // London coordinates
        zoom: 12,
        styles: [
            {
                featureType: "poi",
                elementType: "labels",
                stylers: [{ visibility: "off" }]
            }
        ]
    });
    
    window.map = map;
    
    // Get Alpine.js component data
    const alpineComponent = document.querySelector('[x-data*="serviceDiscovery"]')?._x_dataStack?.[0];
    if (alpineComponent && alpineComponent.providers) {
        updateMapMarkers(alpineComponent.providers);
    }
}

function initStaticMap() {
    const mapElement = document.getElementById('map');
    if (!mapElement) return;
    
    // Show a static map placeholder
    mapElement.innerHTML = `
        <div class="flex items-center justify-center h-full bg-gray-100">
            <div class="text-center text-gray-500">
                <i class="fas fa-map-marker-alt text-4xl mb-2"></i>
                <p>Map view unavailable</p>
                <p class="text-sm">Google Maps API key required</p>
            </div>
        </div>
    `;
}

function updateMapMarkers(providers) {
    if (!window.map || !window.google) return;
    
    // Clear existing markers
    if (window.markers) {
        window.markers.forEach(marker => marker.setMap(null));
    }
    window.markers = [];
    
    // Add markers for providers
    providers.forEach(provider => {
        if (provider.latitude && provider.longitude) {
            const marker = new google.maps.Marker({
                position: { lat: provider.latitude, lng: provider.longitude },
                map: window.map,
                title: provider.businessName,
                icon: {
                    url: 'https://maps.google.com/mapfiles/ms/icons/blue-dot.png',
                    scaledSize: new google.maps.Size(40, 40)
                }
            });
            
            const infoWindow = new google.maps.InfoWindow({
                content: `
                    <div class="p-2">
                        <h3 class="font-semibold">${provider.businessName}</h3>
                        <p class="text-sm text-gray-600">${provider.distance}</p>
                        <p class="text-sm font-medium text-green-600">From £${provider.minPrice}</p>
                    </div>
                `
            });
            
            marker.addListener('click', () => {
                infoWindow.open(window.map, marker);
            });
            
            window.markers.push(marker);
        }
    });
    
    // Fit map to show all markers
    if (window.markers.length > 0) {
        const bounds = new google.maps.LatLngBounds();
        window.markers.forEach(marker => {
            bounds.extend(marker.getPosition());
        });
        window.map.fitBounds(bounds);
    }
}

// Export functions to global scope
window.initMap = initMap;
window.initStaticMap = initStaticMap;
window.updateMapMarkers = updateMapMarkers;