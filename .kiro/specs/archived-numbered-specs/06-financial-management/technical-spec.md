# Financial Management System - Technical Specification

## Component Overview
The Financial Management System provides comprehensive financial tools for service providers including invoicing, expense tracking, payment processing via Stripe, financial reporting, and tax compliance features tailored for UK businesses.

## Database Schema

### Financial Management Tables

```sql
-- FinancialAccounts
CREATE TABLE [dbo].[FinancialAccounts] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL UNIQUE,
    [StripeCustomerId] NVARCHAR(100) NULL,
    [StripeAccountId] NVARCHAR(100) NULL, -- For Connect accounts
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'GBP',
    [Balance] DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    [PendingBalance] DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    [AvailableBalance] DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    [VATRegistered] BIT NOT NULL DEFAULT 0,
    [VATNumber] NVARCHAR(20) NULL,
    [TaxYear] INT NOT NULL DEFAULT 6, -- UK tax year starts April 6
    [AccountStatus] INT NOT NULL DEFAULT 0, -- 0: Active, 1: Suspended, 2: Closed
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_FinancialAccounts_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);

-- Transactions
CREATE TABLE [dbo].[Transactions] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AccountId] INT NOT NULL,
    [TransactionType] INT NOT NULL, -- 0: Payment, 1: Refund, 2: Payout, 3: Fee, 4: Adjustment
    [Category] INT NOT NULL, -- 0: Booking, 1: Subscription, 2: Other
    [Amount] DECIMAL(18, 2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'GBP',
    [Status] INT NOT NULL, -- 0: Pending, 1: Completed, 2: Failed, 3: Cancelled
    [ReferenceType] NVARCHAR(50) NULL, -- Booking, Invoice, Subscription
    [ReferenceId] NVARCHAR(100) NULL,
    [StripeTransactionId] NVARCHAR(200) NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Metadata] NVARCHAR(MAX) NULL, -- JSON
    [ProcessedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_Transactions_AccountId_CreatedAt] ([AccountId], [CreatedAt] DESC),
    INDEX [IX_Transactions_ReferenceType_ReferenceId] ([ReferenceType], [ReferenceId]),
    CONSTRAINT [FK_Transactions_FinancialAccounts] FOREIGN KEY ([AccountId]) REFERENCES [FinancialAccounts]([Id])
);

-- Invoices (Extended)
CREATE TABLE [dbo].[Invoices] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [InvoiceNumber] NVARCHAR(50) NOT NULL UNIQUE,
    [ProviderId] INT NOT NULL,
    [CustomerId] NVARCHAR(450) NULL,
    [CustomerType] INT NOT NULL, -- 0: RegisteredUser, 1: Guest
    [CustomerDetails] NVARCHAR(MAX) NOT NULL, -- JSON (name, email, address)
    [BookingId] INT NULL,
    [Status] INT NOT NULL, -- 0: Draft, 1: Sent, 2: Paid, 3: PartiallyPaid, 4: Overdue, 5: Cancelled, 6: Refunded
    [Type] INT NOT NULL, -- 0: Standard, 1: Recurring, 2: CreditNote
    [IssueDate] DATE NOT NULL,
    [DueDate] DATE NOT NULL,
    [PaidDate] DATE NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'GBP',
    [SubTotal] DECIMAL(18, 2) NOT NULL,
    [DiscountAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    [TaxAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    [TotalAmount] DECIMAL(18, 2) NOT NULL,
    [PaidAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    [OutstandingAmount] AS ([TotalAmount] - [PaidAmount]) PERSISTED,
    [Notes] NVARCHAR(MAX) NULL,
    [Terms] NVARCHAR(MAX) NULL,
    [Footer] NVARCHAR(MAX) NULL,
    [StripeInvoiceId] NVARCHAR(100) NULL,
    [PaymentLink] NVARCHAR(500) NULL,
    [PdfUrl] NVARCHAR(500) NULL,
    [SentAt] DATETIME2 NULL,
    [ViewedAt] DATETIME2 NULL,
    [ReminderCount] INT NOT NULL DEFAULT 0,
    [LastReminderAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_Invoices_ProviderId_Status] ([ProviderId], [Status]),
    INDEX [IX_Invoices_DueDate] ([DueDate]),
    CONSTRAINT [FK_Invoices_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_Invoices_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_Invoices_Bookings] FOREIGN KEY ([BookingId]) REFERENCES [Bookings]([Id])
);

-- InvoiceItems (Extended)
CREATE TABLE [dbo].[InvoiceItems] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [InvoiceId] INT NOT NULL,
    [ItemType] INT NOT NULL, -- 0: Service, 1: Product, 2: Expense, 3: Tax, 4: Discount
    [ServiceId] INT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Quantity] DECIMAL(10, 2) NOT NULL,
    [Unit] NVARCHAR(50) NULL, -- hours, sessions, etc.
    [UnitPrice] DECIMAL(18, 2) NOT NULL,
    [TaxRate] DECIMAL(5, 2) NOT NULL DEFAULT 20.00, -- UK VAT
    [TaxAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0.00,
    [DiscountPercentage] DECIMAL(5, 2) NULL,
    [DiscountAmount] DECIMAL(18, 2) NULL,
    [LineTotal] DECIMAL(18, 2) NOT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_InvoiceItems_Invoices] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices]([Id]),
    CONSTRAINT [FK_InvoiceItems_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([Id])
);

-- Expenses
CREATE TABLE [dbo].[Expenses] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [CategoryId] INT NOT NULL,
    [Date] DATE NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [VATAmount] DECIMAL(18, 2) NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Vendor] NVARCHAR(200) NULL,
    [ReferenceNumber] NVARCHAR(100) NULL,
    [ReceiptUrl] NVARCHAR(500) NULL,
    [PaymentMethod] INT NOT NULL, -- 0: Cash, 1: Card, 2: BankTransfer, 3: Other
    [IsBillable] BIT NOT NULL DEFAULT 0,
    [BilledToInvoiceId] INT NULL,
    [IsRecurring] BIT NOT NULL DEFAULT 0,
    [RecurrencePattern] NVARCHAR(MAX) NULL, -- JSON
    [Tags] NVARCHAR(MAX) NULL, -- JSON array
    [Notes] NVARCHAR(MAX) NULL,
    [Status] INT NOT NULL DEFAULT 0, -- 0: Pending, 1: Approved, 2: Rejected
    [ApprovedBy] NVARCHAR(450) NULL,
    [ApprovedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    INDEX [IX_Expenses_ProviderId_Date] ([ProviderId], [Date]),
    INDEX [IX_Expenses_CategoryId] ([CategoryId]),
    CONSTRAINT [FK_Expenses_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_Expenses_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [ExpenseCategories]([Id]),
    CONSTRAINT [FK_Expenses_Invoices] FOREIGN KEY ([BilledToInvoiceId]) REFERENCES [Invoices]([Id]),
    CONSTRAINT [FK_Expenses_ApprovedBy] FOREIGN KEY ([ApprovedBy]) REFERENCES [AspNetUsers]([Id])
);

-- ExpenseCategories
CREATE TABLE [dbo].[ExpenseCategories] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [TaxDeductible] BIT NOT NULL DEFAULT 1,
    [DefaultVATRate] DECIMAL(5, 2) NULL,
    [ParentCategoryId] INT NULL,
    [Icon] NVARCHAR(50) NULL,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [IsSystem] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ExpenseCategories_Parent] FOREIGN KEY ([ParentCategoryId]) REFERENCES [ExpenseCategories]([Id])
);

-- PaymentMethods
CREATE TABLE [dbo].[PaymentMethods] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [Type] INT NOT NULL, -- 0: BankAccount, 1: Card, 2: PayPal
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [StripePaymentMethodId] NVARCHAR(100) NULL,
    [BankName] NVARCHAR(100) NULL,
    [AccountHolderName] NVARCHAR(200) NULL,
    [AccountNumberLast4] NVARCHAR(4) NULL,
    [SortCode] NVARCHAR(10) NULL, -- Masked
    [CardBrand] NVARCHAR(50) NULL,
    [CardLast4] NVARCHAR(4) NULL,
    [CardExpMonth] INT NULL,
    [CardExpYear] INT NULL,
    [PayPalEmail] NVARCHAR(256) NULL,
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [VerifiedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_PaymentMethods_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);

-- Payouts
CREATE TABLE [dbo].[Payouts] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [PaymentMethodId] INT NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'GBP',
    [Status] INT NOT NULL, -- 0: Pending, 1: Processing, 2: Completed, 3: Failed, 4: Cancelled
    [StripePayoutId] NVARCHAR(100) NULL,
    [ProcessingFee] DECIMAL(18, 2) NULL,
    [NetAmount] DECIMAL(18, 2) NOT NULL,
    [ScheduledFor] DATE NOT NULL,
    [ProcessedAt] DATETIME2 NULL,
    [FailureReason] NVARCHAR(500) NULL,
    [TransactionIds] NVARCHAR(MAX) NULL, -- JSON array
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Payouts_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_Payouts_PaymentMethods] FOREIGN KEY ([PaymentMethodId]) REFERENCES [PaymentMethods]([Id])
);

-- TaxDocuments
CREATE TABLE [dbo].[TaxDocuments] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [DocumentType] INT NOT NULL, -- 0: P60, 1: P45, 2: VAT Return, 3: Self Assessment
    [TaxYear] INT NOT NULL,
    [Period] NVARCHAR(50) NULL, -- Q1, Q2, etc. for VAT returns
    [DocumentUrl] NVARCHAR(500) NOT NULL,
    [Status] INT NOT NULL, -- 0: Draft, 1: Submitted, 2: Accepted, 3: Rejected
    [SubmittedAt] DATETIME2 NULL,
    [DueDate] DATE NULL,
    [TotalIncome] DECIMAL(18, 2) NULL,
    [TotalExpenses] DECIMAL(18, 2) NULL,
    [TaxableAmount] DECIMAL(18, 2) NULL,
    [TaxAmount] DECIMAL(18, 2) NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_TaxDocuments_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);

-- FinancialReports
CREATE TABLE [dbo].[FinancialReports] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [ReportType] INT NOT NULL, -- 0: ProfitLoss, 1: CashFlow, 2: Balance, 3: Tax, 4: Custom
    [PeriodType] INT NOT NULL, -- 0: Monthly, 1: Quarterly, 2: Yearly, 3: Custom
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [GeneratedAt] DATETIME2 NOT NULL,
    [ReportData] NVARCHAR(MAX) NOT NULL, -- JSON
    [FileUrl] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_FinancialReports_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id])
);

-- RecurringInvoices
CREATE TABLE [dbo].[RecurringInvoices] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProviderId] INT NOT NULL,
    [CustomerId] NVARCHAR(450) NOT NULL,
    [TemplateInvoiceId] INT NOT NULL,
    [Frequency] INT NOT NULL, -- 0: Weekly, 1: Monthly, 2: Quarterly, 3: Yearly
    [NextInvoiceDate] DATE NOT NULL,
    [EndDate] DATE NULL,
    [OccurrencesRemaining] INT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastGeneratedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_RecurringInvoices_ServiceProviders] FOREIGN KEY ([ProviderId]) REFERENCES [ServiceProviders]([Id]),
    CONSTRAINT [FK_RecurringInvoices_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [FK_RecurringInvoices_Template] FOREIGN KEY ([TemplateInvoiceId]) REFERENCES [Invoices]([Id])
);
```

