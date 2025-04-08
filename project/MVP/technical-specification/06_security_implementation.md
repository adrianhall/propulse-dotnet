# 6. Security Implementation Plan

This section details the implementation plan for security features within the ProPulse platform, providing guidance on implementation priorities, responsibilities, and integration points.

## 6.1. Security Implementation Priorities

Security features will be implemented according to the following priority levels:

| Priority | Description | Examples |
|----------|-------------|----------|
| P0 | Critical security controls required before any deployment | Authentication, authorization, TLS, input validation |
| P1 | High-priority security features needed before public release | Column-level encryption, audit logging, rate limiting |
| P2 | Important security enhancements for production hardening | Advanced monitoring, additional security headers |
| P3 | Ongoing security improvements | Security feature enhancements, additional privacy controls |

## 6.2. Authentication and Authorization Implementation

### 6.2.1. ASP.NET Identity Configuration

```csharp
// Program.cs
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    
    // Email confirmation required
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
```

### 6.2.2. JWT Authentication Implementation

```csharp
// TokenService.cs
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    
    public async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };
        
        // Add roles as claims
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // Configure token expiration (e.g., 1 hour)
        var expires = DateTime.Now.AddHours(1);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 6.2.3. Multi-Factor Authentication

```csharp
// MfaService.cs
public class MfaService : IMfaService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    
    public MfaService(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }
    
    public async Task<bool> EnableMfaAsync(ApplicationUser user)
    {
        // Generate new TOTP key
        var key = GenerateNewTotpKey();
        
        // Set the key for the user
        var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (!result.Succeeded)
        {
            return false;
        }
        
        // Store the key securely
        await _userManager.SetAuthenticationTokenAsync(
            user, 
            "ProPulse", 
            "TOTP", 
            key);
            
        return true;
    }
    
    public async Task<string> GenerateMfaQrCodeUriAsync(ApplicationUser user)
    {
        var key = await _userManager.GetAuthenticationTokenAsync(user, "ProPulse", "TOTP");
        
        // Create URI for QR code
        var uri = $"otpauth://totp/ProPulse:{user.Email}?secret={key}&issuer=ProPulse";
        
        return uri;
    }
    
    public async Task<bool> VerifyMfaCodeAsync(ApplicationUser user, string code)
    {
        var key = await _userManager.GetAuthenticationTokenAsync(user, "ProPulse", "TOTP");
        
        // Verify the TOTP code
        var isValid = ValidateTotpCode(key, code);
        
        return isValid;
    }
    
    // Helper methods for TOTP implementation
    private string GenerateNewTotpKey() => /* Implementation */;
    private bool ValidateTotpCode(string key, string code) => /* Implementation */;
}
```

## 6.3. Data Protection Implementation

### 6.3.1. Column-Level Encryption

```csharp
// EncryptionService.cs
public class EncryptionService : IEncryptionService
{
    private readonly IKeyVaultService _keyVaultService;
    
    public EncryptionService(IKeyVaultService keyVaultService)
    {
        _keyVaultService = keyVaultService;
    }
    
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
        }
        
        // Get encryption key from Azure Key Vault
        var key = _keyVaultService.GetEncryptionKey();
        
        // Implement AES-256 encryption
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        
        // Write IV to the beginning of the stream
        ms.Write(aes.IV, 0, aes.IV.Length);
        
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }
        
        return Convert.ToBase64String(ms.ToArray());
    }
    
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return cipherText;
        }
        
        // Get encryption key from Azure Key Vault
        var key = _keyVaultService.GetEncryptionKey();
        
        var cipherBytes = Convert.FromBase64String(cipherText);
        
        using var aes = Aes.Create();
        aes.Key = key;
        
        // Extract IV from the beginning of the ciphertext
        var iv = new byte[aes.BlockSize / 8];
        Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
        aes.IV = iv;
        
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        
        return sr.ReadToEnd();
    }
}
```

### 6.3.2. Azure Key Vault Integration

```csharp
// KeyVaultService.cs
public class KeyVaultService : IKeyVaultService
{
    private readonly IConfiguration _configuration;
    private readonly SecretClient _secretClient;
    
