/**
 * Service Discovery Page JavaScript
 * Handles all dynamic functionality for the service provider search page
 */

class ServiceDiscoveryManager {
    constructor() {
        this.map = null;
        this.mobileMap = null;
        this.markers = [];
        this.infoWindows = [];
        this.currentLocation = null;
        
        this.init();
    }

    init() {
        // Initialize maps when Google Maps API is loaded
        if (typeof google !== 'undefined' && google.maps) {
            this.initializeMaps();
        } else {
            // Wait for Google Maps to load
            window.initMap = () => this.initializeMaps();
        }
    }

    initializeMaps() {
        const defaultCenter = { lat: 51.5074, lng: -0.1278 }; // London
        
        const mapOptions = {
            center: defaultCenter,
            zoom: 13,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false,
            styles: [
                {
                    featureType: "poi",
                    elementType: "labels",
                    stylers: [{ visibility: "off" }]
                },
                {
                    featureType: "transit",
                    elementType: "labels",
                    stylers: [{ visibility: "off" }]
                }
            ]
        };

        // Initialize desktop map
        const desktopMapElement = document.getElementById('map');
        if (desktopMapElement) {
            this.map = new google.maps.Map(desktopMapElement, mapOptions);
            this.setupMapEventListeners(this.map);
        }

        // Initialize mobile map
        const mobileMapElement = document.getElementById('mobile-map');
        if (mobileMapElement) {
            this.mobileMap = new google.maps.Map(mobileMapElement, mapOptions);
            this.setupMapEventListeners(this.mobileMap);
        }

        // Try to get user's current location
        this.getCurrentLocation();
    }

    setupMapEventListeners(map) {
        // Add click listener for map
        map.addListener('click', (event) => {
            this.closeAllInfoWindows();
        });

        // Handle resize
        google.maps.event.addListener(map, 'resize', () => {
            map.setCenter(map.getCenter());
        });
    }

    getCurrentLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (position) => {
                    this.currentLocation = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };
                    
