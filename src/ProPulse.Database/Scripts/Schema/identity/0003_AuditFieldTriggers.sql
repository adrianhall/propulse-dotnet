-- Script to create PostgreSQL triggers for automatically updating timestamp columns
-- This follows the technical specification section 3.5.2

-- Apply triggers to the AspNetUsers table
CREATE TRIGGER set_created_timestamp
BEFORE INSERT ON identity."AspNetUsers"
FOR EACH ROW
EXECUTE FUNCTION identity.set_created_timestamp();

CREATE TRIGGER set_auditfield_timestamps
BEFORE UPDATE ON identity."AspNetUsers"
FOR EACH ROW
EXECUTE FUNCTION identity.set_auditfield_timestamps();