## API Endpoints

### Account Management

```yaml
/api/v1/financial/account:
  /:
    GET:
      description: Get financial account overview
      auth: required (provider)
      responses:
        200:
          account: FinancialAccount
          balance: AccountBalance
          pendingPayouts: array[Payout]
          recentActivity: array[Transaction]

    POST:
      description: Initialize financial account
      auth: required (provider)
      body:
        currency: string
        vatRegistered: boolean
        vatNumber: string # If VAT registered
      responses:
        201:
          accountId: number
          stripeAccountUrl: string # Onboarding URL

  /balance:
    GET:
      description: Get detailed balance information
      auth: required (provider)
      responses:
        200:
          available: number
          pending: number
          reserved: number
          payoutSchedule: object
          breakdown: object # By source

  /verify:
    POST:
      description: Verify bank account
      auth: required (provider)
      body:
        microDeposits: array[number] # Two small amounts
      responses:
        200:
          verified: boolean
          paymentMethodId: number
```

### Invoice Management

```yaml
/api/v1/financial/invoices:
  /:
    GET:
      description: Get invoices
      auth: required
      parameters:
        status: enum [all, draft, sent, paid, overdue]
        startDate: date
        endDate: date
        customerId: string
        page: number
        pageSize: number
      responses:
        200:
          invoices: array[InvoiceSummary]
          summary:
            totalAmount: number
            paidAmount: number
            overdueAmount: number
            count: object # By status

    POST:
      description: Create invoice
      auth: required (provider)
      body:
        customerId: string # Optional for guest
        customerDetails:
          name: string
          email: string
          address: object
        bookingId: number # Optional
        items: array[{
          description: string
          quantity: number
          unitPrice: number
          taxRate: number
        }]
        dueDate: date
        notes: string
        terms: string
      responses:
        201:
          invoiceId: number
          invoiceNumber: string
          totalAmount: number

  /{invoiceId}:
    GET:
      description: Get invoice details
      auth: required
      responses:
        200:
          invoice: InvoiceDetail
          items: array[InvoiceItem]
          payments: array[Payment]
          history: array[InvoiceEvent]

    PUT:
      description: Update draft invoice
      auth: required (provider)
      body: InvoiceUpdateDto
      responses:
        200: Updated invoice

    DELETE:
      description: Delete draft invoice
      auth: required (provider)
      responses:
        204: Deleted

  /{invoiceId}/send:
    POST:
      description: Send invoice to customer
      auth: required (provider)
      body:
        emailMessage: string # Optional custom message
        cc: array[string] # Additional recipients
      responses:
        200:
          sent: boolean
          paymentLink: string

  /{invoiceId}/payment:
    POST:
      description: Record payment
      auth: required
      body:
        amount: number
        paymentMethod: string
        paymentDate: date
        reference: string
      responses:
        200:
          paymentId: number
          remainingBalance: number

  /{invoiceId}/reminder:
    POST:
      description: Send payment reminder
      auth: required (provider)
      body:
        message: string # Optional
      responses:
        200:
          sent: boolean
          reminderCount: number

  /templates:
    GET:
      description: Get invoice templates
      auth: required (provider)
      responses:
        200:
          templates: array[InvoiceTemplate]

    POST:
      description: Save invoice template
      auth: required (provider)
      body:
        name: string
        items: array[InvoiceItem]
        notes: string
        terms: string
      responses:
        201:
          templateId: number
```

