-- Create the ProPulse schema
CREATE SCHEMA IF NOT EXISTS propulse;

-- Create required PostgreSQL extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "citext";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";
CREATE EXTENSION IF NOT EXISTS "unaccent";
CREATE EXTENSION IF NOT EXISTS "btree_gin";

-- Set search path for the current session
SET search_path TO propulse, public;

-- Create enum types
CREATE TYPE propulse."ArticleStatus" AS ENUM ('Draft', 'Published', 'Archived');
CREATE TYPE propulse."AttachmentType" AS ENUM ('FeaturedImage', 'ContentImage');

-- Create tables in dependency order
-- 1. Tables with no foreign key dependencies

-- Create Category table
CREATE TABLE IF NOT EXISTS propulse."Categories" (
    -- From BaseEntity --
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP WITH TIME ZONE NULL,

    -- Entity specific columns --
    "Name" CITEXT NOT NULL UNIQUE,
    "Slug" CITEXT NOT NULL UNIQUE,
    "Description" TEXT NULL
);

-- Create Tag table
CREATE TABLE IF NOT EXISTS propulse."Tags" (
    -- From BaseEntity --
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP WITH TIME ZONE NULL,

    -- Entity specific columns --
    "Name" CITEXT NOT NULL UNIQUE,
    "Slug" CITEXT NOT NULL UNIQUE
);

-- 2. Tables that depend on Categories

-- Create Article table
CREATE TABLE IF NOT EXISTS propulse."Articles" (
    -- From BaseEntity --
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP WITH TIME ZONE NULL,

    -- Entity specific columns --
    "CategoryId" UUID NOT NULL REFERENCES propulse."Categories"("Id") ON DELETE SET NULL,
    "Content" TEXT NOT NULL,
    "Excerpt" TEXT NULL,
    "PublishedAt" TIMESTAMP WITH TIME ZONE NULL,
    "Slug" CITEXT NOT NULL UNIQUE,
    "Status" propulse."ArticleStatus" NOT NULL DEFAULT 'Draft',
    "Title" TEXT NOT NULL,
    "ViewCount" INTEGER NOT NULL DEFAULT 0
);

-- 3. Tables that depend on Articles

-- Create Attachment table for article media
CREATE TABLE IF NOT EXISTS propulse."Attachments" (
    -- From BaseEntity --
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP WITH TIME ZONE NULL,

    -- Entity specific columns --
    "ArticleId" UUID NOT NULL REFERENCES propulse."Articles"("Id") ON DELETE CASCADE,
    "AttachmentType" propulse."AttachmentType" NULL,
    "SymbolicName" VARCHAR(255) NOT NULL,
    "StorageLocation" VARCHAR(1024) NOT NULL,
    "ContentType" CITEXT NOT NULL,
    "FileSize" BIGINT NOT NULL
);

-- Create Bookmark table to track user bookmarks
CREATE TABLE IF NOT EXISTS propulse."Bookmarks" (
    -- From BaseEntity --
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP WITH TIME ZONE NULL,

    -- Entity specific columns --
    "ArticleId" UUID NOT NULL REFERENCES propulse."Articles"("Id") ON DELETE CASCADE
);

-- Create ReadingHistory table to track user reading activity
CREATE TABLE IF NOT EXISTS propulse."ReadingHistory" (
    -- From BaseEntity --
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE CASCADE,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" UUID NULL REFERENCES identity."AspNetUsers"("Id") ON DELETE SET NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP WITH TIME ZONE NULL,

    -- Entity specific columns --
    "ArticleId" UUID NOT NULL REFERENCES propulse."Articles"("Id") ON DELETE CASCADE
);

-- 4. Tables with multiple dependencies (junction tables)

-- Create ArticleTag junction table for many-to-many relationship
CREATE TABLE IF NOT EXISTS propulse."ArticleTag" (
    "ArticleId" UUID NOT NULL REFERENCES propulse."Articles"("Id") ON DELETE CASCADE,
    "TagId" UUID NOT NULL REFERENCES propulse."Tags"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("ArticleId", "TagId")
);