                    // Center maps on user location
                    if (this.map) {
                        this.map.setCenter(this.currentLocation);
                        this.addUserLocationMarker(this.map);
                    }
                    if (this.mobileMap) {
                        this.mobileMap.setCenter(this.currentLocation);
                        this.addUserLocationMarker(this.mobileMap);
                    }
                },
                (error) => {
                    console.warn('Geolocation error:', error);
                },
                {
                    enableHighAccuracy: true,
                    timeout: 10000,
                    maximumAge: 300000 // 5 minutes
                }
            );
        }
    }

    addUserLocationMarker(map) {
        if (!this.currentLocation) return;

        const userMarker = new google.maps.Marker({
            position: this.currentLocation,
            map: map,
            title: 'Your Location',
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 10,
                fillColor: '#4285F4',
                fillOpacity: 1,
                strokeColor: '#FFFFFF',
                strokeWeight: 3
            },
            zIndex: 1000
        });

        const infoWindow = new google.maps.InfoWindow({
            content: `
                <div class="p-2">
                    <strong>Your Location</strong>
                    <br>
                    <small class="text-gray-600">Current position</small>
                </div>
            `
        });

        userMarker.addListener('click', () => {
            this.closeAllInfoWindows();
            infoWindow.open(map, userMarker);
        });
    }

    updateMapWithProviders(providers) {
        // Clear existing provider markers
        this.clearProviderMarkers();

        if (!providers || providers.length === 0) return;

        const bounds = new google.maps.LatLngBounds();
        let hasValidCoords = false;

        providers.forEach((provider, index) => {
            if (provider.location && provider.location.latitude && provider.location.longitude) {
                const position = {
                    lat: parseFloat(provider.location.latitude),
                    lng: parseFloat(provider.location.longitude)
                };

                // Add to both maps
                if (this.map) {
                    this.createProviderMarker(this.map, provider, position, index);
                }
                if (this.mobileMap) {
                    this.createProviderMarker(this.mobileMap, provider, position, index);
                }

                bounds.extend(position);
                hasValidCoords = true;
            }
        });

        // Adjust map bounds to show all markers
        if (hasValidCoords) {
            if (this.map) {
                this.map.fitBounds(bounds);
                if (this.map.getZoom() > 15) {
                    this.map.setZoom(15);
                }
            }
            if (this.mobileMap) {
                this.mobileMap.fitBounds(bounds);
                if (this.mobileMap.getZoom() > 15) {
                    this.mobileMap.setZoom(15);
                }
            }
        }
    }

    createProviderMarker(map, provider, position, index) {
        const isPremium = provider.isPremium || provider.isVerified;
        
        const marker = new google.maps.Marker({
            position: position,
            map: map,
            title: provider.businessName,
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: isPremium ? 14 : 10,
                fillColor: isPremium ? '#FFA500' : '#FF8C42',
                fillOpacity: 1,
                strokeColor: '#FFFFFF',
                strokeWeight: 2,
                labelOrigin: new google.maps.Point(0, 0)
            },
            label: {
                text: (index + 1).toString(),
                color: 'white',
                fontSize: '12px',
                fontWeight: 'bold'
            },
            zIndex: 100
        });

        const infoContent = this.createInfoWindowContent(provider);
        const infoWindow = new google.maps.InfoWindow({
            content: infoContent,
            maxWidth: 300
        });

        marker.addListener('click', () => {
            this.closeAllInfoWindows();
            infoWindow.open(map, marker);
        });

        // Store references for cleanup
        this.markers.push(marker);
        this.infoWindows.push(infoWindow);
    }

    createInfoWindowContent(provider) {
        const rating = provider.rating || null;
        const reviewCount = provider.reviewCount > 0 ? provider.reviewCount : null;
        const priceRange = provider.priceRange;
        const services = provider.services || [];

        return `
            <div class="p-3 max-w-xs">
                <div class="flex items-start space-x-3">
                    <img src="${provider.profileImageUrl || 'https://via.placeholder.com/60'}" 
                         alt="${provider.businessName}"
                         class="w-12 h-12 object-cover rounded-lg">
                    <div class="flex-1 min-w-0">
                        <h4 class="font-semibold text-gray-800 text-sm truncate">
                            ${provider.businessName}
                            ${provider.isVerified ? '<i class="fas fa-check-circle text-green-500 ml-1"></i>' : ''}
                            ${provider.isPremium ? '<i class="fas fa-crown text-yellow-500 ml-1"></i>' : ''}
                        </h4>
                        <div class="flex items-center mt-1">
                            ${rating ? `
                                <span class="text-yellow-500 text-sm">★</span>
                                <span class="ml-1 text-sm font-medium">${rating.toFixed(1)}</span>
                            ` : ''}
                            <span class="ml-1 text-sm text-gray-500">${reviewCount > 0 ? `(${reviewCount} reviews)` : '(No reviews yet)'}</span>
                        </div>
                        ${priceRange ? `
                            <div class="text-sm text-pet-orange font-medium mt-1">
                                £${priceRange.minPrice}-${priceRange.maxPrice}
                            </div>
                        ` : ''}
                    </div>
                </div>
                
                ${services.length > 0 ? `
                    <div class="mt-2 text-xs text-gray-600">
                        ${services.slice(0, 3).map(s => s.serviceName).join(', ')}
                        ${services.length > 3 ? `... +${services.length - 3} more` : ''}
                    </div>
                ` : ''}
                
                <div class="mt-3 flex space-x-2">
                    <button onclick="window.serviceDiscovery.viewProvider('${provider.id}')" 
                            class="flex-1 bg-pet-orange text-white text-xs py-2 px-3 rounded hover:bg-orange-600 transition-colors">
                        View Details
                    </button>
                    <button onclick="window.serviceDiscovery.contactProvider('${provider.id}')" 
                            class="flex-1 border border-pet-orange text-pet-orange text-xs py-2 px-3 rounded hover:bg-orange-50 transition-colors">
                        Contact
                    </button>
                </div>
            </div>
        `;
    }

    clearProviderMarkers() {
        // Remove all provider markers (keep user location marker)
        this.markers.forEach(marker => {
            if (marker.getTitle() !== 'Your Location') {
                marker.setMap(null);
            }
        });
        this.markers = this.markers.filter(marker => marker.getTitle() === 'Your Location');
        
        this.infoWindows.forEach(infoWindow => {
            infoWindow.close();
        });
        this.infoWindows = [];
    }

    closeAllInfoWindows() {
        this.infoWindows.forEach(infoWindow => {
            infoWindow.close();
        });
    }

    // Public methods for Alpine.js integration
    viewProvider(providerId) {
        // This will be called from the info window
        // Dispatch custom event that Alpine.js can listen to
        window.dispatchEvent(new CustomEvent('provider-selected', {
            detail: { providerId }
        }));
    }

    contactProvider(providerId) {
        // This will be called from the info window
        // Dispatch custom event that Alpine.js can listen to
        window.dispatchEvent(new CustomEvent('provider-contact', {
            detail: { providerId }
        }));
    }

    // Method to be called by Alpine.js when search results update
    updateProviders(providers) {
        this.updateMapWithProviders(providers);
    }

    // Method to center map on a specific location
    centerOnLocation(lat, lng, zoom = 13) {
        const position = { lat: parseFloat(lat), lng: parseFloat(lng) };
        
        if (this.map) {
            this.map.setCenter(position);
            this.map.setZoom(zoom);
        }
        if (this.mobileMap) {
            this.mobileMap.setCenter(position);
            this.mobileMap.setZoom(zoom);
        }
    }

    // Handle responsive changes
    handleResize() {
        setTimeout(() => {
            if (this.map) {
                google.maps.event.trigger(this.map, 'resize');
            }
            if (this.mobileMap) {
                google.maps.event.trigger(this.mobileMap, 'resize');
            }
        }, 100);
    }
}

