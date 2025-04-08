# 5. User Interface

## 5.1. UI Implementation Approach

Based on the technology comparison in section 3.4, the ProPulse platform will implement a Razor Pages-based architecture with strategic use of API endpoints for dynamic content. This section describes how this implementation choice will shape the user interface architecture and development workflow.

### UI Architecture Overview

The UI layer will be structured using the following pattern:

```mermaid
flowchart TD
    User([User]) --> Browser[Web Browser]
    Browser --> RazorPages[Razor Pages]
    Browser --> APIs[API Endpoints]
    
    RazorPages --> PageModels[Page Models]
    APIs --> Controllers[API Controllers]
    
    PageModels --> Services[Application Services]
    Controllers --> Services
    
    Services --> Domain[Domain Layer]
    Domain --> Data[Data Access Layer]
```

### Key Implementation Details

1. **Razor Pages Structure**
   - Pages will be organized by functional domain (Authentication, Article Management, Social Media, etc.)
   - Shared layouts will provide consistent UI elements across the platform
   - Partial views will be used for reusable UI components
   - Tag helpers will be developed for common UI patterns

2. **JavaScript Enhancement Strategy**
   - Core functionality will work without JavaScript (progressive enhancement)
   - Modern JavaScript modules will enhance the UI where appropriate:
     - Rich text editor for article authoring
     - Real-time comment notifications via SignalR
     - Interactive rating components
     - Social media preview generators
     - Analytics visualizations

3. **API Integration Pattern**
   - Initial page load will be server-rendered via Razor Pages
   - Dynamic content will be fetched using API endpoints
   - AJAX patterns will be used for in-page updates without full refreshes
   - Authentication will be handled using cookie-based auth for pages and token-based auth for APIs

4. **Styling and Design System**
   - CSS architecture using CSS modules and variables
   - Responsive design based on mobile-first principles
   - Design token system for consistent styling
   - Accessibility standards compliance (WCAG 2.1 AA)

5. **Performance Optimization**
   - Server-side caching for frequently accessed content
   - Client-side caching for static resources
   - Lazy loading of non-critical resources
   - Image optimization and responsive loading

This implementation approach provides several advantages:

- **Rapid Development**: The page-focused architecture maps naturally to content workflows
- **SEO Optimization**: Server-rendered content improves search engine indexing
- **Performance**: Initial load performance benefits from server rendering
- **Maintainability**: Clean separation between UI and business logic
- **Progressive Enhancement**: Core functionality works without JavaScript
- **Unified Technology Stack**: Developers can work across the full stack with C#

The combination of Razor Pages with API endpoints strikes an optimal balance for ProPulse, providing the benefits of server-rendered content while enabling rich interactivity where needed.

## 5.2. User Journey Flows

### Authentication Flows

ProPulse will support anonymous browsing for basic article reading, with a sign-in/register button in the top right of the page. Additional features like commenting and rating require authentication.

#### Registration Flow

```mermaid
flowchart TD
    Start([Start]) --> HomePage[Home Page]
    HomePage --> RegisterButton[Click Register Button]
    RegisterButton --> RegisterForm[Registration Form]
    RegisterForm --> EnterDetails[Enter Email, Username, Password]
    EnterDetails --> SubmitForm[Submit Registration]
    SubmitForm --> ValidationCheck{Validation Check}
    ValidationCheck -->|Invalid| ValidationErrors[Show Validation Errors]
    ValidationErrors --> EnterDetails
    ValidationCheck -->|Valid| DuplicateCheck{Duplicate Check}
    DuplicateCheck -->|Duplicate| DuplicateError[Show Duplicate Error]
    DuplicateError --> RegisterForm
    DuplicateCheck -->|New User| AttemptEmail[Attempt Email Sending]
    AttemptEmail -->|Failed| EmailError[Show Email Error]
    EmailError --> ResendOption[Offer Resend Option]
    ResendOption --> AttemptEmail
    AttemptEmail -->|Success| EmailSent[Confirmation Email Sent]
    EmailSent --> WaitForConfirm[Wait for Confirmation]
    WaitForConfirm --> TokenExpired{Token Expired?}
    TokenExpired -->|Yes| ResendLink[Request New Link]
    ResendLink --> AttemptEmail
    TokenExpired -->|No| ConfirmEmail[User Clicks Email Link]
    ConfirmEmail --> LinkValid{Link Valid?}
    LinkValid -->|No| InvalidLink[Show Invalid Link Error]
    InvalidLink --> ResendLink
    LinkValid -->|Yes| AccountActivated[Account Activated]
    AccountActivated --> LoginForm[Redirect to Login Form]
```

#### Login Flow

