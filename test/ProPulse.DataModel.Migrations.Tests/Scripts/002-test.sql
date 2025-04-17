-- This script creates a dummy table for testing DbUp resource filtering.
CREATE TABLE IF NOT EXISTS test_embedded_table (
    id serial PRIMARY KEY,
    value text NOT NULL
);
