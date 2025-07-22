# MeAndMyDoggy Messaging System API Documentation

## Overview

The MeAndMyDoggy messaging system provides comprehensive real-time communication capabilities between users and service providers. The system supports text messages, file attachments, message reactions, read receipts, and real-time features through SignalR.

## Architecture

### Core Components

1. **Controllers**
   - `MessagingController` - REST API endpoints for message operations
   - `ConversationController` - REST API endpoints for conversation management

2. **Services**
   - `MessagingService` - Business logic for message operations
   - `ConversationService` - Business logic for conversation management
   - `FileUploadService` - Handles file attachments and uploads

3. **SignalR Hub**
   - `MessagingHub` - Real-time communication hub

4. **Database Entities**
   - `Conversation` - Conversation metadata
   - `ConversationParticipant` - User participation in conversations
   - `Message` - Individual messages
   - `MessageAttachment` - File attachments
   - `MessageReaction` - Message reactions (emojis)
   - `MessageReadReceipt` - Read status tracking

### Data Flow

```
Client → Controller → Service → Entity Framework → Database
   ↓                                                    ↑
SignalR Hub ←--- Real-time notifications ←-------------┘
```

## API Endpoints

### Messaging Controller (`/api/v1/messaging`)

#### Send Message
```http
POST /api/v1/messaging/send
Authorization: Bearer {token}
Content-Type: application/json

{
    "conversationId": "string",
    "content": "Hello, how are you?",
    "messageType": "Text",
    "parentMessageId": null
}
```

#### Get User Conversations
```http
GET /api/v1/messaging/conversations?type=Direct&archived=false&page=1&pageSize=20
Authorization: Bearer {token}
```

#### Get Conversation Messages
```http
GET /api/v1/messaging/conversations/{conversationId}/messages?page=1&pageSize=50
Authorization: Bearer {token}
```

#### Edit Message
```http
PUT /api/v1/messaging/messages/{messageId}
Authorization: Bearer {token}
Content-Type: application/json

{
    "content": "Updated message content"
}
```

#### Delete Message
```http
DELETE /api/v1/messaging/messages/{messageId}
Authorization: Bearer {token}
```

#### Add Message Reaction
```http
POST /api/v1/messaging/messages/{messageId}/reactions
Authorization: Bearer {token}
Content-Type: application/json

{
    "reaction": "👍"
}
```

#### Search Messages
```http
GET /api/v1/messaging/search?query=dog&conversationId={id}&page=1&pageSize=20
Authorization: Bearer {token}
```

### Conversation Controller (`/api/v1/conversations`)

#### Create Conversation
```http
POST /api/v1/conversations
Authorization: Bearer {token}
Content-Type: application/json

{
    "conversationType": "Direct",
    "participantIds": ["user-id-2"],
    "title": "Chat with John",
    "description": "Service inquiry about dog walking"
}
```

#### Get Conversation
```http
GET /api/v1/conversations/{conversationId}
Authorization: Bearer {token}
```

#### Update Conversation
```http
PUT /api/v1/conversations/{conversationId}
Authorization: Bearer {token}
Content-Type: application/json

{
    "title": "Updated conversation title",
    "description": "Updated description"
}
```

#### Add Participant
```http
POST /api/v1/conversations/{conversationId}/participants
Authorization: Bearer {token}
Content-Type: application/json

{
    "userId": "new-participant-id"
}
```

#### Remove Participant
```http
DELETE /api/v1/conversations/{conversationId}/participants/{participantUserId}
Authorization: Bearer {token}
```

## SignalR Hub (`/messagingHub`)

### Connection Events

#### Connect to Hub
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/messagingHub", {
        accessTokenFactory: () => yourJwtToken
    })
    .build();

await connection.start();
```

#### Join Conversation
```javascript
await connection.invoke("JoinConversation", conversationId);
```

#### Send Message
```javascript
await connection.invoke("SendMessage", conversationId, content, messageType, parentMessageId);
```

#### Typing Indicator
```javascript
// Start typing
await connection.invoke("TypingIndicator", conversationId, true);

// Stop typing
await connection.invoke("TypingIndicator", conversationId, false);
```

#### Mark Message as Read
```javascript
await connection.invoke("MarkAsRead", conversationId, messageId);
```

### Hub Events (Server to Client)

```javascript
// New message received
connection.on("MessageReceived", (message) => {
    console.log("New message:", message);
});

// User started typing
connection.on("UserStartedTyping", (data) => {
    console.log(`${data.UserId} is typing in ${data.ConversationId}`);
});

// User stopped typing
connection.on("UserStoppedTyping", (data) => {
    console.log(`${data.UserId} stopped typing in ${data.ConversationId}`);
});

// Message read receipt
connection.on("MessageRead", (data) => {
    console.log(`Message ${data.MessageId} read by ${data.ReadBy}`);
});

// User status change (online/offline)
connection.on("UserStatusChanged", (data) => {
    console.log(`User ${data.UserId} is ${data.IsOnline ? 'online' : 'offline'}`);
});

