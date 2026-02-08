-- Comments Application Database Schema
-- PostgreSQL 17

-- ============================================
-- Table: comments
-- ============================================
CREATE TABLE comments (
    id                  UUID            PRIMARY KEY,
    user_name           VARCHAR(50)     NOT NULL,
    email               VARCHAR(254)    NOT NULL,
    home_page           VARCHAR(2048),
    text                TEXT            NOT NULL,
    created_at          TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    parent_comment_id   UUID            REFERENCES comments(id) ON DELETE RESTRICT
);

-- Performance indexes
CREATE INDEX ix_comments_created_at_desc ON comments (created_at DESC);
CREATE INDEX ix_comments_user_name ON comments (user_name);
CREATE INDEX ix_comments_email ON comments (email);
CREATE INDEX ix_comments_parent_comment_id ON comments (parent_comment_id);

-- Partial index for top-level comments (key optimization for 1M+ rows)
CREATE INDEX ix_comments_top_level_created_at_desc
    ON comments (created_at DESC)
    WHERE parent_comment_id IS NULL;

-- ============================================
-- Table: attachments
-- ============================================
CREATE TABLE attachments (
    id                  UUID            PRIMARY KEY,
    file_name           VARCHAR(255)    NOT NULL,
    stored_file_name    VARCHAR(255)    NOT NULL,
    content_type        VARCHAR(100)    NOT NULL,
    file_size_bytes     BIGINT          NOT NULL,
    type                INTEGER         NOT NULL,  -- 0 = Image, 1 = TextFile
    comment_id          UUID            NOT NULL REFERENCES comments(id) ON DELETE CASCADE
);

CREATE INDEX ix_attachments_comment_id ON attachments (comment_id);
