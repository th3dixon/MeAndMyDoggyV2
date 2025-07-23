// Profile Manager JavaScript
document.addEventListener('alpine:init', () => {
    Alpine.data('profileManager', () => ({
        // User data
        currentRole: 'serviceProvider', // 'petOwner' or 'serviceProvider' - Start as provider to showcase premium features
        activeTab: 'dashboard',
        profilePhoto: null,
        isPremiumProvider: true, // For premium features demo
        
        // KYC Verification System (Service Providers Only)
        isKYCVerified: false, // Set to false to demonstrate verification flow
        kycStatus: 'not_started', // 'not_started', 'in_progress', 'pending_review', 'verified', 'rejected'
        kycVerificationStep: 1, // Current step in verification process
        showKYCModal: false,
        showKYCHelp: false,
        
        // Weather data for transferred functionality
        weather: {
            temperature: 18,
            condition: 'Partly Cloudy',
            feelsLike: 20,
            location: 'London, UK',
            icon: 'fas fa-cloud-sun',
            petTip: 'Perfect weather for dog walks! Remember to bring water for longer activities.'
        },
        loading: {
            weather: false
        },
        
        // Pet management state
        selectedPet: null,
        selectedPetForEdit: null,
        showPetDetailModal: false,
        showPetManagementModal: false,
        petDetailView: 'health', // 'health', 'care', 'profile'
        userPhotoLibrary: [
            {
                id: 1,
                url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTUwIiBoZWlnaHQ9IjE1MCIgdmlld0JveD0iMCAwIDE1MCAxNTAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIxNTAiIGhlaWdodD0iMTUwIiByeD0iNzUiIGZpbGw9IiNGOTczMTYiLz4KPHBhdGggZD0iTTc1IDc1QzYwLjA4ODMgNzUgNDggNjIuOTExNyA0OCA0OEM0OCAzMy4wODgzIDYwLjA4ODMgMjEgNzUgMjFDODkuOTExNyAyMSAxMDIgMzMuMDg4MyAxMDIgNDhDMTAyIDYyLjkxMTcgODkuOTExNyA3NSA3NSA3NVpNNzUgODRDNTguNDMxNSA4NCAzNSA5MC45MzY4IDM1IDEwNS43NVYxMTRIMTE1VjEwNS43NUMxMTUgOTAuOTM2OCA5MS41Njg1IDg0IDc1IDg0WiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+',
                isPrimary: true,
                uploadedAt: '2024-01-15',
                caption: 'Profile photo'
            },
            {
                id: 2,
                url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTUwIiBoZWlnaHQ9IjE1MCIgdmlld0JveD0iMCAwIDE1MCAxNTAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIxNTAiIGhlaWdodD0iMTUwIiByeD0iNzUiIGZpbGw9IiMzQjgyRjYiLz4KPHBhdGggZD0iTTc1IDc1QzYwLjA4ODMgNzUgNDggNjIuOTExNyA0OCA0OEM0OCAzMy4wODgzIDYwLjA4ODMgMjEgNzUgMjFDODkuOTExNyAyMSAxMDIgMzMuMDg4NCAxMDIgNDhDMTAyIDYyLjkxMTcgODkuOTExNyA3NSA3NSA3NVpNNzUgODRDNTguNDMxNSA4NCAzNSA5MC45MzY4IDM1IDEwNS43NVYxMTRIMTE1VjEwNS43NUMxMTUgOTAuOTM2OCA5MS41Njg1IDg0IDc1IDg0WiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+',
                isPrimary: false,
                uploadedAt: '2024-01-10',
                caption: 'With my pets'
            },
            {
                id: 3,
                url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTUwIiBoZWlnaHQ9IjE1MCIgdmlld0JveD0iMCAwIDE1MCAxNTAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIxNTAiIGhlaWdodD0iMTUwIiByeD0iNzUiIGZpbGw9IiM4QjVDRjYiLz4KPHBhdGggZD0iTTc1IDc1QzYwLjA4ODMgNzUgNDggNjIuOTExNyA0OCA0OEM0OCAzMy4wODgzIDYwLjA4ODMgMjEgNzUgMjFDODkuOTExNyAyMSAxMDIgMzMuMDg4MyAxMDIgNDhDMTAyIDYyLjkxMTcgODkuOTExNyA3NSA3NSA3NVpNNzUgODRDNTguNDMxNSA4NCAzNSA5MC45MzY4IDM1IDEwNS43NVYxMTRIMTE1VjEwNS43NUMxMTUgOTAuOTM2OCA5MS41Njg1IDg0IDc1IDg0WiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+',
                isPrimary: false,
                uploadedAt: '2024-01-05',
                caption: 'Working with clients'
            }
        ],
        profileData: {
            name: 'Sarah Johnson',
            email: 'sarah.johnson@example.com',
            phone: '+44 7700 900123',
            location: 'London, SW1A 1AA',
            bio: 'Passionate dog lover with 2 beautiful Golden Retrievers. Always looking for the best care for my furry friends!',
            memberSince: '2023-01-15',
            friendCode: 'WOOF-1234',
            verificationStatus: 'verified', // 'verified', 'pending', 'unverified'
            diditVerified: true,
            responseTime: '< 1 hour',
            completedJobs: 47,
            rating: 4.9,
            reviews: 23
        },
        
        // Pet data with photo libraries
        pets: [
            {
                id: 1,
                name: 'Max',
                breed: 'Golden Retriever',
                age: 3,
                weight: 32,
                photo: null,
                vaccinated: true,
                neutered: true,
                microchipped: true,
                conditions: ['Hip dysplasia'],
                medications: ['Glucosamine supplement'],
                photoLibrary: [
                    {
                        id: 1,
                        url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgdmlld0JveD0iMCAwIDQwMCAzMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSI0MDAiIGhlaWdodD0iMzAwIiBmaWxsPSIjRkVGM0M3Ii8+CjxwYXRoIGQ9Ik0yMDAgMTUwQzE3OC41NjUgMTUwIDE2MSAxMzIuNDM1IDE2MSAxMTFDMTYxIDg5LjU2NTQgMTc4LjU2NSA3MiAyMDAgNzJDMjIxLjQzNSA3MiAyMzkgODkuNTY1NCAyMzkgMTExQzIzOSAxMzIuNDM1IDIyMS40MzUgMTUwIDIwMCAxNTBaIiBmaWxsPSIjRjk3MzE2Ii8+CjxwYXRoIGQ9Ik0xNTAgMTIwQzEzOC45NTQgMTIwIDEzMCAxMTEuMDQ2IDEzMCAxMDBDMTMwIDg4Ljk1NDMgMTM4Ljk1NCA4MCAxNTAgODBDMTYxLjA0NiA4MCAxNzAgODguOTU0MyAxNzAgMTAwQzE3MCAxMTEuMDQ2IDE2MS4wNDYgMTIwIDE1MCAxMjBaIiBmaWxsPSIjRjk3MzE2Ii8+CjxwYXRoIGQ9Ik0yNTAgMTIwQzIzOC45NTQgMTIwIDIzMCAxMTEuMDQ2IDIzMCAxMDBDMjMwIDg4Ljk1NDMgMjM4Ljk1NCA4MCAyNTAgODBDMjYxLjA0NiA4MCAyNzAgODguOTU0MyAyNzAgMTAwQzI3MCAxMTEuMDQ2IDI2MS4wNDYgMTIwIDI1MCAxMjBaIiBmaWxsPSIjRjk3MzE2Ii8+CjxwYXRoIGQ9Ik0xODAgMjAwQzE4MCAxODkgMTg5IDE4MCAyMDAgMTgwQzIxMSAxODAgMjIwIDE4OSAyMjAgMjAwQzIyMCAyMTEgMjExIDIyMCAyMDAgMjIwQzE4OSAyMjAgMTgwIDIxMSAxODAgMjAwWiIgZmlsbD0iI0Y5NzMxNiIvPgo8L3N2Zz4=',
                        isPrimary: true,
                        uploadedAt: '2024-01-15',
                        caption: 'Max playing in the park'
                    },
                    {
                        id: 2,
                        url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgdmlld0JveD0iMCAwIDQwMCAzMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSI0MDAiIGhlaWdodD0iMzAwIiBmaWxsPSIjRDFGQUU1Ii8+CjxwYXRoIGQ9Ik0yMDAgMTUwQzE3OC41NjUgMTUwIDE2MSAxMzIuNDM1IDE2MSAxMTFDMTYxIDg5LjU2NTQgMTc4LjU2NSA3MiAyMDAgNzJDMjIxLjQzNSA3MiAyMzkgODkuNTY1NCAyMzkgMTExQzIzOSAxMzIuNDM1IDIyMS40MzUgMTUwIDIwMCAxNTBaIiBmaWxsPSIjMTBCOTgxIi8+CjxwYXRoIGQ9Ik0xNTAgMTIwQzEzOC45NTQgMTIwIDEzMCAxMTEuMDQ2IDEzMCAxMDBDMTMwIDg4Ljk1NDMgMTM4Ljk1NCA4MCAxNTAgODBDMTYxLjA0NiA4MCAxNzAgODguOTU0MyAxNzAgMTAwQzE3MCAxMTEuMDQ2IDE2MS4wNDYgMTIwIDE1MCAxMjBaIiBmaWxsPSIjMTBCOTgxIi8+CjxwYXRoIGQ9Ik0yNTAgMTIwQzIzOC45NTQgMTIwIDIzMCAxMTEuMDQ2IDIzMCAxMDBDMjMwIDg4Ljk1NDMgMjM4Ljk1NCA4MCAyNTAgODBDMjYxLjA0NiA4MCAyNzAgODguOTU0MyAyNzAgMTAwQzI3MCAxMTEuMDQ2IDI2MS4wNDYgMTIwIDI1MCAxMjBaIiBmaWxsPSIjMTBCOTgxIi8+CjxwYXRoIGQ9Ik0xODAgMjAwQzE4MCAxODkgMTg5IDE4MCAyMDAgMTgwQzIxMSAxODAgMjIwIDE4OSAyMjAgMjAwQzIyMCAyMTEgMjExIDIyMCAyMDAgMjIwQzE4OSAyMjAgMTgwIDIxMSAxODAgMjAwWiIgZmlsbD0iIzEwQjk4MSIvPgo8L3N2Zz4=',
                        isPrimary: false,
                        uploadedAt: '2024-01-12',
                        caption: 'Max after grooming'
                    },
                    {
                        id: 3,
                        url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgdmlld0JveD0iMCAwIDQwMCAzMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSI0MDAiIGhlaWdodD0iMzAwIiBmaWxsPSIjRkVFMkUyIi8+CjxwYXRoIGQ9Ik0yMDAgMTUwQzE3OC41NjUgMTUwIDE2MSAxMzIuNDM1IDE2MSAxMTFDMTYxIDg5LjU2NTQgMTc4LjU2NSA3MiAyMDAgNzJDMjIxLjQzNSA3MiAyMzkgODkuNTY1NCAyMzkgMTExQzIzOSAxMzIuNDM1IDIyMS40MzUgMTUwIDIwMCAxNTBaIiBmaWxsPSIjRUY0NDQ0Ii8+CjxwYXRoIGQ9Ik0xNTAgMTIwQzEzOC45NTQgMTIwIDEzMCAxMTEuMDQ2IDEzMCAxMDBDMTMwIDg4Ljk1NDMgMTM4Ljk1NCA4MCAxNTAgODBDMTYxLjA0NiA4MCAxNzAgODguOTU0MyAxNzAgMTAwQzE3MCAxMTEuMDQ2IDE2MS4wNDYgMTIwIDE1MCAxMjBaIiBmaWxsPSIjRUY0NDQ0Ii8+CjxwYXRoIGQ9Ik0yNTAgMTIwQzIzOC45NTQgMTIwIDIzMCAxMTEuMDQ2IDIzMCAxMDBDMjMwIDg4Ljk1NDMgMjM4Ljk1NCA4MCAyNTAgODBDMjYxLjA0NiA4MCAyNzAgODguOTU0MyAyNzAgMTAwQzI3MCAxMTEuMDQ2IDI2MS4wNDYgMTIwIDI1MCAxMjBaIiBmaWxsPSIjRUY0NDQ0Ii8+CjxwYXRoIGQ9Ik0xODAgMjAwQzE4MCAxODkgMTg5IDE4MCAyMDAgMTgwQzIxMSAxODAgMjIwIDE4OSAyMjAgMjAwQzIyMCAyMTEgMjExIDIyMCAyMDAgMjIwQzE4OSAyMjAgMTgwIDIxMSAxODAgMjAwWiIgZmlsbD0iI0VGNDQNDQiLz4KPC9zdmc+',
                        isPrimary: false,
                        uploadedAt: '2024-01-08',
                        caption: 'Max sleeping'
                    },
                    {
                        id: 4,
                        url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgdmlld0JveD0iMCAwIDQwMCAzMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSI0MDAiIGhlaWdodD0iMzAwIiBmaWxsPSIjRkZFNEUxIi8+CjxwYXRoIGQ9Ik0yMDAgMTUwQzE3OC41NjUgMTUwIDE2MSAxMzIuNDM1IDE2MSAxMTFDMTYxIDg5LjU2NTQgMTc4LjU2NSA3MiAyMDAgNzJDMjIxLjQzNSA3MiAyMzkgODkuNTY1NCAyMzkgMTExQzIzOSAxMzIuNDM1IDIyMS40MzUgMTUwIDIwMCAxNTBaIiBmaWxsPSIjRjk3MzE2Ii8+CjxwYXRoIGQ9Ik0xNTAgMTIwQzEzOC45NTQgMTIwIDEzMCAxMTEuMDQ2IDEzMCAxMDBDMTMwIDg4Ljk1NDMgMTM4Ljk1NCA4MCAxNTAgODBDMTYxLjA0NiA4MCAxNzAgODguOTU0MyAxNzAgMTAwQzE3MCAxMTEuMDQ2IDE2MS4wNDYgMTIwIDE1MCAxMjBaIiBmaWxsPSIjRjk3MzE2Ii8+CjxwYXRoIGQ9Ik0yNTAgMTIwQzIzOC45NTQgMTIwIDIzMCAxMTEuMDQ2IDIzMCAxMDBDMjMwIDg4Ljk1NDMgMjM4Ljk1NCA4MCAyNTAgODBDMjYxLjA0NiA4MCAyNzAgODguOTU0MyAyNzAgMTAwQzI3MCAxMTEuMDQ2IDI2MS4wNDYgMTIwIDI1MCAxMjBaIiBmaWxsPSIjRjk3MzE2Ii8+CjxwYXRoIGQ9Ik0xODAgMjAwQzE4MCAxODkgMTg5IDE4MCAyMDAgMTgwQzIxMSAxODAgMjIwIDE4OSAyMjAgMjAwQzIyMCAyMTEgMjExIDIyMCAyMDAgMjIwQzE4OSAyMjAgMTgwIDIxMSAxODAgMjAwWiIgZmlsbD0iI0Y5NzMxNiIvPgo8L3N2Zz4=',
                        isPrimary: false,
                        uploadedAt: '2024-01-03',
                        caption: 'Max with his favorite toy'
                    }
                ]
            },
            {
                id: 2,
                name: 'Luna',
                breed: 'Golden Retriever',
                age: 1,
                weight: 28,
                photo: null,
                vaccinated: true,
                neutered: false,
                microchipped: true,
                conditions: [],
                medications: [],
                photoLibrary: [
                    {
                        id: 5,
                        url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgdmlld0JveD0iMCAwIDQwMCAzMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSI0MDAiIGhlaWdodD0iMzAwIiBmaWxsPSIjRkJFQ0Y4Ii8+CjxwYXRoIGQ9Ik0yMDAgMTUwQzE3OC41NjUgMTUwIDE2MSAxMzIuNDM1IDE2MSAxMTFDMTYxIDg5LjU2NTQgMTc4LjU2NSA3MiAyMDAgNzJDMjIxLjQzNSA3MiAyMzkgODkuNTY1NCAyMzkgMTExQzIzOSAxMzIuNDM1IDIyMS40MzUgMTUwIDIwMCAxNTBaIiBmaWxsPSIjOEI1Q0Y2Ii8+CjxwYXRoIGQ9Ik0xNTAgMTIwQzEzOC45NTQgMTIwIDEzMCAxMTEuMDQ2IDEzMCAxMDBDMTMwIDg4Ljk1NDMgMTM4Ljk1NCA4MCAxNTAgODBDMTYxLjA0NiA4MCAxNzAgODguOTU0MyAxNzAgMTAwQzE3MCAxMTEuMDQ2IDE2MS4wNDYgMTIwIDE1MCAxMjBaIiBmaWxsPSIjOEI1Q0Y2Ii8+CjxwYXRoIGQ9Ik0yNTAgMTIwQzIzOC45NTQgMTIwIDIzMCAxMTEuMDQ2IDIzMCAxMDBDMjMwIDg4Ljk1NDMgMjM4Ljk1NCA4MCAyNTAgODBDMjYxLjA0NiA4MCAyNzAgODguOTU0MyAyNzAgMTAwQzI3MCAxMTEuMDQ2IDI2MS4wNDYgMTIwIDI1MCAxMjBaIiBmaWxsPSIjOEI1Q0Y2Ii8+CjxwYXRoIGQ9Ik0xODAgMjAwQzE4MCAxODkgMTg5IDE4MCAyMDAgMTgwQzIxMSAxODAgMjIwIDE4OSAyMjAgMjAwQzIyMCAyMTEgMjExIDIyMCAyMDAgMjIwQzE4OSAyMjAgMTgwIDIxMSAxODAgMjAwWiIgZmlsbD0iIzhCNUNGNiIvPgo8L3N2Zz4=',
                        isPrimary: true,
                        uploadedAt: '2024-01-14',
                        caption: 'Luna as a puppy'
                    },
                    {
                        id: 6,
                        url: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgdmlld0JveD0iMCAwIDQwMCAzMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSI0MDAiIGhlaWdodD0iMzAwIiBmaWxsPSIjRUZGNkZGIi8+CjxwYXRoIGQ9Ik0yMDAgMTUwQzE3OC41NjUgMTUwIDE2MSAxMzIuNDM1IDE2MSAxMTFDMTYxIDg5LjU2NTQgMTc4LjU2NSA3MiAyMDAgNzJDMjIxLjQzNSA3MiAyMzkgODkuNTY1NCAyMzkgMTExQzIzOSAxMzIuNDM1IDIyMS40MzUgMTUwIDIwMCAxNTBaIiBmaWxsPSIjOEI1Q0Y2Ii8+CjxwYXRoIGQ9Ik0xNTAgMTIwQzEzOC45NTQgMTIwIDEzMCAxMTEuMDQ2IDEzMCAxMDBDMTMwIDg4Ljk1NDMgMTM4Ljk1NCA4MCAxNTAgODBDMTYxLjA0NiA4MCAxNzAgODguOTU0MyAxNzAgMTAwQzE3MCAxMTEuMDQ2IDE2MS4wNDYgMTIwIDE1MCAxMjBaIiBmaWxsPSIjOEI1Q0Y2Ii8+CjxwYXRoIGQ9Ik0yNTAgMTIwQzIzOC45NTQgMTIwIDIzMCAxMTEuMDQ2IDIzMCAxMDBDMjMwIDg4Ljk1NDMgMjM4Ljk1NCA4MCAyNTAgODBDMjYxLjA0NiA4MCAyNzAgODguOTU0MyAyNzAgMTAwQzI3MCAxMTEuMDQ2IDI2MS4wNDYgMTIwIDI1MCAxMjBaIiBmaWxsPSIjOEI1Q0Y2Ii8+CjxwYXRoIGQ9Ik0xODAgMjAwQzE4MCAxODkgMTg5IDE4MCAyMDAgMTgwQzIxMSAxODAgMjIwIDE4OSAyMjAgMjAwQzIyMCAyMTEgMjExIDIyMCAyMDAgMjIwQzE4OSAyMjAgMTgwIDIxMSAxODAgMjAwWiIgZmlsbD0iIzhCNUNGNiIvPgo8L3N2Zz4=',
                        isPrimary: false,
                        uploadedAt: '2024-01-10',
                        caption: 'Luna learning to sit'
                    }
                ]
            }
        ],
        
        // Service provider data
        services: [
            {
                id: 1,
                name: 'Dog Walking',
                description: '30-minute walks in local parks',
                price: 15,
                duration: 30,
                enabled: true
            },
            {
                id: 2,
                name: 'Pet Sitting',
                description: 'In-home pet sitting service',
                price: 25,
                duration: 60,
                enabled: true
            }
        ],
        
        // Reviews
        reviews: [
            {
                id: 1,
                author: 'John Smith',
                rating: 5,
                date: '2024-01-10',
                comment: 'Excellent service! My dog Max loves Sarah.',
                response: 'Thank you so much! Max is such a sweetheart.'
            },
            {
                id: 2,
                author: 'Emma Wilson',
                rating: 4,
                date: '2024-01-05',
                comment: 'Very reliable and caring. Would recommend!',
                response: null
            }
        ],
        
        // Favorite providers
        favoriteProviders: [
            {
                id: 1,
                name: 'Happy Paws Grooming',
                service: 'Grooming',
                rating: 4.8,
                distance: '0.5 miles'
            },
            {
                id: 2,
                name: 'Dr. Smith Veterinary',
                service: 'Veterinary',
                rating: 4.9,
                distance: '1.2 miles'
            }
        ],
        
        // Calendar instance
        calendarInstance: null,
        
        // Modals
        showEditProfileModal: false,
        showAddPetModal: false,
        showAddServiceModal: false,
        showPhotoUploadModal: false,
        showVerificationModal: false,
        showUserPhotoLibraryModal: false,
        showPetPhotoLibraryModal: false,
        selectedPetForPhotos: null,
        viewingPhotoId: null,
        showPhotoViewModal: false,
        
        // Form data
        editingProfile: false,
        editingService: null,
        newPet: {
            name: '',
            breed: '',
            age: '',
            weight: '',
            vaccinated: false,
            neutered: false,
            microchipped: false
        },
        newService: {
            name: '',
            description: '',
            price: '',
            duration: 30
        },
        
        init() {
            // Initialize calendar if on availability tab
            this.$watch('activeTab', (newTab) => {
                if (newTab === 'availability' && this.currentRole === 'serviceProvider') {
                    this.$nextTick(() => {
                        this.initCalendar();
                    });
                }
            });
            
            // Set initial profile photo from library
            const primaryPhoto = this.userPhotoLibrary.find(p => p.isPrimary);
            if (primaryPhoto) {
                this.profilePhoto = primaryPhoto.url;
            }
            
            // Set initial pet photos
            this.pets.forEach(pet => {
                const primaryPetPhoto = pet.photoLibrary.find(p => p.isPrimary);
                if (primaryPetPhoto) {
                    pet.photo = primaryPetPhoto.url;
                }
            });
            
            // Load user data from API
            this.loadProfileData();
        },
        
        async loadProfileData() {
            try {
                // In a real app, this would fetch from API
            } catch (error) {
                console.error('Error loading profile data:', error);
            }
        },
        
        switchRole(role) {
            this.currentRole = role;
            this.activeTab = 'dashboard';
        },
        
        // KYC Verification Methods
        startKYCVerification() {
            this.showKYCModal = true;
            this.kycStatus = 'in_progress';
            this.kycVerificationStep = 1;
        },
        
        nextKYCStep() {
            if (this.kycVerificationStep < 4) {
                this.kycVerificationStep++;
            } else {
                this.completeKYCVerification();
            }
        },
        
        completeKYCVerification() {
            this.kycStatus = 'pending_review';
            this.showKYCModal = false;
            // Simulate processing time
            setTimeout(() => {
                this.kycStatus = 'verified';
                this.isKYCVerified = true;
                this.showNotification('KYC verification approved! You can now accept bookings.', 'success');
            }, 3000);
        },
        
        retryKYCVerification() {
            this.kycStatus = 'in_progress';
            this.kycVerificationStep = 1;
            this.startKYCVerification();
        },
        
        canAccessProviderFeatures() {
            // Service providers need KYC verification to access core features
            return this.currentRole !== 'serviceProvider' || this.isKYCVerified;
        },
        
        getKYCStatusMessage() {
            switch (this.kycStatus) {
                case 'not_started':
                    return 'Identity verification required to accept bookings';
                case 'in_progress':
                    return 'Verification in progress...';
                case 'pending_review':
                    return 'Verification under review (typically 24-48 hours)';
                case 'verified':
                    return 'Identity verified ✓';
                case 'rejected':
                    return 'Verification failed - please retry';
                default:
                    return 'Verification status unknown';
            }
        },
        
        showNotification(message, type = 'info') {
            // Simple notification system
        },
        
        async handlePhotoUpload(event) {
            const file = event.target.files[0];
            if (file) {
                // Preview
                const reader = new FileReader();
                reader.onload = (e) => {
                    // Add to user photo library
                    const newPhoto = {
                        id: Date.now(),
                        url: e.target.result,
                        isPrimary: this.userPhotoLibrary.length === 0,
                        uploadedAt: new Date().toISOString().split('T')[0],
                        caption: 'New profile photo'
                    };
                    
                    this.userPhotoLibrary.unshift(newPhoto);
                    
                    // Set as profile photo if it's the primary one
                    if (newPhoto.isPrimary) {
                        this.profilePhoto = newPhoto.url;
                    }
                };
                reader.readAsDataURL(file);
                
                // In real app, would upload to server
            }
        },
        
        openUserPhotoLibrary() {
            this.showUserPhotoLibraryModal = true;
        },
        
        openPetPhotoLibrary(pet) {
            this.selectedPetForPhotos = pet;
            this.showPetPhotoLibraryModal = true;
        },
        
        setPrimaryUserPhoto(photoId) {
            // Remove primary from all photos
            this.userPhotoLibrary.forEach(photo => photo.isPrimary = false);
            // Set new primary
            const photo = this.userPhotoLibrary.find(p => p.id === photoId);
            if (photo) {
                photo.isPrimary = true;
                this.profilePhoto = photo.url;
            }
        },
        
        setPrimaryPetPhoto(petId, photoId) {
            const pet = this.pets.find(p => p.id === petId);
            if (pet) {
                // Remove primary from all pet photos
                pet.photoLibrary.forEach(photo => photo.isPrimary = false);
                // Set new primary
                const photo = pet.photoLibrary.find(p => p.id === photoId);
                if (photo) {
                    photo.isPrimary = true;
                    pet.photo = photo.url;
                }
            }
        },
        
        deleteUserPhoto(photoId) {
            if (confirm('Are you sure you want to delete this photo?')) {
                const photoIndex = this.userPhotoLibrary.findIndex(p => p.id === photoId);
                if (photoIndex > -1) {
                    const wasMain = this.userPhotoLibrary[photoIndex].isPrimary;
                    this.userPhotoLibrary.splice(photoIndex, 1);
                    
                    // If we deleted the main photo, set a new one
                    if (wasMain && this.userPhotoLibrary.length > 0) {
                        this.setPrimaryUserPhoto(this.userPhotoLibrary[0].id);
                    } else if (this.userPhotoLibrary.length === 0) {
                        this.profilePhoto = null;
                    }
                }
            }
        },
        
        deletePetPhoto(petId, photoId) {
            if (confirm('Are you sure you want to delete this photo?')) {
                const pet = this.pets.find(p => p.id === petId);
                if (pet) {
                    const photoIndex = pet.photoLibrary.findIndex(p => p.id === photoId);
                    if (photoIndex > -1) {
                        const wasMain = pet.photoLibrary[photoIndex].isPrimary;
                        pet.photoLibrary.splice(photoIndex, 1);
                        
                        // If we deleted the main photo, set a new one
                        if (wasMain && pet.photoLibrary.length > 0) {
                            this.setPrimaryPetPhoto(petId, pet.photoLibrary[0].id);
                        } else if (pet.photoLibrary.length === 0) {
                            pet.photo = null;
                        }
                    }
                }
            }
        },
        
        addPetPhoto(petId, file) {
            const pet = this.pets.find(p => p.id === petId);
            if (pet && file) {
                const reader = new FileReader();
                reader.onload = (e) => {
                    const newPhoto = {
                        id: Date.now(),
                        url: e.target.result,
                        isPrimary: pet.photoLibrary.length === 0,
                        uploadedAt: new Date().toISOString().split('T')[0],
                        caption: `New photo of ${pet.name}`
                    };
                    
                    pet.photoLibrary.unshift(newPhoto);
                    
                    // Set as pet photo if it's the primary one
                    if (newPhoto.isPrimary) {
                        pet.photo = newPhoto.url;
                    }
                };
                reader.readAsDataURL(file);
            }
        },
        
        viewPhoto(photoId, isUserPhoto = true) {
            this.viewingPhotoId = photoId;
            this.showPhotoViewModal = true;
        },
        
        getCurrentUserPhoto() {
            return this.userPhotoLibrary.find(p => p.isPrimary) || this.userPhotoLibrary[0];
        },
        
        getPetPrimaryPhoto(pet) {
            return pet.photoLibrary.find(p => p.isPrimary) || pet.photoLibrary[0];
        },
        
        copyFriendCode() {
            navigator.clipboard.writeText(this.profileData.friendCode);
            // Show toast notification
            alert('Friend code copied to clipboard!');
        },
        
        async saveProfile() {
            try {
                // API call to save profile
                this.editingProfile = false;
            } catch (error) {
                console.error('Error saving profile:', error);
            }
        },
        
        async addPet() {
            try {
                // API call to add pet
                const newPetData = { ...this.newPet, id: Date.now() };
                this.pets.push(newPetData);
                this.showAddPetModal = false;
                this.resetNewPet();
            } catch (error) {
                console.error('Error adding pet:', error);
            }
        },
        
        resetNewPet() {
            this.newPet = {
                name: '',
                breed: '',
                age: '',
                weight: '',
                vaccinated: false,
                neutered: false,
                microchipped: false
            };
        },
        
        async deletePet(petId) {
            if (confirm('Are you sure you want to remove this pet?')) {
                try {
                    this.pets = this.pets.filter(p => p.id !== petId);
                } catch (error) {
                    console.error('Error deleting pet:', error);
                }
            }
        },
        
        async addService() {
            try {
                const newServiceData = { ...this.newService, id: Date.now(), enabled: true };
                this.services.push(newServiceData);
                this.showAddServiceModal = false;
                this.resetNewService();
            } catch (error) {
                console.error('Error adding service:', error);
            }
        },
        
        resetNewService() {
            this.newService = {
                name: '',
                description: '',
                price: '',
                duration: 30
            };
        },
        
        toggleService(serviceId) {
            const service = this.services.find(s => s.id === serviceId);
            if (service) {
                service.enabled = !service.enabled;
            }
        },
        
        async deleteService(serviceId) {
            if (confirm('Are you sure you want to remove this service?')) {
                try {
                    this.services = this.services.filter(s => s.id !== serviceId);
                } catch (error) {
                    console.error('Error deleting service:', error);
                }
            }
        },
        
        removeFavorite(providerId) {
            this.favoriteProviders = this.favoriteProviders.filter(p => p.id !== providerId);
        },
        
        initCalendar() {
            if (this.calendarInstance) {
                this.calendarInstance.destroy();
            }
            
            const calendarEl = document.getElementById('availability-calendar');
            if (!calendarEl) return;
            
            this.calendarInstance = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek'
                },
                events: [
                    {
                        title: 'Available',
                        start: '2024-01-20T09:00:00',
                        end: '2024-01-20T17:00:00',
                        className: 'available'
                    },
                    {
                        title: 'Booked - Dog Walking',
                        start: '2024-01-20T10:00:00',
                        end: '2024-01-20T11:00:00',
                        className: 'booked'
                    }
                ],
                eventClick: (info) => {
                    // Handle event click
                },
                dateClick: (info) => {
                    // Handle date click
                }
            });
            
            this.calendarInstance.render();
        },
        
        startVerification() {
            this.showVerificationModal = true;
            // In real app, would redirect to Didit KYC flow
        },
        
        getResponseTimeClass() {
            if (this.profileData.responseTime.includes('< 1')) return '';
            if (this.profileData.responseTime.includes('1-2')) return 'medium';
            return 'slow';
        }
    }));
});