### Expense Tracking

```yaml
/api/v1/financial/expenses:
  /:
    GET:
      description: Get expenses
      auth: required (provider)
      parameters:
        startDate: date
        endDate: date
        categoryId: number
        status: enum [all, pending, approved, rejected]
        billable: boolean
        page: number
        pageSize: number
      responses:
        200:
          expenses: array[Expense]
          summary:
            totalAmount: number
            byCategory: object
            taxDeductible: number

    POST:
      description: Record expense
      auth: required (provider)
      body:
        date: date
        amount: number
        vatAmount: number # Optional
        categoryId: number
        description: string
        vendor: string
        receiptFile: file # Optional
        billable: boolean
        tags: array[string]
      responses:
        201:
          expenseId: number

  /{expenseId}:
    GET:
      description: Get expense details
      auth: required (provider)
      responses:
        200: Expense details

    PUT:
      description: Update expense
      auth: required (provider)
      body: ExpenseUpdateDto
      responses:
        200: Updated expense

    DELETE:
      description: Delete expense
      auth: required (provider)
      responses:
        204: Deleted

  /categories:
    GET:
      description: Get expense categories
      auth: required
      responses:
        200:
          categories: array[ExpenseCategory]
          customCategories: array[ExpenseCategory]

  /bulk-upload:
    POST:
      description: Bulk upload expenses via CSV
      auth: required (provider)
      contentType: multipart/form-data
      body:
        file: file # CSV file
        mappings: object # Column mappings
      responses:
        200:
          imported: number
          failed: number
          errors: array[ImportError]
```

