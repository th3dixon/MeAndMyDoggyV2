/**
 * MeAndMyDoggy Service Worker
 * Handles push notifications and offline functionality for messaging
 */

const CACHE_NAME = 'meandmydoggy-messaging-v1.0';
const urlsToCache = [
    '/',
    '/css/site.css',
    '/js/pages/messaging.js',
    '/Messages',
    '/favicon.ico',
    '/android-chrome-192x192.png',
    '/android-chrome-512x512.png'
];

// Install service worker
self.addEventListener('install', (event) => {
    console.log('Service Worker: Installing...');
    
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then((cache) => {
                console.log('Service Worker: Caching files');
                return cache.addAll(urlsToCache);
            })
            .then(() => {
                console.log('Service Worker: Installed successfully');
                self.skipWaiting();
            })
    );
});

// Activate service worker
self.addEventListener('activate', (event) => {
    console.log('Service Worker: Activating...');
    
    event.waitUntil(
        caches.keys().then((cacheNames) => {
            return Promise.all(
                cacheNames.map((cacheName) => {
                    if (cacheName !== CACHE_NAME) {
                        console.log('Service Worker: Deleting old cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        }).then(() => {
            console.log('Service Worker: Activated successfully');
            self.clients.claim();
        })
    );
});

// Fetch event - serve cached content when offline
self.addEventListener('fetch', (event) => {
    // Only handle GET requests
    if (event.request.method !== 'GET') {
        return;
    }
    
    // Skip cross-origin requests
    if (!event.request.url.startsWith(self.location.origin)) {
        return;
    }
    
    event.respondWith(
        caches.match(event.request)
            .then((response) => {
                // Return cached version or fetch from network
                return response || fetch(event.request).catch(() => {
                    // If both cache and network fail, return offline page for navigation requests
                    if (event.request.mode === 'navigate') {
                        return caches.match('/');
                    }
                });
            })
    );
});

// Push event - handle push notifications
self.addEventListener('push', (event) => {
    console.log('Service Worker: Push notification received', event);
    
    let notificationData = {
        title: 'New Message',
        body: 'You have a new message',
        icon: '/android-chrome-192x192.png',
        badge: '/android-chrome-192x192.png',
        tag: 'message-notification',
        requireInteraction: true,
        actions: [
            {
                action: 'view',
                title: 'View Message',
                icon: '/android-chrome-192x192.png'
            },
            {
                action: 'dismiss',
                title: 'Dismiss'
            }
        ],
        data: {
            url: '/Messages',
            conversationId: null,
            messageId: null
        }
    };
    
    // Parse push payload
    if (event.data) {
        try {
            const payload = event.data.json();
            notificationData = {
                ...notificationData,
                ...payload,
                data: {
                    ...notificationData.data,
                    ...payload.data
                }
            };
        } catch (error) {
            console.error('Service Worker: Error parsing push payload:', error);
        }
    }
    
    event.waitUntil(
        self.registration.showNotification(notificationData.title, notificationData)
    );
});

// Notification click event
self.addEventListener('notificationclick', (event) => {
    console.log('Service Worker: Notification clicked', event);
    
    event.notification.close();
    
    const action = event.action;
    const notificationData = event.notification.data || {};
    
    if (action === 'dismiss') {
        return;
    }
    
    // Default action or 'view' action
    const urlToOpen = notificationData.url || '/Messages';
    
    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clientList) => {
            // Check if a window is already open
            for (const client of clientList) {
                if (client.url.includes('/Messages') && 'focus' in client) {
                    // Focus existing window and navigate to specific message if needed
                    if (notificationData.conversationId) {
                        client.postMessage({
                            type: 'NAVIGATE_TO_CONVERSATION',
                            conversationId: notificationData.conversationId,
                            messageId: notificationData.messageId
                        });
                    }
                    return client.focus();
                }
            }
            
            // No existing window found, open a new one
            if (clients.openWindow) {
                return clients.openWindow(urlToOpen);
            }
        })
    );
});

// Message event - handle messages from main thread
self.addEventListener('message', (event) => {
    console.log('Service Worker: Message received from main thread', event.data);
    
    const { type, data } = event.data;
    
    switch (type) {
        case 'SKIP_WAITING':
            self.skipWaiting();
            break;
            
        case 'CACHE_MESSAGES':
            // Cache offline messages for later sync
            event.waitUntil(
                caches.open('meandmydoggy-offline-messages').then((cache) => {
                    return cache.put('offline-messages', new Response(JSON.stringify(data.messages)));
                })
            );
            break;
            
        case 'GET_CACHED_MESSAGES':
            // Retrieve cached offline messages
            event.waitUntil(
                caches.open('meandmydoggy-offline-messages').then((cache) => {
                    return cache.match('offline-messages').then((response) => {
                        if (response) {
                            return response.json().then((messages) => {
                                event.ports[0].postMessage({ messages });
                            });
                        } else {
                            event.ports[0].postMessage({ messages: [] });
                        }
                    });
                })
            );
            break;
            
        default:
            console.log('Service Worker: Unknown message type:', type);
    }
});

// Background sync for offline message queue
self.addEventListener('sync', (event) => {
    console.log('Service Worker: Background sync triggered', event.tag);
    
    if (event.tag === 'sync-offline-messages') {
        event.waitUntil(syncOfflineMessages());
    }
});

async function syncOfflineMessages() {
    try {
        console.log('Service Worker: Syncing offline messages...');
        
        const cache = await caches.open('meandmydoggy-offline-messages');
        const response = await cache.match('offline-messages');
        
        if (!response) {
            console.log('Service Worker: No offline messages to sync');
            return;
        }
        
        const messages = await response.json();
        
        if (messages.length === 0) {
            console.log('Service Worker: No offline messages to sync');
            return;
        }
        
        // Attempt to send each offline message
        const successfulMessages = [];
        
        for (const message of messages) {
            try {
                const syncResponse = await fetch('/api/v1/messages', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify(message)
                });
                
                if (syncResponse.ok) {
                    successfulMessages.push(message.tempId);
                    console.log('Service Worker: Successfully synced message:', message.tempId);
                } else {
                    console.error('Service Worker: Failed to sync message:', message.tempId, syncResponse.status);
                }
            } catch (error) {
                console.error('Service Worker: Error syncing message:', message.tempId, error);
            }
        }
        
        // Remove successfully synced messages from cache
        if (successfulMessages.length > 0) {
            const remainingMessages = messages.filter(msg => !successfulMessages.includes(msg.tempId));
            await cache.put('offline-messages', new Response(JSON.stringify(remainingMessages)));
            
            console.log(`Service Worker: Synced ${successfulMessages.length} messages, ${remainingMessages.length} remaining`);
        }
        
        // Notify main thread about sync completion
        const clients = await self.clients.matchAll();
        clients.forEach(client => {
            client.postMessage({
                type: 'OFFLINE_SYNC_COMPLETE',
                syncedCount: successfulMessages.length,
                remainingCount: messages.length - successfulMessages.length
            });
        });
        
    } catch (error) {
        console.error('Service Worker: Error during offline message sync:', error);
    }
}

// Periodic background sync (if supported)
self.addEventListener('periodicsync', (event) => {
    console.log('Service Worker: Periodic sync triggered', event.tag);
    
    if (event.tag === 'sync-messages-periodic') {
        event.waitUntil(syncOfflineMessages());
    }
});

console.log('Service Worker: Script loaded successfully');