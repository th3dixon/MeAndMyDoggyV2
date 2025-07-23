// Auth Login Page Initialization
// Pass the return URL to the login form
if (typeof window !== 'undefined') {
    // The return URL will be set by the server in a data attribute or meta tag
    const returnUrlMeta = document.querySelector('meta[name="return-url"]');
    if (returnUrlMeta) {
        window.returnUrl = returnUrlMeta.content;
    }
}