### Payment Processing

```yaml
/api/v1/financial/payments:
  /methods:
    GET:
      description: Get payment methods
      auth: required (provider)
      responses:
        200:
          methods: array[PaymentMethod]
          defaultMethodId: number

    POST:
      description: Add payment method
      auth: required (provider)
      body:
        type: enum [bank_account, card]
        stripeToken: string # From Stripe.js
      responses:
        201:
          methodId: number
          requiresVerification: boolean

  /methods/{methodId}:
    DELETE:
      description: Remove payment method
      auth: required (provider)
      responses:
        204: Removed

    POST:
      description: Set as default
      auth: required (provider)
      responses:
        200: Set as default

  /charge:
    POST:
      description: Charge customer for booking
      auth: required (provider)
      body:
        bookingId: number
        amount: number
        description: string
        paymentMethodId: string # Customer's payment method
      responses:
        200:
          chargeId: string
          status: string
          receiptUrl: string

  /refund:
    POST:
      description: Issue refund
      auth: required (provider)
      body:
        chargeId: string
        amount: number # Partial refund if less than original
        reason: string
      responses:
        200:
          refundId: string
          status: string
```

### Payouts & Withdrawals

```yaml
/api/v1/financial/payouts:
  /:
    GET:
      description: Get payout history
      auth: required (provider)
      parameters:
        status: enum [all, pending, completed, failed]
        startDate: date
        endDate: date
      responses:
        200:
          payouts: array[Payout]
          nextPayout: PayoutSchedule

    POST:
      description: Request manual payout
      auth: required (provider)
      body:
        amount: number
        paymentMethodId: number
      responses:
        201:
          payoutId: number
          estimatedArrival: date

  /schedule:
    GET:
      description: Get payout schedule
      auth: required (provider)
      responses:
        200:
          schedule: PayoutSchedule
          canModify: boolean

    PUT:
      description: Update payout schedule
      auth: required (provider)
      body:
        frequency: enum [daily, weekly, monthly]
        dayOfWeek: number # For weekly
        dayOfMonth: number # For monthly
      responses:
        200: Updated schedule
```

### Financial Reports

```yaml
/api/v1/financial/reports:
  /profit-loss:
    GET:
      description: Generate profit & loss report
      auth: required (provider)
      parameters:
        startDate: date
        endDate: date
        comparison: boolean # Compare to previous period
      responses:
        200:
          income: IncomeBreakdown
          expenses: ExpenseBreakdown
          netProfit: number
          comparison: ComparisonData
          downloadUrl: string # PDF

  /cash-flow:
    GET:
      description: Generate cash flow report
      auth: required (provider)
      parameters:
        period: enum [week, month, quarter, year]
      responses:
        200:
          inflowS: array[CashFlowItem]
          outflows: array[CashFlowItem]
          netFlow: number
          projections: array[Projection]

  /tax-summary:
    GET:
      description: Generate tax summary
      auth: required (provider)
      parameters:
        taxYear: number
      responses:
        200:
          income: TaxableIncome
          expenses: DeductibleExpenses
          vatSummary: VATSummary
          estimatedTax: number
          documents: array[TaxDocument]

  /export:
    POST:
      description: Export financial data
      auth: required (provider)
      body:
        type: enum [transactions, invoices, expenses]
        format: enum [csv, excel, pdf]
        startDate: date
        endDate: date
        filters: object
      responses:
        200:
          downloadUrl: string
          expiresAt: datetime
```

## Frontend Components

### Financial Dashboard Components (Vue.js)

```typescript
// FinancialDashboard.vue
interface FinancialDashboardProps {
  providerId: number
  period: 'day' | 'week' | 'month' | 'year'
}

// Sections:
// - RevenueOverview.vue
// - ExpensesSummary.vue
// - CashFlowChart.vue
// - PendingInvoices.vue
// - UpcomingPayouts.vue
// - QuickActions.vue

// RevenueOverview.vue
interface RevenueOverviewProps {
  period: DateRange
  comparison: boolean
}

// Displays:
// - Total revenue
// - Growth percentage
// - Revenue by service
// - Top customers
```

