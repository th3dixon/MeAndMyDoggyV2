# Real-time Messaging Interface - Technical Specification

## Component Overview
The Real-time Messaging Interface provides secure, instant communication between pet owners and service providers using SignalR for WebSocket connections, with features including group chats, file sharing, read receipts, and automated booking-related messages.

## Database Schema

### Messaging-Specific Tables

```sql
-- Conversations (Extended from main schema)
CREATE TABLE [dbo].[Conversations] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ConversationType] INT NOT NULL, -- 0: Direct, 1: Group, 2: Booking, 3: System
    [BookingId] INT NULL,
    [Title] NVARCHAR(200) NULL,
    [Description] NVARCHAR(500) NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(450) NOT NULL,
    [LastMessageId] INT NULL,
    [LastMessageAt] DATETIME2 NULL,
    [LastMessagePreview] NVARCHAR(200) NULL,
    [MessageCount] INT NOT NULL DEFAULT 0,
    [IsArchived] BIT NOT NULL DEFAULT 0,
    [IsPinned] BIT NOT NULL DEFAULT 0,
    [Settings] NVARCHAR(MAX) NULL, -- JSON
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Conversations_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [Bookings]([Id]),
    CONSTRAINT [FK_Conversations_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [AspNetUsers]([Id]),
    INDEX [IX_Conversations_LastMessageAt] ([LastMessageAt] DESC)
);

-- ConversationParticipants (Extended)
CREATE TABLE [dbo].[ConversationParticipants] (
    [ConversationId] INT NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [Role] INT NOT NULL DEFAULT 0, -- 0: Member, 1: Admin, 2: Owner
    [JoinedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LeftAt] DATETIME2 NULL,
    [LastReadMessageId] INT NULL,
    [LastReadAt] DATETIME2 NULL,
    [LastTypingAt] DATETIME2 NULL,
    [UnreadCount] INT NOT NULL DEFAULT 0,
    [IsMuted] BIT NOT NULL DEFAULT 0,
    [MutedUntil] DATETIME2 NULL,
    [NotificationLevel] INT NOT NULL DEFAULT 0, -- 0: All, 1: Mentions, 2: None
    [IsArchived] BIT NOT NULL DEFAULT 0,
    [IsPinned] BIT NOT NULL DEFAULT 0,
    [CustomSettings] NVARCHAR(MAX) NULL, -- JSON
    PRIMARY KEY ([ConversationId], [UserId]),
    CONSTRAINT [FK_ConversationParticipants_Conversations] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations]([Id]),
    CONSTRAINT [FK_ConversationParticipants_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    INDEX [IX_ConversationParticipants_UserId_UnreadCount] ([UserId], [UnreadCount])
);

-- Messages (Extended)
CREATE TABLE [dbo].[Messages] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ConversationId] INT NOT NULL,
    [ParentMessageId] INT NULL, -- For replies
    [SenderId] NVARCHAR(450) NOT NULL,
    [MessageType] INT NOT NULL, -- 0: Text, 1: Image, 2: File, 3: Audio, 4: Location, 5: System
    [Content] NVARCHAR(MAX) NOT NULL,
    [FormattedContent] NVARCHAR(MAX) NULL, -- HTML formatted
    [Attachments] NVARCHAR(MAX) NULL, -- JSON array
    [Mentions] NVARCHAR(MAX) NULL, -- JSON array of user IDs
    [Reactions] NVARCHAR(MAX) NULL, -- JSON object
    [IsEdited] BIT NOT NULL DEFAULT 0,
    [EditedAt] DATETIME2 NULL,
    [EditHistory] NVARCHAR(MAX) NULL, -- JSON array
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(450) NULL,
    [IsForwarded] BIT NOT NULL DEFAULT 0,
    [ForwardedFromId] INT NULL,
    [SystemMessageType] INT NULL, -- For system messages
    [SystemMessageData] NVARCHAR(MAX) NULL, -- JSON
    [Metadata] NVARCHAR(MAX) NULL, -- JSON
    [Status] INT NOT NULL DEFAULT 0, -- 0: Sending, 1: Sent, 2: Delivered, 3: Failed
    [FailureReason] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Messages_Conversations] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations]([Id]),
    CONSTRAINT [FK_Messages_ParentMessage] FOREIGN KEY ([ParentMessageId]) REFERENCES [Messages]([Id]),
    CONSTRAINT [FK_Messages_Senders] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Messages_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [AspNetUsers]([Id]),
    INDEX [IX_Messages_ConversationId_CreatedAt] ([ConversationId], [CreatedAt]),
    INDEX [IX_Messages_SenderId] ([SenderId])
);

-- MessageAttachments
CREATE TABLE [dbo].[MessageAttachments] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [MessageId] INT NOT NULL,
    [AttachmentType] INT NOT NULL, -- 0: Image, 1: Document, 2: Audio, 3: Video
    [FileName] NVARCHAR(255) NOT NULL,
    [FileUrl] NVARCHAR(500) NOT NULL,
    [ThumbnailUrl] NVARCHAR(500) NULL,
    [FileSize] BIGINT NOT NULL,
    [MimeType] NVARCHAR(100) NOT NULL,
    [Width] INT NULL,
    [Height] INT NULL,
    [Duration] INT NULL, -- For audio/video in seconds
    [Metadata] NVARCHAR(MAX) NULL, -- JSON
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_MessageAttachments_Messages] FOREIGN KEY ([MessageId]) REFERENCES [Messages]([Id])
);

-- MessageReadReceipts (Extended)
CREATE TABLE [dbo].[MessageReadReceipts] (
    [MessageId] INT NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [ReadAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeviceInfo] NVARCHAR(200) NULL,
    PRIMARY KEY ([MessageId], [UserId]),
    CONSTRAINT [FK_MessageReadReceipts_Messages] FOREIGN KEY ([MessageId]) REFERENCES [Messages]([Id]),
    CONSTRAINT [FK_MessageReadReceipts_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- MessageReactions
CREATE TABLE [dbo].[MessageReactions] (
    [MessageId] INT NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [Reaction] NVARCHAR(50) NOT NULL, -- Emoji or reaction type
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([MessageId], [UserId], [Reaction]),
    CONSTRAINT [FK_MessageReactions_Messages] FOREIGN KEY ([MessageId]) REFERENCES [Messages]([Id]),
    CONSTRAINT [FK_MessageReactions_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

-- ConversationTemplates
CREATE TABLE [dbo].[ConversationTemplates] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TemplateName] NVARCHAR(100) NOT NULL,
    [TemplateType] INT NOT NULL, -- 0: Booking, 1: ServiceInquiry, 2: Emergency
    [InitialMessage] NVARCHAR(MAX) NOT NULL,
    [SuggestedResponses] NVARCHAR(MAX) NULL, -- JSON array
    [AutomationRules] NVARCHAR(MAX) NULL, -- JSON
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- MessageDrafts
CREATE TABLE [dbo].[MessageDrafts] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] NVARCHAR(450) NOT NULL,
    [ConversationId] INT NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [Attachments] NVARCHAR(MAX) NULL, -- JSON
    [ReplyToMessageId] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_MessageDrafts_Users] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_MessageDrafts_Conversations] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations]([Id])
);

-- BlockedUsers
CREATE TABLE [dbo].[BlockedUsers] (
    [UserId] NVARCHAR(450) NOT NULL,
    [BlockedUserId] NVARCHAR(450) NOT NULL,
    [Reason] NVARCHAR(500) NULL,
    [BlockedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([UserId], [BlockedUserId]),
    CONSTRAINT [FK_BlockedUsers_User] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_BlockedUsers_BlockedUser] FOREIGN KEY ([BlockedUserId]) REFERENCES [AspNetUsers]([Id])
);

-- MessageTranslations
CREATE TABLE [dbo].[MessageTranslations] (
    [MessageId] INT NOT NULL,
    [Language] NVARCHAR(10) NOT NULL,
    [TranslatedContent] NVARCHAR(MAX) NOT NULL,
    [TranslatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY ([MessageId], [Language]),
    CONSTRAINT [FK_MessageTranslations_Messages] FOREIGN KEY ([MessageId]) REFERENCES [Messages]([Id])
);
```