    public KeyVaultService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Use Managed Identity for authentication to Key Vault
        var keyVaultUri = new Uri(_configuration["KeyVault:Uri"]);
        var credential = new DefaultAzureCredential();
        _secretClient = new SecretClient(keyVaultUri, credential);
    }
    
    public byte[] GetEncryptionKey()
    {
        // Get the current encryption key from Key Vault
        var secret = _secretClient.GetSecret("EncryptionKey");
        
        // Convert the Base64 string to a byte array
        return Convert.FromBase64String(secret.Value.Value);
    }
    
    public async Task RotateEncryptionKey()
    {
        // Generate a new encryption key
        using var aes = Aes.Create();
        aes.GenerateKey();
        
        // Store the new key in Key Vault
        var encodedKey = Convert.ToBase64String(aes.Key);
        await _secretClient.SetSecretAsync("EncryptionKey", encodedKey);
    }
}
```

## 6.4. Auditing Implementation

### 6.4.1. Audit Service

```csharp
// AuditService.cs
public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;
    
    public AuditService(
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    
    public async Task RecordAuditTrailAsync(AuditTrail audit)
    {
        // Ensure we have a correlation ID for tracking related audit events
        if (string.IsNullOrEmpty(audit.CorrelationId))
        {
            audit.CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier;
        }
        
        // Add additional context information
        audit.ApplicationName = "ProPulse";
        audit.MachineName = Environment.MachineName;
        
        // Log the audit event
        _logger.LogInformation(
            "Audit: User {UserId} performed {Action} on {EntityType} {EntityId}",
            audit.UserId,
            audit.Action,
            audit.EntityType,
            audit.EntityId);
        
        // Store in database
        _dbContext.AuditTrails.Add(audit);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<AuditTrail>> GetAuditTrailForEntityAsync(
        string entityType, 
        string entityId)
    {
        return await _dbContext.AuditTrails
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<AuditTrail>> GetAuditTrailForUserAsync(
        string userId, 
        DateTime startDate, 
        DateTime endDate)
    {
        return await _dbContext.AuditTrails
            .Where(a => a.UserId == userId && 
                        a.Timestamp >= startDate && 
                        a.Timestamp <= endDate)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
}
```

### 6.4.2. MediatR Audit Behavior

```csharp
// AuditBehavior.cs
public class AuditBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditService _auditService;
    
    public AuditBehavior(
        IHttpContextAccessor httpContextAccessor,
        IAuditService auditService)
    {
        _httpContextAccessor = httpContextAccessor;
        _auditService = auditService;
    }
    
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // Check if request is auditable
        if (request is IAuditableRequest<TResponse> auditableRequest)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            
            // Create pre-execution audit record
            var preAudit = auditableRequest.GetAuditData();
            preAudit.UserId = userId;
            preAudit.ClientIP = ipAddress;
            preAudit.Timestamp = DateTimeOffset.UtcNow;
            preAudit.Phase = AuditPhase.PreExecution;
            
            await _auditService.RecordAuditTrailAsync(preAudit);
        }
        
        // Execute the handler
        var response = await next();
        
        // Create post-execution audit if needed
        if (request is IAuditableRequest<TResponse> auditableRequest)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            
            var postAudit = auditableRequest.GetAuditData();
            postAudit.UserId = userId;
            postAudit.ClientIP = ipAddress;
            postAudit.Timestamp = DateTimeOffset.UtcNow;
            postAudit.Phase = AuditPhase.PostExecution;
            
            // If we have a result we can audit
            if (response != null && auditableRequest is IAuditableResult<TResponse>)
            {
                ((IAuditableResult<TResponse>)auditableRequest).EnrichAuditWithResult(postAudit, response);
            }
            
            await _auditService.RecordAuditTrailAsync(postAudit);
        }
        
        return response;
    }
}
```

## 6.5. Security Monitoring Implementation

### 6.5.1. Security Event Monitoring

```csharp
// SecurityMonitorService.cs
public class SecurityMonitorService : ISecurityMonitorService
{
    private readonly ILogger<SecurityMonitorService> _logger;
    private readonly INotificationService _notificationService;
    
    public SecurityMonitorService(
        ILogger<SecurityMonitorService> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }
    
