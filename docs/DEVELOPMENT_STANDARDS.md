# MeAndMyDoggy Development Standards

## Architecture Overview

This project uses a **Vue-in-.NET** architecture where:
- ASP.NET Core serves the main application and handles routing
- Vue.js components are mounted on specific pages within .cshtml views
- Vite.AspNetCore provides seamless integration between ASP.NET Core and Vue.js
- Each page can have its own Vue app while sharing common components

## Project Structure

```
src/Web/MeAndMyDog.WebApp/
├── Controllers/           # ASP.NET Core controllers
├── Views/                # Razor .cshtml views
│   ├── Auth/             # Authentication pages
│   ├── Dashboard/        # Dashboard pages
│   ├── Home/             # Public pages
│   └── Shared/           # Layout and shared views
├── src/                  # Vue.js source code
│   ├── entries/          # Page-specific entry points
│   ├── components/       # Reusable Vue components
│   ├── views/           # Full Vue page components
│   ├── stores/          # Pinia state management
│   ├── composables/     # Vue composables
│   └── types/           # TypeScript types
└── wwwroot/             # Static assets and built files
```

## Development Guidelines

### 1. Page Creation Pattern

When creating new pages that need Vue.js functionality:

1. **Create ASP.NET Core Controller and Action**
2. **Create .cshtml View** that includes Vue container
3. **Create Vue Entry Point** in `src/entries/`
4. **Mount Vue Component** in the entry point
5. **Update Vite Config** if needed

#### Example: Creating a Search Page

**Controller:**
```csharp
public class SearchController : Controller
{
    public IActionResult Index() => View();
}
```

**View (Views/Search/Index.cshtml):**
```html
@{
    ViewData["Title"] = "Search - MeAndMyDoggy";
}

<div id="search-app"></div>

@section Scripts {
    <script type="module" vite-src="~/src/entries/search.ts"></script>
}
```

**Entry Point (src/entries/search.ts):**
```typescript
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import '../style.css'
import SearchPage from '../views/Search.vue'

const pinia = createPinia()

document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('search-app')
    if (container) {
        const app = createApp(SearchPage)
        app.use(pinia)
        app.mount('#search-app')
    }
})
```

### 2. Component Organization

- **`src/components/common/`** - Shared components (buttons, inputs, modals)
- **`src/components/layout/`** - Layout-specific components (navigation, footer)
- **`src/components/[feature]/`** - Feature-specific components
- **`src/views/`** - Full page Vue components

### 3. State Management

- Use **Pinia** for state management
- Create feature-specific stores in `src/stores/`
- Import stores in entry points, not globally

### 4. Styling Guidelines

- **Primary CSS** in `wwwroot/css/site.css` with custom color scheme
- **Tailwind CSS** through Vite for utility classes
- **Component styles** using Vue SFC `<style>` blocks
- **No inline styles** - use CSS classes

### 5. Routing Strategy

- **ASP.NET Core routing** for main navigation between pages
- **Vue Router** within specific pages that need SPA behavior
- **Example:** Dashboard might use Vue Router for sub-pages

### 6. API Integration

- **HttpClient** configured in ASP.NET Core for server-side calls
- **Axios** in Vue components for client-side API calls
- **Base URL** configured via `window.APP_CONFIG`

## Vite.AspNetCore Integration

### Tag Helpers

Use Vite tag helpers in .cshtml files:

```html
<!-- CSS -->
<link rel="stylesheet" vite-href="~/src/style.css" asp-append-version="true" />

<!-- JavaScript -->
<script type="module" vite-src="~/src/entries/[page].ts"></script>
```

### Development vs Production

- **Development:** Vite dev server runs automatically, hot reload enabled
- **Production:** Vite builds assets to `wwwroot/dist/`, served by ASP.NET Core

## File Naming Conventions

- **Controllers:** `PascalCase` (e.g., `AuthController.cs`)
- **Views:** `PascalCase` (e.g., `Login.cshtml`)
- **Vue Components:** `PascalCase` (e.g., `UserProfile.vue`)
- **TypeScript files:** `camelCase` (e.g., `userAuth.ts`)
- **Entry points:** `lowercase` (e.g., `auth.ts`, `dashboard.ts`)

## Required Packages

### ASP.NET Core
- `Vite.AspNetCore` - Vite integration
- `Microsoft.AspNetCore.SpaServices.Extensions` - SPA support

### Frontend
- `vue` - Vue.js framework
- `pinia` - State management
- `vue-router` - Client-side routing (when needed)
- `tailwindcss` - Utility CSS framework
- `@vitejs/plugin-vue` - Vite Vue plugin

## Build Process

### Development
```bash
dotnet run  # Starts ASP.NET Core and Vite dev server
```

### Production
```bash
dotnet publish  # Builds Vue.js assets and publishes .NET app
```

## Best Practices

1. **Keep Vue apps page-specific** - don't create one massive SPA
2. **Use ASP.NET Core for authentication** and session management
3. **Share common components** across entry points
4. **Lazy load** Vue components when possible
5. **Use TypeScript** for all Vue code
6. **Follow Vue 3 Composition API** patterns
7. **Test both server-side and client-side** functionality

## Migration from Full SPA

When converting existing full SPA pages to this architecture:

1. **Identify page boundaries** - what should be separate .cshtml pages
2. **Extract reusable components** to `src/components/`
3. **Create entry points** for each page type
4. **Update routing** to use ASP.NET Core controllers
5. **Test integration** between .NET and Vue parts

## Forbidden Patterns

❌ **Don't do:**
- Create single large SPA that replaces entire .NET app
- Use Vue Router for main navigation between features
- Include Vue components directly in Layout.cshtml
- Use inline styles in .cshtml files
- Load all Vue code on every page

✅ **Do:**
- Create page-specific Vue apps
- Use ASP.NET Core routing for main navigation
- Mount Vue components in individual pages
- Use CSS classes and Tailwind utilities
- Only load necessary Vue code per page