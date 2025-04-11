-- Database Reset Utility Script for ProPulse Development Environment
-- This script deletes the propulse and identity schemas along with the schema migration tracking table.
-- After running this script, the DbUp migrations will need to be reapplied.

-- Stop if we're not in development
DO $$
DECLARE
    app_env text;
BEGIN
    -- Try to get environment from a temporary table or configuration
    -- This is a safety check to prevent accidentally running in production
    app_env := current_setting('propulse.environment', true);

    IF app_env IS NULL THEN
        app_env := 'development'; -- Default to development if not set
    END IF;

    IF app_env != 'development' THEN
        RAISE EXCEPTION 'This script can only be run in development environment. Current environment: %', app_env;
    END IF;
END $$;

-- Disable notices and warnings temporarily to make output cleaner
SET client_min_messages TO error;

-- Function to drop schema if it exists
CREATE OR REPLACE FUNCTION drop_schema_if_exists(schema_name text) RETURNS void AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name = $1) THEN
        EXECUTE 'DROP SCHEMA ' || schema_name || ' CASCADE';
        RAISE NOTICE 'Schema "%" has been dropped', schema_name;
    ELSE
        RAISE NOTICE 'Schema "%" does not exist, skipping', schema_name;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Drop ProPulse application schemas
SELECT drop_schema_if_exists('propulse');
SELECT drop_schema_if_exists('identity');

-- Drop migration tracking table if it exists
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = '__schema_migrations') THEN
        DROP TABLE public.__schema_migrations;
        RAISE NOTICE 'Migration tracking table "__schema_migrations" has been dropped';
    ELSE
        RAISE NOTICE 'Migration tracking table "__schema_migrations" does not exist, skipping';
    END IF;
END $$;

-- Clean up the temporary function
DROP FUNCTION IF EXISTS drop_schema_if_exists(text);

-- Restore normal message level
SET client_min_messages TO notice;

-- Summary
DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '=================================================';
    RAISE NOTICE 'ProPulse database has been reset successfully';
    RAISE NOTICE 'To recreate the database, run the DbUp migrations';
    RAISE NOTICE '=================================================';
END $$;