// Unread count update
connection.on("UnreadCountUpdated", (data) => {
    console.log(`Unread count for ${data.ConversationId}: ${data.UnreadCount}`);
});
```

## Data Models

### Enums

```csharp
public enum MessageType
{
    Text, Image, File, Audio, Video, System, Location, Booking
}

public enum MessageStatus
{
    Sent, Delivered, Read, Failed, Pending, Blocked
}

public enum ConversationType
{
    Direct, Group, Support, Booking, System
}

public enum ConversationRole
{
    Member, Admin, Owner, Moderator
}

public enum AttachmentType
{
    File, Image, Video, Audio, Document, Location
}
```

### DTOs

#### MessageDto
```csharp
{
    "id": "string",
    "conversationId": "string",
    "senderId": "string",
    "senderName": "string",
    "messageType": "Text",
    "content": "string",
    "createdAt": "2025-01-21T12:00:00Z",
    "status": "Sent",
    "isEdited": false,
    "editedAt": null,
    "attachments": [],
    "reactions": []
}
```

#### ConversationDto
```csharp
{
    "id": "string",
    "conversationType": "Direct",
    "title": "string",
    "description": "string",
    "imageUrl": "string",
    "createdAt": "2025-01-21T12:00:00Z",
    "lastMessageAt": "2025-01-21T12:30:00Z",
    "lastMessagePreview": "Last message preview...",
    "messageCount": 42,
    "unreadCount": 3,
    "participants": [
        {
            "userId": "string",
            "userName": "string",
            "role": "Member",
            "joinedAt": "2025-01-21T12:00:00Z",
            "lastReadAt": "2025-01-21T12:25:00Z"
        }
    ],
    "isArchived": false,
    "isPinned": false,
    "isMuted": false
}
```

## Features Implemented

### ✅ Core Messaging
- [x] Send/receive text messages
- [x] Message editing (within 24 hours)
- [x] Message deletion (soft delete)
- [x] Message threading/replies
- [x] Message search functionality

### ✅ Conversation Management
- [x] Create direct conversations
- [x] Create group conversations
- [x] Add/remove participants
- [x] Update conversation metadata
- [x] Archive/unarchive conversations
- [x] Pin/unpin conversations
- [x] Mute/unmute conversations

### ✅ Real-time Features
- [x] Live message delivery
- [x] Typing indicators
- [x] Read receipts
- [x] Online/offline status
- [x] Unread message counts

### ✅ File Attachments
- [x] File upload support
- [x] Image/video thumbnails
- [x] File size validation
- [x] Security validation

### ✅ Message Reactions
- [x] Emoji reactions
- [x] Reaction counts
- [x] Toggle reactions

### ✅ Security & Permissions
- [x] JWT authentication
- [x] Participant validation
- [x] Content moderation ready
- [x] Role-based permissions

## Database Schema

### Key Relationships
- `Conversation` 1:N `Message`
- `Conversation` 1:N `ConversationParticipant`
- `Message` 1:N `MessageAttachment`
- `Message` 1:N `MessageReaction`
- `Message` 1:N `MessageReadReceipt`
- `ApplicationUser` 1:N `Message` (as sender)
- `ApplicationUser` 1:N `ConversationParticipant`

### Indexes for Performance
- Conversation-Timestamp composite index on messages
- User-Conversation composite index on participants
- Message-User composite index on reactions and read receipts

## Error Handling

### Common HTTP Status Codes
- `200 OK` - Success
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### SignalR Error Handling
```javascript
connection.on("Error", (errorMessage) => {
    console.error("Hub error:", errorMessage);
});
```

## Best Practices

### Client Implementation
1. **Connection Management**: Implement reconnection logic for SignalR
2. **Message Queuing**: Queue messages when offline, send when reconnected
3. **Typing Indicators**: Debounce typing events (500ms recommended)
4. **Pagination**: Load messages in chunks for better performance
5. **Caching**: Cache conversation lists and recent messages

### Security Considerations
1. **Authentication**: Always validate JWT tokens
2. **Authorization**: Check user participation before message operations
3. **Content Validation**: Sanitize message content
4. **Rate Limiting**: Implement rate limiting for message sending
5. **File Validation**: Validate file types and sizes for attachments

### Performance Optimization
1. **Database Queries**: Use appropriate indexes and pagination
2. **SignalR Groups**: Efficiently manage connection groups
3. **Caching**: Cache frequently accessed conversation metadata
4. **File Storage**: Use CDN for attachment serving
5. **Connection Pooling**: Configure proper connection pool sizes

## Next Steps

1. **Frontend Integration**: Implement Alpine.js messaging interface
2. **Testing**: Create comprehensive test suite
3. **Performance Testing**: Load testing for concurrent users
4. **Mobile Support**: Optimize for mobile applications
5. **Push Notifications**: Add mobile push notification support

## Support

For technical support or questions about the messaging system API, please refer to:
- API documentation: `/swagger` endpoint when running in development
- SignalR documentation: [Microsoft SignalR Docs](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- Entity Framework documentation: [EF Core Docs](https://docs.microsoft.com/en-us/ef/core/)