## API Endpoints

### Conversation Management

```yaml
/api/v1/messaging/conversations:
  /:
    GET:
      description: Get user's conversations
      auth: required
      parameters:
        type: enum [all, direct, group, booking]
        archived: boolean
        page: number
        pageSize: number
      responses:
        200:
          conversations: array[ConversationSummary]
          unreadTotal: number
          hasMore: boolean

    POST:
      description: Create new conversation
      auth: required
      body:
        participants: array[string] # User IDs
        title: string # For group chats
        type: enum [direct, group]
        initialMessage: string
      responses:
        201:
          conversationId: number
          conversation: Conversation

  /{conversationId}:
    GET:
      description: Get conversation details
      auth: required
      responses:
        200:
          conversation: ConversationDetail
          participants: array[Participant]
          settings: ConversationSettings

    PUT:
      description: Update conversation settings
      auth: required
      body:
        title: string
        description: string
        imageUrl: string
      responses:
        200: Updated conversation

    DELETE:
      description: Leave/delete conversation
      auth: required
      responses:
        204: Left conversation

  /{conversationId}/participants:
    GET:
      description: Get conversation participants
      auth: required
      responses:
        200:
          participants: array[ParticipantDetail]

    POST:
      description: Add participants to group
      auth: required
      body:
        userIds: array[string]
      responses:
        200: Participants added

    DELETE:
      description: Remove participant from group
      auth: required
      body:
        userId: string
      responses:
        204: Participant removed

  /{conversationId}/mute:
    POST:
      description: Mute/unmute conversation
      auth: required
      body:
        muted: boolean
        until: datetime # Optional
      responses:
        200: Mute status updated

  /{conversationId}/archive:
    POST:
      description: Archive/unarchive conversation
      auth: required
      body:
        archived: boolean
      responses:
        200: Archive status updated
```

