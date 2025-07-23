// Pets Page JavaScript
function petProfileApp() {
    return {
        // State
        loading: true,
        saving: false,
        addingPet: false,
        pets: [],
        selectedPetId: null,
        selectedPet: null,
        editingPet: {},
        showAddPetModal: false,
        newPet: {
            name: '',
            breed: ''
        },
        
        // Breed autocomplete state
        breedSuggestions: [],
        showBreedSuggestions: {
            new: false,
            edit: false
        },
        breedSearchTimeout: null,
        activeTab: 'basic',
        tabs: [
            { id: 'basic', label: 'Basic Info', icon: 'fas fa-info-circle' },
            { id: 'health', label: 'Health', icon: 'fas fa-heartbeat' },
            { id: 'care', label: 'Care', icon: 'fas fa-calendar-check' },
            { id: 'photos', label: 'Photos', icon: 'fas fa-camera' }
        ],

        // Initialize
        async init() {
            await this.loadPets();
            this.loading = false;
        },

        // Load pets from API
        async loadPets() {
            try {
                const response = await fetch('/api/v1/pets');
                if (response.ok) {
                    this.pets = await response.json();
                    if (this.pets.length > 0) {
                        this.selectedPetId = this.pets[0].id;
                        this.switchPet();
                    }
                }
            } catch (error) {
                console.error('Error loading pets:', error);
            }
        },

        // Switch selected pet
        switchPet() {
            this.selectedPet = this.pets.find(p => p.id === this.selectedPetId);
            if (this.selectedPet) {
                this.editingPet = { ...this.selectedPet };
                if (this.editingPet.dateOfBirth) {
                    this.editingPet.dateOfBirth = this.editingPet.dateOfBirth.split('T')[0];
                }
            }
        },

        // Save pet information
        async savePetInfo() {
            this.saving = true;
            try {
                const response = await fetch(`/api/v1/pets/${this.selectedPetId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(this.editingPet)
                });

                if (response.ok) {
                    const updatedPet = await response.json();
                    // Update the pet in the list
                    const index = this.pets.findIndex(p => p.id === this.selectedPetId);
                    if (index !== -1) {
                        this.pets[index] = updatedPet;
                        this.selectedPet = updatedPet;
                    }
                    this.showSuccessMessage('Pet information updated successfully!');
                } else {
                    this.showErrorMessage('Failed to update pet information');
                }
            } catch (error) {
                console.error('Error saving pet:', error);
                this.showErrorMessage('An error occurred while saving');
            } finally {
                this.saving = false;
            }
        },

        // Cancel editing
        cancelEdit() {
            if (this.selectedPet) {
                this.editingPet = { ...this.selectedPet };
                if (this.editingPet.dateOfBirth) {
                    this.editingPet.dateOfBirth = this.editingPet.dateOfBirth.split('T')[0];
                }
            }
        },

        // Add new pet
        async addNewPet() {
            this.addingPet = true;
            try {
                const response = await fetch('/api/v1/pets', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(this.newPet)
                });

                if (response.ok) {
                    const newPet = await response.json();
                    this.pets.push(newPet);
                    this.selectedPetId = newPet.id;
                    this.switchPet();
                    this.showAddPetModal = false;
                    this.resetNewPet();
                    this.showSuccessMessage('Pet added successfully!');
                } else {
                    this.showErrorMessage('Failed to add pet');
                }
            } catch (error) {
                console.error('Error adding pet:', error);
                this.showErrorMessage('An error occurred while adding pet');
            } finally {
                this.addingPet = false;
            }
        },

        // Reset new pet form
        resetNewPet() {
            this.newPet = {
                name: '',
                breed: ''
            };
            this.breedSuggestions = [];
            this.showBreedSuggestions.new = false;
            this.showBreedSuggestions.edit = false;
        },

        // Breed autocomplete methods
        searchBreeds(query, context) {
            // Clear previous timeout
            if (this.breedSearchTimeout) {
                clearTimeout(this.breedSearchTimeout);
            }
            
            // Reset suggestions if query is too short
            if (!query || query.length < 2) {
                this.breedSuggestions = [];
                return;
            }
            
            // Debounce the search
            this.breedSearchTimeout = setTimeout(async () => {
                try {
                    const response = await fetch(`/api/DogBreedsProxy/search?query=${encodeURIComponent(query)}&limit=8`);
                    if (response.ok) {
                        this.breedSuggestions = await response.json();
                    } else {
                        console.warn('Breed search failed, using fallback');
                        this.breedSuggestions = this.getFallbackBreeds(query);
                    }
                } catch (error) {
                    console.error('Error searching breeds:', error);
                    this.breedSuggestions = this.getFallbackBreeds(query);
                }
            }, 300);
        },
        
        selectBreed(breedName, context) {
            if (context === 'new') {
                this.newPet.breed = breedName;
                this.showBreedSuggestions.new = false;
            } else if (context === 'edit') {
                this.editingPet.breed = breedName;
                this.showBreedSuggestions.edit = false;
            }
            this.breedSuggestions = [];
        },
        
        getFallbackBreeds(query) {
            const fallbackBreeds = [
                { id: 1, name: 'Labrador Retriever', sizeCategory: 'Large' },
                { id: 2, name: 'Golden Retriever', sizeCategory: 'Large' },
                { id: 3, name: 'German Shepherd', sizeCategory: 'Large' },
                { id: 4, name: 'Bulldog', sizeCategory: 'Medium' },
                { id: 5, name: 'Poodle', sizeCategory: 'Medium' },
                { id: 6, name: 'Beagle', sizeCategory: 'Medium' },
                { id: 7, name: 'Rottweiler', sizeCategory: 'Large' },
                { id: 8, name: 'Yorkshire Terrier', sizeCategory: 'Small' },
                { id: 9, name: 'Dachshund', sizeCategory: 'Small' },
                { id: 10, name: 'Siberian Husky', sizeCategory: 'Large' },
                { id: 11, name: 'Mixed Breed', sizeCategory: 'Medium' }
            ];
            
            return fallbackBreeds.filter(breed => 
                breed.name.toLowerCase().includes(query.toLowerCase())
            ).slice(0, 8);
        },

        // Get health status CSS class
        getHealthStatusClass(status) {
            switch (status?.toLowerCase()) {
                case 'excellent':
                    return 'bg-green-100 text-green-800';
                case 'good':
                    return 'bg-blue-100 text-blue-800';
                case 'fair':
                    return 'bg-yellow-100 text-yellow-800';
                case 'poor':
                    return 'bg-red-100 text-red-800';
                default:
                    return 'bg-gray-100 text-gray-800';
            }
        },

        // Show success message
        showSuccessMessage(message) {
            // Implementation for success toast/notification
        },

        // Show error message
        showErrorMessage(message) {
            // Implementation for error toast/notification
        }
    }
}