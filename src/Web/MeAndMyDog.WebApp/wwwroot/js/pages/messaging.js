/**
 * MeAndMyDoggy Messaging System - Alpine.js Integration
 * 
 * This module provides a comprehensive messaging interface using Alpine.js
 * Integrates with the backend messaging API and SignalR for real-time communication
 */

// Global messaging application state and methods
function messagingApp() {
    return {
        // Connection and authentication
        connection: null,
        currentUserId: null,
        isConnected: false,
        
        // Data state
        conversations: [],
        filteredConversations: [],
        messages: [],
        selectedConversation: null,
        
        // UI state
        searchQuery: '',
        conversationFilter: 'all',
        newMessage: '',
        typingUsers: [],
        totalUnreadCount: 0,
        
        // Typing indicator state
        typingTimeout: null,
        lastTypingEvent: 0,
        
        // Initialize the messaging system
        async initializeMessaging() {
            console.log('Initializing messaging system...');
            
            try {
                // Get current user info
                await this.getCurrentUser();
                
                // Initialize SignalR connection
                await this.initializeSignalR();
                
                // Load conversations
                await this.loadConversations();
                
                console.log('Messaging system initialized successfully');
            } catch (error) {
                console.error('Error initializing messaging system:', error);
                this.showNotification('Error connecting to messaging system', 'error');
            }
        },
        
        // Get current user information
        async getCurrentUser() {
            try {
                const response = await fetch('/api/v1/auth/me', {
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                });
                
                if (response.ok) {
                    const result = await response.json();
                    this.currentUserId = result.data?.id || result.id;
                } else {
                    // Fallback - try to get from local storage or cookie
                    this.currentUserId = localStorage.getItem('userId') || 'current-user-id';
                }
            } catch (error) {
                console.warn('Could not get current user, using fallback:', error);
                this.currentUserId = 'current-user-id';
            }
        },
        
        // Initialize SignalR connection
        async initializeSignalR() {
            try {
                // Get JWT token for SignalR authentication
                const token = await this.getJWTTokenFromServer();
                
                if (!token) {
                    throw new Error('No JWT token available for SignalR authentication');
                }
                
                const signalRUrl = window.APP_CONFIG?.signalRUrl || 'https://localhost:63343/hubs/messaging';
                console.log('Connecting to SignalR at:', signalRUrl);
                console.log('JWT token available:', token ? 'Yes' : 'No');
                
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl(signalRUrl, {
                        accessTokenFactory: () => {
                            console.log('SignalR requesting token...');
                            return token;
                        }
                    })
                    .withAutomaticReconnect()
                    .configureLogging(signalR.LogLevel.Debug)
                    .build();
                
                // Set up event handlers
                this.setupSignalREventHandlers();
                
                // Add connection state change handlers
                this.connection.onreconnecting(() => {
                    console.log('SignalR reconnecting...');
                    this.isConnected = false;
                });
                
                this.connection.onreconnected(() => {
                    console.log('SignalR reconnected successfully');
                    this.isConnected = true;
                });
                
                this.connection.onclose((error) => {
                    console.log('SignalR connection closed:', error);
                    this.isConnected = false;
                });
                
                // Start the connection
                console.log('Starting SignalR connection...');
                await this.connection.start();
                this.isConnected = true;
                
                console.log('SignalR connected successfully to:', signalRUrl);
            } catch (error) {
                console.error('Error connecting to SignalR:', error);
                console.error('Error details:', {
                    message: error.message,
                    stack: error.stack,
                    signalRUrl: window.APP_CONFIG?.signalRUrl || 'https://localhost:63343/hubs/messaging'
                });
                this.isConnected = false;
                
                // Show user-friendly error message
                this.showNotification('Failed to connect to messaging service. Please check your connection and try again.', 'error');
            }
        },
        
        // Set up SignalR event handlers
        setupSignalREventHandlers() {
            if (!this.connection) return;
            
            // New message received
            this.connection.on('MessageReceived', (message) => {
                console.log('Message received:', message);
                
                // Add message to current conversation if it's selected
                if (this.selectedConversation && message.conversationId === this.selectedConversation.id) {
                    this.messages.push(message);
                    this.scrollToBottom();
                    
                    // Mark as read if conversation is active
                    this.markMessageAsRead(message.id);
                }
                
                // Update conversation in list
                this.updateConversationFromMessage(message);
            });
            
            // User started typing
            this.connection.on('UserStartedTyping', (data) => {
                if (this.selectedConversation && data.conversationId === this.selectedConversation.id) {
                    if (!this.typingUsers.includes(data.userId)) {
                        this.typingUsers.push(data.userId);
                    }
                }
            });
            
            // User stopped typing
            this.connection.on('UserStoppedTyping', (data) => {
                if (this.selectedConversation && data.conversationId === this.selectedConversation.id) {
                    this.typingUsers = this.typingUsers.filter(userId => userId !== data.userId);
                }
            });
            
            // Message read receipt
            this.connection.on('MessageRead', (data) => {
                console.log('Message read:', data);
                // Update message status in current conversation
                if (this.selectedConversation && data.conversationId === this.selectedConversation.id) {
                    const message = this.messages.find(m => m.id === data.messageId);
                    if (message && message.senderId === this.currentUserId) {
                        message.status = 'Read';
                    }
                }
            });
            
            // Unread count updated
            this.connection.on('UnreadCountUpdated', (data) => {
                console.log('Unread count updated:', data);
                const conversation = this.conversations.find(c => c.id === data.conversationId);
                if (conversation) {
                    conversation.unreadCount = data.unreadCount;
                    this.calculateTotalUnreadCount();
                    this.filterConversations();
                }
            });
            
            // User status changed (online/offline)
            this.connection.on('UserStatusChanged', (data) => {
                console.log('User status changed:', data);
                // You can update user status indicators here
            });
            
            // Connection events
            this.connection.onreconnecting(() => {
                console.log('SignalR reconnecting...');
                this.isConnected = false;
            });
            
            this.connection.onreconnected(() => {
                console.log('SignalR reconnected');
                this.isConnected = true;
            });
            
            this.connection.onclose(() => {
                console.log('SignalR connection closed');
                this.isConnected = false;
            });
        },
        
        // Get JWT authentication token from server for SignalR
        async getJWTTokenFromServer() {
            try {
                console.log('Requesting JWT token from server...');
                
                const response = await fetch('/api/v1/auth/token', {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                });
                
                console.log('JWT token response status:', response.status);
                
                if (response.ok) {
                    const result = await response.json();
                    console.log('JWT token response:', result);
                    const token = result.token || result.data?.token || '';
                    
                    if (!token) {
                        console.error('JWT token not found in response:', result);
                    }
                    
                    return token;
                } else {
                    const errorText = await response.text();
                    console.error('JWT token request failed:', response.status, errorText);
                    
                    if (response.status === 401) {
                        console.error('User not authenticated - JWT token unavailable');
                        this.showNotification('Please log in again to access messaging', 'error');
                    }
                }
            } catch (error) {
                console.error('Error getting JWT token for SignalR:', error);
                this.showNotification('Authentication error. Please refresh the page and try again.', 'error');
            }
            return '';
        },
        
        // Load conversations from API
        async loadConversations() {
            try {
                const response = await fetch('/api/v1/messaging/conversations', {
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                });
                
                if (response.ok) {
                    const data = await response.json();
                    this.conversations = data.Conversations || data.conversations || [];
                    this.totalUnreadCount = data.UnreadTotal || data.unreadTotal || 0;
                    this.filterConversations();
                } else {
                    throw new Error('Failed to load conversations');
                }
            } catch (error) {
                console.error('Error loading conversations:', error);
                this.showNotification('Error loading conversations', 'error');
            }
        },
        
        // Filter conversations based on search and filter
        filterConversations() {
            let filtered = [...this.conversations];
            
            // Apply search filter
            if (this.searchQuery.trim()) {
                const query = this.searchQuery.toLowerCase();
                filtered = filtered.filter(conv => 
                    conv.title.toLowerCase().includes(query) ||
                    conv.lastMessagePreview.toLowerCase().includes(query) ||
                    conv.participants.some(p => p.userName.toLowerCase().includes(query))
                );
            }
            
            // Apply status filter
            switch (this.conversationFilter) {
                case 'unread':
                    filtered = filtered.filter(conv => conv.unreadCount > 0);
                    break;
                case 'archived':
                    filtered = filtered.filter(conv => conv.isArchived);
                    break;
                case 'all':
                default:
                    filtered = filtered.filter(conv => !conv.isArchived);
                    break;
            }
            
            // Sort by last message time, with pinned conversations at top
            filtered.sort((a, b) => {
                if (a.isPinned && !b.isPinned) return -1;
                if (!a.isPinned && b.isPinned) return 1;
                return new Date(b.lastMessageAt) - new Date(a.lastMessageAt);
            });
            
            this.filteredConversations = filtered;
        },
        
        // Search conversations
        searchConversations() {
            this.filterConversations();
        },
        
        // Select a conversation
        async selectConversation(conversation) {
            try {
                this.selectedConversation = conversation;
                this.messages = [];
                
                // Join the conversation via SignalR
                if (this.connection && this.isConnected) {
                    await this.connection.invoke('JoinConversation', conversation.id);
                }
                
                // Load messages for this conversation
                await this.loadMessages(conversation.id);
                
                // Mark conversation as read
                if (conversation.unreadCount > 0) {
                    await this.markConversationAsRead(conversation.id);
                }
                
                // Scroll to bottom
                this.scrollToBottom();
            } catch (error) {
                console.error('Error selecting conversation:', error);
                this.showNotification('Error loading conversation', 'error');
            }
        },
        
        // Load messages for a conversation
        async loadMessages(conversationId) {
            try {
                const response = await fetch(`/api/v1/messaging/conversations/${conversationId}/messages`, {
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                });
                
                if (response.ok) {
                    const data = await response.json();
                    this.messages = data.messages || [];
                } else {
                    throw new Error('Failed to load messages');
                }
            } catch (error) {
                console.error('Error loading messages:', error);
                this.showNotification('Error loading messages', 'error');
            }
        },
        
        // Send a message
        async sendMessage() {
            if (!this.newMessage.trim() || !this.selectedConversation) return;
            
            try {
                const messageContent = this.newMessage.trim();
                this.newMessage = '';
                
                // Send via SignalR for real-time delivery
                if (this.connection && this.isConnected) {
                    await this.connection.invoke('SendMessage', this.selectedConversation.id, messageContent, 'Text', null);
                } else {
                    // Fallback to REST API
                    const response = await fetch('/api/v1/messaging/send', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        credentials: 'include',
                        body: JSON.stringify({
                            conversationId: this.selectedConversation.id,
                            content: messageContent,
                            messageType: 'Text'
                        })
                    });
                    
                    if (!response.ok) {
                        throw new Error('Failed to send message');
                    }
                    
                    // Reload messages to show the sent message
                    await this.loadMessages(this.selectedConversation.id);
                }
                
                // Stop typing indicator
                this.stopTyping();
            } catch (error) {
                console.error('Error sending message:', error);
                this.showNotification('Error sending message', 'error');
                this.newMessage = messageContent; // Restore message content
            }
        },
        
        // Handle typing indicator
        handleTyping() {
            if (!this.selectedConversation || !this.connection || !this.isConnected) return;
            
            const now = Date.now();
            
            // Start typing if not already indicated
            if (now - this.lastTypingEvent > 3000) {
                this.connection.invoke('TypingIndicator', this.selectedConversation.id, true);
                this.lastTypingEvent = now;
            }
            
            // Reset the stop typing timeout
            clearTimeout(this.typingTimeout);
            this.typingTimeout = setTimeout(() => {
                this.stopTyping();
            }, 2000);
        },
        
        // Stop typing indicator
        stopTyping() {
            if (!this.selectedConversation || !this.connection || !this.isConnected) return;
            
            clearTimeout(this.typingTimeout);
            this.connection.invoke('TypingIndicator', this.selectedConversation.id, false);
            this.lastTypingEvent = 0;
        },
        
        // Mark message as read
        async markMessageAsRead(messageId) {
            if (!this.connection || !this.isConnected) return;
            
            try {
                await this.connection.invoke('MarkAsRead', this.selectedConversation.id, messageId);
            } catch (error) {
                console.error('Error marking message as read:', error);
            }
        },
        
        // Mark conversation as read
        async markConversationAsRead(conversationId) {
            const conversation = this.conversations.find(c => c.id === conversationId);
            if (conversation && conversation.unreadCount > 0) {
                conversation.unreadCount = 0;
                this.calculateTotalUnreadCount();
                this.filterConversations();
            }
        },
        
        // Toggle message reaction
        async toggleReaction(messageId, reaction) {
            try {
                const response = await fetch(`/api/v1/messaging/messages/${messageId}/reactions`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify({ reaction })
                });
                
                if (response.ok) {
                    // Reload messages to show updated reactions
                    await this.loadMessages(this.selectedConversation.id);
                } else {
                    throw new Error('Failed to toggle reaction');
                }
            } catch (error) {
                console.error('Error toggling reaction:', error);
                this.showNotification('Error updating reaction', 'error');
            }
        },
        
        // Handle file upload
        async handleFileUpload(event) {
            const files = event.target.files;
            if (!files.length || !this.selectedConversation) return;
            
            try {
                for (const file of files) {
                    // Validate file size (10MB limit)
                    if (file.size > 10 * 1024 * 1024) {
                        this.showNotification('File size must be less than 10MB', 'error');
                        continue;
                    }
                    
                    // Create form data
                    const formData = new FormData();
                    formData.append('file', file);
                    formData.append('conversationId', this.selectedConversation.id);
                    
                    // Upload file
                    const response = await fetch('/api/v1/messaging/upload', {
                        method: 'POST',
                        credentials: 'include',
                        body: formData
                    });
                    
                    if (!response.ok) {
                        throw new Error('Failed to upload file');
                    }
                }
                
                // Clear the file input
                event.target.value = '';
                
                this.showNotification('File(s) uploaded successfully', 'success');
            } catch (error) {
                console.error('Error uploading file:', error);
                this.showNotification('Error uploading file', 'error');
            }
        },
        
        // Conversation actions
        async togglePin() {
            if (!this.selectedConversation) return;
            
            try {
                const response = await fetch(`/api/v1/messaging/conversations/${this.selectedConversation.id}/pin`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify({ isPinned: !this.selectedConversation.isPinned })
                });
                
                if (response.ok) {
                    this.selectedConversation.isPinned = !this.selectedConversation.isPinned;
                    this.filterConversations();
                    this.showNotification(`Conversation ${this.selectedConversation.isPinned ? 'pinned' : 'unpinned'}`, 'success');
                }
            } catch (error) {
                console.error('Error toggling pin:', error);
                this.showNotification('Error updating conversation', 'error');
            }
        },
        
        async toggleMute() {
            if (!this.selectedConversation) return;
            
            try {
                const response = await fetch(`/api/v1/messaging/conversations/${this.selectedConversation.id}/mute`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify({ isMuted: !this.selectedConversation.isMuted })
                });
                
                if (response.ok) {
                    this.selectedConversation.isMuted = !this.selectedConversation.isMuted;
                    this.showNotification(`Conversation ${this.selectedConversation.isMuted ? 'muted' : 'unmuted'}`, 'success');
                }
            } catch (error) {
                console.error('Error toggling mute:', error);
                this.showNotification('Error updating conversation', 'error');
            }
        },
        
        async toggleArchive() {
            if (!this.selectedConversation) return;
            
            try {
                const response = await fetch(`/api/v1/messaging/conversations/${this.selectedConversation.id}/archive`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify({ isArchived: !this.selectedConversation.isArchived })
                });
                
                if (response.ok) {
                    this.selectedConversation.isArchived = !this.selectedConversation.isArchived;
                    this.filterConversations();
                    this.showNotification(`Conversation ${this.selectedConversation.isArchived ? 'archived' : 'unarchived'}`, 'success');
                    
                    // If archived, deselect the conversation
                    if (this.selectedConversation.isArchived) {
                        this.selectedConversation = null;
                    }
                }
            } catch (error) {
                console.error('Error toggling archive:', error);
                this.showNotification('Error updating conversation', 'error');
            }
        },
        
        // New conversation functionality removed - conversations initiated from user search only
        
        // Utility functions
        updateConversationFromMessage(message) {
            const conversation = this.conversations.find(c => c.id === message.conversationId);
            if (conversation) {
                conversation.lastMessageAt = message.createdAt;
                conversation.lastMessagePreview = message.content;
                
                // Update unread count if not current conversation
                if (!this.selectedConversation || this.selectedConversation.id !== message.conversationId) {
                    conversation.unreadCount++;
                }
                
                this.calculateTotalUnreadCount();
                this.filterConversations();
            }
        },
        
        calculateTotalUnreadCount() {
            this.totalUnreadCount = this.conversations.reduce((total, conv) => total + conv.unreadCount, 0);
        },
        
        scrollToBottom() {
            this.$nextTick(() => {
                const container = this.$refs.messagesContainer;
                if (container) {
                    container.scrollTop = container.scrollHeight;
                }
            });
        },
        
        getTypingText() {
            if (this.typingUsers.length === 0) return '';
            if (this.typingUsers.length === 1) return 'is typing...';
            if (this.typingUsers.length === 2) return 'are typing...';
            return `${this.typingUsers.length} people are typing...`;
        },
        
        formatTime(dateString) {
            const date = new Date(dateString);
            const now = new Date();
            const diff = now - date;
            
            // Less than 1 minute
            if (diff < 60000) return 'Just now';
            
            // Less than 1 hour
            if (diff < 3600000) return `${Math.floor(diff / 60000)}m ago`;
            
            // Same day
            if (date.toDateString() === now.toDateString()) {
                return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
            }
            
            // Yesterday
            const yesterday = new Date(now);
            yesterday.setDate(now.getDate() - 1);
            if (date.toDateString() === yesterday.toDateString()) {
                return 'Yesterday';
            }
            
            // This week
            if (diff < 604800000) {
                return date.toLocaleDateString([], { weekday: 'short' });
            }
            
            // Older
            return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
        },
        
        formatFileSize(bytes) {
            if (bytes === 0) return '0 B';
            const k = 1024;
            const sizes = ['B', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
        },
        
        getAttachmentIcon(attachmentType) {
            switch (attachmentType) {
                case 'Image': return 'fas fa-image';
                case 'Video': return 'fas fa-video';
                case 'Audio': return 'fas fa-volume-up';
                case 'Document': return 'fas fa-file-alt';
                case 'Location': return 'fas fa-map-marker-alt';
                default: return 'fas fa-paperclip';
            }
        },
        
        getMessageStatusIcon(status) {
            switch (status) {
                case 'Sent': return 'fas fa-check text-gray-400';
                case 'Delivered': return 'fas fa-check-double text-gray-400';
                case 'Read': return 'fas fa-check-double text-blue-500';
                case 'Failed': return 'fas fa-exclamation-triangle text-red-500';
                default: return 'fas fa-clock text-gray-400';
            }
        },
        
        showNotification(message, type = 'info') {
            // You can integrate with a toast notification library here
            console.log(`[${type.toUpperCase()}] ${message}`);
            
            // Simple alert fallback
            if (type === 'error') {
                alert(message);
            }
        }
    };
}