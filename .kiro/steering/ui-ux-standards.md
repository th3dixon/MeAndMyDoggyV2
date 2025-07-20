---
inclusion: always
---

# MeAndMyDog UI/UX Standards

## Design System

### Color Palette
**Primary Colors**
- Primary Golden: #F1C232 (`text-primary-500`, `bg-primary-500`)
- Primary Dark: #C18A0E (`text-primary-700`, `bg-primary-700`)
- Primary Light: #FEFBF0 (`text-primary-100`, `bg-primary-100`)

**Secondary Colors**
- Accent Orange: #F59E0B (`text-amber-500`, `bg-amber-500`)
- Success Green: #10B981 (`text-emerald-500`, `bg-emerald-500`)
- Warning Yellow: #F59E0B (`text-amber-500`, `bg-amber-500`)
- Error Red: #EF4444 (`text-red-500`, `bg-red-500`)

**Neutral Colors**
- Text Primary: #111827 (`text-gray-900`)
- Text Secondary: #6B7280 (`text-gray-500`)
- Text Muted: #9CA3AF (`text-gray-400`)
- Background: #FFFFFF (`bg-white`)
- Background Secondary: #F9FAFB (`bg-gray-50`)
- Border: #E5E7EB (`border-gray-200`)

### Typography Scale
- **Heading 1**: `text-4xl font-bold` (36px)
- **Heading 2**: `text-3xl font-semibold` (30px)
- **Heading 3**: `text-2xl font-semibold` (24px)
- **Heading 4**: `text-xl font-medium` (20px)
- **Body Large**: `text-lg font-normal` (18px)
- **Body**: `text-base font-normal` (16px)
- **Body Small**: `text-sm font-normal` (14px)
- **Caption**: `text-xs font-medium` (12px)

### Spacing System (4px grid)
- **xs**: `space-1` (4px)
- **sm**: `space-2` (8px)
- **md**: `space-4` (16px)
- **lg**: `space-6` (24px)
- **xl**: `space-8` (32px)
- **2xl**: `space-12` (48px)
- **3xl**: `space-16` (64px)

## Component Standards

### Button Components
**Primary Button**
```html
<button class="bg-primary-500 hover:bg-primary-600 text-white font-medium py-3 px-6 rounded-lg transition-colors">
  Button Text
</button>
```

**Secondary Button**
```html
<button class="border border-primary-500 text-primary-500 hover:bg-primary-50 font-medium py-3 px-6 rounded-lg transition-colors">
  Button Text
</button>
```

**Danger Button**
```html
<button class="bg-red-500 hover:bg-red-600 text-white font-medium py-3 px-6 rounded-lg transition-colors">
  Button Text
</button>
```

### Form Elements
**Input Fields**
```html
<input class="w-full px-4 py-3 border border-gray-200 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-primary-500 outline-none transition-colors">
```

**Labels**
```html
<label class="block text-sm font-medium text-gray-900 mb-2">Label Text</label>
```

**Form Groups**
```html
<div class="mb-6">
  <label class="block text-sm font-medium text-gray-900 mb-2">Label</label>
  <input class="w-full px-4 py-3 border border-gray-200 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-primary-500 outline-none transition-colors">
</div>
```

### Card Components
**Standard Card**
```html
<div class="bg-white border border-gray-200 rounded-xl p-6 shadow-sm">
  <!-- Card content -->
</div>
```

**Interactive Card**
```html
<div class="bg-white border border-gray-200 rounded-xl p-6 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
  <!-- Card content -->
</div>
```

## Layout Standards

### Container Widths
- **Full Width**: `w-full`
- **Container**: `max-w-7xl mx-auto px-4 sm:px-6 lg:px-8`
- **Content Width**: `max-w-4xl mx-auto`
- **Form Width**: `max-w-md mx-auto`

### Grid System
```html
<!-- 12-column grid -->
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
  <!-- Grid items -->
</div>
```