```mermaid
flowchart TD
    Start([Start]) --> HomePage[Home Page]
    HomePage --> LoginButton[Click Login Button]
    LoginButton --> LoginForm[Login Form]
    LoginForm --> CredentialOption{Login Method}
    CredentialOption -->|Standard| EnterCredentials[Enter Email/Username and Password]
    CredentialOption -->|Social| SelectProvider[Select Social Provider]
    EnterCredentials --> SubmitLogin[Submit Login]
    SelectProvider --> OAuthFlow[OAuth Authentication]
    OAuthFlow --> SocialAuthCheck{Auth Success?}
    SocialAuthCheck -->|No| SocialAuthError[Show OAuth Error]
    SocialAuthError --> LoginForm
    SocialAuthCheck -->|Yes| AccountExists{Account Exists?}
    AccountExists -->|No| CreateLinkedAccount[Create Linked Account]
    CreateLinkedAccount --> LoginSuccess[Authentication Successful]
    AccountExists -->|Yes| LoginSuccess
    
    SubmitLogin --> Validation{Valid Credentials?}
    Validation -->|Yes| AccountStatus{Account Status}
    Validation -->|No| LoginError[Show Invalid Credentials]
    LoginError --> EnterCredentials
    AccountStatus -->|Locked| AccountLocked[Show Account Locked]
    AccountLocked --> UnlockOptions[Show Unlock Options]
    UnlockOptions --> LoginForm
    AccountStatus -->|Email Not Verified| VerifyEmail[Show Verification Required]
    VerifyEmail --> ResendEmail[Offer Resend Verification]
    ResendEmail --> LoginForm
    AccountStatus -->|Active| LoginSuccess
    Validation -->|Too Many Failures| RateLimit[Temporary Rate Limit]
    RateLimit --> LoginForm
    
    LoginSuccess --> RoleDirect{Role-based Redirect}
    RoleDirect -->|Author| AuthorDashboard[Author Dashboard]
    RoleDirect -->|Reader| ReaderHome[Reader Homepage]
    RoleDirect -->|Social Manager| ManagerDashboard[Manager Dashboard]
    RoleDirect -->|Admin| AdminDashboard[Admin Dashboard]
```

#### Logout Flow

```mermaid
flowchart TD
    Start([Start]) --> LoggedInState[User is Logged In]
    LoggedInState --> LogoutButton[Click Logout Button]
    LogoutButton --> ConfirmLogout{Confirm?}
    ConfirmLogout -->|Yes| ProcessLogout[Process Logout]
    ConfirmLogout -->|No| CancelLogout[Cancel]
    ProcessLogout --> ClearSession[Clear Session]
    ClearSession --> HomePage[Redirect to Home Page]
    CancelLogout --> ReturnToPrevious[Return to Previous Page]
```

#### Password Reset Flow

```mermaid
flowchart TD
    Start([Start]) --> LoginForm[Login Form]
    LoginForm --> ForgotPassword[Click "Forgot Password"]
    ForgotPassword --> ResetForm[Password Reset Form]
    ResetForm --> EnterEmail[Enter Email Address]
    EnterEmail --> SubmitEmail[Submit]
    SubmitEmail --> EmailCheck{Email Exists?}
    EmailCheck -->|No| EmailNotFound[Show Generic Success]
    EmailCheck -->|Yes| AttemptSend[Attempt Email Send]
    AttemptSend -->|Failed| SendError[Handle Send Error]
    SendError --> ContactSupport[Show Contact Support]
    AttemptSend -->|Success| EmailSent[Reset Link Sent]
    EmailNotFound --> WaitPeriod[Security Wait Period]
    WaitPeriod --> LoginForm
    
    EmailSent --> ClickLink[User Clicks Reset Link]
    ClickLink --> ValidateToken{Token Valid?}
    ValidateToken -->|No| InvalidToken[Show Invalid/Expired Token]
    InvalidToken --> RequestNewLink[Request New Link]
    RequestNewLink --> ResetForm
    ValidateToken -->|Yes| NewPasswordForm[New Password Form]
    
    NewPasswordForm --> EnterNewPassword[Enter New Password]
    EnterNewPassword --> ConfirmNewPassword[Confirm New Password]
    ConfirmNewPassword --> SubmitNewPassword[Submit New Password]
    SubmitNewPassword --> PasswordValidation{Password Valid?}
    PasswordValidation -->|No| ValidationErrors[Show Validation Errors]
    ValidationErrors --> EnterNewPassword
    PasswordValidation -->|Yes| UpdatePassword[Update Password]
    UpdatePassword --> PasswordUpdated[Password Updated]
    PasswordUpdated --> LoginForm[Return to Login Form]
```

### Author Flows

#### Creating and Editing Articles

