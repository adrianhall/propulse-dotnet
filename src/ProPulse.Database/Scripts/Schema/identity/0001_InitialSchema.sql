-- Create the identity schema
CREATE SCHEMA IF NOT EXISTS identity;

-- Create required PostgreSQL extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "citext";

-- Set search path for the current session
SET search_path TO identity, public;

-- Create roles table
CREATE TABLE IF NOT EXISTS identity."AspNetRoles" (
    "Id" UUID PRIMARY KEY,
    "Name" CITEXT NOT NULL UNIQUE,
    "NormalizedName" CITEXT NOT NULL UNIQUE,
    "ConcurrencyStamp" TEXT NULL,
    "Description" VARCHAR(255) NULL
);

CREATE INDEX IF NOT EXISTS "IX_AspNetRoles_Name" ON identity."AspNetRoles"("Name");
CREATE INDEX IF NOT EXISTS "IX_AspNetRoles_NormalizedName" ON identity."AspNetRoles"("NormalizedName");

-- Create default roles
INSERT INTO identity."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description")
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Admin', 'ADMIN', gen_random_uuid()::text, 'Administrator with full access'),
    ('22222222-2222-2222-2222-222222222222', 'Author', 'AUTHOR', gen_random_uuid()::text, 'Content creator with limited access'),
    ('33333333-3333-3333-3333-333333333333', 'Reader', 'READER', gen_random_uuid()::text, 'Standard reader with minimal access');

-- Create users table
CREATE TABLE IF NOT EXISTS identity."AspNetUsers" (
    -- IdentityUser --
    "Id" UUID PRIMARY KEY,
    "UserName" CITEXT NOT NULL UNIQUE,
    "NormalizedUserName" CITEXT NOT NULL UNIQUE,
    "Email" CITEXT NOT NULL UNIQUE,
    "NormalizedEmail" CITEXT NOT NULL UNIQUE,
    "EmailConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "PasswordHash" VARCHAR(256) NULL,
    "SecurityStamp" VARCHAR(256) NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" VARCHAR(20) NULL,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "TwoFactorEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "LockoutEnd" TIMESTAMP WITH TIME ZONE NULL,
    "LockoutEnabled" BOOLEAN NOT NULL DEFAULT TRUE,
    "AccessFailedCount" INTEGER NOT NULL DEFAULT 0,

    -- ProPulse additions
    "DisplayName" TEXT NULL,
    "Bio" TEXT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastLoginAt" TIMESTAMP WITH TIME ZONE NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP WITH TIME ZONE NULL
);

CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_NormalizedEmail" ON identity."AspNetUsers"("NormalizedEmail");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_NormalizedUserName" ON identity."AspNetUsers"("NormalizedUserName");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_Email" ON identity."AspNetUsers"("Email");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_UserName" ON identity."AspNetUsers"("UserName");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_EmailConfirmed" ON identity."AspNetUsers"("EmailConfirmed");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_PhoneNumberConfirmed" ON identity."AspNetUsers"("PhoneNumberConfirmed");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_CreatedAt" ON identity."AspNetUsers"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_IsDeleted" ON identity."AspNetUsers"("IsDeleted");
CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_DeletedAt" ON identity."AspNetUsers"("DeletedAt");

-- Create user_roles junction table
CREATE TABLE IF NOT EXISTS identity."AspNetUserRoles" (
    "UserId" UUID NOT NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    "RoleId" UUID NOT NULL REFERENCES identity."AspNetRoles"("Id") ON DELETE CASCADE,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId")
);

CREATE INDEX IF NOT EXISTS "IX_AspNetUserRoles_RoleId" ON identity."AspNetUserRoles"("RoleId");


-- Create user_logins table
CREATE TABLE IF NOT EXISTS identity."AspNetUserLogins" (
    "LoginProvider" VARCHAR(128) NOT NULL,
    "ProviderKey" VARCHAR(128) NOT NULL,
    "ProviderDisplayName" VARCHAR(128) NOT NULL,
    "UserId" UUID NOT NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey")
);

CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId" ON identity."AspNetUserLogins"("UserId");
CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_LoginProvider" ON identity."AspNetUserLogins"("LoginProvider");

-- Create user_tokens table
CREATE TABLE IF NOT EXISTS identity."AspNetUserTokens" (
    "UserId" UUID NOT NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    "LoginProvider" VARCHAR(128) NOT NULL,
    "Name" VARCHAR(128) NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name")
);

-- Create user_claims table
CREATE TABLE IF NOT EXISTS identity."AspNetUserClaims" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" UUID NOT NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    "ClaimType" VARCHAR(256) NOT NULL,
    "ClaimValue" VARCHAR(256) NULL
);

CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId" ON identity."AspNetUserClaims"("UserId");
CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_ClaimType" ON identity."AspNetUserClaims"("ClaimType");

-- Create role_claims table
CREATE TABLE IF NOT EXISTS identity."AspNetRoleClaims" (
    "Id" SERIAL PRIMARY KEY,
    "RoleId" UUID NOT NULL REFERENCES identity."AspNetRoles"("Id") ON DELETE CASCADE,
    "ClaimType" VARCHAR(256) NOT NULL,
    "ClaimValue" VARCHAR(256) NULL
);

CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId" ON identity."AspNetRoleClaims"("RoleId");
CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_ClaimType" ON identity."AspNetRoleClaims"("ClaimType");

-- Create refresh_tokens table
CREATE TABLE IF NOT EXISTS identity."RefreshTokens" (
    "Id" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    "Token" VARCHAR(256) NOT NULL UNIQUE,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedByIp" VARCHAR(45) NULL,
    "RevokedAt" TIMESTAMP WITH TIME ZONE NULL,
    "RevokedByIp" VARCHAR(45) NULL,
    "ReplacedByToken" VARCHAR(256) NULL,
    "ReasonRevoked" VARCHAR(256) NULL
);

CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_UserId" ON identity."RefreshTokens"("UserId");
CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_ExpiresAt" ON identity."RefreshTokens"("ExpiresAt");
CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_RevokedAt" ON identity."RefreshTokens"("RevokedAt");
