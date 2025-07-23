# Code Validation Report
**Files Checked**: 592
**Compliance Score**: 98.6/100
**Total Violations**: 62

## Summary
- **Errors**: 28
- **Warnings**: 34
- **Info**: 0

## Violations
### ERROR: Error Issues
**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:8`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: SubscriptionDto, PaymentMethodDto, BillingHistoryDto, AddPaymentMethodDto, ChangePlanDto, TaxInfoDto, DeleteAccountDto
**Suggestion**: Move class "SubscriptionDto" to its own file: SubscriptionDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:23`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: SubscriptionDto, PaymentMethodDto, BillingHistoryDto, AddPaymentMethodDto, ChangePlanDto, TaxInfoDto, DeleteAccountDto
**Suggestion**: Move class "PaymentMethodDto" to its own file: PaymentMethodDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:37`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: SubscriptionDto, PaymentMethodDto, BillingHistoryDto, AddPaymentMethodDto, ChangePlanDto, TaxInfoDto, DeleteAccountDto
**Suggestion**: Move class "BillingHistoryDto" to its own file: BillingHistoryDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:51`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: SubscriptionDto, PaymentMethodDto, BillingHistoryDto, AddPaymentMethodDto, ChangePlanDto, TaxInfoDto, DeleteAccountDto
**Suggestion**: Move class "AddPaymentMethodDto" to its own file: AddPaymentMethodDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:60`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: SubscriptionDto, PaymentMethodDto, BillingHistoryDto, AddPaymentMethodDto, ChangePlanDto, TaxInfoDto, DeleteAccountDto
**Suggestion**: Move class "ChangePlanDto" to its own file: ChangePlanDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:69`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: SubscriptionDto, PaymentMethodDto, BillingHistoryDto, AddPaymentMethodDto, ChangePlanDto, TaxInfoDto, DeleteAccountDto
**Suggestion**: Move class "TaxInfoDto" to its own file: TaxInfoDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:80`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: SubscriptionDto, PaymentMethodDto, BillingHistoryDto, AddPaymentMethodDto, ChangePlanDto, TaxInfoDto, DeleteAccountDto
**Suggestion**: Move class "DeleteAccountDto" to its own file: DeleteAccountDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\API\UserSettingsApiController.cs:100`
**Rule**: no_incomplete_implementations
**Message**: Incomplete implementation: TODO comment found
**Suggestion**: Complete the implementation before committing

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\API\UserSettingsApiController.cs:123`
**Rule**: no_incomplete_implementations
**Message**: Incomplete implementation: TODO comment found
**Suggestion**: Complete the implementation before committing

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\API\UserSettingsApiController.cs:346`
**Rule**: no_incomplete_implementations
**Message**: Incomplete implementation: TODO comment found
**Suggestion**: Complete the implementation before committing

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\API\UserSettingsApiController.cs:497`
**Rule**: no_incomplete_implementations
**Message**: Incomplete implementation: TODO comment found
**Suggestion**: Complete the implementation before committing

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\TwoFactorAuthDto.cs:7`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: Enable2FADto, Setup2FAResponseDto, Verify2FADto, Disable2FADto, TwoFactorStatusDto
**Suggestion**: Move class "Enable2FADto" to its own file: Enable2FADto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\TwoFactorAuthDto.cs:21`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: Enable2FADto, Setup2FAResponseDto, Verify2FADto, Disable2FADto, TwoFactorStatusDto
**Suggestion**: Move class "Setup2FAResponseDto" to its own file: Setup2FAResponseDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\TwoFactorAuthDto.cs:33`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: Enable2FADto, Setup2FAResponseDto, Verify2FADto, Disable2FADto, TwoFactorStatusDto
**Suggestion**: Move class "Verify2FADto" to its own file: Verify2FADto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\TwoFactorAuthDto.cs:44`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: Enable2FADto, Setup2FAResponseDto, Verify2FADto, Disable2FADto, TwoFactorStatusDto
**Suggestion**: Move class "Disable2FADto" to its own file: Disable2FADto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\TwoFactorAuthDto.cs:56`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: Enable2FADto, Setup2FAResponseDto, Verify2FADto, Disable2FADto, TwoFactorStatusDto
**Suggestion**: Move class "TwoFactorStatusDto" to its own file: TwoFactorStatusDto.cs

**File**: `src\Web\MeAndMyDog.WebApp\Services\ApiRoleNavigationService.cs:55`
**Rule**: no_incomplete_implementations
**Message**: Incomplete implementation: TODO comment found
**Suggestion**: Complete the implementation before committing