### Message Operations

```yaml
/api/v1/messaging/conversations/{conversationId}/messages:
  /:
    GET:
      description: Get conversation messages
      auth: required
      parameters:
        before: number # Message ID
        after: number # Message ID
        limit: number
        includeDeleted: boolean
      responses:
        200:
          messages: array[Message]
          hasMore: boolean
          oldestMessageId: number
          newestMessageId: number

    POST:
      description: Send message
      auth: required
      body:
        content: string
        type: enum [text, image, file, audio, location]
        attachments: array[AttachmentInput]
        replyToMessageId: number # Optional
        mentions: array[string] # User IDs
      responses:
        201:
          message: Message
          tempId: string # For client correlation

  /{messageId}:
    GET:
      description: Get message details
      auth: required
      responses:
        200:
          message: MessageDetail
          readBy: array[ReadReceipt]
          reactions: array[Reaction]

    PUT:
      description: Edit message
      auth: required
      body:
        content: string
      responses:
        200: Updated message

    DELETE:
      description: Delete message
      auth: required
      parameters:
        forEveryone: boolean
      responses:
        204: Message deleted

  /{messageId}/react:
    POST:
      description: Add reaction to message
      auth: required
      body:
        reaction: string # Emoji
      responses:
        200: Reaction added

    DELETE:
      description: Remove reaction
      auth: required
      body:
        reaction: string
      responses:
        204: Reaction removed

  /{messageId}/translate:
    POST:
      description: Translate message
      auth: required
      body:
        targetLanguage: string
      responses:
        200:
          translatedContent: string
          detectedLanguage: string

  /search:
    GET:
      description: Search messages
      auth: required
      parameters:
        query: string
        conversationId: number # Optional
        startDate: date
        endDate: date
      responses:
        200:
          results: array[MessageSearchResult]
          totalCount: number
```

### Real-time Operations

```yaml
/api/v1/messaging/typing:
  /start:
    POST:
      description: Notify typing started
      auth: required
      body:
        conversationId: number
      responses:
        200: Typing notification sent

  /stop:
    POST:
      description: Notify typing stopped
      auth: required
      body:
        conversationId: number
      responses:
        200: Typing notification sent

/api/v1/messaging/presence:
  /update:
    POST:
      description: Update user presence
      auth: required
      body:
        status: enum [online, away, busy, offline]
        statusMessage: string # Optional
      responses:
        200: Presence updated

  /get:
    POST:
      description: Get users' presence
      auth: required
      body:
        userIds: array[string]
      responses:
        200:
          presence: object # userId -> PresenceInfo
```

### File & Media Handling

```yaml
/api/v1/messaging/attachments:
  /upload:
    POST:
      description: Upload attachment
      auth: required
      contentType: multipart/form-data
      body:
        file: file
        type: enum [image, document, audio, video]
      responses:
        201:
          attachmentId: string
          url: string
          thumbnailUrl: string # For images/videos
          metadata: object

  /voice:
    POST:
      description: Upload voice message
      auth: required
      contentType: multipart/form-data
      body:
        audio: file
        duration: number # seconds
      responses:
        201:
          attachmentId: string
          url: string
          waveform: array[number] # Audio visualization
```