// Utility functions for formatting
window.ServiceDiscoveryUtils = {
    formatDistance(miles) {
        if (miles < 1) {
            return `${Math.round(miles * 1760)} yards`;
        }
        return `${miles.toFixed(1)} miles`;
    },

    formatPrice(price) {
        return `£${price.toFixed(2)}`;
    },

    formatRating(rating, reviewCount) {
        if (!rating && (!reviewCount || reviewCount === 0)) {
            return 'No reviews yet';
        }
        let result = '';
        if (rating) {
            result += rating.toFixed(1);
        }
        if (reviewCount > 0) {
            result += ` (${reviewCount} review${reviewCount !== 1 ? 's' : ''})`;
        } else if (!rating) {
            result = 'No reviews yet';
        }
        return result;
    },

    timeAgo(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffInMinutes = Math.floor((now - date) / (1000 * 60));
        
        if (diffInMinutes < 60) {
            return `${diffInMinutes} minutes ago`;
        } else if (diffInMinutes < 1440) {
            const hours = Math.floor(diffInMinutes / 60);
            return `${hours} hour${hours !== 1 ? 's' : ''} ago`;
        } else {
            const days = Math.floor(diffInMinutes / 1440);
            return `${days} day${days !== 1 ? 's' : ''} ago`;
        }
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.serviceDiscovery = new ServiceDiscoveryManager();
    
    // Listen for Alpine.js events
    window.addEventListener('provider-selected', (event) => {
        // Handle provider selection
        // This can trigger Alpine.js methods or navigate to provider details
    });
    
    window.addEventListener('provider-contact', (event) => {
        // Handle provider contact
        // This can trigger Alpine.js methods or show contact modal
    });
    
    // Handle window resize
    window.addEventListener('resize', () => {
        window.serviceDiscovery.handleResize();
    });
});

// Global map initialization callback for Google Maps API
window.initMap = function() {
    if (window.serviceDiscovery) {
        window.serviceDiscovery.initializeMaps();
    }
};