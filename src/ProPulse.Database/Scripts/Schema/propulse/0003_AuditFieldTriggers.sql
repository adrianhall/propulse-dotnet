-- Script to create PostgreSQL triggers for automatically updating timestamp columns
-- This follows the technical specification section 3.5.2

-- Apply triggers to the Articles table
CREATE TRIGGER set_created_timestamp
BEFORE INSERT ON propulse."Articles"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_created_timestamp();

CREATE TRIGGER set_auditfield_timestamps
BEFORE UPDATE ON propulse."Articles"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_auditfield_timestamps();

-- Apply triggers to the Attachments table
CREATE TRIGGER set_created_timestamp
BEFORE INSERT ON propulse."Attachments"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_created_timestamp();

CREATE TRIGGER set_auditfield_timestamps
BEFORE UPDATE ON propulse."Attachments"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_auditfield_timestamps();

-- Apply triggers to the Bookmarks table
CREATE TRIGGER set_created_timestamp
BEFORE INSERT ON propulse."Bookmarks"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_created_timestamp();

CREATE TRIGGER set_auditfield_timestamps
BEFORE UPDATE ON propulse."Bookmarks"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_auditfield_timestamps();

-- Apply triggers to the Categories table
CREATE TRIGGER set_created_timestamp
BEFORE INSERT ON propulse."Categories"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_created_timestamp();

CREATE TRIGGER set_auditfield_timestamps
BEFORE UPDATE ON propulse."Categories"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_auditfield_timestamps();

-- Apply triggers to the ReadingHistory table
CREATE TRIGGER set_created_timestamp
BEFORE INSERT ON propulse."ReadingHistory"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_created_timestamp();

CREATE TRIGGER set_auditfield_timestamps
BEFORE UPDATE ON propulse."ReadingHistory"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_auditfield_timestamps();

-- Apply triggers to the Tags table
CREATE TRIGGER set_created_timestamp
BEFORE INSERT ON propulse."Tags"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_created_timestamp();

CREATE TRIGGER set_auditfield_timestamps
BEFORE UPDATE ON propulse."Tags"
FOR EACH ROW
EXECUTE FUNCTION propulse.set_auditfield_timestamps();
