-- Set search path for the current session
SET search_path TO identity, propulse, public;

-- Insert an admin user (password will need to be updated with a proper hash in a real environment)
-- Default password is 'Admin123!' (this is just a placeholder, not a real hash)
-- Only insert if the admin user doesn't already exist
INSERT INTO identity."AspNetUsers" (
    "Id",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "TwoFactorEnabled",
    "LockoutEnabled",
    "AccessFailedCount",
    "DisplayName",
    "Bio",
    "CreatedAt",
    "UpdatedAt",
    "IsDeleted"
)
SELECT
    'a1b2c3d4-e5f6-4321-8765-1a2b3c4d5e6f',
    'admin',
    'ADMIN',
    'admin@propulse.local',
    'ADMIN@PROPULSE.LOCAL',
    TRUE,
    'AQAAAAEAACcQAAAAEBvlZ7YH27qQ/yZdQYm/CsYbj7nRXJLUibVXm12IXkEXvk1/xZJlKx5+oyEE0MHOAg==', -- This is a placeholder hash
    'HMACSHA512:72:1:K5Ub0Gzl5GQHXLGSlTx0LOiJXIYYQV+fMzZkfZOCuio=:KIGvODZWVcGezXlLEa7RLBb+n02/eR3OVloImTPAB/73A8XlXiEwvYuyDfCqbvXQj6jCjVmTuReTZbxHaj0eMw==',
    'efb0ca63-0eec-40d1-9ddc-54a563ba13be',
    FALSE,
    TRUE,
    0,
    'System Administrator',
    'ProPulse system administrator with full access to all features and content.',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP,
    FALSE
WHERE NOT EXISTS (
    SELECT 1 FROM identity."AspNetUsers" WHERE "NormalizedUserName" = 'ADMIN'
);

-- Assign the Admin role to the admin user if not already assigned
INSERT INTO identity."AspNetUserRoles" ("UserId", "RoleId")
SELECT 'a1b2c3d4-e5f6-4321-8765-1a2b3c4d5e6f', '11111111-1111-1111-1111-111111111111'
WHERE
    EXISTS (SELECT 1 FROM identity."AspNetUsers" WHERE "Id" = 'a1b2c3d4-e5f6-4321-8765-1a2b3c4d5e6f') AND
    EXISTS (SELECT 1 FROM identity."AspNetRoles" WHERE "Id" = '11111111-1111-1111-1111-111111111111') AND
    NOT EXISTS (
        SELECT 1 FROM identity."AspNetUserRoles"
        WHERE "UserId" = 'a1b2c3d4-e5f6-4321-8765-1a2b3c4d5e6f'
        AND "RoleId" = '11111111-1111-1111-1111-111111111111'
    );