```mermaid
flowchart TD
    Start([Start]) --> Login[Login]
    Login --> Dashboard[Author Dashboard]
    Dashboard --> CreateOrEdit{Create or Edit?}
    
    %% Creation Path
    CreateOrEdit -->|Create New| Create[Create New Article]
    Create --> EnterTitle[Enter Title & Summary]
    EnterTitle --> ValidateTitle{Title Valid?}
    ValidateTitle -->|No| TitleError[Show Error Message]
    TitleError --> EnterTitle
    ValidateTitle -->|Yes| UploadCover[Upload Cover Image]
    
    UploadCover --> ImageCheck{Image Valid?}
    ImageCheck -->|No| ImageError[Show Format/Size Error]
    ImageError --> UploadCover
    ImageCheck -->|Yes| AddTags[Add Tags]
    
    %% Edit Path
    CreateOrEdit -->|Edit Existing| ViewArticles[View My Articles]
    ViewArticles --> SelectArticle[Select Article to Edit]
    SelectArticle --> LoadingCheck{Article Loads?}
    LoadingCheck -->|Error| LoadError[Show Loading Error]
    LoadError --> RetryLoad[Retry or Return to Dashboard]
    RetryLoad --> SelectArticle
    LoadingCheck -->|Success| EditForm[Edit Article Form]
    EditForm --> EditTitle[Edit Title & Summary]
    EditTitle --> ValidateEditTitle{Title Valid?}
    ValidateEditTitle -->|No| EditTitleError[Show Error]
    EditTitleError --> EditTitle
    ValidateEditTitle -->|Yes| EditCover[Change Cover Image]
    EditCover --> EditTags[Edit Tags]
    EditTags --> EditArticleContent[Edit Content]
    
    %% Common Paths
    AddTags --> EditContent[Edit Article Content]
    EditArticleContent --> SaveCheck{Auto-Save}
    SaveCheck -->|Failure| SaveError[Show Save Error]
    SaveError --> RetryAutoSave[Retry]
    RetryAutoSave --> SaveCheck
    SaveCheck -->|Success| PreviewOption[Preview Option]
    
    EditContent --> SaveCheck
    
    PreviewOption --> Preview[Preview Article]
    Preview --> BrowserCheck{Browser Renders?}
    BrowserCheck -->|Issue| RenderError[Show Render Warning]
    RenderError --> EditContent
    BrowserCheck -->|OK| EditOption[Make Changes?]
    
    EditOption -->|Yes| ReturnToEdit[Return to Editor]
    ReturnToEdit --> EditContent
    EditOption -->|No| SaveOptions{Save Options}
    
    SaveOptions -->|Save Draft| SaveDraft[Save as Draft]
    SaveDraft --> SaveProcess[Processing]
    SaveProcess --> SaveSuccess{Save Success?}
    SaveSuccess -->|No| SaveFailure[Show Error]
    SaveFailure --> RetrySave[Retry Save]
    RetrySave --> SaveDraft
    SaveSuccess -->|Yes| SuccessDraft([Draft Saved])
    SuccessDraft --> DashboardReturn[Return to Dashboard]
    DashboardReturn --> Dashboard
    
    SaveOptions -->|Discard| ConfirmDiscard{Confirm?}
    ConfirmDiscard -->|No| ReturnToEdit
    ConfirmDiscard -->|Yes| DiscardChanges[Discard Changes]
    DiscardChanges --> Dashboard
```

#### Publishing an Article

```mermaid
flowchart TD
    Start([Start]) --> Login[Login]
    Login --> Dashboard[Author Dashboard]
    Dashboard --> ViewDrafts[View Draft Articles]
    ViewDrafts --> SelectDraft[Select Draft]
    SelectDraft --> EditIfNeeded[Edit If Needed]
    EditIfNeeded --> FinalReview[Final Review]
    FinalReview --> Publish{How to Publish?}
    Publish -->|Publish Now| PublishNow[Publish Immediately]
    Publish -->|Schedule| SchedulePublish[Set Publication Date]
    PublishNow --> Published([Article Published])
    SchedulePublish --> Scheduled([Article Scheduled])
    Published --> ShareOptions[Social Media Options]
    Scheduled --> ShareOptions
    ShareOptions --> CreatePost[Create Social Post]
    CreatePost --> SelectAccounts[Select Social Accounts]
    SelectAccounts --> WritePost[Write Post Content]
    WritePost --> SchedulePost[Schedule Post]
    SchedulePost --> AwaitApproval([Await Manager Approval])
```

**Note:** This diagram represents the happy path flow. Comprehensive error handling is expected at each step, including validation errors, network failures, authorization issues, and database errors.

#### Managing Article Comments

