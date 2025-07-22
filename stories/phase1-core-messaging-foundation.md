# Phase 1: Core Messaging Foundation (Weeks 1-4)

## Overview

This phase establishes the fundamental messaging infrastructure for MeAndMyDoggy, implementing basic text messaging, file sharing, and real-time communication using SignalR. It focuses on creating a solid foundation that can support premium features in later phases.

## Goals

- ✅ Establish backend messaging infrastructure (controllers, services, SignalR hub)
- ✅ Implement basic messaging features (text, images, read receipts)
- ✅ Create real-time communication system
- ✅ Connect frontend prototypes to live backend
- ✅ Build scalable messaging architecture for premium features

## Week 1-2: Backend Infrastructure (Critical Priority)

### Epic 1.1: Core Messaging Controllers
**Estimated Effort:** 16 hours  
**Dependencies:** None  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Controllers/MessagingController.cs`
- `src/API/MeAndMyDog.API/Controllers/ConversationController.cs`

#### Tasks:

- [x] **Task 1.1.1: Create MessagingController**
  - [x] Implement `POST /api/v1/messaging/send` endpoint
  - [x] Implement `GET /api/v1/messaging/conversations` endpoint
  - [x] Implement `GET /api/v1/messaging/conversations/{id}/messages` endpoint
  - [x] Add proper authentication and authorization
  - [x] Include comprehensive input validation
  - [x] Add API documentation with Swagger attributes

- [x] **Task 1.1.2: Create ConversationController**
  - [x] Implement `POST /api/v1/conversations` endpoint (create conversation)
  - [x] Implement `GET /api/v1/conversations/{id}` endpoint
  - [x] Implement `PUT /api/v1/conversations/{id}` endpoint (update conversation)
  - [x] Implement `POST /api/v1/conversations/{id}/participants` endpoint
  - [x] Implement `DELETE /api/v1/conversations/{id}/participants/{userId}` endpoint
  - [x] Add conversation search functionality

- [x] **Task 1.1.3: Add Message Management Endpoints**
  - [x] Implement `PUT /api/v1/messages/{id}` (edit message)
  - [x] Implement `DELETE /api/v1/messages/{id}` (delete message)
  - [x] Implement `POST /api/v1/messages/{id}/reactions` (add reaction)
  - [x] Implement `GET /api/v1/conversations/{id}/unread-count`
  - [x] Add message search within conversations

**Acceptance Criteria:**
- All endpoints return proper HTTP status codes
- API follows RESTful conventions
- Comprehensive error handling with meaningful messages
- All endpoints protected with authentication
- Swagger documentation complete and accurate

### Epic 1.2: SignalR Hub Implementation
**Estimated Effort:** 12 hours  
**Dependencies:** None  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Hubs/MessagingHub.cs`
- `src/API/MeAndMyDog.API/Program.cs` (SignalR configuration)

#### Tasks:

- [x] **Task 1.2.1: Create MessagingHub Class**
  - [x] Set up base SignalR Hub with authentication
  - [x] Implement connection management
  - [x] Add user identification and connection mapping
  - [x] Configure Redis backplane for scalability
  - [x] Add connection logging for debugging

- [x] **Task 1.2.2: Implement Core Messaging Methods**
  ```csharp
  // Methods implemented:
  - SendMessage(string conversationId, MessageDto message)
  - JoinConversation(string conversationId)  
  - LeaveConversation(string conversationId)
  - TypingIndicator(string conversationId, bool isTyping)
  - MarkAsRead(string conversationId, string messageId)
  - UpdateMessageStatus(string messageId, string status)
  ```

- [x] **Task 1.2.3: Add Real-time Notifications**
  - [x] Implement message delivery confirmation
  - [x] Add typing indicator broadcasting
  - [x] Implement read receipt broadcasting
  - [x] Add user online/offline status
  - [x] Configure connection timeout handling

- [x] **Task 1.2.4: Configure SignalR in Program.cs**
  - [x] Add SignalR services and hub routing
  - [x] Configure CORS for SignalR connections
  - [x] Set up Redis backplane configuration
  - [x] Add authentication for SignalR connections
  - [x] Configure connection limits and timeouts

**Acceptance Criteria:**
- SignalR connections authenticate properly
- Real-time message delivery works bidirectionally
- Multiple users can join conversations simultaneously
- Typing indicators work without message conflicts
- Connection state handled gracefully (reconnection, timeout)

