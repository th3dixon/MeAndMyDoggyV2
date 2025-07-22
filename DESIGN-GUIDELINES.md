# MeAndMyDoggy Design Guidelines

## Button Standards and Transitions

### 🎨 Button Design Philosophy

All buttons across the MeAndMyDoggy platform should provide **consistent visual feedback** through smooth color and transform transitions. This creates a cohesive, professional user experience that builds trust and engagement.

### 🔥 Core Button Classes

#### **Primary Buttons (.btn-primary)**
- **Use For**: Main actions, CTAs, primary search functions
- **Visual**: Orange gradient background with shimmer effect
- **Transitions**: 
  - Color gradients with animation
  - Vertical transform (-2px on hover)
  - Box-shadow enhancement
  - Shimmer effect on hover
- **Duration**: 300ms with cubic-bezier easing

```css
.btn-primary {
    background: linear-gradient(135deg, var(--pet-orange), #ff8533);
    transition: all 0.3s ease;
    transform: translateY(-2px) on hover;
    box-shadow: enhanced on hover;
    shimmer effect: white overlay slide;
}
```

#### **Secondary Buttons (.btn-secondary)**
- **Use For**: Alternative actions, less prominent CTAs
- **Visual**: White background with orange border, fills orange on hover
- **Transitions**:
  - Background fill animation (left to right)
  - Color inversion (orange text → white text)
  - Vertical transform (-2px on hover)
  - Box-shadow enhancement

#### **Ghost Buttons (.btn-ghost)**
- **Use For**: Subtle actions, tertiary functions
- **Visual**: Transparent with orange text, subtle background on hover
- **Transitions**:
  - Background scale animation (from center)
  - Border appearance
  - Subtle color intensification

### ⚡ Transition Standards

#### **Mandatory Transition Properties**
All interactive buttons MUST include:

1. **Color Transitions** (300ms ease)
   - Background color changes
   - Text color changes
   - Border color changes

2. **Transform Transitions** (300ms cubic-bezier)
   - Hover: `translateY(-2px)` (lift effect)
   - Active: `translateY(0)` (press effect)
   - Scale: `scale(1.02)` for emphasis (optional)

3. **Box-Shadow Transitions** (300ms ease)
   - Enhanced shadow depth on hover
   - Color-matched shadows for brand consistency

4. **Special Effects** (where appropriate)
   - Shimmer/shine effects for primary actions
   - Background fill animations for secondary buttons
   - Ripple effects for touch interactions

#### **Timing and Easing**
- **Standard Duration**: 300ms
- **Hover Easing**: `cubic-bezier(0.4, 0, 0.2, 1)`
- **Active States**: Faster 150ms for immediate feedback
- **Complex Animations**: Up to 500ms for multi-stage effects

### 🎯 Implementation Requirements

#### **Button Hierarchy**
1. **Primary**: Main search buttons, booking CTAs, sign-up
2. **Secondary**: Alternative actions, "Learn More", filters
3. **Ghost**: Subtle actions, navigation, close buttons

#### **Consistency Rules**
- ✅ **DO**: Use established button classes consistently
- ✅ **DO**: Apply transitions to ALL interactive buttons
- ✅ **DO**: Include hover, active, and focus states
- ✅ **DO**: Use color-matched shadows and effects

- ❌ **DON'T**: Create custom button styles without transitions
- ❌ **DON'T**: Use different transition durations without reason
- ❌ **DON'T**: Skip hover/active states for any interactive element
- ❌ **DON'T**: Use generic shadow colors instead of brand-matched ones

### 📱 Responsive Considerations

#### **Mobile Adaptations**
- Reduce transform distances on smaller screens
- Ensure touch targets meet 44px minimum
- Adapt transition timing for touch devices (slightly faster)

#### **Accessibility**
- Respect `prefers-reduced-motion` for users with motion sensitivity
- Maintain sufficient color contrast in all states
- Ensure focus states are clearly visible

### 🔧 Code Examples

#### **Correct Implementation**
```html
<!-- Primary Action -->
<button class="btn-primary">
    <i class="fas fa-search mr-2"></i>
    Quick Search
</button>

<!-- Secondary Action -->
<button class="btn-secondary">
    <i class="fas fa-list mr-2"></i>
    Full Search
</button>
```

#### **Custom Button Extensions**
When extending button styles, maintain transition standards:

```css
.btn-search-large {
    @extend .btn-primary;
    font-size: 1.25rem;
    padding: 1.25rem 2rem;
    /* Transitions inherited from .btn-primary */
}
```

### 🎨 Color Palette for Buttons

#### **Brand Colors**
- **Primary Orange**: `#F97316` (var(--pet-orange))
- **Secondary Blue**: `#3B82F6` (var(--pet-blue))
- **Success Green**: `#10B981` (var(--pet-green))
- **Warning Yellow**: `#F59E0B`
- **Error Red**: `#EF4444`

#### **Shadow Colors**
- Orange buttons: `rgba(249, 115, 22, 0.4)`
- Blue buttons: `rgba(59, 130, 246, 0.4)`
- Green buttons: `rgba(16, 185, 129, 0.4)`

### ✅ Quality Checklist

Before deploying any button:

- [ ] Includes smooth color transitions (300ms)
- [ ] Has hover transform effect (-2px translateY)
- [ ] Includes active state feedback
- [ ] Uses brand-appropriate shadow colors
- [ ] Has focus state for accessibility
- [ ] Respects motion preferences
- [ ] Works consistently across browsers
- [ ] Maintains visual hierarchy

### 🚀 Performance Notes

- Use `transform` and `opacity` for animations (GPU accelerated)
- Avoid animating `width`, `height`, or `top/left` properties
- Use `will-change` sparingly and remove after animations
- Test on lower-end devices for smooth performance

---

**Last Updated**: Current Date  
**Next Review**: Quarterly basis  
**Maintained By**: Frontend Development Team

> **Note**: These guidelines should be followed for ALL interactive elements, not just buttons. This includes links, cards, form inputs, and any clickable/tappable interface elements.