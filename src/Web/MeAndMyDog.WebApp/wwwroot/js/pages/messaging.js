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
        
        // Video call state
        videoCall: {
            isActive: false,
            isConnected: false,
            status: 'idle', // idle, calling, ringing, connecting, connected
            duration: '00:00',
            isMuted: false,
            isCameraOff: false,
            isScreenSharing: false,
            localStream: null,
            remoteStream: null,
            peerConnection: null,
            callStartTime: null,
            durationInterval: null
        },
        
        // Voice recording state
        voiceRecording: {
            isActive: false,
            isRecording: false,
            isPlaying: false,
            hasRecording: false,
            duration: '00:00',
            mediaRecorder: null,
            audioChunks: [],
            recordedBlob: null,
            audioContext: null,
            analyser: null,
            animationFrame: null,
            startTime: null,
            durationInterval: null
        },
        
        // Message search state
        messageSearch: {
            isOpen: false,
            query: '',
            dateFilter: '',
            senderFilter: '',
            attachmentsOnly: false,
            results: [],
            isLoading: false
        },
        
        // Emoji picker state
        emojiPicker: {
            isOpen: false,
            selectedMessageId: null,
            emojis: ['😀', '😂', '😍', '😘', '😊', '😎', '🤔', '😢', '😭', '😡', '😴', '🤗', '👍', '👎', '❤️', '💔', '🔥', '💯', '🎉', '🎊', '👏', '🙌', '🤝', '🙏', '💪', '🎯', '⭐', '🌟', '✨', '⚡', '🌈', '🦴']
        },
        
        // Push notification state
        pushNotifications: {
            permission: 'default',
            isSupported: 'serviceWorker' in navigator && 'PushManager' in window,
            registration: null
        },
        
        // Image modal state
        imageModal: {
            isOpen: false,
            imageUrl: '',
            fileName: '',
            fileSize: 0,
            attachment: null
        },
        
        // Location modal state
        locationModal: {
            isOpen: false,
            coordinates: '',
            attachment: null
        },
        
        // Message threading state
        replyingTo: null,
        
        // Mobile gesture state
        touch: {
            startX: 0,
            startY: 0,
            currentX: 0,
            currentY: 0,
            startTime: 0,
            longPressTimer: null,
            isLongPress: false,
            swipeThreshold: 50,
            longPressDelay: 500
        },
        
        // Message context menu state
        messageMenu: {
            isOpen: false,
            message: null,
            position: { x: 0, y: 0 }
        },
        
        // Bulk operations state
        selectedMessages: [],
        isSelectionMode: false,
        
        // Pagination state
        currentPage: 1,
        messagesPerPage: 50,
        canLoadMore: true,
        isLoadingMessages: false,
        
        // Initialize the messaging system
        async initializeMessaging() {
            
            try {
                // Get current user info
                await this.getCurrentUser();
                
                // Initialize SignalR connection
                await this.initializeSignalR();
                
                // Load conversations
                await this.loadConversations();
                
                // Initialize push notifications
                await this.initializePushNotifications();
                
                // Set up offline/online event listeners
                this.setupOfflineHandling();
                
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
                
                this.connection = new signalR.HubConnectionBuilder()
                    .withUrl(signalRUrl, {
                        accessTokenFactory: () => {
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
                    this.isConnected = false;
                });
                
                this.connection.onreconnected(() => {
                    this.isConnected = true;
                });
                
                this.connection.onclose((error) => {
                    this.isConnected = false;
                });
                
                // Start the connection
                await this.connection.start();
                this.isConnected = true;
                
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
                const conversation = this.conversations.find(c => c.id === data.conversationId);
                if (conversation) {
                    conversation.unreadCount = data.unreadCount;
                    this.calculateTotalUnreadCount();
                    this.filterConversations();
                }
            });
            
            // User status changed (online/offline)
            this.connection.on('UserStatusChanged', (data) => {
                // You can update user status indicators here
            });
            
            // Connection events
            this.connection.onreconnecting(() => {
                this.isConnected = false;
            });
            
            this.connection.onreconnected(() => {
                this.isConnected = true;
            });
            
            this.connection.onclose(() => {
                this.isConnected = false;
            });
        },
        
        // Get JWT authentication token from server for SignalR
        async getJWTTokenFromServer() {
            try {
                
                const response = await fetch('/api/v1/auth/token', {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include'
                });
                
                
                if (response.ok) {
                    const result = await response.json();
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
            
            // Simple alert fallback
            if (type === 'error') {
                alert(message);
            }
        },

        // =============================================================================
        // VIDEO CALLING FUNCTIONALITY
        // =============================================================================

        async initiateVideoCall() {
            if (!this.selectedConversation) return;
            
            try {
                this.videoCall.isActive = true;
                this.videoCall.status = 'calling';
                
                // Get user media
                this.videoCall.localStream = await navigator.mediaDevices.getUserMedia({
                    video: true,
                    audio: true
                });
                
                // Display local video
                this.$refs.localVideo.srcObject = this.videoCall.localStream;
                
                // Initialize WebRTC peer connection
                await this.initializePeerConnection();
                
                // Add local stream to peer connection
                this.videoCall.localStream.getTracks().forEach(track => {
                    this.videoCall.peerConnection.addTrack(track, this.videoCall.localStream);
                });
                
                // Create offer
                const offer = await this.videoCall.peerConnection.createOffer();
                await this.videoCall.peerConnection.setLocalDescription(offer);
                
                // Send call invitation via SignalR
                if (this.connection && this.isConnected) {
                    await this.connection.invoke('InitiateVideoCall', {
                        conversationId: this.selectedConversation.id,
                        offer: offer
                    });
                }
                
                this.startCallDurationTimer();
                
            } catch (error) {
                console.error('Error initiating video call:', error);
                this.showNotification('Failed to start video call. Please check your camera and microphone permissions.', 'error');
                this.endVideoCall();
            }
        },

        async initiateVoiceCall() {
            if (!this.selectedConversation) return;
            
            try {
                this.videoCall.isActive = true;
                this.videoCall.status = 'calling';
                this.videoCall.isCameraOff = true; // Voice call - camera is off
                
                // Get audio only
                this.videoCall.localStream = await navigator.mediaDevices.getUserMedia({
                    video: false,
                    audio: true
                });
                
                await this.initializePeerConnection();
                
                this.videoCall.localStream.getTracks().forEach(track => {
                    this.videoCall.peerConnection.addTrack(track, this.videoCall.localStream);
                });
                
                const offer = await this.videoCall.peerConnection.createOffer();
                await this.videoCall.peerConnection.setLocalDescription(offer);
                
                if (this.connection && this.isConnected) {
                    await this.connection.invoke('InitiateVoiceCall', {
                        conversationId: this.selectedConversation.id,
                        offer: offer
                    });
                }
                
                this.startCallDurationTimer();
                
            } catch (error) {
                console.error('Error initiating voice call:', error);
                this.showNotification('Failed to start voice call. Please check your microphone permissions.', 'error');
                this.endVideoCall();
            }
        },

        async acceptVideoCall() {
            try {
                this.videoCall.status = 'connecting';
                
                // Get user media
                this.videoCall.localStream = await navigator.mediaDevices.getUserMedia({
                    video: true,
                    audio: true
                });
                
                this.$refs.localVideo.srcObject = this.videoCall.localStream;
                
                this.videoCall.localStream.getTracks().forEach(track => {
                    this.videoCall.peerConnection.addTrack(track, this.videoCall.localStream);
                });
                
                // Create answer
                const answer = await this.videoCall.peerConnection.createAnswer();
                await this.videoCall.peerConnection.setLocalDescription(answer);
                
                if (this.connection && this.isConnected) {
                    await this.connection.invoke('AcceptVideoCall', {
                        conversationId: this.selectedConversation.id,
                        answer: answer
                    });
                }
                
                this.startCallDurationTimer();
                
            } catch (error) {
                console.error('Error accepting video call:', error);
                this.showNotification('Failed to accept video call.', 'error');
                this.endVideoCall();
            }
        },

        async declineVideoCall() {
            if (this.connection && this.isConnected) {
                await this.connection.invoke('DeclineVideoCall', {
                    conversationId: this.selectedConversation.id
                });
            }
            this.endVideoCall();
        },

        async initializePeerConnection() {
            // STUN servers for NAT traversal
            const configuration = {
                iceServers: [
                    { urls: 'stun:stun.l.google.com:19302' },
                    { urls: 'stun:stun1.l.google.com:19302' }
                ]
            };
            
            this.videoCall.peerConnection = new RTCPeerConnection(configuration);
            
            // Handle remote stream
            this.videoCall.peerConnection.ontrack = (event) => {
                this.videoCall.remoteStream = event.streams[0];
                this.$refs.remoteVideo.srcObject = this.videoCall.remoteStream;
                this.videoCall.isConnected = true;
                this.videoCall.status = 'connected';
            };
            
            // Handle ICE candidates
            this.videoCall.peerConnection.onicecandidate = async (event) => {
                if (event.candidate && this.connection && this.isConnected) {
                    await this.connection.invoke('SendIceCandidate', {
                        conversationId: this.selectedConversation.id,
                        candidate: event.candidate
                    });
                }
            };
            
            // Handle connection state changes
            this.videoCall.peerConnection.onconnectionstatechange = () => {
                if (this.videoCall.peerConnection.connectionState === 'disconnected' || 
                    this.videoCall.peerConnection.connectionState === 'failed') {
                    this.endVideoCall();
                }
            };
        },

        toggleMicrophone() {
            if (this.videoCall.localStream) {
                const audioTrack = this.videoCall.localStream.getAudioTracks()[0];
                if (audioTrack) {
                    audioTrack.enabled = !audioTrack.enabled;
                    this.videoCall.isMuted = !audioTrack.enabled;
                }
            }
        },

        toggleCamera() {
            if (this.videoCall.localStream) {
                const videoTrack = this.videoCall.localStream.getVideoTracks()[0];
                if (videoTrack) {
                    videoTrack.enabled = !videoTrack.enabled;
                    this.videoCall.isCameraOff = !videoTrack.enabled;
                }
            }
        },

        async toggleScreenShare() {
            try {
                if (this.videoCall.isScreenSharing) {
                    // Stop screen sharing, return to camera
                    const videoTrack = this.videoCall.localStream.getVideoTracks()[0];
                    const newStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
                    const newVideoTrack = newStream.getVideoTracks()[0];
                    
                    const sender = this.videoCall.peerConnection.getSenders().find(s => 
                        s.track && s.track.kind === 'video'
                    );
                    
                    if (sender) {
                        await sender.replaceTrack(newVideoTrack);
                    }
                    
                    videoTrack.stop();
                    this.videoCall.localStream = newStream;
                    this.$refs.localVideo.srcObject = newStream;
                    this.videoCall.isScreenSharing = false;
                } else {
                    // Start screen sharing
                    const screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
                    const screenTrack = screenStream.getVideoTracks()[0];
                    
                    const sender = this.videoCall.peerConnection.getSenders().find(s => 
                        s.track && s.track.kind === 'video'
                    );
                    
                    if (sender) {
                        await sender.replaceTrack(screenTrack);
                    }
                    
                    // Stop screen sharing when user ends it via browser UI
                    screenTrack.onended = () => {
                        this.toggleScreenShare();
                    };
                    
                    this.videoCall.isScreenSharing = true;
                }
            } catch (error) {
                console.error('Error toggling screen share:', error);
                this.showNotification('Screen sharing not available.', 'error');
            }
        },

        adjustCallQuality(quality) {
            // Adjust video bitrate based on quality setting
            const sender = this.videoCall.peerConnection.getSenders().find(s => 
                s.track && s.track.kind === 'video'
            );
            
            if (sender) {
                const params = sender.getParameters();
                if (params.encodings) {
                    switch (quality) {
                        case 'high':
                            params.encodings[0].maxBitrate = 1000000; // 1 Mbps
                            break;
                        case 'medium':
                            params.encodings[0].maxBitrate = 500000; // 500 kbps
                            break;
                        case 'low':
                            params.encodings[0].maxBitrate = 200000; // 200 kbps
                            break;
                    }
                    sender.setParameters(params);
                }
            }
        },

        startCallDurationTimer() {
            this.videoCall.callStartTime = Date.now();
            this.videoCall.durationInterval = setInterval(() => {
                const elapsed = Date.now() - this.videoCall.callStartTime;
                const minutes = Math.floor(elapsed / 60000);
                const seconds = Math.floor((elapsed % 60000) / 1000);
                this.videoCall.duration = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
            }, 1000);
        },

        endVideoCall() {
            // Stop duration timer
            if (this.videoCall.durationInterval) {
                clearInterval(this.videoCall.durationInterval);
                this.videoCall.durationInterval = null;
            }
            
            // Stop local stream
            if (this.videoCall.localStream) {
                this.videoCall.localStream.getTracks().forEach(track => track.stop());
                this.videoCall.localStream = null;
            }
            
            // Close peer connection
            if (this.videoCall.peerConnection) {
                this.videoCall.peerConnection.close();
                this.videoCall.peerConnection = null;
            }
            
            // Reset video call state
            this.videoCall = {
                isActive: false,
                isConnected: false,
                status: 'idle',
                duration: '00:00',
                isMuted: false,
                isCameraOff: false,
                isScreenSharing: false,
                localStream: null,
                remoteStream: null,
                peerConnection: null,
                callStartTime: null,
                durationInterval: null
            };
            
            // Notify other participants
            if (this.connection && this.isConnected && this.selectedConversation) {
                this.connection.invoke('EndVideoCall', {
                    conversationId: this.selectedConversation.id
                });
            }
        },

        // =============================================================================
        // VOICE MESSAGE FUNCTIONALITY
        // =============================================================================

        openVoiceRecording() {
            this.voiceRecording.isActive = true;
        },

        async startVoiceRecording() {
            try {
                const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
                
                this.voiceRecording.mediaRecorder = new MediaRecorder(stream);
                this.voiceRecording.audioChunks = [];
                this.voiceRecording.isRecording = true;
                this.voiceRecording.startTime = Date.now();
                
                // Set up audio visualization
                this.setupWaveformVisualization(stream);
                
                this.voiceRecording.mediaRecorder.ondataavailable = (event) => {
                    this.voiceRecording.audioChunks.push(event.data);
                };
                
                this.voiceRecording.mediaRecorder.onstop = () => {
                    this.voiceRecording.recordedBlob = new Blob(this.voiceRecording.audioChunks, { type: 'audio/wav' });
                    this.voiceRecording.hasRecording = true;
                    this.voiceRecording.isRecording = false;
                    
                    // Stop all tracks
                    stream.getTracks().forEach(track => track.stop());
                };
                
                this.voiceRecording.mediaRecorder.start();
                this.startRecordingTimer();
                
            } catch (error) {
                console.error('Error starting voice recording:', error);
                this.showNotification('Failed to start recording. Please check your microphone permissions.', 'error');
            }
        },

        stopVoiceRecording() {
            if (this.voiceRecording.mediaRecorder && this.voiceRecording.isRecording) {
                this.voiceRecording.mediaRecorder.stop();
                this.stopRecordingTimer();
                this.stopWaveformVisualization();
            }
        },

        async playVoiceRecording() {
            if (this.voiceRecording.recordedBlob) {
                const audio = new Audio(URL.createObjectURL(this.voiceRecording.recordedBlob));
                
                if (this.voiceRecording.isPlaying) {
                    audio.pause();
                    this.voiceRecording.isPlaying = false;
                } else {
                    audio.play();
                    this.voiceRecording.isPlaying = true;
                    
                    audio.onended = () => {
                        this.voiceRecording.isPlaying = false;
                    };
                }
            }
        },

        async sendVoiceMessage() {
            if (!this.voiceRecording.recordedBlob || !this.selectedConversation) return;
            
            try {
                const formData = new FormData();
                formData.append('conversationId', this.selectedConversation.id);
                formData.append('voiceMessage', this.voiceRecording.recordedBlob, 'voice-message.wav');
                
                const response = await fetch('/api/v1/messages/voice', {
                    method: 'POST',
                    body: formData,
                    credentials: 'include'
                });
                
                if (response.ok) {
                    this.cancelVoiceRecording();
                    this.showNotification('Voice message sent!', 'success');
                } else {
                    throw new Error('Failed to send voice message');
                }
                
            } catch (error) {
                console.error('Error sending voice message:', error);
                this.showNotification('Failed to send voice message.', 'error');
            }
        },

        cancelVoiceRecording() {
            this.stopRecordingTimer();
            this.stopWaveformVisualization();
            
            if (this.voiceRecording.mediaRecorder && this.voiceRecording.isRecording) {
                this.voiceRecording.mediaRecorder.stop();
            }
            
            this.voiceRecording = {
                isActive: false,
                isRecording: false,
                isPlaying: false,
                hasRecording: false,
                duration: '00:00',
                mediaRecorder: null,
                audioChunks: [],
                recordedBlob: null,
                audioContext: null,
                analyser: null,
                animationFrame: null,
                startTime: null,
                durationInterval: null
            };
        },

        setupWaveformVisualization(stream) {
            const canvas = this.$refs.waveformCanvas;
            const ctx = canvas.getContext('2d');
            
            this.voiceRecording.audioContext = new AudioContext();
            this.voiceRecording.analyser = this.voiceRecording.audioContext.createAnalyser();
            const source = this.voiceRecording.audioContext.createMediaStreamSource(stream);
            
            source.connect(this.voiceRecording.analyser);
            this.voiceRecording.analyser.fftSize = 256;
            
            const bufferLength = this.voiceRecording.analyser.frequencyBinCount;
            const dataArray = new Uint8Array(bufferLength);
            
            const draw = () => {
                this.voiceRecording.animationFrame = requestAnimationFrame(draw);
                
                this.voiceRecording.analyser.getByteFrequencyData(dataArray);
                
                ctx.fillStyle = '#f3f4f6';
                ctx.fillRect(0, 0, canvas.width, canvas.height);
                
                const barWidth = (canvas.width / bufferLength) * 2.5;
                let x = 0;
                
                for (let i = 0; i < bufferLength; i++) {
                    const barHeight = (dataArray[i] / 255) * canvas.height;
                    
                    ctx.fillStyle = `rgb(249, 115, 22)`;
                    ctx.fillRect(x, canvas.height - barHeight, barWidth, barHeight);
                    
                    x += barWidth + 1;
                }
            };
            
            draw();
        },

        stopWaveformVisualization() {
            if (this.voiceRecording.animationFrame) {
                cancelAnimationFrame(this.voiceRecording.animationFrame);
                this.voiceRecording.animationFrame = null;
            }
            
            if (this.voiceRecording.audioContext) {
                this.voiceRecording.audioContext.close();
                this.voiceRecording.audioContext = null;
            }
        },

        startRecordingTimer() {
            this.voiceRecording.durationInterval = setInterval(() => {
                const elapsed = Date.now() - this.voiceRecording.startTime;
                const minutes = Math.floor(elapsed / 60000);
                const seconds = Math.floor((elapsed % 60000) / 1000);
                this.voiceRecording.duration = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
            }, 1000);
        },

        stopRecordingTimer() {
            if (this.voiceRecording.durationInterval) {
                clearInterval(this.voiceRecording.durationInterval);
                this.voiceRecording.durationInterval = null;
            }
        },

        // =============================================================================
        // MESSAGE SEARCH FUNCTIONALITY
        // =============================================================================

        openMessageSearch() {
            this.messageSearch.isOpen = true;
        },

        closeMessageSearch() {
            this.messageSearch.isOpen = false;
            this.messageSearch.query = '';
            this.messageSearch.results = [];
        },

        async searchMessages() {
            if (!this.messageSearch.query.trim()) {
                this.messageSearch.results = [];
                return;
            }
            
            this.messageSearch.isLoading = true;
            
            try {
                const params = new URLSearchParams({
                    query: this.messageSearch.query,
                    conversationId: this.selectedConversation?.id || '',
                    dateFilter: this.messageSearch.dateFilter,
                    senderFilter: this.messageSearch.senderFilter,
                    attachmentsOnly: this.messageSearch.attachmentsOnly
                });
                
                const response = await fetch(`/api/v1/messages/search?${params}`, {
                    credentials: 'include'
                });
                
                if (response.ok) {
                    const result = await response.json();
                    this.messageSearch.results = result.data || result;
                } else {
                    throw new Error('Search failed');
                }
                
            } catch (error) {
                console.error('Error searching messages:', error);
                this.showNotification('Search failed. Please try again.', 'error');
                this.messageSearch.results = [];
            } finally {
                this.messageSearch.isLoading = false;
            }
        },

        navigateToMessage(message) {
            // Switch to the conversation containing the message
            const conversation = this.conversations.find(c => c.id === message.conversationId);
            if (conversation) {
                this.selectConversation(conversation);
                this.closeMessageSearch();
                
                // Scroll to the message (you might need to load more messages first)
                setTimeout(() => {
                    const messageElement = document.querySelector(`[data-message-id="${message.id}"]`);
                    if (messageElement) {
                        messageElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        messageElement.classList.add('highlight-message');
                        setTimeout(() => messageElement.classList.remove('highlight-message'), 3000);
                    }
                }, 500);
            }
        },

        // =============================================================================
        // EMOJI PICKER FUNCTIONALITY
        // =============================================================================

        openEmojiPicker(messageId = null) {
            this.emojiPicker.isOpen = true;
            this.emojiPicker.selectedMessageId = messageId;
        },

        closeEmojiPicker() {
            this.emojiPicker.isOpen = false;
            this.emojiPicker.selectedMessageId = null;
        },

        selectEmoji(emoji) {
            if (this.emojiPicker.selectedMessageId) {
                // Add reaction to specific message
                this.toggleReaction(this.emojiPicker.selectedMessageId, emoji);
            } else {
                // Add emoji to current message input
                this.newMessage += emoji;
            }
            this.closeEmojiPicker();
        },

        // =============================================================================
        // PUSH NOTIFICATIONS FUNCTIONALITY
        // =============================================================================

        async initializePushNotifications() {
            if (!this.pushNotifications.isSupported) {
                return;
            }
            
            try {
                // Register service worker
                this.pushNotifications.registration = await navigator.serviceWorker.register('/sw.js');
                
                // Request notification permission
                this.pushNotifications.permission = await Notification.requestPermission();
                
                if (this.pushNotifications.permission === 'granted') {
                    
                    // Check if VAPID key is configured (not placeholder)
                    const vapidKey = 'YOUR_VAPID_PUBLIC_KEY'; // Replace with actual VAPID key
                    
                    if (vapidKey === 'YOUR_VAPID_PUBLIC_KEY' || !vapidKey) {
                        console.warn('Push notifications: VAPID public key not configured. Skipping push subscription.');
                        return;
                    }
                    
                    // Subscribe to push notifications
                    const subscription = await this.pushNotifications.registration.pushManager.subscribe({
                        userVisibleOnly: true,
                        applicationServerKey: this.urlBase64ToUint8Array(vapidKey)
                    });
                    
                    // Send subscription to server
                    await this.sendPushSubscriptionToServer(subscription);
                }
                
            } catch (error) {
                console.error('Error initializing push notifications:', error);
            }
        },

        async sendPushSubscriptionToServer(subscription) {
            try {
                await fetch('/api/v1/notifications/subscribe', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify({
                        subscription: subscription,
                        userId: this.currentUserId
                    })
                });
            } catch (error) {
                console.error('Error sending push subscription to server:', error);
            }
        },

        urlBase64ToUint8Array(base64String) {
            const padding = '='.repeat((4 - base64String.length % 4) % 4);
            const base64 = (base64String + padding)
                .replace(/-/g, '+')
                .replace(/_/g, '/');
                
            const rawData = window.atob(base64);
            const outputArray = new Uint8Array(rawData.length);
            
            for (let i = 0; i < rawData.length; ++i) {
                outputArray[i] = rawData.charCodeAt(i);
            }
            return outputArray;
        },

        showPushNotification(title, options) {
            if (this.pushNotifications.permission === 'granted' && document.hidden) {
                new Notification(title, {
                    icon: '/favicon.ico',
                    badge: '/android-chrome-192x192.png',
                    ...options
                });
            }
        },

        // =============================================================================
        // RICH ATTACHMENT PREVIEW FUNCTIONALITY
        // =============================================================================

        openImageModal(attachment) {
            this.imageModal = {
                isOpen: true,
                imageUrl: attachment.fileUrl,
                fileName: attachment.fileName,
                fileSize: attachment.fileSize,
                attachment: attachment
            };
        },

        closeImageModal() {
            this.imageModal = {
                isOpen: false,
                imageUrl: '',
                fileName: '',
                fileSize: 0,
                attachment: null
            };
        },

        openLocationModal(attachment) {
            this.locationModal = {
                isOpen: true,
                coordinates: attachment.coordinates || 'Unknown location',
                attachment: attachment
            };
        },

        closeLocationModal() {
            this.locationModal = {
                isOpen: false,
                coordinates: '',
                attachment: null
            };
        },

        async downloadAttachment(attachment) {
            try {
                const response = await fetch(attachment.fileUrl);
                const blob = await response.blob();
                
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = attachment.fileName;
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);
                
                this.showNotification('File downloaded successfully!', 'success');
            } catch (error) {
                console.error('Error downloading file:', error);
                this.showNotification('Failed to download file.', 'error');
            }
        },

        openInMaps() {
            if (this.locationModal.attachment && this.locationModal.attachment.coordinates) {
                const coords = this.locationModal.attachment.coordinates;
                const url = `https://www.google.com/maps?q=${coords}`;
                window.open(url, '_blank');
            }
        },

        async shareLocation() {
            if (navigator.share && this.locationModal.attachment) {
                try {
                    await navigator.share({
                        title: 'Shared Location',
                        text: 'Check out this location',
                        url: `https://www.google.com/maps?q=${this.locationModal.attachment.coordinates}`
                    });
                } catch (error) {
                    console.error('Error sharing location:', error);
                }
            } else {
                // Fallback - copy to clipboard
                if (this.locationModal.attachment && this.locationModal.attachment.coordinates) {
                    const url = `https://www.google.com/maps?q=${this.locationModal.attachment.coordinates}`;
                    navigator.clipboard.writeText(url).then(() => {
                        this.showNotification('Location link copied to clipboard!', 'success');
                    });
                }
            }
        },

        // =============================================================================
        // MESSAGE THREADING FUNCTIONALITY
        // =============================================================================

        replyToMessage(message) {
            this.replyingTo = {
                id: message.id,
                content: message.content,
                senderName: message.senderName,
                senderId: message.senderId
            };
            
            // Focus the message input
            setTimeout(() => {
                const textarea = document.querySelector('textarea[x-model="newMessage"]');
                if (textarea) {
                    textarea.focus();
                }
            }, 100);
        },

        cancelReply() {
            this.replyingTo = null;
        },

        getReplyPreview(replyToMessage) {
            if (!replyToMessage) return '';
            
            const preview = replyToMessage.content || '';
            const maxLength = 50;
            
            if (preview.length > maxLength) {
                return preview.substring(0, maxLength) + '...';
            }
            
            return preview;
        },

        // Enhanced sendMessage method with reply support
        async sendMessage() {
            if (!this.newMessage.trim() || !this.selectedConversation) return;
            
            try {
                const messageData = {
                    conversationId: this.selectedConversation.id,
                    content: this.newMessage.trim(),
                    replyToMessageId: this.replyingTo?.id || null
                };
                
                const response = await fetch('/api/v1/messages', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify(messageData)
                });
                
                if (response.ok) {
                    const result = await response.json();
                    
                    // Add the message to the local messages array immediately for responsiveness
                    const newMessage = {
                        id: result.data?.id || Date.now().toString(),
                        content: this.newMessage.trim(),
                        senderId: this.currentUserId,
                        senderName: 'You',
                        createdAt: new Date().toISOString(),
                        status: 'Sending',
                        replyTo: this.replyingTo ? {
                            id: this.replyingTo.id,
                            content: this.replyingTo.content,
                            senderName: this.replyingTo.senderName
                        } : null,
                        attachments: [],
                        reactions: []
                    };
                    
                    this.messages.push(newMessage);
                    this.newMessage = '';
                    this.replyingTo = null;
                    
                    // Scroll to bottom
                    this.scrollToBottom();
                    
                    // Update conversation in sidebar
                    if (this.selectedConversation) {
                        this.selectedConversation.lastMessagePreview = newMessage.content;
                        this.selectedConversation.lastMessageAt = newMessage.createdAt;
                    }
                } else {
                    throw new Error('Failed to send message');
                }
                
            } catch (error) {
                console.error('Error sending message:', error);
                this.showNotification('Failed to send message. Please try again.', 'error');
            }
        },

        // =============================================================================
        // MOBILE GESTURE SUPPORT
        // =============================================================================

        handleTouchStart(event, message) {
            this.touch.startX = event.touches[0].clientX;
            this.touch.startY = event.touches[0].clientY;
            this.touch.currentX = this.touch.startX;
            this.touch.currentY = this.touch.startY;
            this.touch.startTime = Date.now();
            this.touch.isLongPress = false;
            
            // Start long press timer
            this.touch.longPressTimer = setTimeout(() => {
                this.touch.isLongPress = true;
                this.showMessageMenu(event, message);
                
                // Haptic feedback if supported
                if (navigator.vibrate) {
                    navigator.vibrate(50);
                }
            }, this.touch.longPressDelay);
        },

        handleTouchMove(event, message) {
            if (!this.touch.longPressTimer) return;
            
            this.touch.currentX = event.touches[0].clientX;
            this.touch.currentY = event.touches[0].clientY;
            
            const deltaX = this.touch.currentX - this.touch.startX;
            const deltaY = this.touch.currentY - this.touch.startY;
            
            // Cancel long press if user moves too much
            if (Math.abs(deltaX) > 10 || Math.abs(deltaY) > 10) {
                clearTimeout(this.touch.longPressTimer);
                this.touch.longPressTimer = null;
            }
            
            // Handle swipe gesture
            if (Math.abs(deltaX) > this.touch.swipeThreshold && Math.abs(deltaY) < 50) {
                const messageElement = event.currentTarget;
                
                if (deltaX > 0) {
                    // Swipe right - reply action
                    messageElement.style.transform = `translateX(${Math.min(deltaX, 100)}px)`;
                    messageElement.classList.add('swiping');
                } else {
                    // Swipe left - could be for other actions
                    messageElement.style.transform = `translateX(${Math.max(deltaX, -100)}px)`;
                }
            }
        },

        handleTouchEnd(event, message) {
            if (this.touch.longPressTimer) {
                clearTimeout(this.touch.longPressTimer);
                this.touch.longPressTimer = null;
            }
            
            if (this.touch.isLongPress) {
                // Long press was handled in touchstart
                return;
            }
            
            const deltaX = this.touch.currentX - this.touch.startX;
            const deltaY = this.touch.currentY - this.touch.startY;
            const deltaTime = Date.now() - this.touch.startTime;
            
            const messageElement = event.currentTarget;
            
            // Reset transform
            messageElement.style.transform = '';
            messageElement.classList.remove('swiping');
            
            // Handle swipe gestures
            if (Math.abs(deltaX) > this.touch.swipeThreshold && Math.abs(deltaY) < 50 && deltaTime < 300) {
                if (deltaX > 0) {
                    // Swipe right - reply
                    this.replyToMessage(message);
                    
                    // Visual feedback
                    this.showNotification('Reply started', 'info');
                } else if (deltaX < -this.touch.swipeThreshold) {
                    // Swipe left - could be for marking as read or other actions
                    this.showNotification('Swipe left action', 'info');
                }
            }
            
            // Reset touch state
            this.touch = {
                startX: 0,
                startY: 0,
                currentX: 0,
                currentY: 0,
                startTime: 0,
                longPressTimer: null,
                isLongPress: false,
                swipeThreshold: 50,
                longPressDelay: 500
            };
        },

        showMessageMenu(event, message) {
            this.messageMenu = {
                isOpen: true,
                message: message,
                position: {
                    x: event.clientX || (event.touches && event.touches[0].clientX) || 0,
                    y: event.clientY || (event.touches && event.touches[0].clientY) || 0
                }
            };
        },

        closeMessageMenu() {
            this.messageMenu = {
                isOpen: false,
                message: null,
                position: { x: 0, y: 0 }
            };
        },

        // =============================================================================
        // MESSAGE OPERATIONS
        // =============================================================================

        copyMessage(message) {
            if (navigator.clipboard && message.content) {
                navigator.clipboard.writeText(message.content).then(() => {
                    this.showNotification('Message copied to clipboard', 'success');
                }).catch(() => {
                    this.showNotification('Failed to copy message', 'error');
                });
            }
        },

        forwardMessage(message) {
            // For now, just copy the message content for forwarding
            this.newMessage = `📤 Forwarded: ${message.content}`;
            
            // Focus the input
            setTimeout(() => {
                const textarea = document.querySelector('textarea[x-model="newMessage"]');
                if (textarea) {
                    textarea.focus();
                }
            }, 100);
            
            this.showNotification('Message ready to forward', 'info');
        },

        selectMessage(message) {
            this.isSelectionMode = true;
            
            const index = this.selectedMessages.findIndex(m => m.id === message.id);
            if (index === -1) {
                this.selectedMessages.push(message);
            } else {
                this.selectedMessages.splice(index, 1);
            }
            
            // If no messages selected, exit selection mode
            if (this.selectedMessages.length === 0) {
                this.isSelectionMode = false;
            }
        },

        editMessage(message) {
            if (message.senderId === this.currentUserId) {
                this.newMessage = message.content;
                this.editingMessageId = message.id;
                
                // Focus the input
                setTimeout(() => {
                    const textarea = document.querySelector('textarea[x-model="newMessage"]');
                    if (textarea) {
                        textarea.focus();
                    }
                }, 100);
                
                this.showNotification('Editing message', 'info');
            }
        },

        async deleteMessage(message) {
            if (message.senderId !== this.currentUserId) {
                this.showNotification('You can only delete your own messages', 'error');
                return;
            }
            
            if (confirm('Are you sure you want to delete this message?')) {
                try {
                    const response = await fetch(`/api/v1/messages/${message.id}`, {
                        method: 'DELETE',
                        credentials: 'include'
                    });
                    
                    if (response.ok) {
                        // Remove from local messages
                        this.messages = this.messages.filter(m => m.id !== message.id);
                        this.showNotification('Message deleted', 'success');
                    } else {
                        throw new Error('Failed to delete message');
                    }
                } catch (error) {
                    console.error('Error deleting message:', error);
                    this.showNotification('Failed to delete message', 'error');
                }
            }
        },

        // =============================================================================
        // BULK OPERATIONS
        // =============================================================================

        clearSelectedMessages() {
            this.selectedMessages = [];
            this.isSelectionMode = false;
        },

        forwardSelectedMessages() {
            if (this.selectedMessages.length === 0) return;
            
            const messages = this.selectedMessages.map(m => m.content).join('\n\n');
            this.newMessage = `📤 Forwarded (${this.selectedMessages.length} messages):\n\n${messages}`;
            
            this.clearSelectedMessages();
            
            // Focus the input
            setTimeout(() => {
                const textarea = document.querySelector('textarea[x-model="newMessage"]');
                if (textarea) {
                    textarea.focus();
                }
            }, 100);
            
            this.showNotification('Messages ready to forward', 'info');
        },

        async deleteSelectedMessages() {
            if (this.selectedMessages.length === 0) return;
            
            const userMessages = this.selectedMessages.filter(m => m.senderId === this.currentUserId);
            
            if (userMessages.length === 0) {
                this.showNotification('You can only delete your own messages', 'error');
                return;
            }
            
            if (confirm(`Are you sure you want to delete ${userMessages.length} message(s)?`)) {
                try {
                    const deletePromises = userMessages.map(message => 
                        fetch(`/api/v1/messages/${message.id}`, {
                            method: 'DELETE',
                            credentials: 'include'
                        })
                    );
                    
                    const results = await Promise.allSettled(deletePromises);
                    const successful = results.filter(r => r.status === 'fulfilled' && r.value.ok).length;
                    
                    if (successful > 0) {
                        // Remove deleted messages from local array
                        const deletedIds = userMessages.slice(0, successful).map(m => m.id);
                        this.messages = this.messages.filter(m => !deletedIds.includes(m.id));
                        
                        this.showNotification(`${successful} message(s) deleted`, 'success');
                    }
                    
                    this.clearSelectedMessages();
                    
                } catch (error) {
                    console.error('Error deleting messages:', error);
                    this.showNotification('Failed to delete some messages', 'error');
                }
            }
        },

        // =============================================================================
        // OFFLINE MESSAGE QUEUE FUNCTIONALITY
        // =============================================================================

        async queueOfflineMessage(messageData) {
            const offlineMessage = {
                ...messageData,
                tempId: Date.now().toString(),
                timestamp: Date.now(),
                status: 'queued'
            };
            
            // Store in service worker for background sync
            if ('serviceWorker' in navigator && navigator.serviceWorker.controller) {
                navigator.serviceWorker.controller.postMessage({
                    type: 'CACHE_MESSAGES',
                    data: { messages: [offlineMessage] }
                });
            }
            
            // Also store locally
            const queuedMessages = JSON.parse(localStorage.getItem('queuedMessages') || '[]');
            queuedMessages.push(offlineMessage);
            localStorage.setItem('queuedMessages', JSON.stringify(queuedMessages));
            
            this.showNotification('Message queued for sending when online', 'info');
        },

        async syncOfflineMessages() {
            const queuedMessages = JSON.parse(localStorage.getItem('queuedMessages') || '[]');
            
            if (queuedMessages.length === 0) return;
            
            
            const successfulMessages = [];
            
            for (const message of queuedMessages) {
                try {
                    const response = await fetch('/api/v1/messages', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        credentials: 'include',
                        body: JSON.stringify(message)
                    });
                    
                    if (response.ok) {
                        successfulMessages.push(message.tempId);
                    }
                } catch (error) {
                    console.error('Error syncing message:', message.tempId, error);
                }
            }
            
            // Remove successfully synced messages
            if (successfulMessages.length > 0) {
                const remainingMessages = queuedMessages.filter(m => !successfulMessages.includes(m.tempId));
                localStorage.setItem('queuedMessages', JSON.stringify(remainingMessages));
                
                this.showNotification(`${successfulMessages.length} queued messages sent`, 'success');
            }
        },

        // Enhanced sendMessage with offline queue support
        async sendMessage() {
            if (!this.newMessage.trim() || !this.selectedConversation) return;
            
            const messageData = {
                conversationId: this.selectedConversation.id,
                content: this.newMessage.trim(),
                replyToMessageId: this.replyingTo?.id || null
            };
            
            // Check if online
            if (!navigator.onLine) {
                await this.queueOfflineMessage(messageData);
                this.newMessage = '';
                this.replyingTo = null;
                return;
            }
            
            try {
                const response = await fetch('/api/v1/messages', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify(messageData)
                });
                
                if (response.ok) {
                    const result = await response.json();
                    
                    // Add the message to the local messages array immediately for responsiveness
                    const newMessage = {
                        id: result.data?.id || Date.now().toString(),
                        content: this.newMessage.trim(),
                        senderId: this.currentUserId,
                        senderName: 'You',
                        createdAt: new Date().toISOString(),
                        status: 'Sending',
                        replyTo: this.replyingTo ? {
                            id: this.replyingTo.id,
                            content: this.replyingTo.content,
                            senderName: this.replyingTo.senderName
                        } : null,
                        attachments: [],
                        reactions: []
                    };
                    
                    this.messages.push(newMessage);
                    this.newMessage = '';
                    this.replyingTo = null;
                    
                    // Scroll to bottom
                    this.scrollToBottom();
                    
                    // Update conversation in sidebar
                    if (this.selectedConversation) {
                        this.selectedConversation.lastMessagePreview = newMessage.content;
                        this.selectedConversation.lastMessageAt = newMessage.createdAt;
                    }
                } else {
                    throw new Error('Failed to send message');
                }
                
            } catch (error) {
                console.error('Error sending message:', error);
                
                // If network error, queue the message
                if (!navigator.onLine || error.message.includes('network')) {
                    await this.queueOfflineMessage(messageData);
                    this.newMessage = '';
                    this.replyingTo = null;
                } else {
                    this.showNotification('Failed to send message. Please try again.', 'error');
                }
            }
        },

        // =============================================================================
        // OFFLINE/ONLINE HANDLING
        // =============================================================================

        setupOfflineHandling() {
            // Listen for online/offline events
            window.addEventListener('online', () => {
                this.showNotification('Connection restored', 'success');
                this.syncOfflineMessages();
            });

            window.addEventListener('offline', () => {
                this.showNotification('You are now offline. Messages will be queued.', 'info');
            });

            // Check initial connection status
            if (!navigator.onLine) {
                this.showNotification('You are offline. Messages will be queued until connection is restored.', 'info');
            }
        },

        // =============================================================================
        // MESSAGE PAGINATION & INFINITE SCROLL
        // =============================================================================

        handleScroll() {
            const container = this.$refs.messagesContainer;
            if (!container || this.isLoadingMessages || !this.canLoadMore) return;
            
            // Check if user scrolled to the top (for loading older messages)
            if (container.scrollTop <= 100) {
                this.loadMoreMessages();
            }
        },

        async loadMoreMessages() {
            if (this.isLoadingMessages || !this.canLoadMore || !this.selectedConversation) return;
            
            this.isLoadingMessages = true;
            
            try {
                const response = await fetch(`/api/v1/conversations/${this.selectedConversation.id}/messages?page=${this.currentPage + 1}&limit=${this.messagesPerPage}`, {
                    credentials: 'include'
                });
                
                if (response.ok) {
                    const result = await response.json();
                    const newMessages = result.data || result.messages || [];
                    
                    if (newMessages.length > 0) {
                        // Store current scroll position
                        const container = this.$refs.messagesContainer;
                        const oldScrollHeight = container.scrollHeight;
                        
                        // Add older messages to the beginning of the array
                        this.messages = [...newMessages.reverse(), ...this.messages];
                        this.currentPage++;
                        
                        // Restore scroll position to prevent jumping
                        this.$nextTick(() => {
                            const newScrollHeight = container.scrollHeight;
                            container.scrollTop = newScrollHeight - oldScrollHeight;
                        });
                        
                        // Check if there are more messages to load
                        if (newMessages.length < this.messagesPerPage) {
                            this.canLoadMore = false;
                        }
                    } else {
                        this.canLoadMore = false;
                    }
                } else {
                    throw new Error('Failed to load messages');
                }
                
            } catch (error) {
                console.error('Error loading more messages:', error);
                this.showNotification('Failed to load older messages', 'error');
            } finally {
                this.isLoadingMessages = false;
            }
        },

        // Enhanced selectConversation with pagination reset
        async selectConversation(conversation) {
            this.selectedConversation = conversation;
            this.messages = [];
            this.currentPage = 1;
            this.canLoadMore = true;
            this.isLoadingMessages = false;
            
            // Load initial messages
            await this.loadConversationMessages();
            
            // Mark as read
            if (conversation.unreadCount > 0) {
                conversation.unreadCount = 0;
                this.calculateTotalUnreadCount();
            }
            
            // Join the conversation for real-time updates
            if (this.connection && this.isConnected) {
                await this.connection.invoke('JoinConversation', conversation.id);
            }
        },

        async loadConversationMessages() {
            if (!this.selectedConversation) return;
            
            this.isLoadingMessages = true;
            
            try {
                const response = await fetch(`/api/v1/conversations/${this.selectedConversation.id}/messages?page=${this.currentPage}&limit=${this.messagesPerPage}`, {
                    credentials: 'include'
                });
                
                if (response.ok) {
                    const result = await response.json();
                    this.messages = (result.data || result.messages || []).reverse();
                    
                    // Check if there are more messages to load
                    if (this.messages.length < this.messagesPerPage) {
                        this.canLoadMore = false;
                    }
                    
                    // Scroll to bottom for initial load
                    this.scrollToBottom();
                } else {
                    throw new Error('Failed to load messages');
                }
                
            } catch (error) {
                console.error('Error loading conversation messages:', error);
                this.showNotification('Failed to load messages', 'error');
            } finally {
                this.isLoadingMessages = false;
            }
        }
    };
}