    public async Task LogSecurityEventAsync(SecurityEvent securityEvent)
    {
        // Log the security event
        _logger.LogWarning(
            "Security Event: {EventType} - User: {UserId}, IP: {IpAddress}, Details: {Details}",
            securityEvent.EventType,
            securityEvent.UserId ?? "anonymous",
            securityEvent.IpAddress,
            securityEvent.Details);
            
        // Store event for analysis
        await StoreSecurityEventAsync(securityEvent);
        
        // Check if this event requires alerting
        if (IsAlertableEvent(securityEvent))
        {
            await _notificationService.SendSecurityAlertAsync(
                "Security Alert: " + securityEvent.EventType,
                FormatSecurityEventMessage(securityEvent));
        }
    }
    
    public async Task<IEnumerable<SecurityEvent>> GetSecurityEventsAsync(
        DateTime startTime, 
        DateTime endTime,
        string userId = null,
        string ipAddress = null,
        SecurityEventType? eventType = null)
    {
        // Implementation to retrieve security events from storage
        // based on the provided filters
    }
    
    private bool IsAlertableEvent(SecurityEvent securityEvent)
    {
        // Logic to determine if an event requires immediate alert
        return securityEvent.EventType == SecurityEventType.BruteForceAttempt ||
               securityEvent.EventType == SecurityEventType.UnauthorizedAccess ||
               securityEvent.EventType == SecurityEventType.DataExfiltration;
    }
    
    private string FormatSecurityEventMessage(SecurityEvent securityEvent)
    {
        // Format event for notification
        return $"Security event {securityEvent.EventType} detected at {securityEvent.Timestamp}.\n" +
               $"User: {securityEvent.UserId ?? "anonymous"}\n" +
               $"IP Address: {securityEvent.IpAddress}\n" +
               $"Details: {securityEvent.Details}";
    }
    
    private Task StoreSecurityEventAsync(SecurityEvent securityEvent)
    {
        // Implementation to store security event in database
    }
}
```

### 6.5.2. Login Monitoring

```csharp
// LoginMonitoringService.cs
public class LoginMonitoringService : ILoginMonitoringService
{
    private readonly ISecurityMonitorService _securityMonitorService;
    private readonly IDistributedCache _cache;
    
    public LoginMonitoringService(
        ISecurityMonitorService securityMonitorService,
        IDistributedCache cache)
    {
        _securityMonitorService = securityMonitorService;
        _cache = cache;
    }
    
    public async Task RecordLoginAttemptAsync(string username, string ipAddress, bool successful)
    {
        // Record the login attempt
        var attempt = new LoginAttempt
        {
            Username = username,
            IpAddress = ipAddress,
            Timestamp = DateTimeOffset.UtcNow,
            Successful = successful
        };
        
        // Store attempt for analysis
        await StoreLoginAttemptAsync(attempt);
        
        // Check for suspicious patterns
        if (!successful)
        {
            await CheckForBruteForceAttackAsync(username, ipAddress);
        }
        else if (await IsLoginFromNewLocationAsync(username, ipAddress))
        {
            await _securityMonitorService.LogSecurityEventAsync(new SecurityEvent
            {
                EventType = SecurityEventType.NewLoginLocation,
                UserId = username,
                IpAddress = ipAddress,
                Timestamp = DateTimeOffset.UtcNow,
                Details = $"Login from new location: {await GetLocationFromIpAsync(ipAddress)}"
            });
        }
    }
    
    private async Task CheckForBruteForceAttackAsync(string username, string ipAddress)
    {
        // Check username-specific attempts
        var userKey = $"login:user:{username}";
        var userAttempts = await GetLoginAttemptsCountAsync(userKey);
        
        if (userAttempts >= 5) // Threshold for user-specific lockout
        {
            await _securityMonitorService.LogSecurityEventAsync(new SecurityEvent
            {
                EventType = SecurityEventType.BruteForceAttempt,
                UserId = username,
                IpAddress = ipAddress,
                Timestamp = DateTimeOffset.UtcNow,
                Details = $"5+ failed login attempts for user {username}"
            });
        }
        
        // Check IP-specific attempts (across users)
        var ipKey = $"login:ip:{ipAddress}";
        var ipAttempts = await GetLoginAttemptsCountAsync(ipKey);
        
        if (ipAttempts >= 10) // Threshold for IP-based alerting
        {
            await _securityMonitorService.LogSecurityEventAsync(new SecurityEvent
            {
                EventType = SecurityEventType.BruteForceAttempt,
                IpAddress = ipAddress,
                Timestamp = DateTimeOffset.UtcNow,
                Details = $"10+ failed login attempts from IP {ipAddress}"
            });
        }
    }
    
