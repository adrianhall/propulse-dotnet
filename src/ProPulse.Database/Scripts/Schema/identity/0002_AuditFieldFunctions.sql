-- Script to create PostgreSQL functions for automatically updating timestamp columns
-- This follows the technical specification section 3.5.2

-- Function to set the CreatedAt column for new records
CREATE OR REPLACE FUNCTION identity.set_created_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    -- Only set CreatedAt if it's not already set
    IF NEW."CreatedAt" IS NULL THEN
        NEW."CreatedAt" = NOW();
    END IF;

    -- Set UpdatedAt to the same value as CreatedAt for new records
    NEW."UpdatedAt" = NEW."CreatedAt";

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Function to handle all audit field timestamp updates (UpdatedAt and DeletedAt)
CREATE OR REPLACE FUNCTION identity.set_auditfield_timestamps()
RETURNS TRIGGER AS $$
BEGIN
    -- Always update the UpdatedAt timestamp on every update
    NEW."UpdatedAt" = NOW();

    -- Handle DeletedAt timestamp based on IsDeleted flag changes
    -- If IsDeleted changed from false to true, set DeletedAt to current timestamp
    IF NEW."IsDeleted" = TRUE AND (OLD."IsDeleted" = FALSE OR OLD."IsDeleted" IS NULL) THEN
        NEW."DeletedAt" = NOW();
    -- If IsDeleted changed from true to false, clear DeletedAt
    ELSIF NEW."IsDeleted" = FALSE AND OLD."IsDeleted" = TRUE THEN
        NEW."DeletedAt" = NULL;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