-- Create all indexes organized by table

-- Categories indexes
CREATE INDEX IF NOT EXISTS "IX_Categories_CreatedAt" ON propulse."Categories"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Categories_CreatedBy" ON propulse."Categories"("CreatedBy");
CREATE INDEX IF NOT EXISTS "IX_Categories_UpdatedAt" ON propulse."Categories"("UpdatedAt");
CREATE INDEX IF NOT EXISTS "IX_Categories_UpdatedBy" ON propulse."Categories"("UpdatedBy");
CREATE INDEX IF NOT EXISTS "IX_Categories_IsDeleted" ON propulse."Categories"("IsDeleted");
CREATE INDEX IF NOT EXISTS "IX_Categories_DeletedAt" ON propulse."Categories"("DeletedAt");
CREATE INDEX IF NOT EXISTS "IX_Categories_Name" ON propulse."Categories"("Name");
CREATE INDEX IF NOT EXISTS "IX_Categories_Slug" ON propulse."Categories"("Slug");
CREATE INDEX IF NOT EXISTS "IX_Categories_Name_gin" ON propulse."Categories" USING gin (to_tsvector('english', "Name"));
CREATE INDEX IF NOT EXISTS "IX_Categories_Description_gin" ON propulse."Categories" USING gin (to_tsvector('english', "Description"));

-- Tags indexes
CREATE INDEX IF NOT EXISTS "IX_Tags_CreatedAt" ON propulse."Tags"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Tags_CreatedBy" ON propulse."Tags"("CreatedBy");
CREATE INDEX IF NOT EXISTS "IX_Tags_UpdatedAt" ON propulse."Tags"("UpdatedAt");
CREATE INDEX IF NOT EXISTS "IX_Tags_UpdatedBy" ON propulse."Tags"("UpdatedBy");
CREATE INDEX IF NOT EXISTS "IX_Tags_IsDeleted" ON propulse."Tags"("IsDeleted");
CREATE INDEX IF NOT EXISTS "IX_Tags_DeletedAt" ON propulse."Tags"("DeletedAt");
CREATE INDEX IF NOT EXISTS "IX_Tags_Name" ON propulse."Tags"("Name");
CREATE INDEX IF NOT EXISTS "IX_Tags_Slug" ON propulse."Tags"("Slug");
CREATE INDEX IF NOT EXISTS "IX_Tags_Name_gin" ON propulse."Tags" USING gin (to_tsvector('english', "Name"));

-- Articles indexes
CREATE INDEX IF NOT EXISTS "IX_Articles_CreatedAt" ON propulse."Articles"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Articles_CreatedBy" ON propulse."Articles"("CreatedBy");
CREATE INDEX IF NOT EXISTS "IX_Articles_UpdatedAt" ON propulse."Articles"("UpdatedAt");
CREATE INDEX IF NOT EXISTS "IX_Articles_UpdatedBy" ON propulse."Articles"("UpdatedBy");
CREATE INDEX IF NOT EXISTS "IX_Articles_IsDeleted" ON propulse."Articles"("IsDeleted");
CREATE INDEX IF NOT EXISTS "IX_Articles_DeletedAt" ON propulse."Articles"("DeletedAt");
CREATE INDEX IF NOT EXISTS "IX_Article_CategoryId" ON propulse."Articles"("CategoryId");
CREATE INDEX IF NOT EXISTS "IX_Article_PublishedAt" ON propulse."Articles"("PublishedAt");
CREATE INDEX IF NOT EXISTS "IX_Article_Slug" ON propulse."Articles"("Slug");
CREATE INDEX IF NOT EXISTS "IX_Article_Status" ON propulse."Articles"("Status");
CREATE INDEX IF NOT EXISTS "IX_Articles_Content_gin" ON propulse."Articles" USING gin (to_tsvector('english', "Content"));
CREATE INDEX IF NOT EXISTS "IX_Articles_Excerpt_gin" ON propulse."Articles" USING gin (to_tsvector('english', "Excerpt"));
CREATE INDEX IF NOT EXISTS "IX_Articles_Title_gin" ON propulse."Articles" USING gin (to_tsvector('english', "Title"));