## SignalR Hub Implementation

```csharp
public class MessagingHub : Hub
{
    private readonly IMessagingService _messagingService;
    private readonly IPresenceTracker _presenceTracker;
    private readonly IConnectionMapping _connections;
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        await _connections.Add(userId, Context.ConnectionId);
        
        // Update presence
        await _presenceTracker.UserConnected(userId);
        
        // Join user's conversation groups
        var conversations = await _messagingService.GetUserConversations(userId);
        foreach (var conv in conversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conv-{conv.Id}");
        }
        
        // Notify contacts of online status
        await NotifyPresenceUpdate(userId, "online");
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier;
        await _connections.Remove(userId, Context.ConnectionId);
        
        // Check if user has other connections
        if (!await _connections.HasConnections(userId))
        {
            await _presenceTracker.UserDisconnected(userId);
            await NotifyPresenceUpdate(userId, "offline");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
    // Send message
    public async Task SendMessage(SendMessageDto dto)
    {
        var userId = Context.UserIdentifier;
        
        // Validate user is participant
        if (!await _messagingService.IsParticipant(userId, dto.ConversationId))
        {
            throw new HubException("Not authorized to send to this conversation");
        }
        
        // Create message
        var message = await _messagingService.CreateMessage(userId, dto);
        
        // Send to conversation participants
        await Clients.Group($"conv-{dto.ConversationId}")
            .SendAsync("ReceiveMessage", new
            {
                message,
                tempId = dto.TempId
            });
        
        // Update conversation
        await Clients.Group($"conv-{dto.ConversationId}")
            .SendAsync("ConversationUpdated", new
            {
                conversationId = dto.ConversationId,
                lastMessage = message,
                lastMessageAt = message.CreatedAt
            });
        
        // Send push notifications
        await SendPushNotifications(dto.ConversationId, message);
    }
    
    // Mark messages as read
    public async Task MarkAsRead(int conversationId, int lastReadMessageId)
    {
        var userId = Context.UserIdentifier;
        
        await _messagingService.MarkMessagesAsRead(userId, conversationId, lastReadMessageId);
        
        // Notify other participants
        await Clients.OthersInGroup($"conv-{conversationId}")
            .SendAsync("MessagesRead", new
            {
                conversationId,
                userId,
                lastReadMessageId
            });
    }
    
    // Typing indicators
    public async Task StartTyping(int conversationId)
    {
        var userId = Context.UserIdentifier;
        
        await Clients.OthersInGroup($"conv-{conversationId}")
            .SendAsync("UserTyping", new
            {
                conversationId,
                userId,
                isTyping = true
            });
    }
    
    public async Task StopTyping(int conversationId)
    {
        var userId = Context.UserIdentifier;
        
        await Clients.OthersInGroup($"conv-{conversationId}")
            .SendAsync("UserTyping", new
            {
                conversationId,
                userId,
                isTyping = false
            });
    }
    
    // Message reactions
    public async Task AddReaction(int messageId, string reaction)
    {
        var userId = Context.UserIdentifier;
        var message = await _messagingService.GetMessage(messageId);
        
        await _messagingService.AddReaction(userId, messageId, reaction);
        
        await Clients.Group($"conv-{message.ConversationId}")
            .SendAsync("ReactionAdded", new
            {
                messageId,
                userId,
                reaction
            });
    }
    
    // Voice/video calls (signaling)
    public async Task InitiateCall(int conversationId, string callType)
    {
        var userId = Context.UserIdentifier;
        var participants = await _messagingService.GetParticipants(conversationId);
        
        var callId = Guid.NewGuid().ToString();
        
        await Clients.OthersInGroup($"conv-{conversationId}")
            .SendAsync("IncomingCall", new
            {
                callId,
                conversationId,
                callerId = userId,
                callType
            });
    }
}
```

## Frontend Components

### Chat Interface Components (Vue.js)

```typescript
// ChatContainer.vue
interface ChatContainerProps {
  conversationId?: number
  userId?: string
  embedded?: boolean // For embedding in other pages
}

// Components:
// - ConversationList.vue
// - MessageThread.vue
// - MessageInput.vue
// - ParticipantsList.vue
// - ChatHeader.vue

// ConversationList.vue
interface ConversationListProps {
  conversations: Conversation[]
  activeConversationId?: number
  onConversationSelect: (conversation: Conversation) => void
}

// Features:
// - Search/filter conversations
// - Unread badges
// - Last message preview
// - Online status indicators
// - Swipe actions (mobile)
```