### Invoice Components

```typescript
// InvoiceBuilder.vue
interface InvoiceBuilderProps {
  invoice?: Invoice // For editing
  bookingId?: number // Pre-fill from booking
  customerId?: string
}

// Features:
// - Line item management
// - Tax calculations
// - Discount application
// - Preview mode
// - Template selection

// InvoicePreview.vue
interface InvoicePreviewProps {
  invoice: Invoice
  editable: boolean
}

// Shows:
// - Professional invoice layout
// - Company branding
// - Payment instructions
// - Download/print options
```

### Expense Management Components

```typescript
// ExpenseTracker.vue
interface ExpenseTrackerProps {
  categories: ExpenseCategory[]
  defaultCategory?: number
}

// Features:
// - Quick expense entry
// - Receipt upload
// - Category selection
// - Mileage tracking
// - Recurring expenses

// ExpenseReport.vue
interface ExpenseReportProps {
  expenses: Expense[]
  period: DateRange
  groupBy: 'category' | 'date' | 'vendor'
}

// Visualizations:
// - Pie chart by category
// - Timeline view
// - Tax deductible summary
```

## Technical Implementation Details

### Stripe Integration

```csharp
public class StripePaymentService
{
    private readonly StripeClient _stripeClient;
    private readonly IConfiguration _config;
    
    public async Task<PaymentIntent> CreatePaymentIntent(
        decimal amount, 
        string currency, 
        string customerId,
        Dictionary<string, string> metadata)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Convert to pence
            Currency = currency.ToLower(),
            Customer = customerId,
            Metadata = metadata,
            SetupFutureUsage = "on_session",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        };
        
        return await _stripeClient.PaymentIntents.CreateAsync(options);
    }
    
    public async Task<Account> CreateConnectedAccount(ServiceProvider provider)
    {
        var options = new AccountCreateOptions
        {
            Type = "express",
            Country = "GB",
            Email = provider.Email,
            Capabilities = new AccountCapabilitiesOptions
            {
                CardPayments = new AccountCapabilitiesCardPaymentsOptions
                {
                    Requested = true
                },
                Transfers = new AccountCapabilitiesTransfersOptions
                {
                    Requested = true
                }
            },
            BusinessType = provider.BusinessType == BusinessType.Company 
                ? "company" 
                : "individual",
            BusinessProfile = new AccountBusinessProfileOptions
            {
                Name = provider.BusinessName,
                ProductDescription = "Pet care services",
                Mcc = "7299" // Miscellaneous personal services
            }
        };
        
        var account = await _stripeClient.Accounts.CreateAsync(options);
        
        // Generate onboarding link
        var linkOptions = new AccountLinkCreateOptions
        {
            Account = account.Id,
            RefreshUrl = $"{_config["BaseUrl"]}/financial/stripe-refresh",
            ReturnUrl = $"{_config["BaseUrl"]}/financial/stripe-return",
            Type = "account_onboarding"
        };
        
        var accountLink = await _stripeClient.AccountLinks.CreateAsync(linkOptions);
        
        return account;
    }
    
    public async Task<Transfer> CreateTransfer(
        string connectedAccountId, 
        decimal amount, 
        string chargeId)
    {
        var options = new TransferCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = "gbp",
            Destination = connectedAccountId,
            SourceTransaction = chargeId
        };
        
        return await _stripeClient.Transfers.CreateAsync(options);
    }
}

// Webhook handler
[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            _webhookSecret
        );
        
        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                await HandlePaymentSuccess(stripeEvent.Data.Object as PaymentIntent);
                break;
                
            case "invoice.payment_succeeded":
                await HandleInvoicePayment(stripeEvent.Data.Object as Invoice);
                break;
                
            case "payout.paid":
                await HandlePayoutCompleted(stripeEvent.Data.Object as Payout);
                break;
                
            case "account.updated":
                await HandleAccountUpdate(stripeEvent.Data.Object as Account);
                break;
        }
        
        return Ok();
    }
}
```

### Invoice Generation

