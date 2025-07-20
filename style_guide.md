# MeAndMyDog Style Guide

## Overview
This style guide serves as the single source of truth for all design decisions, UI components, and user experience patterns in the MeAndMyDog application.

## Brand Identity

### Color Palette
**Primary Colors (Golden Yellow Theme)**
- Primary: #F1C232 (Golden Yellow)
- Primary Dark: #C18A0E
- Primary Light: #FEFBF0
- Primary 50: #FEFDF8
- Primary 100: #FEFBF0
- Primary 200: #FCF4D9
- Primary 300: #F9EAB3
- Primary 400: #F5D97D
- Primary 500: #F1C232
- Primary 600: #E6A91A
- Primary 700: #C18A0E
- Primary 800: #9D6F0F
- Primary 900: #825C10

**Secondary Colors (Slate Theme)**
- Secondary: #64748B
- Secondary 50: #F8FAFC
- Secondary 100: #F1F5F9
- Secondary 200: #E2E8F0
- Secondary 300: #CBD5E1
- Secondary 400: #94A3B8
- Secondary 500: #64748B
- Secondary 600: #475569
- Secondary 700: #334155
- Secondary 800: #1E293B
- Secondary 900: #0F172A

**Status Colors**
- Success Green: #10B981
- Warning Orange: #F59E0B
- Error Red: #EF4444
- Info Blue: #3B82F6

**Neutral Colors**
- Text Primary: #111827
- Text Secondary: #6B7280
- Text Muted: #9CA3AF
- Background: #FFFFFF
- Background Secondary: #F9FAFB
- Border: #E5E7EB

### Typography
**Font Family**
- Primary: Inter, system-ui, sans-serif
- Monospace: 'Fira Code', monospace

**Font Scales**
- Heading 1: 2.25rem (36px) - font-bold
- Heading 2: 1.875rem (30px) - font-semibold
- Heading 3: 1.5rem (24px) - font-semibold
- Heading 4: 1.25rem (20px) - font-medium
- Body Large: 1.125rem (18px) - font-normal
- Body: 1rem (16px) - font-normal
- Body Small: 0.875rem (14px) - font-normal
- Caption: 0.75rem (12px) - font-medium

## Spacing System
Based on 4px grid system:
- xs: 4px
- sm: 8px
- md: 16px
- lg: 24px
- xl: 32px
- 2xl: 48px
- 3xl: 64px

## Component Library