```mermaid
flowchart TD
    Start([Start]) --> Login[Login]
    Login --> Dashboard[Author Dashboard]
    Dashboard --> MyArticles[View My Articles]
    MyArticles --> SelectArticle[Select Article]
    SelectArticle --> ViewComments[View Comments]
    ViewComments --> CommentAction{Comment Action}
    CommentAction -->|Reply| AddReply[Write Reply]
    CommentAction -->|Moderate| ModerateOptions{Moderation}
    ModerateOptions -->|Hide| HideComment[Hide Comment]
    ModerateOptions -->|Report| ReportComment[Report Comment]
    AddReply --> SubmitReply[Submit Reply]
    SubmitReply --> ViewComments
    HideComment --> ConfirmHide[Confirm]
    ConfirmHide --> ViewComments
    ReportComment --> EnterReason[Enter Reason]
    EnterReason --> SubmitReport[Submit Report]
    SubmitReport --> ViewComments
```

**Note:** This diagram represents the happy path flow. Comprehensive error handling is expected at each step, including validation errors, network failures, authorization issues, and database errors.

### Reader: Browse, Rate, and Comment on Articles

```mermaid
flowchart TD
    Start([Start]) --> HomePage[Browse Homepage]
    HomePage --> Search[Search for Articles]
    HomePage --> Discover[Discover Featured Articles]
    Search --> Results[View Search Results]
    Discover --> Results
    Results --> SelectArticle[Select Article]
    SelectArticle --> ReadArticle[Read Article]
    ReadArticle --> Rate[Rate Article]
    ReadArticle --> Comment[Leave Comment]
    ReadArticle --> Share[Share Article]
    Comment --> ViewComments[View Other Comments]
    ViewComments --> ReplyToComment[Reply to Comment]
    Share --> SelectPlatform[Select Social Platform]
    SelectPlatform --> ShareExternal([Share on External Platform])
```

**Note:** This diagram represents the happy path flow. Comprehensive error handling is expected at each step, including validation errors, network failures, authorization issues, and database errors.

### Social Media Manager: Review and Approve Posts

```mermaid
flowchart TD
    Start([Start]) --> Login[Login]
    Login --> Dashboard[Manager Dashboard]
    Dashboard --> ReviewQueue[Review Pending Posts]
    ReviewQueue --> Select[Select Post to Review]
    Select --> ViewDetails[View Post Details]
    ViewDetails --> Decision{Decision}
    Decision -->|Approve| Approve[Approve Post]
    Decision -->|Reject| Reject[Reject Post]
    Decision -->|Request Changes| RequestChanges[Request Changes]
    Approve --> ConfirmApproval[Confirm Approval]
    Reject --> ProvideReason[Provide Rejection Reason]
    RequestChanges --> ProvideFeedback[Provide Feedback]
    ConfirmApproval --> ScheduledQueue([Post Moved to Scheduled Queue])
    ProvideReason --> NotifyAuthor([Author Notified of Rejection])
    ProvideFeedback --> NotifyAuthor2([Author Notified of Requested Changes])
    Dashboard --> ManageAccounts[Manage Social Media Accounts]
    ManageAccounts --> AddAccount[Add New Account]
    ManageAccounts --> EditAccount[Edit Existing Account]
    ManageAccounts --> RemoveAccount[Remove Account]
    Dashboard --> ViewAnalytics[View Social Media Analytics]
    ViewAnalytics --> FilterByPeriod[Filter by Time Period]
    ViewAnalytics --> FilterByPlatform[Filter by Platform]
    ViewAnalytics --> ExportData[Export Analytics Data]
```

**Note:** This diagram represents the happy path flow. Comprehensive error handling is expected at each step, including validation errors, network failures, authorization issues, and database errors.

### Administrator: System Management

```mermaid
flowchart TD
    Start([Start]) --> Login[Login as Admin]
    Login --> AdminDashboard[Administrator Dashboard]
    AdminDashboard --> ManageUsers[Manage Users]
    AdminDashboard --> SystemSettings[System Settings]
    AdminDashboard --> ViewLogs[View System Logs]
    ManageUsers --> ListUsers[List All Users]
    ListUsers --> CreateUser[Create New User]
    ListUsers --> EditUser[Edit User Details]
    ListUsers --> DeactivateUser[Deactivate User]
    EditUser --> ChangeRole[Change User Role]
    SystemSettings --> GeneralSettings[General Settings]
    SystemSettings --> SecuritySettings[Security Settings]
    SystemSettings --> IntegrationSettings[Integration Settings]
    ViewLogs --> FilterLogs[Filter Logs]
    FilterLogs --> ExportLogs[Export Logs]
```

**Note:** This diagram represents the happy path flow. Comprehensive error handling is expected at each step, including validation errors, network failures, authorization issues, and database errors.
