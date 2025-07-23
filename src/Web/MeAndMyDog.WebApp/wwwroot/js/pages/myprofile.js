// My Profile Page JavaScript
(function () {
    'use strict';

    // Profile API Service
    window.profileService = {
        // Get user profile data
        async getProfile() {
            try {
                const response = await fetch('/api/profile', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to fetch profile');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error fetching profile:', error);
                return null;
            }
        },

        // Update profile
        async updateProfile(profileData) {
            try {
                const response = await fetch('/api/profile', {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    },
                    body: JSON.stringify(profileData)
                });
                
                if (!response.ok) throw new Error('Failed to update profile');
                
                const result = await response.json();
                return result;
            } catch (error) {
                console.error('Error updating profile:', error);
                throw error;
            }
        },

        // Upload profile photo
        async uploadPhoto(file) {
            try {
                const formData = new FormData();
                formData.append('photo', file);
                
                const response = await fetch('/api/profile/photo', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    },
                    body: formData
                });
                
                if (!response.ok) throw new Error('Failed to upload photo');
                
                const result = await response.json();
                return result.data.photoUrl;
            } catch (error) {
                console.error('Error uploading photo:', error);
                throw error;
            }
        },

        // Get pets
        async getPets() {
            try {
                const response = await fetch('/api/pets', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to fetch pets');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error fetching pets:', error);
                return [];
            }
        },

        // Add pet
        async addPet(petData) {
            try {
                const response = await fetch('/api/pets', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    },
                    body: JSON.stringify(petData)
                });
                
                if (!response.ok) throw new Error('Failed to add pet');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error adding pet:', error);
                throw error;
            }
        },

        // Get services (for providers)
        async getServices() {
            try {
                const response = await fetch('/api/provider/services', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to fetch services');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error fetching services:', error);
                return [];
            }
        },

        // Add service
        async addService(serviceData) {
            try {
                const response = await fetch('/api/provider/services', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    },
                    body: JSON.stringify(serviceData)
                });
                
                if (!response.ok) throw new Error('Failed to add service');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error adding service:', error);
                throw error;
            }
        },

        // Get provider availability
        async getAvailability(startDate, endDate) {
            try {
                const response = await fetch(`/api/provider/availability?start=${startDate}&end=${endDate}`, {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to fetch availability');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error fetching availability:', error);
                return [];
            }
        },

        // Update availability
        async updateAvailability(availabilityData) {
            try {
                const response = await fetch('/api/provider/availability', {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    },
                    body: JSON.stringify(availabilityData)
                });
                
                if (!response.ok) throw new Error('Failed to update availability');
                
                const result = await response.json();
                return result;
            } catch (error) {
                console.error('Error updating availability:', error);
                throw error;
            }
        },

        // Get reviews
        async getReviews() {
            try {
                const response = await fetch('/api/reviews', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to fetch reviews');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error fetching reviews:', error);
                return [];
            }
        },

        // Respond to review
        async respondToReview(reviewId, response) {
            try {
                const apiResponse = await fetch(`/api/reviews/${reviewId}/response`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    },
                    body: JSON.stringify({ response })
                });
                
                if (!apiResponse.ok) throw new Error('Failed to respond to review');
                
                const result = await apiResponse.json();
                return result;
            } catch (error) {
                console.error('Error responding to review:', error);
                throw error;
            }
        },

        // Get favorite providers
        async getFavorites() {
            try {
                const response = await fetch('/api/favorites', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to fetch favorites');
                
                const result = await response.json();
                return result.data;
            } catch (error) {
                console.error('Error fetching favorites:', error);
                return [];
            }
        },

        // Remove favorite
        async removeFavorite(providerId) {
            try {
                const response = await fetch(`/api/favorites/${providerId}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to remove favorite');
                
                return true;
            } catch (error) {
                console.error('Error removing favorite:', error);
                throw error;
            }
        },

        // Start KYC verification
        async startVerification() {
            try {
                const response = await fetch('/api/verification/start', {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) throw new Error('Failed to start verification');
                
                const result = await response.json();
                // Redirect to Didit KYC flow
                window.location.href = result.data.verificationUrl;
            } catch (error) {
                console.error('Error starting verification:', error);
                throw error;
            }
        }
    };

    // Helper functions
    window.profileHelpers = {
        // Format date
        formatDate(dateString) {
            const date = new Date(dateString);
            return date.toLocaleDateString('en-GB', {
                day: 'numeric',
                month: 'short',
                year: 'numeric'
            });
        },

        // Format currency
        formatCurrency(amount) {
            return new Intl.NumberFormat('en-GB', {
                style: 'currency',
                currency: 'GBP'
            }).format(amount);
        },

        // Validate email
        validateEmail(email) {
            const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        },

        // Validate phone
        validatePhone(phone) {
            const re = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{4,6}$/;
            return re.test(phone);
        },

        // Show toast notification
        showToast(message, type = 'success') {
            // Create toast element
            const toast = document.createElement('div');
            toast.className = `fixed bottom-4 right-4 px-6 py-3 rounded-lg text-white z-50 transform transition-transform duration-300 ${
                type === 'success' ? 'bg-green-600' : type === 'error' ? 'bg-red-600' : 'bg-blue-600'
            }`;
            toast.innerHTML = `
                <div class="flex items-center gap-3">
                    <i class="fas ${type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-exclamation-circle' : 'fa-info-circle'}"></i>
                    <span>${message}</span>
                </div>
            `;
            
            // Add to DOM
            document.body.appendChild(toast);
            
            // Animate in
            setTimeout(() => {
                toast.style.transform = 'translateY(0)';
            }, 100);
            
            // Remove after 3 seconds
            setTimeout(() => {
                toast.style.transform = 'translateY(100px)';
                setTimeout(() => {
                    toast.remove();
                }, 300);
            }, 3000);
        },

        // Generate friend code
        generateFriendCode() {
            const prefix = 'WOOF';
            const numbers = Math.floor(Math.random() * 10000).toString().padStart(4, '0');
            return `${prefix}-${numbers}`;
        },

        // Calculate profile completion
        calculateCompletion(profile) {
            const fields = [
                'name',
                'email',
                'phone',
                'location',
                'bio',
                'profilePhoto',
                'verificationStatus'
            ];
            
            const completed = fields.filter(field => profile[field] && profile[field] !== '').length;
            return Math.round((completed / fields.length) * 100);
        }
    };

    // Initialize on page load
    document.addEventListener('DOMContentLoaded', () => {
    });
})();