### Buttons
**Primary Button**
- Background: Primary 500 (#F1C232)
- Text: White
- Padding: 12px 24px
- Border Radius: 8px
- Font Weight: Medium
- Hover: Primary 600 (#E6A91A)

**Secondary Button**
- Background: Transparent
- Border: 1px solid Primary 500
- Text: Primary 700 (#C18A0E)
- Padding: 12px 24px
- Border Radius: 8px
- Hover: Primary 50 background

**Outline Button**
- Background: White
- Border: 1px solid Secondary 300
- Text: Secondary 700
- Padding: 12px 24px
- Border Radius: 8px
- Hover: Secondary 50 background

**Danger Button**
- Background: Error Red (#EF4444)
- Text: White
- Padding: 12px 24px
- Border Radius: 8px
- Hover: #DC2626

### Form Elements
**Input Fields**
- Border: 1px solid Border color (#E5E7EB)
- Border Radius: 6px
- Padding: 12px 16px
- Focus: Border color changes to Primary 500 (#F1C232)
- Error State: Border color #EF4444, background #FEF2F2
- Disabled State: Background #F9FAFB, text #9CA3AF

**Labels**
- Font Weight: Medium (500)
- Color: Text Primary (#111827)
- Font Size: 14px
- Margin Bottom: 8px
- Required fields: Add red asterisk (*)

**Form Validation**
- Inline validation on blur
- Error messages below field in red (#EF4444)
- Success state with green border (#10B981)
- Loading state with spinner in field

**Select Dropdowns**
- Same styling as input fields
- Chevron down icon on right
- Max height with scroll for long lists
- Search functionality for 10+ options

**Checkboxes & Radio Buttons**
- Custom styled with Primary colors
- 20px size for touch targets
- Clear focus indicators
- Proper spacing between options

### Cards
**Standard Card**
- Background: White
- Border: 1px solid Border color (#E5E7EB)
- Border Radius: 12px
- Padding: 24px
- Shadow: 0 1px 3px rgba(0, 0, 0, 0.1)
- Hover: Shadow increases to 0 4px 6px rgba(0, 0, 0, 0.1)

**Dog Profile Card**
- Same as standard card
- Image aspect ratio: 4:3
- Title: Heading 4 (20px, medium)
- Subtitle: Body small (14px, secondary color)
- Action buttons in footer

**Service Provider Card**
- Same as standard card
- Rating stars with Primary 500 color
- Price display: Heading 4, Primary 700
- Badge for availability status

### Navigation Components
**Primary Navigation**
- Background: White
- Border bottom: 1px solid Border color
- Logo: 32px height
- Menu items: Body font, Secondary 700
- Active state: Primary 500 underline
- Mobile: Hamburger menu, slide-out drawer

**Breadcrumbs**
- Font: Body small (14px)
- Color: Secondary 500
- Separator: "/" in Secondary 300
- Current page: Primary 700, no link

**Pagination**
- Button style: Outline buttons
- Current page: Primary button
- Disabled: Secondary 300 background
- Show 5 pages max with ellipsis

### Data Display
**Tables**
- Header: Secondary 50 background, Secondary 700 text
- Rows: Alternating white/Secondary 50
- Borders: 1px solid Border color
- Padding: 12px 16px
- Sortable columns: Arrow icons

**Lists**
- Item padding: 16px
- Dividers: 1px solid Border color
- Hover: Secondary 50 background
- Icons: 20px, Secondary 500

**Badges & Tags**
- Small: 6px 12px padding, 12px font
- Medium: 8px 16px padding, 14px font
- Colors: Primary, Success, Warning, Error backgrounds
- Border radius: 9999px (pill shape)

### Modals & Overlays
**Modal Dialog**
- Backdrop: rgba(0, 0, 0, 0.5)
- Container: White, 24px padding, 16px border radius
- Max width: 500px (mobile: 90vw)
- Header: Close button (X) top right
- Footer: Buttons right-aligned

**Toast Notifications**
- Position: Top right, 24px from edges
- Width: 400px (mobile: 90vw)
- Auto-dismiss: 5 seconds
- Colors match status (success, error, warning, info)
- Icon + message + close button

**Tooltips**
- Background: Secondary 800
- Text: White, 12px
- Padding: 8px 12px
- Border radius: 6px
- Arrow: 6px triangle
- Max width: 200px

## Layout Principles

### Grid System
- Container Max Width: 1200px
- Gutter: 24px
- Columns: 12-column grid
- Breakpoints:
  - Mobile: < 768px
  - Tablet: 768px - 1024px
  - Desktop: > 1024px

### Navigation
**Header Height**: 64px
**Sidebar Width**: 256px (desktop), collapsible on mobile

## User Experience Patterns

### Loading States
- Skeleton screens for content loading
- Spinner for actions (buttons, forms)
- Progress bars for file uploads

### Error Handling
- Inline validation for forms
- Toast notifications for system messages
- Error pages with clear next steps

### Accessibility
- Minimum contrast ratio: 4.5:1
- Focus indicators on all interactive elements
- Semantic HTML structure
- Alt text for all images
- Keyboard navigation support

## Animation Guidelines
- Duration: 200-300ms for micro-interactions
- Easing: ease-out for entrances, ease-in for exits
- Subtle hover effects on interactive elements
- Page transitions: slide or fade effects

## Voice & Tone
- Friendly and approachable
- Clear and concise
- Encouraging for dog owners
- Professional but warm

## CSS Implementation Standards

### CSS Variables Integration
Always use CSS custom properties for consistent theming:
```css
:root {
  --color-primary: #f1c232;
  --color-primary-dark: #c18a0e;
  --color-secondary: #64748b;
  --color-success: #10b981;
  --color-warning: #f59e0b;
  --color-error: #ef4444;
  --color-info: #3b82f6;
}
```

### Component Class Standards
Use standardized component classes that leverage CSS variables:

**Button Components**
```css
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed;
}

.btn-primary {
  background-color: var(--color-primary);
  color: white;
  transition: all 0.2s ease-in-out;
}

.btn-primary:hover {
  background-color: var(--color-primary-dark);
  transform: translateY(-1px);
}
```

**Card Components**
```css
.card {
  @apply bg-white overflow-hidden shadow rounded-lg transition-all duration-200;
}

.card:hover {
  @apply shadow-md;
}

.widget {
  @apply bg-white rounded-xl p-4 shadow-sm transition-all duration-200;
}

.widget:hover {
  @apply shadow-md;
}
```

### Animation Standards
Include standardized animations that match the system:

**Core Animations**
```css
.fade-in {
  animation: fadeIn 0.5s ease-in-out;
}

.slide-up {
  animation: slideUp 0.3s ease-out;
}

.bounce-gentle {
  animation: bounceGentle 2s infinite;
}

@keyframes fadeIn {
  0% { opacity: 0; }
  100% { opacity: 1; }
}

@keyframes slideUp {
  0% { transform: translateY(10px); opacity: 0; }
  100% { transform: translateY(0); opacity: 1; }
}

@keyframes bounceGentle {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-5px); }
}
```

**Interactive Animations**
```css
.quick-action {
  background-color: var(--color-primary);
  transition: all 0.2s ease-in-out;
}

.quick-action:hover {
  background-color: var(--color-primary-dark);
  transform: translateY(-2px);
}

.notification-badge {
  background-color: var(--color-error);
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.8; }
}
```

### Implementation Requirements
1. **Always use CSS variables** instead of hardcoded hex values
2. **Include hover states** with subtle transform effects (translateY(-1px to -2px))
3. **Add transition animations** (0.2s ease-in-out for interactions)
4. **Use consistent class naming** (.btn, .card, .widget, .fade-in, etc.)
5. **Include focus states** for accessibility
6. **Leverage Tailwind @apply** for consistent spacing and layout
7. **Add loading and interactive states** with appropriate animations

## Enhanced Navigation UX System

### Information Architecture
**Primary Navigation Structure**
1. **Dashboard** - User's main hub
2. **My Dogs** - Dog profiles and management
3. **Services** - Find and book services
4. **Bookings** - Manage appointments
5. **Messages** - Communication center
6. **Profile** - Account settings

### Responsive Navigation Patterns
**Desktop Navigation (>1024px)**
- Horizontal top navigation bar
- Logo left, main nav center, user menu right
- Dropdown menus for sub-navigation
- Persistent breadcrumbs below header
- Sidebar for secondary navigation when needed

**Tablet Navigation (768px-1024px)**
- Collapsible horizontal navigation
- Hamburger menu for overflow items
- Touch-friendly dropdown menus
- Swipe gestures for tab navigation

**Mobile Navigation (<768px)**
- Bottom tab bar for primary navigation
- Hamburger menu for secondary items
- Full-screen overlays for complex menus
- Thumb-friendly positioning

### Navigation State Management
**Active States**
- Current page: Primary 500 color, bold text
- Hover states: Primary 100 background
- Focus states: Primary 500 outline
- Visited states: Subtle visual indication

**Loading States**
- Skeleton navigation during initial load
- Progressive enhancement of menu items
- Smooth transitions between states
- Loading indicators for dynamic content

### Breadcrumb System
**Breadcrumb Structure**
```
Home > My Dogs > [Dog Name] > Medical Records
```

**Breadcrumb Rules**
- Always show current location
- Maximum 4 levels deep
- Truncate long page names with ellipsis
- Mobile: Show only current and parent level
- Include structured data for SEO

### Search & Filter Navigation
**Global Search**
- Prominent search bar in header
- Auto-complete with recent searches
- Scoped search by section
- Voice search support on mobile
- Search history and suggestions

**Filter Navigation**
- Persistent filter bar for list views
- Clear active filter indicators
- Quick filter presets
- Advanced filter modal/drawer
- Filter state preservation in URL

### User Context Navigation
**Role-Based Navigation**
- **Pet Owner**: Focus on dogs, services, bookings
- **Service Provider**: Business tools, calendar, clients
- **Admin**: User management, system settings

**Personalization**
- Recently viewed items
- Favorite services/providers
- Quick actions based on usage
- Customizable dashboard widgets

### Navigation Accessibility
**Screen Reader Support**
- Skip navigation links
- Landmark roles (navigation, main, aside)
- Descriptive link text
- Menu state announcements
- Keyboard navigation support

**Keyboard Navigation**
- Tab order follows visual hierarchy
- Arrow keys for menu navigation
- Enter/Space for activation
- Escape to close menus
- Focus management on page changes

### Mobile-Specific Navigation Enhancements
**Bottom Tab Bar**
- 5 primary sections maximum
- Icons with labels
- Badge notifications for messages/bookings
- Haptic feedback on selection
- Safe area considerations

**Gesture Navigation**
- Swipe between tabs
- Pull-down to refresh
- Swipe back for navigation history
- Long press for context menus
- Pinch to zoom where appropriate

**Progressive Web App Navigation**
- App-like navigation experience
- Custom splash screen
- Standalone display mode
- Native-feeling transitions
- Offline navigation support

### Navigation Performance
**Loading Optimization**
- Preload critical navigation data
- Lazy load secondary menu items
- Cache navigation state
- Progressive enhancement
- Minimize layout shifts

**Animation Performance**
- Hardware-accelerated transitions
- 60fps navigation animations
- Reduced motion preferences
- Smooth page transitions
- Optimized for low-end devices

### Context-Aware Navigation
**Smart Defaults**
- Remember last visited sections
- Suggest relevant next actions
- Contextual quick actions
- Location-based suggestions
- Time-based recommendations

**Adaptive Navigation**
- Hide unused features
- Promote frequently used items
- Seasonal navigation adjustments
- A/B test navigation patterns
- Analytics-driven improvements

## Prototype Implementation Standards

### HTML Structure Requirements
```html
<!-- Always include proper meta tags -->
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">

<!-- Include CSS variables in style block -->
<style>
:root {
  --color-primary: #f1c232;
  --color-primary-dark: #c18a0e;
  --color-secondary: #64748b;
  --color-success: #10b981;
  --color-warning: #f59e0b;
  --color-error: #ef4444;
  --color-info: #3b82f6;
}
</style>

<!-- Use semantic HTML structure -->
<header><!-- Navigation and branding --></header>
<main><!-- Primary content --></main>
<nav><!-- Bottom navigation for mobile --></nav>
```

### Component Class Usage
```html
<!-- Buttons -->
<button class="btn-primary">Primary Action</button>
<button class="quick-action">Quick Action</button>

<!-- Cards and Widgets -->
<div class="card">Standard Card</div>
<div class="widget fade-in">Dashboard Widget</div>

<!-- Interactive Elements -->
<button class="hover:text-primary-700 transition-colors">Link</button>
<span class="notification-badge">3</span>
```

### Animation Implementation
```html
<!-- Progressive Loading -->
<div class="widget fade-in">Content loads with fade</div>
<div class="widget slide-up">Content slides up</div>

<!-- Interactive Feedback -->
<button class="quick-action">Hover for transform effect</button>
<span class="notification-badge">Pulsing notification</span>
```

### Accessibility Standards
```html
<!-- Screen Reader Support -->
<button aria-label="Emergency contact">
  <i class="fas fa-exclamation-triangle" aria-hidden="true"></i>
  Emergency
</button>

<!-- Keyboard Navigation -->
<nav role="navigation" aria-label="Main navigation">
  <a href="#" class="focus:ring-2 focus:ring-primary-500">Home</a>
</nav>

<!-- Color Independence -->
<span class="bg-red-100 text-red-800" role="status">2 Due</span>
```

### Performance Optimization
```html
<!-- Image Optimization -->
<img src="image.jpg" 
     alt="Descriptive text" 
     loading="lazy"
     class="w-10 h-10 rounded-full">

<!-- Font Loading -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&family=Poppins:wght@600&display=swap" rel="stylesheet">
```

### Mobile-First Responsive Design
```html
<!-- Touch Targets -->
<button class="w-12 h-12 flex items-center justify-center">
  <!-- Minimum 44px touch target -->
</button>

<!-- Responsive Images -->
<img srcset="image-small.jpg 300w, image-large.jpg 600w"
     sizes="(max-width: 768px) 300px, 600px"
     src="image-large.jpg" alt="Description">

<!-- Safe Areas -->
<nav class="pb-safe-area-inset-bottom">
  <!-- Account for iPhone home indicator -->
</nav>
```

## Mobile-First UX Patterns

### Touch Interaction Guidelines
**Touch Targets**
- Minimum size: 44px x 44px (iOS) / 48px x 48px (Android)
- Spacing between targets: 8px minimum
- Primary actions: 56px x 56px for better accessibility
- Icon buttons: 40px x 40px with 4px padding

**Gesture Support**
- Swipe to delete on list items
- Pull-to-refresh on data lists
- Pinch to zoom on images
- Swipe navigation between screens
- Long press for context menus

### Mobile Navigation Patterns
**Bottom Navigation (Primary)**
- 5 main sections maximum
- Icons with labels
- Active state: Primary 500 color
- Height: 64px
- Safe area padding for iPhone

**Hamburger Menu (Secondary)**
- Slide-out drawer from left
- Overlay with backdrop
- Width: 280px (max 85% of screen)
- Smooth animation (300ms ease-out)
- Close on backdrop tap

**Tab Navigation**
- Horizontal scrollable tabs
- Snap to center alignment
- Active tab indicator (Primary 500)
- Minimum tab width: 120px

### Mobile Form Patterns
**Input Fields**
- Full width on mobile
- Larger touch targets (48px height)
- Clear button (X) on right when focused
- Floating labels for better UX
- Auto-focus next field on completion

**Form Layout**
- Single column layout
- Grouped related fields
- Progress indicator for multi-step forms
- Sticky action buttons at bottom
- Keyboard-aware scrolling

**Date/Time Pickers**
- Native mobile pickers preferred
- Custom pickers for specific needs
- Clear visual feedback
- Easy cancellation

### Mobile Card Layouts
**List Cards**
- Full width with 16px margins
- Minimum height: 80px
- Swipe actions (edit, delete)
- Loading skeleton states
- Infinite scroll or pagination

**Grid Cards**
- 2 columns on mobile
- 16px gap between cards
- Aspect ratio maintained
- Lazy loading for images
- Pull-to-refresh support

### Mobile Modal Patterns
**Bottom Sheets**
- Slide up from bottom
- Drag handle at top
- Backdrop dismissal
- Keyboard-aware positioning
- Safe area considerations

**Full Screen Modals**
- For complex forms or content
- Close button top-left
- Save/Done button top-right
- Scroll to top on open
- Prevent body scroll

### Mobile Loading States
**Skeleton Screens**
- Match content layout
- Subtle animation (shimmer effect)
- Appropriate for content loading
- Maintain layout stability

**Pull-to-Refresh**
- Standard iOS/Android patterns
- Custom spinner with brand colors
- Haptic feedback on trigger
- Clear success/error states

**Infinite Scroll**
- Load more indicator at bottom
- Error state with retry button
- End of list indicator
- Smooth loading animation

### Mobile Error Handling
**Inline Errors**
- Below form fields
- Red color (#EF4444)
- Clear, actionable messages
- Icon for visual emphasis

**Toast Messages**
- Bottom position on mobile
- 4 second auto-dismiss
- Swipe to dismiss
- Action button for undo/retry

**Error Pages**
- Friendly illustrations
- Clear explanation
- Primary action button
- Secondary navigation options

### Mobile Typography Scale
**Adjusted for Mobile**
- Heading 1: 1.875rem (30px) - Reduced from desktop
- Heading 2: 1.5rem (24px) - Reduced from desktop
- Heading 3: 1.25rem (20px) - Reduced from desktop
- Body: 1rem (16px) - Same as desktop
- Body Small: 0.875rem (14px) - Same as desktop
- Minimum text size: 16px (prevents zoom on iOS)

### Mobile Spacing Adjustments
**Container Padding**
- Mobile: 16px horizontal padding
- Tablet: 24px horizontal padding
- Desktop: 32px horizontal padding

**Component Spacing**
- Tighter spacing on mobile
- More generous touch targets
- Adequate white space for readability

### Mobile Performance Patterns
**Image Optimization**
- Responsive images with srcset
- WebP format with fallbacks
- Lazy loading below fold
- Placeholder while loading
- Compression for mobile networks

**Progressive Enhancement**
- Core functionality works without JS
- Enhanced features with JS
- Graceful degradation
- Offline-first approach where possible

### Mobile Accessibility
**Screen Reader Support**
- Proper heading hierarchy
- Descriptive link text
- Form labels and instructions
- Focus management
- Semantic HTML structure

**Motor Accessibility**
- Large touch targets
- Adequate spacing
- Alternative input methods
- Voice control support
- Switch navigation support

### Platform-Specific Considerations
**iOS Specific**
- Safe area insets
- Status bar considerations
- iOS-style navigation
- Haptic feedback
- iOS keyboard behavior

**Android Specific**
- Material Design principles
- Android back button behavior
- Android keyboard handling
- System navigation gestures
- Android-specific animations

## Standardized Form UX Patterns

### Form Validation Strategy
**Real-time Validation**
- Validate on blur (when user leaves field)
- Show success state immediately when valid
- Debounce validation for search/autocomplete (300ms)
- Never validate on first keystroke
- Re-validate on change after initial validation

**Validation States**
- **Default**: Border #E5E7EB, no message
- **Focus**: Border Primary 500 (#F1C232), no message
- **Valid**: Border Success (#10B981), green checkmark icon
- **Invalid**: Border Error (#EF4444), red error message below
- **Loading**: Border Primary 500, spinner icon in field

### Error Message Patterns
**Error Message Format**
```
[Icon] Clear, actionable error message
```

**Error Message Examples**
- ❌ "Email is required"
- ❌ "Please enter a valid email address"
- ❌ "Password must be at least 8 characters"
- ❌ "Dog's name cannot contain special characters"
- ❌ "Please select your dog's breed"

**Error Message Guidelines**
- Start with what's wrong, then how to fix it
- Use plain language, avoid technical jargon
- Be specific about requirements
- Include examples when helpful
- Keep under 60 characters when possible

### Form Layout Standards
**Single Column Layout (Mobile-First)**
- Stack all fields vertically
- Full width inputs with 16px margins
- 24px spacing between field groups
- Group related fields with subtle dividers

**Field Grouping**
- Use fieldset and legend for screen readers
- Visual grouping with background color or borders
- Clear section headers (Heading 4 style)
- Logical tab order throughout form

**Required Field Indicators**
- Red asterisk (*) after label text
- "Required" text for screen readers
- Clear indication of optional fields
- Summary of required fields at form start

### Progressive Form Patterns
**Multi-Step Forms**
- Progress indicator at top
- Step titles and numbers
- Previous/Next navigation
- Save draft functionality
- Clear exit points

**Conditional Fields**
- Smooth show/hide animations (200ms)
- Maintain form layout stability
- Clear dependency relationships
- Accessible state announcements

**Auto-Save Patterns**
- Save draft every 30 seconds
- Visual indicator when saving
- Restore draft on return
- Clear saved state on submit

### Form Submission Patterns
**Submit Button States**
- **Default**: Primary button styling
- **Loading**: Disabled with spinner, "Submitting..."
- **Success**: Brief success state, then redirect/update
- **Error**: Return to enabled state, show error

**Form-Level Error Handling**
- Error summary at top of form
- Link to first error field
- Maintain scroll position
- Clear, actionable error messages

**Success Patterns**
- Immediate feedback on successful submission
- Clear next steps for user
- Option to perform related actions
- Confirmation details when appropriate

### Specific Form Types

#### Dog Registration Form
**Field Order**
1. Dog's name (required)
2. Breed selection (required, searchable dropdown)
3. Date of birth (required, date picker)
4. Gender (required, radio buttons)
5. Photo upload (optional, drag & drop)
6. Description (optional, textarea)

**Validation Rules**
- Name: 2-50 characters, letters and spaces only
- Breed: Must select from approved list
- Date: Cannot be future date, reasonable age limits
- Photo: Max 5MB, JPG/PNG only

#### Service Booking Form
**Field Order**
1. Service type (required, radio buttons)
2. Date selection (required, calendar picker)
3. Time slot (required, depends on date)
4. Special requirements (optional, textarea)
5. Contact preferences (required, checkboxes)

**Smart Defaults**
- Pre-select user's preferred contact method
- Show available times based on selected date
- Remember previous service preferences

#### User Profile Form
**Sections**
1. Personal Information
2. Contact Details
3. Preferences
4. Privacy Settings

**Auto-Complete Support**
- Use appropriate autocomplete attributes
- Support browser password managers
- Pre-fill known information
- Smart address completion

### Accessibility Standards
**Screen Reader Support**
- Proper label associations
- Error announcements
- Field descriptions with aria-describedby
- Form instructions clearly communicated

**Keyboard Navigation**
- Logical tab order
- Skip links for long forms
- Enter key submits forms
- Escape key cancels modals

**Visual Accessibility**
- High contrast error states
- Clear focus indicators
- Sufficient color contrast (4.5:1 minimum)
- Icons paired with text

### Form Performance
**Loading Optimization**
- Lazy load non-critical form sections
- Optimize dropdown data loading
- Cache frequently used data
- Progressive enhancement approach

**Validation Performance**
- Client-side validation first
- Server-side validation backup
- Debounced async validation
- Minimal API calls during typing

### Error Recovery Patterns
**Network Errors**
- Retry button with exponential backoff
- Offline form saving
- Clear error explanation
- Alternative contact methods

**Validation Errors**
- Focus first error field
- Scroll to error location
- Maintain user's entered data
- Provide correction suggestions

**Session Timeout**
- Warning before timeout
- Auto-save form data
- Easy re-authentication
- Restore form state after login

## Enhanced Navigation UX System

### Information Architecture
**Primary Navigation Structure**
1. **Dashboard** - User's main hub
2. **My Dogs** - Dog profiles and management
3. **Services** - Find and book services
4. **Bookings** - Manage appointments
5. **Messages** - Communication center
6. **Profile** - Account settings

### Responsive Navigation Patterns
**Desktop Navigation (>1024px)**
- Horizontal top navigation bar
- Logo left, main nav center, user menu right
- Dropdown menus for sub-navigation
- Persistent breadcrumbs below header
- Sidebar for secondary navigation when needed

**Tablet Navigation (768px-1024px)**
- Collapsible horizontal navigation
- Hamburger menu for overflow items
- Touch-friendly dropdown menus
- Swipe gestures for tab navigation

**Mobile Navigation (<768px)**
- Bottom tab bar for primary navigation
- Hamburger menu for secondary items
- Full-screen overlays for complex menus
- Thumb-friendly positioning

### Navigation State Management
**Active States**
- Current page: Primary 500 color, bold text
- Hover states: Primary 100 background
- Focus states: Primary 500 outline
- Visited states: Subtle visual indication

**Loading States**
- Skeleton navigation during initial load
- Progressive enhancement of menu items
- Smooth transitions between states
- Loading indicators for dynamic content

### Breadcrumb System
**Breadcrumb Structure**
```
Home > My Dogs > [Dog Name] > Medical Records
```

**Breadcrumb Rules**
- Always show current location
- Maximum 4 levels deep
- Truncate long page names with ellipsis
- Mobile: Show only current and parent level
- Include structured data for SEO

### Search & Filter Navigation
**Global Search**
- Prominent search bar in header
- Auto-complete with recent searches
- Scoped search by section
- Voice search support on mobile
- Search history and suggestions

**Filter Navigation**
- Persistent filter bar for list views
- Clear active filter indicators
- Quick filter presets
- Advanced filter modal/drawer
- Filter state preservation in URL

### User Context Navigation
**Role-Based Navigation**
- **Pet Owner**: Focus on dogs, services, bookings
- **Service Provider**: Business tools, calendar, clients
- **Admin**: User management, system settings

**Personalization**
- Recently viewed items
- Favorite services/providers
- Quick actions based on usage
- Customizable dashboard widgets

### Navigation Accessibility
**Screen Reader Support**
- Skip navigation links
- Landmark roles (navigation, main, aside)
- Descriptive link text
- Menu state announcements
- Keyboard navigation support

**Keyboard Navigation**
- Tab order follows visual hierarchy
- Arrow keys for menu navigation
- Enter/Space for activation
- Escape to close menus
- Focus management on page changes

### Mobile-Specific Navigation Enhancements
**Bottom Tab Bar**
- 5 primary sections maximum
- Icons with labels
- Badge notifications for messages/bookings
- Haptic feedback on selection
- Safe area considerations

**Gesture Navigation**
- Swipe between tabs
- Pull-down to refresh
- Swipe back for navigation history
- Long press for context menus
- Pinch to zoom where appropriate

**Progressive Web App Navigation**
- App-like navigation experience
- Custom splash screen
- Standalone display mode
- Native-feeling transitions
- Offline navigation support

### Navigation Performance
**Loading Optimization**
- Preload critical navigation data
- Lazy load secondary menu items
- Cache navigation state
- Progressive enhancement
- Minimize layout shifts

**Animation Performance**
- Hardware-accelerated transitions
- 60fps navigation animations
- Reduced motion preferences
- Smooth page transitions
- Optimized for low-end devices

### Context-Aware Navigation
**Smart Defaults**
- Remember last visited sections
- Suggest relevant next actions
- Contextual quick actions
- Location-based suggestions
- Time-based recommendations

**Adaptive Navigation**
- Hide unused features
- Promote frequently used items
- Seasonal navigation adjustments
- A/B test navigation patterns
- Analytics-driven improvements

## Entity Framework Core Best Practices

### CRITICAL: Always Verify Entity Properties

**Before writing any EF Core queries, ALWAYS:**
1. Check the actual entity model files to verify property names
2. Check the DbContext to verify DbSet names
3. Never assume navigation property names - verify them in the entity classes
4. Use IntelliSense or check the model files to ensure properties exist

```csharp
// ❌ Bad - Assuming property names without verification
var provider = await _context.ServiceProviders
    .Include(sp => sp.ProviderServices) // This property might not exist!
    .FirstOrDefaultAsync();

// ✅ Good - First check ServiceProvider.cs to see actual properties
// If ServiceProvider has ICollection<Service> Services, use:
var provider = await _context.ServiceProviders
    .Include(sp => sp.Services)
    .FirstOrDefaultAsync();
```

### Query Optimization

#### Use .Include() for Eager Loading
**Best Practice**: Use `.Include()` and `.ThenInclude()` to load related data in a single query instead of making multiple database calls.

```csharp
// ❌ Bad - Multiple separate queries (N+1 problem)
var provider = await _context.ServiceProviders
    .FirstOrDefaultAsync(sp => sp.Id == providerId);
var services = await _context.ProviderService
    .Where(ps => ps.ProviderId == provider.Id)
    .ToListAsync();
var pricing = await _context.ProviderServicePricing
    .Where(psp => services.Select(s => s.Id).Contains(psp.ProviderServiceId))
    .ToListAsync();

// ✅ Good - Single query with eager loading
var provider = await _context.ServiceProviders
    .Include(sp => sp.User)
    .Include(sp => sp.ProviderServices)
        .ThenInclude(ps => ps.ServiceCategory)
    .Include(sp => sp.ProviderServices)
        .ThenInclude(ps => ps.ProviderServicePricings)
            .ThenInclude(psp => psp.SubService)
    .FirstOrDefaultAsync(sp => sp.Id == providerId);
```

#### Use .AsNoTracking() for Read-Only Queries
**Best Practice**: Always use `.AsNoTracking()` when querying data that won't be modified. This improves performance by preventing EF Core from tracking entity changes.

```csharp
// ❌ Bad - Tracking entities unnecessarily
var providers = await _context.ServiceProviders
    .Include(sp => sp.User)
    .Where(sp => sp.IsActive)
    .ToListAsync();

// ✅ Good - No tracking for read-only operations
var providers = await _context.ServiceProviders
    .AsNoTracking()
    .Include(sp => sp.User)
    .Where(sp => sp.IsActive)
    .ToListAsync();
```

#### Avoid N+1 Query Problems
**Best Practice**: Never execute queries inside loops. Use `.Include()` or projection to load all required data upfront.

```csharp
// ❌ Bad - Query inside loop
foreach (var provider in providers)
{
    var services = await _context.ProviderService
        .Where(ps => ps.ProviderId == provider.Id)
        .ToListAsync();
}

// ✅ Good - Load all data upfront
var providers = await _context.ServiceProviders
    .AsNoTracking()
    .Include(sp => sp.ProviderServices)
    .ToListAsync();
```

#### Use Projection for Better Performance
**Best Practice**: Project to DTOs to avoid loading unnecessary columns and improve query performance.

```csharp
// ❌ Bad - Loading entire entity
var providers = await _context.ServiceProviders
    .Include(sp => sp.User)
    .ToListAsync();
var dtos = providers.Select(p => new ProviderDto { 
    Name = p.BusinessName,
    Email = p.User.Email 
});

// ✅ Good - Project directly to DTO
var dtos = await _context.ServiceProviders
    .AsNoTracking()
    .Select(sp => new ProviderDto
    {
        Name = sp.BusinessName,
        Email = sp.User.Email
    })
    .ToListAsync();
```

#### Use AddRange for Bulk Operations
**Best Practice**: Use `AddRange()` instead of multiple `Add()` calls when inserting multiple entities.

```csharp
// ❌ Bad - Multiple Add calls
foreach (var item in items)
{
    _context.Items.Add(item);
}

// ✅ Good - Single AddRange call
_context.Items.AddRange(items);
```

### Query Patterns

#### Filtering and Pagination
**Best Practice**: Apply filtering and pagination at the database level, not in memory.

```csharp
// ❌ Bad - Loading all data then filtering
var allProviders = await _context.ServiceProviders.ToListAsync();
var filtered = allProviders.Where(p => p.IsActive).Skip(10).Take(20);

// ✅ Good - Filter and paginate in the query
var providers = await _context.ServiceProviders
    .AsNoTracking()
    .Where(p => p.IsActive)
    .Skip(10)
    .Take(20)
    .ToListAsync();
```

#### Complex Queries with Multiple Includes
**Best Practice**: Structure complex queries with multiple includes for readability and performance.

```csharp
var result = await _context.ServiceProviders
    .AsNoTracking()
    .Include(sp => sp.User)
    .Include(sp => sp.ProviderServices)
        .ThenInclude(ps => ps.ServiceCategory)
    .Include(sp => sp.ProviderServices)
        .ThenInclude(ps => ps.ProviderServicePricings)
            .ThenInclude(psp => psp.SubService)
    .Where(sp => sp.IsActive && sp.User.IsActive)
    .FirstOrDefaultAsync(sp => sp.Id == providerId);
```

### Performance Guidelines

1. **Always use `.AsNoTracking()`** for read-only queries
2. **Use `.Include()` and `.ThenInclude()`** to eager load related data
3. **Avoid queries in loops** - load all data upfront
4. **Project to DTOs** when you don't need the full entity
5. **Use `AddRange()`** for bulk inserts
6. **Apply filters at the database level**, not in memory
7. **Consider using compiled queries** for frequently executed queries
8. **Use raw SQL or stored procedures** for complex queries when needed

### Testing Considerations

Even in test code, follow these practices for consistency:

```csharp
// Test queries should also use AsNoTracking
var testData = await _context.Items
    .AsNoTracking()
    .Include(i => i.RelatedData)
    .Where(i => i.IsTest)
    .ToListAsync();
```

### Common Anti-Patterns to Avoid

1. **Lazy Loading**: Can cause N+1 problems unexpectedly
2. **Loading entire tables**: Always filter and paginate
3. **Using `.ToList()` too early**: Apply all filters first
4. **Ignoring async patterns**: Use async/await consistently
5. **Not disposing contexts**: Use proper dependency injection

---

*This style guide is a living document and should be updated as the design system evolves.*