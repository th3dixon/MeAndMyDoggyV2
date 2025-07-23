// Pet Health Dashboard JavaScript
function petHealthDashboard() {
    return {
        // State
        pets: [],
        selectedPet: null,
        medicalRecords: [],
        healthReminders: [],
        showHealthSummary: true,
        showAddRecordModal: false,
        filterRecords: 'all',
        
        // New record form
        newRecord: {
            dogId: '',
            recordType: '',
            title: '',
            description: '',
            eventDate: '',
            veterinarianName: '',
            clinicName: '',
            cost: null
        },
        
        // Health statistics
        healthStats: {
            totalPets: 0,
            vaccinationsUpToDate: 0,
            recentCheckups: 0,
            activeMedications: 0
        },

        // Initialize
        async init() {
            await this.loadPetsHealthSummary();
            await this.loadHealthReminders();
            
            if (this.pets.length > 0) {
                this.selectPet(this.pets[0]);
            }
            
            this.calculateHealthStats();
        },

        // API calls
        async loadPetsHealthSummary() {
            try {
                const response = await fetch('/PetHealth/GetPetsHealthSummary');
                if (response.ok) {
                    this.pets = await response.json();
                }
            } catch (error) {
                console.error('Error loading pets health summary:', error);
            }
        },

        async loadMedicalRecords(petId) {
            try {
                const response = await fetch(`/PetHealth/GetMedicalRecords?petId=${petId}`);
                if (response.ok) {
                    this.medicalRecords = await response.json();
                }
            } catch (error) {
                console.error('Error loading medical records:', error);
                this.medicalRecords = [];
            }
        },

        async loadHealthReminders() {
            try {
                const response = await fetch('/PetHealth/GetHealthReminders');
                if (response.ok) {
                    this.healthReminders = await response.json();
                }
            } catch (error) {
                console.error('Error loading health reminders:', error);
                this.healthReminders = [];
            }
        },

        async addMedicalRecord() {
            try {
                const response = await fetch('/PetHealth/AddMedicalRecord', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(this.newRecord)
                });
                
                if (response.ok) {
                    this.showAddRecordModal = false;
                    this.resetNewRecord();
                    if (this.selectedPet) {
                        await this.loadMedicalRecords(this.selectedPet.id);
                    }
                    this.showNotification('Medical record added successfully!', 'success');
                } else {
                    this.showNotification('Failed to add medical record', 'error');
                }
            } catch (error) {
                console.error('Error adding medical record:', error);
                this.showNotification('Error adding medical record', 'error');
            }
        },

        // UI interactions
        async selectPet(pet) {
            this.selectedPet = pet;
            await this.loadMedicalRecords(pet.id);
        },

        resetNewRecord() {
            this.newRecord = {
                dogId: this.selectedPet?.id || '',
                recordType: '',
                title: '',
                description: '',
                eventDate: '',
                veterinarianName: '',
                clinicName: '',
                cost: null
            };
        },

        // Computed properties
        get filteredMedicalRecords() {
            if (this.filterRecords === 'all') {
                return this.medicalRecords;
            }
            return this.medicalRecords.filter(record => 
                record.recordType.toLowerCase() === this.filterRecords
            );
        },

        // Helper methods
        calculateHealthStats() {
            this.healthStats.totalPets = this.pets.length;
            this.healthStats.vaccinationsUpToDate = this.pets.filter(pet => 
                pet.vaccinationStatus === 'Up to date'
            ).length;
            this.healthStats.recentCheckups = this.medicalRecords.filter(record => 
                record.recordType === 'Checkup' && 
                new Date(record.eventDate) > new Date(Date.now() - 6 * 30 * 24 * 60 * 60 * 1000)
            ).length;
        },

        getHealthStatusColor(status) {
            const colors = {
                'excellent': 'bg-green-500',
                'good': 'bg-blue-500',
                'fair': 'bg-yellow-500',
                'poor': 'bg-red-500',
                'unknown': 'bg-gray-400'
            };
            return colors[status?.toLowerCase()] || colors.unknown;
        },

        getReminderClass(priority) {
            const classes = {
                'high': 'reminder-urgent',
                'medium': 'reminder-soon',
                'low': 'reminder-normal'
            };
            return classes[priority] || classes.low;
        },

        getRecordIcon(recordType) {
            const icons = {
                'vaccination': 'fas fa-syringe',
                'checkup': 'fas fa-stethoscope',
                'treatment': 'fas fa-medkit',
                'medication': 'fas fa-pills',
                'surgery': 'fas fa-cut',
                'emergency': 'fas fa-exclamation-triangle'
            };
            return icons[recordType?.toLowerCase()] || 'fas fa-file-medical';
        },

        getRecordIconClass(recordType) {
            const classes = {
                'vaccination': 'border-green-500 bg-green-50',
                'checkup': 'border-blue-500 bg-blue-50',
                'treatment': 'border-red-500 bg-red-50',
                'medication': 'border-purple-500 bg-purple-50',
                'surgery': 'border-red-600 bg-red-50',
                'emergency': 'border-yellow-500 bg-yellow-50'
            };
            return classes[recordType?.toLowerCase()] || 'border-gray-500 bg-gray-50';
        },

        formatDate(dateString) {
            return new Date(dateString).toLocaleDateString('en-GB', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });
        },

        editMedicalRecord(record) {
            // Placeholder for edit functionality
        },

        showNotification(message, type = 'info') {
            // Simple notification system
            const notification = document.createElement('div');
            notification.className = `fixed top-4 right-4 z-50 px-6 py-3 rounded-lg shadow-lg transition-all duration-500 ${
                type === 'success' ? 'bg-green-500' : 
                type === 'error' ? 'bg-red-500' : 'bg-blue-500'
            } text-white`;
            notification.textContent = message;
            
            document.body.appendChild(notification);
            
            setTimeout(() => {
                notification.style.transform = 'translateX(100%)';
                setTimeout(() => document.body.removeChild(notification), 500);
            }, 3000);
        }
    };
}