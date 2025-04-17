-- This script is intentionally invalid and should cause a migration failure.
CREATE TABLE bad_table (
    id serial PRIMARY KEY,
    -- Missing column type below will cause a syntax error
    broken_column
);