```csharp
public class InvoiceService
{
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IEmailService _emailService;
    private readonly IBlobStorage _storage;
    
    public async Task<Invoice> CreateInvoice(CreateInvoiceDto dto)
    {
        // Calculate totals
        var subTotal = dto.Items.Sum(i => i.Quantity * i.UnitPrice);
        var taxAmount = dto.Items.Sum(i => 
            i.Quantity * i.UnitPrice * (i.TaxRate / 100));
        var totalAmount = subTotal + taxAmount - (dto.DiscountAmount ?? 0);
        
        var invoice = new Invoice
        {
            InvoiceNumber = GenerateInvoiceNumber(),
            ProviderId = dto.ProviderId,
            CustomerId = dto.CustomerId,
            CustomerDetails = JsonSerializer.Serialize(dto.CustomerDetails),
            Status = InvoiceStatus.Draft,
            IssueDate = DateTime.UtcNow.Date,
            DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(30).Date,
            SubTotal = subTotal,
            TaxAmount = taxAmount,
            TotalAmount = totalAmount,
            Items = dto.Items.Select(i => new InvoiceItem
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TaxRate = i.TaxRate,
                TaxAmount = i.Quantity * i.UnitPrice * (i.TaxRate / 100),
                LineTotal = i.Quantity * i.UnitPrice
            }).ToList()
        };
        
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
        
        // Generate PDF
        invoice.PdfUrl = await GenerateInvoicePdf(invoice);
        await _context.SaveChangesAsync();
        
        return invoice;
    }
    
    private async Task<string> GenerateInvoicePdf(Invoice invoice)
    {
        var provider = await _context.ServiceProviders
            .Include(p => p.User)
            .FirstAsync(p => p.Id == invoice.ProviderId);
        
        var html = await _templateEngine.RenderAsync("InvoiceTemplate", new
        {
            Invoice = invoice,
            Provider = provider,
            Customer = JsonSerializer.Deserialize<CustomerDetails>(invoice.CustomerDetails),
            LogoUrl = provider.LogoUrl,
            GeneratedAt = DateTime.UtcNow
        });
        
        var pdf = await _pdfGenerator.GenerateFromHtml(html, new PdfOptions
        {
            PageSize = PageSize.A4,
            MarginTop = 20,
            MarginBottom = 20,
            MarginLeft = 20,
            MarginRight = 20
        });
        
        var fileName = $"invoices/{invoice.ProviderId}/{invoice.InvoiceNumber}.pdf";
        var url = await _storage.UploadAsync(pdf, fileName, "application/pdf");
        
        return url;
    }
    
    public async Task<bool> SendInvoice(int invoiceId, SendInvoiceDto dto)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Provider)
            .ThenInclude(p => p.User)
            .FirstAsync(i => i.Id == invoiceId);
        
        // Generate payment link
        var paymentLink = await GeneratePaymentLink(invoice);
        invoice.PaymentLink = paymentLink;
        invoice.Status = InvoiceStatus.Sent;
        invoice.SentAt = DateTime.UtcNow;
        
        // Send email
        var customer = JsonSerializer.Deserialize<CustomerDetails>(invoice.CustomerDetails);
        await _emailService.SendInvoiceEmail(new InvoiceEmailData
        {
            To = customer.Email,
            Cc = dto.CcEmails,
            InvoiceNumber = invoice.InvoiceNumber,
            DueDate = invoice.DueDate,
            Amount = invoice.TotalAmount,
            PaymentLink = paymentLink,
            PdfUrl = invoice.PdfUrl,
            CustomMessage = dto.Message,
            ProviderName = invoice.Provider.BusinessName
        });
        
        await _context.SaveChangesAsync();
        
        return true;
    }
}
```

### Financial Analytics

```csharp
public class FinancialAnalyticsService
{
    public async Task<ProfitLossReport> GenerateProfitLossReport(
        int providerId, 
        DateTime startDate, 
        DateTime endDate)
    {
        // Income
        var income = await _context.Transactions
            .Where(t => t.Account.ProviderId == providerId
                && t.TransactionType == TransactionType.Payment
                && t.Status == TransactionStatus.Completed
                && t.ProcessedAt >= startDate
                && t.ProcessedAt <= endDate)
            .GroupBy(t => t.Category)
            .Select(g => new IncomeCategory
            {
                Category = g.Key.ToString(),
                Amount = g.Sum(t => t.Amount),
                Count = g.Count()
            })
            .ToListAsync();
        
        // Expenses
        var expenses = await _context.Expenses
            .Where(e => e.ProviderId == providerId
                && e.Date >= startDate
                && e.Date <= endDate
                && e.Status == ExpenseStatus.Approved)
            .Include(e => e.Category)
            .GroupBy(e => e.Category)
            .Select(g => new ExpenseCategory
            {
                Category = g.Key.Name,
                Amount = g.Sum(e => e.Amount),
                Count = g.Count(),
                TaxDeductible = g.Where(e => e.Category.TaxDeductible)
                    .Sum(e => e.Amount)
            })
            .ToListAsync();
        
        // Calculate metrics
        var totalIncome = income.Sum(i => i.Amount);
        var totalExpenses = expenses.Sum(e => e.Amount);
        var netProfit = totalIncome - totalExpenses;
        var profitMargin = totalIncome > 0 ? (netProfit / totalIncome) * 100 : 0;
        
        return new ProfitLossReport
        {
            Period = new DateRange(startDate, endDate),
            Income = new IncomeSection
            {
                Total = totalIncome,
                ByCategory = income,
                TopServices = await GetTopServices(providerId, startDate, endDate)
            },
            Expenses = new ExpenseSection
            {
                Total = totalExpenses,
                ByCategory = expenses,
                TaxDeductible = expenses.Sum(e => e.TaxDeductible)
            },
            NetProfit = netProfit,
            ProfitMargin = profitMargin,
            Comparison = await GetPeriodComparison(providerId, startDate, endDate)
        };
    }
    
    public async Task<CashFlowStatement> GenerateCashFlow(
        int providerId, 
        DateTime startDate, 
        DateTime endDate)
    {
        var transactions = await _context.Transactions
            .Where(t => t.Account.ProviderId == providerId
                && t.ProcessedAt >= startDate
                && t.ProcessedAt <= endDate)
            .OrderBy(t => t.ProcessedAt)
            .ToListAsync();
        
        var cashFlow = new CashFlowStatement
        {
            Period = new DateRange(startDate, endDate),
            OpeningBalance = await GetOpeningBalance(providerId, startDate),
            Inflows = transactions
                .Where(t => t.Amount > 0)
                .GroupBy(t => t.ProcessedAt.Value.Date)
                .Select(g => new CashFlowItem
                {
                    Date = g.Key,
                    Amount = g.Sum(t => t.Amount),
                    Details = g.Select(t => new TransactionDetail
                    {
                        Description = t.Description,
                        Amount = t.Amount
                    }).ToList()
                })
                .ToList(),
            Outflows = transactions
                .Where(t => t.Amount < 0)
                .GroupBy(t => t.ProcessedAt.Value.Date)
                .Select(g => new CashFlowItem
                {
                    Date = g.Key,
                    Amount = Math.Abs(g.Sum(t => t.Amount)),
                    Details = g.Select(t => new TransactionDetail
                    {
                        Description = t.Description,
                        Amount = Math.Abs(t.Amount)
                    }).ToList()
                })
                .ToList()
        };
        
        cashFlow.ClosingBalance = cashFlow.OpeningBalance + 
            cashFlow.Inflows.Sum(i => i.Amount) - 
            cashFlow.Outflows.Sum(o => o.Amount);
        
        return cashFlow;
    }
}
```

