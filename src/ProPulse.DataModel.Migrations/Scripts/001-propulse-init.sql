-- propulse-init.sql
-- This script initializes the propulse schema, installs required extensions, and creates the initial tables and triggers for the content publishing data model.

-- 1. Create schema
CREATE SCHEMA IF NOT EXISTS "propulse";

-- 2. Install required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "citext";

-- 3. Create trigger functions
CREATE OR REPLACE FUNCTION update_audit_fields()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW."CreatedAt" IS NULL THEN
        NEW."CreatedAt" = now();
    END IF;
    NEW."UpdatedAt" = now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 4. Create Articles table
CREATE TABLE IF NOT EXISTS "propulse"."Articles" (
    "Id" uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Title" citext NOT NULL,
    "Content" text NOT NULL,
    "CreatedAt" timestamptz NOT NULL DEFAULT now(),
    "DeletedAt" timestamptz NULL,
    "UpdatedAt" timestamptz NOT NULL DEFAULT now()
);

-- Indices for the Articles table
CREATE INDEX IF NOT EXISTS IDX_Articles_CreatedAt ON "propulse"."Articles" ( "CreatedAt" );
CREATE INDEX IF NOT EXISTS IDX_Articles_DeletedAt ON "propulse"."Articles" ( "DeletedAt" );
CREATE INDEX IF NOT EXISTS IDX_Articles_UpdatedAt ON "propulse"."Articles" ( "UpdatedAt" );

-- Triggers for the Articles table
CREATE OR REPLACE TRIGGER "TRG_Articles_AuditFields"
    BEFORE UPDATE OR INSERT
    ON "propulse"."Articles"
    FOR EACH ROW EXECUTE FUNCTION update_audit_fields();