### Message Components

```typescript
// MessageBubble.vue
interface MessageBubbleProps {
  message: Message
  isOwn: boolean
  showAvatar: boolean
  showTimestamp: boolean
  continuedMessage: boolean
}

// Features:
// - Text formatting (markdown)
// - Link previews
// - Reactions
// - Read receipts
// - Reply preview

// MessageInput.vue
interface MessageInputProps {
  conversationId: number
  replyTo?: Message
  onSend: (content: string, attachments: File[]) => void
}

// Features:
// - Rich text editor
// - Emoji picker
// - File attachments
// - Voice recording
// - @mentions
// - Typing indicators
```

### Real-time Features

```typescript
// useMessaging.ts - Composable for messaging
import { useSignalR } from '@/composables/useSignalR';

export function useMessaging(conversationId: Ref<number>) {
  const { connection } = useSignalR();
  const messages = ref<Message[]>([]);
  const typingUsers = ref<Set<string>>(new Set());
  const onlineUsers = ref<Set<string>>(new Set());
  
  // Message handlers
  connection.on('ReceiveMessage', (data: { message: Message, tempId: string }) => {
    messages.value.push(data.message);
    // Update temp message with real ID
  });
  
  connection.on('MessagesRead', (data: { userId: string, lastReadMessageId: number }) => {
    // Update read receipts
  });
  
  connection.on('UserTyping', (data: { userId: string, isTyping: boolean }) => {
    if (data.isTyping) {
      typingUsers.value.add(data.userId);
    } else {
      typingUsers.value.delete(data.userId);
    }
  });
  
  // Send message
  const sendMessage = async (content: string, attachments?: File[]) => {
    const tempId = generateTempId();
    
    // Optimistic update
    messages.value.push({
      id: tempId,
      content,
      senderId: currentUser.value.id,
      status: 'sending',
      createdAt: new Date()
    });
    
    try {
      await connection.invoke('SendMessage', {
        conversationId: conversationId.value,
        content,
        tempId,
        attachments
      });
    } catch (error) {
      // Handle error, update message status
    }
  };
  
  return {
    messages,
    typingUsers,
    onlineUsers,
    sendMessage
  };
}
```

## Technical Implementation Details

### Message Encryption

```csharp
public class MessageEncryptionService
{
    private readonly IKeyVaultService _keyVault;
    
    public async Task<EncryptedMessage> EncryptMessage(Message message)
    {
        // For sensitive conversations (e.g., containing payment info)
        if (RequiresEncryption(message))
        {
            var key = await _keyVault.GetEncryptionKey();
            var encryptedContent = Encrypt(message.Content, key);
            
            return new EncryptedMessage
            {
                Content = encryptedContent,
                IsEncrypted = true,
                EncryptionVersion = "1.0"
            };
        }
        
        return new EncryptedMessage { Content = message.Content, IsEncrypted = false };
    }
    
    private bool RequiresEncryption(Message message)
    {
        // Check for sensitive patterns
        var sensitivePatterns = new[]
        {
            @"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b", // Credit card
            @"\b\d{3}-\d{2}-\d{4}\b", // SSN
            @"password|pin|secret", // Sensitive keywords
        };
        
        return sensitivePatterns.Any(pattern => 
            Regex.IsMatch(message.Content, pattern, RegexOptions.IgnoreCase));
    }
}
```

### Message Processing Pipeline

```csharp
public class MessageProcessingPipeline
{
    private readonly List<IMessageProcessor> _processors;
    
    public MessageProcessingPipeline()
    {
        _processors = new List<IMessageProcessor>
        {
            new LinkPreviewProcessor(),
            new MentionProcessor(),
            new EmoticonProcessor(),
            new ProfanityFilter(),
            new SpamDetector(),
            new AutoTranslator()
        };
    }
    
    public async Task<ProcessedMessage> ProcessMessage(Message message)
    {
        var processed = new ProcessedMessage(message);
        
        foreach (var processor in _processors)
        {
            processed = await processor.Process(processed);
        }
        
        return processed;
    }
}

public class LinkPreviewProcessor : IMessageProcessor
{
    public async Task<ProcessedMessage> Process(ProcessedMessage message)
    {
        var urls = ExtractUrls(message.Content);
        
        if (urls.Any())
        {
            var previews = await Task.WhenAll(
                urls.Select(url => GeneratePreview(url))
            );
            
            message.Metadata["linkPreviews"] = previews;
        }
        
        return message;
    }
    
    private async Task<LinkPreview> GeneratePreview(string url)
    {
        var metadata = await FetchOpenGraphData(url);
        
        return new LinkPreview
        {
            Url = url,
            Title = metadata.Title,
            Description = metadata.Description,
            ImageUrl = metadata.Image,
            SiteName = metadata.SiteName
        };
    }
}
```