**File**: `src\Web\MeAndMyDog.WebApp\Services\ApiAuthService.cs:36`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: ApiAuthService, UserInfo, ProviderEligibilityInfo, ApiResponse
**Suggestion**: Move class "ApiAuthService" to its own file: ApiAuthService.cs

**File**: `src\Web\MeAndMyDog.WebApp\Services\ApiAuthService.cs:145`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: ApiAuthService, UserInfo, ProviderEligibilityInfo, ApiResponse
**Suggestion**: Move class "UserInfo" to its own file: UserInfo.cs

**File**: `src\Web\MeAndMyDog.WebApp\Services\ApiAuthService.cs:158`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: ApiAuthService, UserInfo, ProviderEligibilityInfo, ApiResponse
**Suggestion**: Move class "ProviderEligibilityInfo" to its own file: ProviderEligibilityInfo.cs

**File**: `src\Web\MeAndMyDog.WebApp\Services\ApiAuthService.cs:168`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: ApiAuthService, UserInfo, ProviderEligibilityInfo, ApiResponse
**Suggestion**: Move class "ApiResponse" to its own file: ApiResponse.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\NavigationModels.cs:2`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: UserNavigationContext, UserRole, NavigationMenuItem
**Suggestion**: Move class "UserNavigationContext" to its own file: UserNavigationContext.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\NavigationModels.cs:38`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: UserNavigationContext, UserRole, NavigationMenuItem
**Suggestion**: Move class "UserRole" to its own file: UserRole.cs

**File**: `src\Web\MeAndMyDog.WebApp\Models\NavigationModels.cs:79`
**Rule**: class_single_per_file
**Message**: Multiple public classes found in file: UserNavigationContext, UserRole, NavigationMenuItem
**Suggestion**: Move class "NavigationMenuItem" to its own file: NavigationMenuItem.cs

**File**: `src\API\MeAndMyDog.API\Services\Implementations\ProviderUpgradeService.cs:109`
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

### WARNING: Warning Issues
**File**: `src\API\MeAndMyDog.API\Controllers\DashboardController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardController"

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\BillingDtos.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DeleteAccountDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DeleteAccountDto"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\API\UserSettingsApiController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "UserSettingsApiController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "UserSettingsApiController"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\DashboardController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardController"

**File**: `src\API\MeAndMyDog.API\Migrations\20250723142013_AccountSettings.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AccountSettings" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AccountSettings"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\AccountSettingsController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AccountSettingsController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AccountSettingsController"

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\TwoFactorAuthDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "TwoFactorStatusDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "TwoFactorStatusDto"

**File**: `src\Web\MeAndMyDog.WebApp\Models\DTOs\UserProfileDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "UserProfileDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "UserProfileDto"

**File**: `src\API\MeAndMyDog.API\Migrations\20250723104348_ProviderDashboardV2.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "ProviderDashboardV2" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "ProviderDashboardV2"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\AuthController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AuthController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AuthController"

**File**: `src\Web\MeAndMyDog.WebApp\Controllers\PetHealthController.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "PetHealthController" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "PetHealthController"

**File**: `src\API\MeAndMyDog.API\Migrations\20250723071109_ProviderDashboard.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "ProviderDashboard" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "ProviderDashboard"

**File**: `src\API\MeAndMyDog.API\Migrations\20250723025408_AddProviderBusinessEntities.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "AddProviderBusinessEntities" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "AddProviderBusinessEntities"

**File**: `src\API\MeAndMyDog.API\Migrations\20250722172000_DashboardFeaturesV3.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardFeaturesV3" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardFeaturesV3"

**File**: `src\API\MeAndMyDog.API\Migrations\20250722170927_DashboardEnhancements.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardEnhancements" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardEnhancements"

**File**: `src\API\MeAndMyDog.API\Migrations\20250722165018_DashboardFeaturesV2.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardFeaturesV2" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardFeaturesV2"

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

**File**: `tests\MeAndMyDog.API.MigrationTests\MigrationIntegrationTests.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "MigrationIntegrationTests" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "MigrationIntegrationTests"

**File**: `src\Web\MeAndMyDog.WebApp\Hubs\DashboardHub.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "DashboardHub" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "DashboardHub"

**File**: `src\API\MeAndMyDog.API\Models\DTOs\Friends\FriendDto.cs:1`
**Rule**: xml_documentation_required
**Message**: Public class "FriendDto" missing XML documentation
**Suggestion**: Add /// <summary> documentation above class "FriendDto"

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
