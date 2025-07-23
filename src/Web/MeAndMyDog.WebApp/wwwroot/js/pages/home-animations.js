// Home Page Animations JavaScript

// About Us Section Animations
function aboutUsAnimations() {
    return {
        headerVisible: false,
        imageVisible: false,
        storyVisible: false,
        textBlock1Visible: false,
        textBlock2Visible: false,
        missionVisible: false,
        textBlock3Visible: false,
        ctaVisible: false,
        benefitsVisible: false,
        
        benefits: [
            {
                id: 1,
                icon: 'fas fa-money-bill-wave-alt text-green-500',
                title: 'Zero Booking Fees',
                description: 'More affordable for pet owners, better earnings for providers'
            },
            {
                id: 2,
                icon: 'fas fa-chart-line text-pet-blue',
                title: 'Business Tools',
                description: 'Helping providers grow their businesses with our platform'
            },
            {
                id: 3,
                icon: 'fas fa-shield-alt text-pet-green',
                title: 'Trusted Platform',
                description: 'Verified providers you can trust with your beloved pets'
            },
            {
                id: 4,
                icon: 'fas fa-heart text-pet-pink',
                title: 'Community Focus',
                description: 'Building a community of pet lovers across the UK'
            }
        ],
        
        initScrollObserver() {
            const options = {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            };
            
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        this.triggerAnimations(entry.target);
                    }
                });
            }, options);
            
            // Observe all animation elements
            observer.observe(this.$refs.sectionHeader);
            observer.observe(this.$refs.teamImage);
            observer.observe(this.$refs.storyCard);
        },
        
        triggerAnimations(target) {
            if (target === this.$refs.sectionHeader) {
                this.headerVisible = true;
            }
            
            if (target === this.$refs.teamImage) {
                this.imageVisible = true;
            }
            
            if (target === this.$refs.storyCard) {
                this.storyVisible = true;
                
                // Stagger text animations
                setTimeout(() => this.textBlock1Visible = true, 200);
                setTimeout(() => this.textBlock2Visible = true, 400);
                setTimeout(() => this.missionVisible = true, 600);
                setTimeout(() => this.textBlock3Visible = true, 800);
                setTimeout(() => this.ctaVisible = true, 1000);
                setTimeout(() => this.benefitsVisible = true, 1200);
            }
        }
    }
}

// Testimonials Section Animations
function testimonialsAnimations() {
    return {
        headerVisible: false,
        trustIndicatorsVisible: false,
        cardsVisible: false,
        ctaVisible: false,
        
        testimonials: [
            {
                id: 1,
                name: 'Sarah M.',
                location: 'London, SW1',
                quote: 'Amazing service! Emma takes such good care of Buddy. The GPS tracking and photo updates give me peace of mind while I\'m at work.',
                avatarBg: 'bg-pet-orange',
                service: 'Dog Walking',
                serviceIcon: 'fas fa-walking'
            },
            {
                id: 2,
                name: 'James L.',
                location: 'Manchester, M1',
                quote: 'The booking system is so easy to use. I love that I can chat with the walker and get updates throughout the day. Highly recommend!',
                avatarBg: 'bg-pet-blue',
                service: 'Pet Sitting',
                serviceIcon: 'fas fa-home'
            },
            {
                id: 3,
                name: 'Lisa K.',
                location: 'Birmingham, B1',
                quote: 'Professional, reliable, and genuinely care about pets. The platform makes finding trusted providers so much easier than other services.',
                avatarBg: 'bg-pet-green',
                service: 'Grooming',
                serviceIcon: 'fas fa-cut'
            }
        ],
        
        initTestimonialsObserver() {
            const options = {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            };
            
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        this.triggerTestimonialsAnimations(entry.target);
                    }
                });
            }, options);
            
            // Observe testimonials header
            observer.observe(this.$refs.testimonialsHeader);
        },
        
        triggerTestimonialsAnimations(target) {
            if (target === this.$refs.testimonialsHeader) {
                this.headerVisible = true;
                
                // Stagger animations
                setTimeout(() => this.trustIndicatorsVisible = true, 300);
                setTimeout(() => this.cardsVisible = true, 600);
                setTimeout(() => this.ctaVisible = true, 1400);
            }
        }
    }
}

// Google Maps Init Function
function initMap() {
    // Empty init function - we only need the Geocoding service
}