### Epic 1.3: Core Service Layer
**Estimated Effort:** 12 hours  
**Dependencies:** Database entities (already exist)  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Services/IMessagingService.cs`
- `src/API/MeAndMyDog.API/Services/MessagingService.cs`
- `src/API/MeAndMyDog.API/Services/IConversationService.cs`
- `src/API/MeAndMyDog.API/Services/ConversationService.cs`

#### Tasks:

- [x] **Task 1.3.1: Create IMessagingService Interface**
  ```csharp
  // Key methods defined:
  - Task<MessageDto> SendMessageAsync(string conversationId, string senderId, string content, MessageType messageType)
  - Task<List<MessageDto>> GetMessagesAsync(string conversationId, int skip, int take)
  - Task<MessageDto> GetMessageAsync(string messageId)
  - Task<MessageDto> UpdateMessageAsync(string messageId, string newContent)
  - Task<bool> DeleteMessageAsync(string messageId, string userId)
  - Task<bool> MarkAsReadAsync(string messageId, string userId)
  ```

- [x] **Task 1.3.2: Implement MessagingService**
  - [x] Implement all interface methods
  - [x] Add database operations using Entity Framework
  - [x] Include message validation and sanitization
  - [x] Add automatic moderation checks (AI content moderation)
  - [x] Implement message history and pagination
  - [x] Add comprehensive error handling and logging

- [x] **Task 1.3.3: Create IConversationService Interface**
  ```csharp
  // Key methods defined:
  - Task<ConversationDto> CreateConversationAsync(string creatorId, List<string> participantIds, ConversationType conversationType)
  - Task<ConversationDto> GetConversationAsync(string conversationId, string userId)
  - Task<ConversationListResponse> GetUserConversationsAsync(string userId)
  - Task<ConversationDto> AddParticipantAsync(string conversationId, string userId, string newParticipantId)
  - Task<ConversationDto> RemoveParticipantAsync(string conversationId, string userId, string participantToRemoveId)
  - Task<int> GetUnreadCountAsync(string conversationId, string userId)
  ```

- [x] **Task 1.3.4: Implement ConversationService**
  - [x] Implement all interface methods
  - [x] Add participant management logic
  - [x] Implement conversation search and filtering
  - [x] Add conversation metadata management
  - [x] Include proper authorization checks
  - [x] Add conversation archiving functionality

**Acceptance Criteria:**
- All service methods include proper error handling
- Database operations are optimized with appropriate includes
- Services use DTOs for data transfer (not entities directly)
- Comprehensive logging for debugging and monitoring
- Unit tests cover all service methods (separate epic)

## Week 3-4: Basic Messaging Features (Critical Priority)

### Epic 1.4: Text Messaging Implementation
**Estimated Effort:** 16 hours  
**Dependencies:** Week 1-2 backend infrastructure  
**Files to Create/Modify:**
- Frontend integration files
- Message DTOs and mapping

#### Tasks:

- [ ] **Task 1.4.1: Create Message DTOs**
  - [ ] Create `MessageDto` for API responses
  - [ ] Create `SendMessageRequest` for API requests
  - [ ] Create `ConversationDto` for conversation data
  - [ ] Create `ParticipantDto` for user data
  - [ ] Add AutoMapper profiles for entity-DTO mapping

- [ ] **Task 1.4.2: Implement Real-time Message Delivery**
  - [ ] Connect SignalR to service layer
  - [ ] Implement message broadcasting to conversation participants
  - [ ] Add message delivery confirmations
  - [ ] Handle message ordering and timestamps
  - [ ] Add duplicate message detection

- [ ] **Task 1.4.3: Add Message Persistence**
  - [ ] Implement database save operations
  - [ ] Add transaction handling for message operations
  - [ ] Implement message history retrieval
  - [ ] Add pagination for large conversation histories
  - [ ] Include soft delete functionality

- [ ] **Task 1.4.4: Create Conversation Management**
  - [ ] Implement participant management
  - [ ] Add conversation creation logic
  - [ ] Implement conversation listing for users
  - [ ] Add conversation metadata updates
  - [ ] Include conversation search functionality

**Acceptance Criteria:**
- Messages are delivered in real-time to all participants
- Message history persists correctly in database
- Conversations can be created and managed
- Message ordering is maintained consistently
- No message loss during normal operation

### Epic 1.5: File Upload and Image Sharing
**Estimated Effort:** 10 hours  
**Dependencies:** Task 1.4 completion  
**Files to Create/Modify:**
- `src/API/MeAndMyDog.API/Services/IFileUploadService.cs`
- `src/API/MeAndMyDog.API/Services/FileUploadService.cs`
- `src/API/MeAndMyDog.API/Controllers/FileUploadController.cs`

#### Tasks:

- [x] **Task 1.5.1: Create File Upload Service**
  - [x] Implement `IFileUploadService` interface
  - [x] Add support for local file storage (cloud storage ready for production)
  - [x] Implement file validation (type, size, security)
  - [x] Add image compression and thumbnail generation
  - [x] Include secure file naming and storage

- [x] **Task 1.5.2: Add File Upload Controller**
  - [x] Create `POST /api/v1/files/upload` endpoint
  - [x] Add file size limits (basic: 5MB, premium: 50MB)
  - [x] Implement file type validation
  - [x] Add security checks for dangerous file extensions
  - [x] Return secure URLs for file access

- [x] **Task 1.5.3: Integrate File Attachments with Messages**
  - [x] Update MessageAttachment entity usage
  - [x] Add file attachment support in MessagingService
  - [x] Implement FileUploadRecord entity for tracking
  - [x] Add thumbnail generation for images
  - [x] Include file upload tracking and metadata

- [x] **Task 1.5.4: Add Basic File Sharing Limits**
  - [x] Implement daily file upload limits for free users (10 files/day)
  - [x] Add file type restrictions and MIME type validation
  - [x] Create premium vs free user differentiation
  - [x] Track usage for future premium validation
  - [x] Add file deletion functionality

**Acceptance Criteria:**
- Images can be uploaded and shared in conversations
- File uploads are secured and validated
- Thumbnails are generated for image files
- Basic limits are enforced for free users
- File storage is efficient and scalable

### Epic 1.6: Read Receipts and Typing Indicators
**Estimated Effort:** 8 hours  
**Dependencies:** Task 1.4 completion  
**Files to Modify:**
- MessagingHub.cs (add methods)
- MessagingService.cs (add read receipt logic)

#### Tasks:

- [ ] **Task 1.6.1: Implement Read Receipts**
  - [ ] Add read receipt tracking in database
  - [ ] Implement `MarkAsRead` SignalR method
  - [ ] Broadcast read status to message sender
  - [ ] Add unread message count API
  - [ ] Include read receipt timestamp tracking

- [ ] **Task 1.6.2: Add Typing Indicators**
  - [ ] Implement `TypingIndicator` SignalR method
  - [ ] Broadcast typing status to conversation participants
  - [ ] Add automatic timeout for typing indicators
  - [ ] Prevent typing indicator spam
  - [ ] Include user identification in typing status

- [ ] **Task 1.6.3: Add Last Seen Timestamps**
  - [ ] Track user last activity in conversations
  - [ ] Update last seen on message read
  - [ ] Add last seen display in frontend
  - [ ] Include privacy settings for last seen
  - [ ] Add online/offline status tracking

- [ ] **Task 1.6.4: Implement Advanced Read Receipt Features**
  - [ ] Add read receipt settings (enable/disable)
  - [ ] Implement group message read receipts
  - [ ] Add message delivery confirmation
  - [ ] Include failed delivery handling
  - [ ] Add read receipt privacy controls

**Acceptance Criteria:**
- Read receipts are displayed accurately and in real-time
- Typing indicators work smoothly without conflicts
- Last seen timestamps update properly
- Users can control read receipt privacy
- Group conversations handle read receipts correctly

### Epic 1.7: Frontend Integration
**Estimated Effort:** 12 hours  
**Dependencies:** All backend tasks completion  
**Files to Create/Modify:**
- Create new messaging views and components
- Update existing Alpine.js components

#### Tasks:

- [ ] **Task 1.7.1: Create Messaging Views**
  - [ ] Create `Views/Messaging/Index.cshtml` (main messaging interface)
  - [ ] Create `Views/Messaging/Conversation.cshtml` (individual conversation view)
  - [ ] Add responsive design for mobile and desktop
  - [ ] Include accessibility features
  - [ ] Add loading states and error handling

- [ ] **Task 1.7.2: Implement SignalR Client Connection**
  - [ ] Create JavaScript SignalR client setup
  - [ ] Add connection management and reconnection logic
  - [ ] Implement message sending and receiving
  - [ ] Add typing indicator handling
  - [ ] Include read receipt functionality

- [ ] **Task 1.7.3: Create Message Components**
  - [ ] Design message bubble components
  - [ ] Add file attachment display
  - [ ] Implement message reactions UI
  - [ ] Add message editing interface
  - [ ] Include message status indicators

- [ ] **Task 1.7.4: Add Conversation Management UI**
  - [ ] Create conversation list component
  - [ ] Add conversation creation interface
  - [ ] Implement participant management UI
  - [ ] Add conversation search functionality
  - [ ] Include unread message indicators

**Acceptance Criteria:**
- Messaging interface is intuitive and responsive
- Real-time updates work seamlessly
- File sharing UI is user-friendly
- All messaging features accessible via UI
- Interface works well on mobile and desktop

## Testing Requirements

### Epic 1.8: Comprehensive Testing
**Estimated Effort:** 16 hours  
**Dependencies:** All implementation tasks  

#### Tasks:

- [ ] **Task 1.8.1: Unit Tests**
  - [ ] Test all service layer methods
  - [ ] Test controller endpoints
  - [ ] Mock external dependencies
  - [ ] Achieve 80%+ code coverage
  - [ ] Test error handling scenarios

- [ ] **Task 1.8.2: Integration Tests**
  - [ ] Test SignalR hub functionality
  - [ ] Test database operations
  - [ ] Test file upload workflows
  - [ ] Test authentication integration
  - [ ] Test real-time message delivery

- [ ] **Task 1.8.3: Performance Tests**
  - [ ] Test concurrent user messaging
  - [ ] Test large conversation history loading
  - [ ] Test file upload performance
  - [ ] Test SignalR connection limits
  - [ ] Identify performance bottlenecks

- [ ] **Task 1.8.4: Frontend Tests**
  - [ ] Test SignalR client functionality
  - [ ] Test UI component interactions
  - [ ] Test responsive design
  - [ ] Test accessibility features
  - [ ] Test error state handling

## Phase 1 Deliverables

### Functional Deliverables
✅ **Core Messaging System**
- Text messaging with real-time delivery
- Conversation creation and management  
- Multi-participant conversations
- Message history and pagination

✅ **File Sharing System**
- Image upload and sharing
- File validation and security
- Thumbnail generation
- Basic usage limits

✅ **Real-time Features**
- Instant message delivery
- Typing indicators
- Read receipts
- Online/offline status

✅ **User Interface**
- Responsive messaging interface
- Conversation management UI
- File sharing components
- Mobile-optimized design

### Technical Deliverables
✅ **Backend Infrastructure**
- RESTful API endpoints
- SignalR real-time hub
- Service layer implementation
- Database integration

✅ **Security & Validation**
- Authentication integration
- Input validation
- File upload security
- Basic rate limiting

✅ **Performance & Scalability**
- Redis backplane configuration
- Database optimization
- Efficient file storage
- Connection pooling

## Success Metrics

### Primary Metrics
- [ ] **Message Delivery Success Rate**: >99% of messages delivered successfully
- [ ] **Real-time Performance**: Messages delivered within 500ms
- [ ] **File Upload Success**: >98% success rate for file uploads under limits
- [ ] **User Experience**: All core messaging features work seamlessly

### Secondary Metrics
- [ ] **Concurrent Users**: Support 100+ concurrent messaging users
- [ ] **Database Performance**: Message queries execute in <100ms
- [ ] **File Storage**: Efficient storage with proper cleanup
- [ ] **Error Rate**: <1% error rate across all messaging operations

## Risk Mitigation

### High-Risk Areas
1. **SignalR Scalability**: 
   - Mitigation: Implement Redis backplane early
   - Test concurrent connections regularly

2. **File Upload Security**:
   - Mitigation: Comprehensive validation and scanning
   - Use reputable cloud storage services

3. **Real-time Performance**:
   - Mitigation: Performance testing throughout development
   - Optimize database queries early

### Contingency Plans
- **SignalR Issues**: Have polling fallback mechanism ready
- **File Storage Issues**: Multiple cloud provider support
- **Performance Issues**: Implement caching strategies early

## Dependencies for Next Phase

Phase 1 completion is required for:
- **Phase 2**: Premium video calling features need messaging foundation
- **User Authentication**: Messaging requires authenticated users
- **Database Schema**: All messaging entities must be properly migrated
- **Basic UI Framework**: Frontend structure needed for messaging interface

## Post-Phase 1 Preparation

To prepare for Phase 2 (Premium Video Calling):
- [ ] Ensure ServiceProvider premium subscription logic is tested
- [ ] Validate user permission checking works correctly
- [ ] Confirm SignalR infrastructure can handle video call signaling
- [ ] Test real-time communication reliability under load

This foundation will enable premium features while providing immediate value to users through robust basic messaging capabilities.