### Tax Compliance

```csharp
public class TaxService
{
    private readonly IHMRCService _hmrcService;
    
    public async Task<VATReturn> PrepareVATReturn(
        int providerId, 
        int year, 
        int quarter)
    {
        var (startDate, endDate) = GetVATQuarterDates(year, quarter);
        
        // Box 1: VAT due on sales
        var vatOnSales = await CalculateOutputVAT(providerId, startDate, endDate);
        
        // Box 2: VAT due on acquisitions
        var vatOnAcquisitions = 0m; // Usually 0 for services
        
        // Box 3: Total VAT due
        var totalVATDue = vatOnSales + vatOnAcquisitions;
        
        // Box 4: VAT reclaimed on purchases
        var vatReclaimed = await CalculateInputVAT(providerId, startDate, endDate);
        
        // Box 5: Net VAT
        var netVAT = totalVATDue - vatReclaimed;
        
        // Box 6: Total sales excluding VAT
        var totalSales = await GetTotalSales(providerId, startDate, endDate);
        
        // Box 7: Total purchases excluding VAT
        var totalPurchases = await GetTotalPurchases(providerId, startDate, endDate);
        
        return new VATReturn
        {
            ProviderId = providerId,
            Year = year,
            Quarter = quarter,
            Box1_VATDueSales = vatOnSales,
            Box2_VATDueAcquisitions = vatOnAcquisitions,
            Box3_TotalVATDue = totalVATDue,
            Box4_VATReclaimedPurchases = vatReclaimed,
            Box5_NetVAT = netVAT,
            Box6_TotalSalesExVAT = totalSales,
            Box7_TotalPurchasesExVAT = totalPurchases,
            Box8_TotalGoodsSuppliedExVAT = 0m, // Services only
            Box9_TotalAcquisitionsExVAT = 0m
        };
    }
    
    public async Task<bool> SubmitVATReturn(VATReturn vatReturn)
    {
        // Submit to HMRC via MTD API
        var submission = new VATSubmission
        {
            periodKey = GetPeriodKey(vatReturn.Year, vatReturn.Quarter),
            vatDueSales = vatReturn.Box1_VATDueSales,
            vatDueAcquisitions = vatReturn.Box2_VATDueAcquisitions,
            totalVatDue = vatReturn.Box3_TotalVATDue,
            vatReclaimedCurrPeriod = vatReturn.Box4_VATReclaimedPurchases,
            netVatDue = Math.Abs(vatReturn.Box5_NetVAT),
            totalValueSalesExVAT = (int)vatReturn.Box6_TotalSalesExVAT,
            totalValuePurchasesExVAT = (int)vatReturn.Box7_TotalPurchasesExVAT,
            totalValueGoodsSuppliedExVAT = (int)vatReturn.Box8_TotalGoodsSuppliedExVAT,
            totalAcquisitionsExVAT = (int)vatReturn.Box9_TotalAcquisitionsExVAT,
            finalised = true
        };
        
        var result = await _hmrcService.SubmitVATReturn(submission);
        
        if (result.Success)
        {
            vatReturn.SubmittedAt = DateTime.UtcNow;
            vatReturn.Status = TaxDocumentStatus.Submitted;
            await _context.SaveChangesAsync();
        }
        
        return result.Success;
    }
}
```

## Security Considerations

### Financial Data Security
1. **PCI DSS Compliance**: 
   - No card details stored
   - Tokenization via Stripe
   - Secure transmission
2. **Access Control**:
   - Provider-only access to financial data
   - Role-based permissions
   - Audit logging
3. **Data Encryption**:
   - Sensitive financial data encrypted at rest
   - Bank account details masked

