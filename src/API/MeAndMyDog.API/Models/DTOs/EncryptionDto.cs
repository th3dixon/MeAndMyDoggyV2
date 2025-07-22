// This file has been refactored - all classes have been moved to separate files in the Encryption/ folder.
//
// Classes moved to individual files:
// - MessageEncryptionDto -> Encryption/MessageEncryptionDto.cs
// - UserEncryptionKeyDto -> Encryption/UserEncryptionKeyDto.cs
// - EncryptMessageRequest -> Encryption/EncryptMessageRequest.cs
// - EncryptedMessageResponse -> Encryption/EncryptedMessageResponse.cs
// - DecryptMessageRequest -> Encryption/DecryptMessageRequest.cs
// - DecryptedMessageResponse -> Encryption/DecryptedMessageResponse.cs
// - GenerateKeyPairRequest -> Encryption/GenerateKeyPairRequest.cs
// - GenerateKeyPairResponse -> Encryption/GenerateKeyPairResponse.cs
// - RotateConversationKeyRequest -> Encryption/RotateConversationKeyRequest.cs
// - RevokeKeyRequest -> Encryption/RevokeKeyRequest.cs
// - EncryptionSettings -> Encryption/EncryptionSettings.cs
//
// This file can be deleted once all references are updated.