### Responsive Breakpoints
- **Mobile**: `< 768px` (default)
- **Tablet**: `md:` (768px+)
- **Desktop**: `lg:` (1024px+)
- **Large Desktop**: `xl:` (1280px+)

## Navigation Standards

### Header Structure
```html
<header class="bg-white border-b border-gray-200 sticky top-0 z-50">
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
    <div class="flex justify-between items-center h-16">
      <!-- Navigation content -->
    </div>
  </div>
</header>
```

### Sidebar Navigation
```html
<aside class="w-64 bg-white border-r border-gray-200 fixed inset-y-0 left-0 z-40 lg:static lg:inset-auto">
  <!-- Sidebar content -->
</aside>
```

## Interactive States

### Hover Effects
- **Buttons**: `hover:bg-primary-600` (darker shade)
- **Cards**: `hover:shadow-md`
- **Links**: `hover:text-primary-600`
- **Icons**: `hover:text-primary-500`

### Focus States
- **Form Elements**: `focus:ring-2 focus:ring-primary-500 focus:border-primary-500`
- **Buttons**: `focus:ring-2 focus:ring-primary-500 focus:ring-offset-2`
- **Links**: `focus:outline-none focus:ring-2 focus:ring-primary-500`

### Loading States
- **Skeleton**: Use `animate-pulse` with gray backgrounds
- **Spinners**: Use consistent spinner component
- **Button Loading**: Disable and show loading indicator

## Accessibility Standards

### ARIA Labels
- Use `aria-label` for icon buttons
- Use `aria-describedby` for form help text
- Use `role` attributes appropriately
- Implement `aria-expanded` for dropdowns

### Keyboard Navigation
- Ensure all interactive elements are focusable
- Implement logical tab order
- Use `tabindex="-1"` for programmatic focus
- Support keyboard shortcuts where appropriate

### Color Contrast
- Maintain 4.5:1 contrast ratio for normal text
- Maintain 3:1 contrast ratio for large text
- Use color plus other indicators (icons, text)
- Test with color blindness simulators

## Animation Standards

### Transition Classes
- **Default**: `transition-colors duration-200`
- **Transform**: `transition-transform duration-200`
- **All**: `transition-all duration-200`
- **Shadow**: `transition-shadow duration-200`

### Animation Timing
- **Micro-interactions**: 200ms
- **Page transitions**: 300ms
- **Loading animations**: 1000ms+
- **Hover effects**: 150ms

### Easing Functions
- **Ease-out**: For entrances (`ease-out`)
- **Ease-in**: For exits (`ease-in`)
- **Ease-in-out**: For transforms (`ease-in-out`)

## Mobile-First Design

### Touch Targets
- Minimum 44px touch targets
- Adequate spacing between interactive elements
- Consider thumb-friendly navigation
- Use appropriate input types

### Mobile Navigation
- Implement hamburger menu for mobile
- Use bottom navigation for primary actions
- Consider swipe gestures
- Optimize for one-handed use

### Responsive Images
```html
<img class="w-full h-auto object-cover rounded-lg" 
     src="image.jpg" 
     alt="Descriptive alt text"
     loading="lazy">
```

## Content Guidelines

### Voice & Tone
- Friendly and approachable
- Clear and concise
- Encouraging for dog owners
- Professional but warm
- Use active voice
- Avoid jargon

### Error Messages
- Be specific about the problem
- Provide clear next steps
- Use friendly, non-technical language
- Include helpful suggestions
- Avoid blame or negative language

### Loading States
- Use skeleton screens for content
- Show progress indicators for long operations
- Provide estimated time when possible
- Keep users informed of progress
- Allow cancellation when appropriate

## Performance Guidelines

### Image Optimization
- Use appropriate formats (WebP, AVIF)
- Implement lazy loading
- Provide multiple sizes for responsive images
- Compress images appropriately
- Use CDN for static assets

### Bundle Optimization
- Code splitting for routes
- Tree shaking for unused code
- Minimize CSS and JavaScript
- Use compression (gzip/brotli)
- Implement caching strategies