### Presence Tracking

```csharp
public class PresenceTracker
{
    private readonly IConnectionTracker _connections;
    private readonly IDistributedCache _cache;
    
    public async Task<UserPresence> GetPresence(string userId)
    {
        var cacheKey = $"presence:{userId}";
        var cached = await _cache.GetAsync<UserPresence>(cacheKey);
        
        if (cached != null) return cached;
        
        var isOnline = await _connections.IsOnline(userId);
        var lastSeen = await GetLastSeen(userId);
        
        var presence = new UserPresence
        {
            UserId = userId,
            Status = isOnline ? "online" : "offline",
            LastSeen = lastSeen,
            ActiveDevices = await _connections.GetActiveDevices(userId)
        };
        
        await _cache.SetAsync(cacheKey, presence, TimeSpan.FromMinutes(1));
        
        return presence;
    }
    
    public async Task UpdateActivity(string userId)
    {
        var key = $"lastActivity:{userId}";
        await _cache.SetAsync(key, DateTime.UtcNow, TimeSpan.FromHours(24));
    }
}
```

### File Upload Service

```csharp
public class MessageAttachmentService
{
    private readonly IBlobStorageService _blobStorage;
    private readonly IImageProcessor _imageProcessor;
    private readonly IVirusScanService _virusScan;
    
    public async Task<AttachmentResult> UploadAttachment(
        IFormFile file, 
        AttachmentType type,
        string userId)
    {
        // Validate file
        ValidateFile(file, type);
        
        // Virus scan
        var scanResult = await _virusScan.ScanFile(file.OpenReadStream());
        if (!scanResult.IsClean)
        {
            throw new SecurityException("File failed security scan");
        }
        
        // Process based on type
        var processedFiles = type switch
        {
            AttachmentType.Image => await ProcessImage(file),
            AttachmentType.Audio => await ProcessAudio(file),
            AttachmentType.Video => await ProcessVideo(file),
            _ => await ProcessDocument(file)
        };
        
        // Upload to blob storage
        var uploadTasks = processedFiles.Select(f => 
            UploadToStorage(f, userId));
        
        var urls = await Task.WhenAll(uploadTasks);
        
        return new AttachmentResult
        {
            Urls = urls,
            Metadata = ExtractMetadata(file, type)
        };
    }
    
    private async Task<ProcessedFile[]> ProcessImage(IFormFile file)
    {
        return await _imageProcessor.ProcessImage(file.OpenReadStream(), new[]
        {
            new ImageVariant { MaxWidth = 1920, MaxHeight = 1080, Suffix = "_full" },
            new ImageVariant { MaxWidth = 400, MaxHeight = 400, Suffix = "_thumb" },
            new ImageVariant { Width = 50, Height = 50, Suffix = "_micro" }
        });
    }
}
```

### Push Notifications

```csharp
public class MessageNotificationService
{
    private readonly IPushNotificationService _pushService;
    private readonly IUserPreferenceService _preferences;
    
    public async Task SendMessageNotification(Message message, List<string> recipientIds)
    {
        var tasks = recipientIds.Select(async recipientId =>
        {
            var prefs = await _preferences.GetNotificationPreferences(recipientId);
            
            if (!ShouldSendNotification(prefs, message))
                return;
            
            var notification = new PushNotification
            {
                Title = await GetNotificationTitle(message),
                Body = GetNotificationBody(message, prefs),
                Data = new
                {
                    conversationId = message.ConversationId,
                    messageId = message.Id,
                    type = "new_message"
                },
                Badge = await GetUnreadCount(recipientId)
            };
            
            await _pushService.SendToUser(recipientId, notification);
        });
        
        await Task.WhenAll(tasks);
    }
    
    private bool ShouldSendNotification(NotificationPreferences prefs, Message message)
    {
        if (!prefs.MessagesEnabled) return false;
        
        if (prefs.OnlyWhenOffline && IsUserOnline(message.RecipientId))
            return false;
        
        if (prefs.MutedConversations.Contains(message.ConversationId))
            return false;
        
        return true;
    }
}
```