-- Attachments indexes
CREATE INDEX IF NOT EXISTS "IX_Attachments_CreatedAt" ON propulse."Attachments"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Attachments_CreatedBy" ON propulse."Attachments"("CreatedBy");
CREATE INDEX IF NOT EXISTS "IX_Attachments_UpdatedAt" ON propulse."Attachments"("UpdatedAt");
CREATE INDEX IF NOT EXISTS "IX_Attachments_UpdatedBy" ON propulse."Attachments"("UpdatedBy");
CREATE INDEX IF NOT EXISTS "IX_Attachments_IsDeleted" ON propulse."Attachments"("IsDeleted");
CREATE INDEX IF NOT EXISTS "IX_Attachments_DeletedAt" ON propulse."Attachments"("DeletedAt");
CREATE INDEX IF NOT EXISTS "IX_Attachments_ArticleId" ON propulse."Attachments"("ArticleId");
CREATE INDEX IF NOT EXISTS "IX_Attachments_SymbolicName" ON propulse."Attachments"("SymbolicName");
CREATE INDEX IF NOT EXISTS "IX_Attachments_StorageLocation" ON propulse."Attachments"("StorageLocation");
CREATE INDEX IF NOT EXISTS "IX_Attachments_ContentType" ON propulse."Attachments"("ContentType");
CREATE INDEX IF NOT EXISTS "IX_Attachments_FileSize" ON propulse."Attachments"("FileSize");

-- Bookmarks indexes
CREATE INDEX IF NOT EXISTS "IX_Bookmarks_CreatedAt" ON propulse."Bookmarks"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Bookmarks_CreatedBy" ON propulse."Bookmarks"("CreatedBy");
CREATE INDEX IF NOT EXISTS "IX_Bookmarks_UpdatedAt" ON propulse."Bookmarks"("UpdatedAt");
CREATE INDEX IF NOT EXISTS "IX_Bookmarks_UpdatedBy" ON propulse."Bookmarks"("UpdatedBy");
CREATE INDEX IF NOT EXISTS "IX_Bookmarks_IsDeleted" ON propulse."Bookmarks"("IsDeleted");
CREATE INDEX IF NOT EXISTS "IX_Bookmarks_DeletedAt" ON propulse."Bookmarks"("DeletedAt");
CREATE INDEX IF NOT EXISTS "IX_Bookmarks_ArticleId" ON propulse."Bookmarks"("ArticleId");

-- ReadingHistory indexes
CREATE INDEX IF NOT EXISTS "IX_ReadingHistory_CreatedAt" ON propulse."ReadingHistory"("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_ReadingHistory_CreatedBy" ON propulse."ReadingHistory"("CreatedBy");
CREATE INDEX IF NOT EXISTS "IX_ReadingHistory_UpdatedAt" ON propulse."ReadingHistory"("UpdatedAt");
CREATE INDEX IF NOT EXISTS "IX_ReadingHistory_UpdatedBy" ON propulse."ReadingHistory"("UpdatedBy");
CREATE INDEX IF NOT EXISTS "IX_ReadingHistory_IsDeleted" ON propulse."ReadingHistory"("IsDeleted");
CREATE INDEX IF NOT EXISTS "IX_ReadingHistory_DeletedAt" ON propulse."ReadingHistory"("DeletedAt");
CREATE INDEX IF NOT EXISTS "IX_ReadingHistory_ArticleId" ON propulse."ReadingHistory"("ArticleId");

-- ArticleTag indexes
CREATE INDEX IF NOT EXISTS "IX_ArticleTag_ArticleId" ON propulse."ArticleTag"("ArticleId");
CREATE INDEX IF NOT EXISTS "IX_ArticleTag_TagId" ON propulse."ArticleTag"("TagId");