    // Helper methods
    private async Task<int> GetLoginAttemptsCountAsync(string key) 
    {
        // Implementation using distributed cache
    }
    
    private async Task StoreLoginAttemptAsync(LoginAttempt attempt)
    {
        // Implementation to store attempt and update counters
    }
    
    private async Task<bool> IsLoginFromNewLocationAsync(string username, string ipAddress)
    {
        // Implementation to detect new login locations
    }
    
    private async Task<string> GetLocationFromIpAsync(string ipAddress)
    {
        // Implementation to get location information from IP
    }
}
```

## 6.6. Privacy Implementation Plan

### 6.6.1. User Data Export Service

```csharp
// UserDataExportService.cs
public class UserDataExportService : IUserDataExportService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IEmailSender _emailSender;
    
    public UserDataExportService(
        ApplicationDbContext dbContext,
        IBlobStorageService blobStorageService,
        IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _blobStorageService = blobStorageService;
        _emailSender = emailSender;
    }
    
    public async Task<Guid> RequestDataExportAsync(string userId)
    {
        // Create export request
        var exportId = Guid.NewGuid();
        var exportRequest = new DataExportRequest
        {
            Id = exportId.ToString(),
            UserId = userId,
            RequestDate = DateTimeOffset.UtcNow,
            Status = DataExportStatus.Pending,
            ExpirationDate = DateTimeOffset.UtcNow.AddDays(7)
        };
        
        // Save request
        _dbContext.DataExportRequests.Add(exportRequest);
        await _dbContext.SaveChangesAsync();
        
        // Queue background job to process export
        // Using a background job system like Hangfire
        BackgroundJob.Enqueue(() => ProcessDataExportAsync(exportId));
        
        return exportId;
    }
    
    public async Task ProcessDataExportAsync(Guid exportId)
    {
        // Get export request
        var exportRequest = await _dbContext.DataExportRequests
            .FirstOrDefaultAsync(e => e.Id == exportId.ToString());
            
        if (exportRequest == null)
        {
            return;
        }
        
        try
        {
            // Update status to processing
            exportRequest.Status = DataExportStatus.Processing;
            await _dbContext.SaveChangesAsync();
            
            // Get user data
            var userData = await CollectUserDataAsync(exportRequest.UserId);
            
            // Serialize to JSON
            var json = JsonSerializer.Serialize(userData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            // Add to ZIP file with README
            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                var jsonEntry = archive.CreateEntry("user_data.json");
                using var jsonStream = jsonEntry.Open();
                using var writer = new StreamWriter(jsonStream);
                await writer.WriteAsync(json);
                
                var readmeEntry = archive.CreateEntry("README.txt");
                using var readmeStream = readmeEntry.Open();
                using var readmeWriter = new StreamWriter(readmeStream);
                await readmeWriter.WriteAsync(GetReadmeContent(exportRequest.UserId));
            }
            
            // Reset stream position
            ms.Position = 0;
            
            // Upload to secure blob storage
            var blobName = $"exports/{exportRequest.UserId}/{exportId}.zip";
            await _blobStorageService.UploadBlobAsync(blobName, ms);
            
            // Generate secure download URL
            var downloadUrl = await _blobStorageService.GetSharedAccessSignatureAsync(
                blobName, 
                TimeSpan.FromDays(2));
                
            // Store URL with request
            exportRequest.DownloadUrl = downloadUrl;
            exportRequest.Status = DataExportStatus.Completed;
            await _dbContext.SaveChangesAsync();
            
            // Send email to user
            var user = await _dbContext.Users.FindAsync(exportRequest.UserId);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Your Data Export is Ready",
                $"Your requested data export is now ready for download. Please use this secure link to download your data: {downloadUrl}\n\n" +
                $"This link will expire in 48 hours for security reasons.");
        }
        catch (Exception ex)
        {
            // Log error
            exportRequest.Status = DataExportStatus.Failed;
            exportRequest.ErrorDetails = ex.Message;
            await _dbContext.SaveChangesAsync();
        }
    }
    
    private async Task<UserDataExport> CollectUserDataAsync(string userId)
    {
        // Collect all relevant user data from various tables
        var user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDataExport.UserInfo
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                DisplayName = u.DisplayName,
                ProfilePictureUrl = u.ProfilePictureUrl,
                Bio = u.Bio,
                EmailConfirmed = u.EmailConfirmed,
                RegistrationDate = u.CreatedAt
            })
            .FirstOrDefaultAsync();
            
        var articles = await _dbContext.Articles
            .Where(a => a.CreatedById == userId)
            .Select(a => new UserDataExport.ArticleInfo
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                Content = a.Content,
                Status = a.Status.ToString(),
                CreatedAt = a.CreatedAt,
                PublishedAt = a.PublishedAt
            })
            .ToListAsync();
            
        var comments = await _dbContext.Comments
            .Where(c => c.CreatedById == userId)
            .Select(c => new UserDataExport.CommentInfo
            {
                Id = c.Id,
                Content = c.Content,
                ArticleId = c.ArticleId,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
            
        var ratings = await _dbContext.Ratings
            .Where(r => r.CreatedById == userId)
            .Select(r => new UserDataExport.RatingInfo
            {
                ArticleId = r.ArticleId,
                Score = r.Score,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
            
        var consents = await _dbContext.UserConsents
            .Where(c => c.UserId == userId)
            .Select(c => new UserDataExport.ConsentInfo
            {
                Type = c.ConsentType.ToString(),
                IsGranted = c.IsGranted,
                LastUpdated = c.UpdatedAt
            })
            .ToListAsync();
            
        // Combine all data
        return new UserDataExport
        {
            ExportDate = DateTimeOffset.UtcNow,
            UserInfo = user,
            Articles = articles,
            Comments = comments,
            Ratings = ratings,
            Consents = consents
        };
    }
    
    private string GetReadmeContent(string userId)
    {
        return $"Data Export for User: {userId}\n" +
               $"Export Date: {DateTimeOffset.UtcNow}\n\n" +
               "This archive contains your personal data from the ProPulse platform.\n" +
               "The data is provided in JSON format in the user_data.json file.\n\n" +
               "For any questions about this data, please contact privacy@propulse.com";
    }
}
```

### 6.6.2. Account Deletion Service

```csharp
// AccountDeletionService.cs
public class AccountDeletionService : IAccountDeletionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountDeletionService> _logger;
    
    public AccountDeletionService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountDeletionService> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
    }
    
    public async Task<bool> RequestAccountDeletionAsync(string userId)
    {
        // Create deletion request with cooling-off period
        var deletionRequest = new AccountDeletionRequest
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            RequestDate = DateTimeOffset.UtcNow,
            ScheduledDeletionDate = DateTimeOffset.UtcNow.AddDays(14), // 14-day cooling off
            Status = AccountDeletionStatus.Scheduled
        };
        
        // Save request
        _dbContext.AccountDeletionRequests.Add(deletionRequest);
        await _dbContext.SaveChangesAsync();
        
        // Schedule job for account deletion
        // Using a background job system like Hangfire
        BackgroundJob.Schedule(
            () => ProcessAccountDeletionAsync(deletionRequest.Id),
            TimeSpan.FromDays(14));
            
        _logger.LogInformation(
            "Account deletion requested for user {UserId}, scheduled for {DeletionDate}",
            userId,
            deletionRequest.ScheduledDeletionDate);
            
        return true;
    }
    
    public async Task<bool> CancelAccountDeletionAsync(string userId)
    {
        // Find pending deletion request
        var deletionRequest = await _dbContext.AccountDeletionRequests
            .Where(d => d.UserId == userId && d.Status == AccountDeletionStatus.Scheduled)
            .FirstOrDefaultAsync();
            
        if (deletionRequest == null)
        {
            return false;
        }
        
        // Update status to cancelled
        deletionRequest.Status = AccountDeletionStatus.Cancelled;
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation(
            "Account deletion cancelled for user {UserId}",
            userId);
            
        return true;
    }
    
    public async Task ProcessAccountDeletionAsync(string requestId)
    {
        // Get deletion request
        var deletionRequest = await _dbContext.AccountDeletionRequests
            .FirstOrDefaultAsync(d => d.Id == requestId);
            
        if (deletionRequest == null || deletionRequest.Status != AccountDeletionStatus.Scheduled)
        {
            return;
        }
        
        try
        {
            // Update status to processing
            deletionRequest.Status = AccountDeletionStatus.Processing;
            await _dbContext.SaveChangesAsync();
            
            // Get user
            var user = await _userManager.FindByIdAsync(deletionRequest.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"User {deletionRequest.UserId} not found");
            }
            
            // Begin transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                // Anonymize user data instead of hard delete
                await AnonymizeUserDataAsync(deletionRequest.UserId);
                
                // Delete or anonymize user record
                user.UserName = $"deleted_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                user.NormalizedUserName = user.UserName.ToUpper();
                user.Email = $"deleted_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com";
                user.NormalizedEmail = user.Email.ToUpper();
                user.PasswordHash = null;
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.DisplayName = "Deleted User";
                user.ProfilePictureUrl = null;
                user.Bio = null;
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue; // Permanent lockout
                
                // Update user
                await _userManager.UpdateAsync(user);
                
                // Update deletion request status
                deletionRequest.Status = AccountDeletionStatus.Completed;
                deletionRequest.CompletionDate = DateTimeOffset.UtcNow;
                await _dbContext.SaveChangesAsync();
                
                // Commit transaction
                await transaction.CommitAsync();
                
                _logger.LogInformation(
                    "Account deletion completed for user {UserId}",
                    deletionRequest.UserId);
            }
            catch (Exception ex)
            {
                // Rollback on error
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            // Log error
            _logger.LogError(
                ex,
                "Error deleting account for user {UserId}",
                deletionRequest.UserId);
                
            deletionRequest.Status = AccountDeletionStatus.Failed;
            deletionRequest.ErrorDetails = ex.Message;
            await _dbContext.SaveChangesAsync();
        }
    }
    
    private async Task AnonymizeUserDataAsync(string userId)
    {
        // Anonymize comments
        var comments = await _dbContext.Comments
            .Where(c => c.CreatedById == userId)
            .ToListAsync();
            
        foreach (var comment in comments)
        {
            comment.Content = "[Comment removed]";
        }
        
        // Remove ratings
        var ratings = await _dbContext.Ratings
            .Where(r => r.CreatedById == userId)
            .ToListAsync();
            
        _dbContext.Ratings.RemoveRange(ratings);
        
        // Anonymize or transfer articles based on policy
        var articles = await _dbContext.Articles
            .Where(a => a.CreatedById == userId)
            .ToListAsync();
            
        foreach (var article in articles)
        {
            // For published articles, preserve but anonymize author
            if (article.Status == ArticleStatus.Published)
            {
                // Keep the content but mark as from anonymous user
                article.UpdatedById = null;
            }
            else
            {
                // For drafts, delete them
                _dbContext.Articles.Remove(article);
            }
        }
        
        // Save changes
        await _dbContext.SaveChangesAsync();
    }
}
```

## 6.7. Implementation Schedule

| Phase | Security Feature | Timeline | Priority | Responsible Team |
|-------|------------------|----------|----------|------------------|
| 1 | Authentication & Authorization | Week 1-2 | P0 | Identity Team |
| 1 | Input Validation | Week 1-2 | P0 | API Team |
| 1 | TLS Configuration | Week 1 | P0 | Infrastructure Team |
| 1 | Basic Logging | Week 1 | P0 | Core Team |
| 2 | Rate Limiting | Week 3 | P1 | API Team |
| 2 | Column-Level Encryption | Week 3-4 | P1 | Data Team |
| 2 | Audit Trails | Week 3-4 | P1 | Core Team |
| 2 | Content Security | Week 4 | P1 | UI Team |
| 3 | MFA Implementation | Week 5-6 | P2 | Identity Team |
| 3 | Security Monitoring | Week 5-6 | P2 | Operations Team |
| 3 | Key Vault Integration | Week 5 | P2 | Infrastructure Team |
| 3 | Privacy Controls | Week 6-7 | P2 | Core Team |
| 4 | Advanced Analytics | Week 8+ | P3 | Analytics Team |
| 4 | Security Headers | Week 8 | P3 | UI Team |
| 4 | User Data Export | Week 8-9 | P3 | Privacy Team |
| 4 | Account Deletion | Week 9-10 | P3 | Privacy Team |