### API Security
```csharp
[Authorize(Policy = "ServiceProvider")]
[ApiController]
[Route("api/v1/financial")]
public class FinancialController : ControllerBase
{
    [HttpPost("invoices")]
    [RateLimit(10, 1)] // 10 invoices per minute
    public async Task<IActionResult> CreateInvoice(CreateInvoiceDto dto)
    {
        var providerId = User.GetProviderId();
        
        // Verify provider owns the booking if specified
        if (dto.BookingId.HasValue)
        {
            var booking = await _bookingService.GetBooking(dto.BookingId.Value);
            if (booking.ProviderId != providerId)
                return Forbid();
        }
        
        // Additional validation
        if (dto.Items.Sum(i => i.Quantity * i.UnitPrice) > 10000)
        {
            // Flag for manual review
            await _auditService.LogHighValueInvoice(providerId, dto);
        }
        
        var invoice = await _invoiceService.CreateInvoice(dto);
        return Created($"/api/v1/financial/invoices/{invoice.Id}", invoice);
    }
}
```

## Performance Optimization

### Financial Data Caching
```csharp
public class FinancialCacheService
{
    private readonly IDistributedCache _cache;
    
    public async Task<AccountBalance> GetAccountBalance(int providerId)
    {
        var cacheKey = $"balance:{providerId}";
        var cached = await _cache.GetAsync<AccountBalance>(cacheKey);
        
        if (cached != null && cached.CachedAt > DateTime.UtcNow.AddMinutes(-5))
            return cached;
        
        var balance = await CalculateAccountBalance(providerId);
        balance.CachedAt = DateTime.UtcNow;
        
        await _cache.SetAsync(cacheKey, balance, TimeSpan.FromMinutes(5));
        
        return balance;
    }
    
    public async Task InvalidateFinancialCache(int providerId)
    {
        var keys = new[]
        {
            $"balance:{providerId}",
            $"transactions:{providerId}",
            $"invoices:{providerId}",
            $"metrics:{providerId}"
        };
        
        foreach (var key in keys)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
```

### Report Generation Optimization
```csharp
public class ReportGenerationService
{
    private readonly IBackgroundJobClient _jobClient;
    
    public async Task<string> GenerateLargeReport(ReportRequest request)
    {
        // Queue background job for large reports
        var jobId = _jobClient.Enqueue(() => 
            GenerateReportAsync(request));
        
        return jobId;
    }
    
    public async Task GenerateReportAsync(ReportRequest request)
    {
        // Use read replica for reporting
        using var context = new ReportingDbContext(_readOnlyConnection);
        
        // Batch process large datasets
        const int batchSize = 1000;
        var offset = 0;
        var reportData = new List<object>();
        
        while (true)
        {
            var batch = await context.Transactions
                .Where(request.GetFilter())
                .Skip(offset)
                .Take(batchSize)
                .ToListAsync();
            
            if (!batch.Any()) break;
            
            reportData.AddRange(ProcessBatch(batch));
            offset += batchSize;
        }
        
        // Generate and store report
        var report = GenerateReport(reportData);
        await StoreReport(request.UserId, report);
        
        // Notify user
        await NotifyReportReady(request.UserId, report.Id);
    }
}
```

## Monitoring & Analytics

### Financial Metrics
```csharp
public class FinancialMetricsCollector
{
    public async Task CollectDailyMetrics()
    {
        var providers = await _context.ServiceProviders
            .Where(p => p.IsActive)
            .ToListAsync();
        
        foreach (var provider in providers)
        {
            var metrics = new DailyFinancialMetrics
            {
                ProviderId = provider.Id,
                Date = DateTime.UtcNow.Date,
                Revenue = await CalculateDailyRevenue(provider.Id),
                Expenses = await CalculateDailyExpenses(provider.Id),
                InvoicesSent = await CountInvoicesSent(provider.Id),
                PaymentsReceived = await CountPaymentsReceived(provider.Id),
                AveragePaymentTime = await CalculateAveragePaymentTime(provider.Id)
            };
            
            await _metricsService.RecordMetrics(metrics);
        }
    }
}
```

## Testing Strategy

### Financial Calculations Testing
```csharp
[TestClass]
public class InvoiceCalculationTests
{
    [TestMethod]
    public void CalculateInvoiceTotal_WithVATAndDiscount_CorrectTotal()
    {
        // Arrange
        var items = new[]
        {
            new InvoiceItem { Quantity = 2, UnitPrice = 50, TaxRate = 20 },
            new InvoiceItem { Quantity = 1, UnitPrice = 100, TaxRate = 20 }
        };
        var discount = 10m;
        
        // Act
        var subTotal = items.Sum(i => i.Quantity * i.UnitPrice); // 200
        var taxAmount = items.Sum(i => i.Quantity * i.UnitPrice * (i.TaxRate / 100)); // 40
        var total = subTotal + taxAmount - discount; // 230
        
        // Assert
        Assert.AreEqual(230m, total);
    }
}
```

### Integration Testing
- Stripe webhook handling
- Invoice generation and delivery
- Payment processing flow
- VAT calculation accuracy