## Security Considerations

### Message Security
1. **End-to-End Encryption**: Optional for sensitive conversations
2. **Content Filtering**: Profanity and spam detection
3. **File Scanning**: Virus/malware scanning for uploads
4. **Rate Limiting**: Message and file upload limits

### Access Control
```csharp
public class MessageAuthorizationHandler : AuthorizationHandler<MessageRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MessageRequirement requirement)
    {
        var userId = context.User.GetUserId();
        var message = context.Resource as Message;
        
        // Check if user is participant in conversation
        var isParticipant = await _messagingService
            .IsParticipant(userId, message.ConversationId);
        
        // Check if user is blocked
        var isBlocked = await _blockService
            .IsBlocked(message.SenderId, userId);
        
        if (isParticipant && !isBlocked)
        {
            context.Succeed(requirement);
        }
    }
}
```

## Performance Optimization

### Message Caching
```csharp
public class MessageCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    
    public async Task<List<Message>> GetConversationMessages(
        int conversationId, 
        int offset, 
        int limit)
    {
        var cacheKey = $"messages:{conversationId}:{offset}:{limit}";
        
        // Try memory cache first
        if (_memoryCache.TryGetValue(cacheKey, out List<Message> cached))
            return cached;
        
        // Try distributed cache
        var distributedCached = await _distributedCache.GetAsync<List<Message>>(cacheKey);
        if (distributedCached != null)
        {
            _memoryCache.Set(cacheKey, distributedCached, TimeSpan.FromMinutes(1));
            return distributedCached;
        }
        
        // Load from database
        var messages = await _repository.GetMessages(conversationId, offset, limit);
        
        // Cache results
        await _distributedCache.SetAsync(cacheKey, messages, TimeSpan.FromMinutes(5));
        _memoryCache.Set(cacheKey, messages, TimeSpan.FromMinutes(1));
        
        return messages;
    }
}
```

### Connection Management
```csharp
public class ConnectionMapping
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _connections;
    
    public async Task Add(string userId, string connectionId)
    {
        _connections.AddOrUpdate(userId,
            new HashSet<string> { connectionId },
            (key, connections) =>
            {
                connections.Add(connectionId);
                return connections;
            });
    }
    
    public async Task<bool> HasConnections(string userId)
    {
        return _connections.TryGetValue(userId, out var connections) 
            && connections.Any();
    }
}
```

## Monitoring & Analytics

### Messaging Metrics
```csharp
public class MessagingAnalytics
{
    public async Task TrackMessage(Message message)
    {
        await _analytics.TrackEvent("MessageSent", new
        {
            ConversationId = message.ConversationId,
            MessageType = message.Type,
            HasAttachments = message.Attachments?.Any() ?? false,
            MessageLength = message.Content?.Length ?? 0,
            ResponseTime = CalculateResponseTime(message)
        });
    }
    
    public async Task TrackEngagement(string userId)
    {
        var metrics = new EngagementMetrics
        {
            DailyMessages = await GetDailyMessageCount(userId),
            ActiveConversations = await GetActiveConversations(userId),
            AverageResponseTime = await GetAverageResponseTime(userId),
            PreferredMessageType = await GetPreferredMessageType(userId)
        };
        
        await _analytics.TrackMetrics("UserEngagement", metrics);
    }
}
```

## Testing Strategy

### SignalR Testing
```csharp
[TestClass]
public class MessagingHubTests
{
    private TestServer _server;
    private HubConnection _connection;
    
    [TestMethod]
    public async Task SendMessage_ValidMessage_BroadcastsToGroup()
    {
        // Arrange
        var receivedMessage = new TaskCompletionSource<Message>();
        _connection.On<Message>("ReceiveMessage", msg => 
            receivedMessage.SetResult(msg));
        
        // Act
        await _connection.InvokeAsync("SendMessage", new SendMessageDto
        {
            ConversationId = 1,
            Content = "Test message"
        });
        
        // Assert
        var message = await receivedMessage.Task;
        Assert.AreEqual("Test message", message.Content);
    }
}
```

### Load Testing
- Target: 10,000 concurrent connections
- Message throughput: 1000 messages/second
- Latency: < 50ms for message delivery
- File upload: Support 10MB files