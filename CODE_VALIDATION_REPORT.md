# Code Validation Report
**Files Checked**: 432
**Compliance Score**: 98.6/100
**Total Violations**: 38

## Summary
- **Errors**: 16
- **Warnings**: 22
- **Info**: 0

## Violations
### ERROR: Error Issues
**File**: `src\API\MeAndMyDog.API\Controllers\DashboardController.cs:13`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: DashboardController, DashboardPreferences
**Suggestion**: Move class "DashboardController" to its own file: DashboardController.cs

**File**: `src\API\MeAndMyDog.API\Controllers\DashboardController.cs:550`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: DashboardController, DashboardPreferences
**Suggestion**: Move class "DashboardPreferences" to its own file: DashboardPreferences.cs

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\DashboardController.cs:308`
**Rule**: no_incomplete_implementations
**Message**: Incomplete implementation: TODO comment found
**Suggestion**: Complete the implementation before committing

**File**: `src\API\MeAndMyDog.API\Services\FriendshipValidationService.cs:6`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipValidationService, ConversationValidationResult, FriendshipStatusInfo
**Suggestion**: Move class "FriendshipValidationService" to its own file: FriendshipValidationService.cs

**File**: `src\API\MeAndMyDog.API\Services\FriendshipValidationService.cs:208`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipValidationService, ConversationValidationResult, FriendshipStatusInfo
**Suggestion**: Move class "ConversationValidationResult" to its own file: ConversationValidationResult.cs

**File**: `src\API\MeAndMyDog.API\Services\FriendshipValidationService.cs:229`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipValidationService, ConversationValidationResult, FriendshipStatusInfo
**Suggestion**: Move class "FriendshipStatusInfo" to its own file: FriendshipStatusInfo.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendDto.cs:5`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendDto, FriendRequestDto, UserBasicInfoDto
**Suggestion**: Move class "FriendDto" to its own file: FriendDto.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendDto.cs:61`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendDto, FriendRequestDto, UserBasicInfoDto
**Suggestion**: Move class "FriendRequestDto" to its own file: FriendRequestDto.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendDto.cs:102`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendDto, FriendRequestDto, UserBasicInfoDto
**Suggestion**: Move class "UserBasicInfoDto" to its own file: UserBasicInfoDto.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Common\ApiResponse.cs:6`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: ApiResponse, ApiResponse
**Suggestion**: Move class "ApiResponse" to its own file: ApiResponse.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Common\ApiResponse.cs:77`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: ApiResponse, ApiResponse
**Suggestion**: Move class "ApiResponse" to its own file: ApiResponse.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendshipDto.cs:4`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipDto, SendFriendRequestDto, FriendRequestResponseDto, FriendCodeLookupDto, FriendsListResponse
**Suggestion**: Move class "FriendshipDto" to its own file: FriendshipDto.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendshipDto.cs:55`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipDto, SendFriendRequestDto, FriendRequestResponseDto, FriendCodeLookupDto, FriendsListResponse
**Suggestion**: Move class "SendFriendRequestDto" to its own file: SendFriendRequestDto.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendshipDto.cs:71`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipDto, SendFriendRequestDto, FriendRequestResponseDto, FriendCodeLookupDto, FriendsListResponse
**Suggestion**: Move class "FriendRequestResponseDto" to its own file: FriendRequestResponseDto.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendshipDto.cs:87`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipDto, SendFriendRequestDto, FriendRequestResponseDto, FriendCodeLookupDto, FriendsListResponse
**Suggestion**: Move class "FriendCodeLookupDto" to its own file: FriendCodeLookupDto.cs

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendshipDto.cs:123`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: FriendshipDto, SendFriendRequestDto, FriendRequestResponseDto, FriendCodeLookupDto, FriendsListResponse
**Suggestion**: Move class "FriendsListResponse" to its own file: FriendsListResponse.cs

### WARNING: Warning Issues
**File**: `src\API\MeAndMyDog.API\Migrations\20250720151421_AddAuthServiceAndRoleEntities.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AddAuthServiceAndRoleEntities" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AddAuthServiceAndRoleEntities"

**File**: `src\API\MeAndMyDog.API\Migrations\20250722160051_DashboardFeatures.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardFeatures" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardFeatures"

**File**: `src\API\MeAndMyDog.API\Migrations\20250722081837_MessagingSystem.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "MessagingSystem" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "MessagingSystem"

**File**: `src\API\MeAndMyDog.API\Migrations\20250721112907_PostcodeWork.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "PostcodeWork" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "PostcodeWork"

**File**: `src\API\MeAndMyDog.API\Migrations\20250721100130_IsPremiumScript.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "IsPremiumScript" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "IsPremiumScript"

**File**: `src\API\MeAndMyDog.API\Controllers\DashboardController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardPreferences" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardPreferences"

**File**: `tests\MeAndMyDog.API.MigrationTests\MigrationIntegrationTests.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "MigrationIntegrationTests" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "MigrationIntegrationTests"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\DashboardController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardController"

**File**: `src\Web\MeAndMyDog.WebApp\Hubs\DashboardHub.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardHub" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardHub"

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "UserBasicInfoDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "UserBasicInfoDto"

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Common\ApiResponse.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "ApiResponse" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "ApiResponse"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\AuthProxyController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AuthProxyController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AuthProxyController"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\MessagingProxyController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "MessagingProxyController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "MessagingProxyController"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\AuthController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AuthController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AuthController"

**File**: `src\API\MeAndMyDog.API\DTOs\Address\AddressSearchResultDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AddressSearchResultDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AddressSearchResultDto"

**File**: `src\API\MeAndMyDog.API\DTOs\Address\PostcodeInfoDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "PostcodeInfoDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "PostcodeInfoDto"

**File**: `src\API\MeAndMyDog.API\DTOs\Address\CitySearchResultDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "CitySearchResultDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "CitySearchResultDto"

**File**: `src\API\MeAndMyDog.API\DTOs\Address\AddressDetailDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AddressDetailDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AddressDetailDto"

**File**: `src\API\MeAndMyDog.API\Services\Implementations\AddressLookupService.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AddressLookupService" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AddressLookupService"

**File**: `src\API\MeAndMyDog.API\Controllers\AddressLookupController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AddressLookupController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AddressLookupController"

**File**: `tests\messaging-system-verification.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "MessagingSystemVerification" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "MessagingSystemVerification"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\AddressLookupProxyController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AddressLookupProxyController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AddressLookupProxyController"
