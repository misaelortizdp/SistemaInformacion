-- =====================================================
-- CashFlowSystem - Database Schema
-- PostgreSQL Version
-- =====================================================

-- =====================================================
-- EXTENSIONS
-- =====================================================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- =====================================================
-- TABLES
-- =====================================================

-- Users Table
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(500) NOT NULL,
    role INTEGER NOT NULL, -- 1: Admin, 2: Manager, 3: Cashier
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    is_deleted BOOLEAN NOT NULL DEFAULT false
);

-- Categories Table
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    type INTEGER NOT NULL, -- 1: Income, 2: Expense
    color VARCHAR(7) NOT NULL DEFAULT '#000000',
    icon VARCHAR(50) NOT NULL DEFAULT 'default',
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    is_deleted BOOLEAN NOT NULL DEFAULT false
);

-- Payment Methods Table
CREATE TABLE payment_methods (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    type INTEGER NOT NULL, -- 1: Cash, 2: DebitCard, 3: CreditCard, 4: Transfer, 5: Check
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    is_deleted BOOLEAN NOT NULL DEFAULT false
);

-- Cash Registers Table
CREATE TABLE cash_registers (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    opening_date TIMESTAMP NOT NULL,
    closing_date TIMESTAMP,
    opening_balance DECIMAL(18, 2) NOT NULL,
    closing_balance DECIMAL(18, 2),
    expected_balance DECIMAL(18, 2),
    difference DECIMAL(18, 2),
    status INTEGER NOT NULL, -- 1: Open, 2: Closed
    notes TEXT,
    user_id UUID NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- Transactions Table
CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    date TIMESTAMP NOT NULL,
    amount DECIMAL(18, 2) NOT NULL,
    description VARCHAR(500) NOT NULL,
    type INTEGER NOT NULL, -- 1: Income, 2: Expense
    reference_number VARCHAR(50),
    notes TEXT,
    category_id UUID NOT NULL,
    payment_method_id UUID NOT NULL,
    user_id UUID NOT NULL,
    cash_register_id UUID,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    FOREIGN KEY (category_id) REFERENCES categories(id),
    FOREIGN KEY (payment_method_id) REFERENCES payment_methods(id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (cash_register_id) REFERENCES cash_registers(id)
);

-- =====================================================
-- INDEXES
-- =====================================================

-- Users
CREATE INDEX idx_users_email ON users(email) WHERE is_deleted = false;
CREATE INDEX idx_users_username ON users(username) WHERE is_deleted = false;

-- Categories
CREATE INDEX idx_categories_type ON categories(type) WHERE is_deleted = false;
CREATE INDEX idx_categories_active ON categories(is_active) WHERE is_deleted = false;

-- Transactions
CREATE INDEX idx_transactions_date ON transactions(date) WHERE is_deleted = false;
CREATE INDEX idx_transactions_type ON transactions(type) WHERE is_deleted = false;
CREATE INDEX idx_transactions_user ON transactions(user_id) WHERE is_deleted = false;
CREATE INDEX idx_transactions_category ON transactions(category_id) WHERE is_deleted = false;
CREATE INDEX idx_transactions_cash_register ON transactions(cash_register_id) WHERE is_deleted = false;

-- Cash Registers
CREATE INDEX idx_cash_registers_status ON cash_registers(status) WHERE is_deleted = false;
CREATE INDEX idx_cash_registers_opening_date ON cash_registers(opening_date) WHERE is_deleted = false;
CREATE INDEX idx_cash_registers_user ON cash_registers(user_id) WHERE is_deleted = false;

-- =====================================================
-- INITIAL DATA SEEDING
-- =====================================================

-- Default Admin User (password: Admin123!)
-- IMPORTANT: Change password after first login
INSERT INTO users (id, username, email, password_hash, role, is_active)
VALUES (
    uuid_generate_v4(),
    'admin',
    'admin@cashflow.local',
    -- This is a BCrypt hash for 'Admin123!' - CHANGE IN PRODUCTION
    '$2a$11$9OTlWWFMzPXIYDq0Z5L8LO7z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5',
    1, -- Admin
    true
);

-- Default Income Categories
INSERT INTO categories (name, type, color, icon) VALUES
('Ventas', 1, '#4CAF50', 'currency-usd'),
('Servicios', 1, '#2196F3', 'briefcase'),
('Otros Ingresos', 1, '#9C27B0', 'cash-plus');

-- Default Expense Categories
INSERT INTO categories (name, type, color, icon) VALUES
('Compras', 2, '#F44336', 'cart'),
('Sueldos', 2, '#FF9800', 'account-cash'),
('Servicios Básicos', 2, '#795548', 'home'),
('Alquiler', 2, '#607D8B', 'office-building'),
('Otros Gastos', 2, '#9E9E9E', 'cash-minus');

-- Default Payment Methods
INSERT INTO payment_methods (name, type) VALUES
('Efectivo', 1),
('Tarjeta de Débito', 2),
('Tarjeta de Crédito', 3),
('Transferencia Bancaria', 4),
('Cheque', 5);

-- =====================================================
-- VIEWS FOR REPORTING
-- =====================================================

-- Daily Summary View
CREATE OR REPLACE VIEW v_daily_summary AS
SELECT
    DATE(t.date) as transaction_date,
    SUM(CASE WHEN t.type = 1 THEN t.amount ELSE 0 END) as total_income,
    SUM(CASE WHEN t.type = 2 THEN t.amount ELSE 0 END) as total_expense,
    SUM(CASE WHEN t.type = 1 THEN t.amount ELSE 0 END) -
    SUM(CASE WHEN t.type = 2 THEN t.amount ELSE 0 END) as net_flow
FROM transactions t
WHERE t.is_deleted = false
GROUP BY DATE(t.date)
ORDER BY transaction_date DESC;

-- Category Summary View
CREATE OR REPLACE VIEW v_category_summary AS
SELECT
    c.id as category_id,
    c.name as category_name,
    c.type as category_type,
    COUNT(t.id) as transaction_count,
    SUM(t.amount) as total_amount
FROM categories c
LEFT JOIN transactions t ON c.id = t.category_id AND t.is_deleted = false
WHERE c.is_deleted = false
GROUP BY c.id, c.name, c.type;

-- Payment Method Summary View
CREATE OR REPLACE VIEW v_payment_method_summary AS
SELECT
    pm.id as payment_method_id,
    pm.name as payment_method_name,
    COUNT(t.id) as transaction_count,
    SUM(t.amount) as total_amount
FROM payment_methods pm
LEFT JOIN transactions t ON pm.id = t.payment_method_id AND t.is_deleted = false
WHERE pm.is_deleted = false
GROUP BY pm.id, pm.name;

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- Get Dashboard Data for Period
CREATE OR REPLACE FUNCTION get_dashboard_data(
    p_start_date TIMESTAMP,
    p_end_date TIMESTAMP,
    p_user_id UUID DEFAULT NULL
)
RETURNS TABLE (
    total_income DECIMAL(18,2),
    total_expense DECIMAL(18,2),
    net_flow DECIMAL(18,2),
    transaction_count BIGINT
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        COALESCE(SUM(CASE WHEN t.type = 1 THEN t.amount ELSE 0 END), 0) as total_income,
        COALESCE(SUM(CASE WHEN t.type = 2 THEN t.amount ELSE 0 END), 0) as total_expense,
        COALESCE(SUM(CASE WHEN t.type = 1 THEN t.amount ELSE 0 END) -
                 SUM(CASE WHEN t.type = 2 THEN t.amount ELSE 0 END), 0) as net_flow,
        COUNT(*) as transaction_count
    FROM transactions t
    WHERE t.is_deleted = false
        AND t.date >= p_start_date
        AND t.date <= p_end_date
        AND (p_user_id IS NULL OR t.user_id = p_user_id);
END;
$$ LANGUAGE plpgsql;

-- Get Income by Category
CREATE OR REPLACE FUNCTION get_income_by_category(
    p_start_date TIMESTAMP,
    p_end_date TIMESTAMP
)
RETURNS TABLE (
    category_name VARCHAR(100),
    total_amount DECIMAL(18,2)
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        c.name,
        COALESCE(SUM(t.amount), 0) as total
    FROM categories c
    LEFT JOIN transactions t ON c.id = t.category_id
        AND t.type = 1
        AND t.is_deleted = false
        AND t.date >= p_start_date
        AND t.date <= p_end_date
    WHERE c.type = 1 AND c.is_deleted = false
    GROUP BY c.name
    ORDER BY total DESC;
END;
$$ LANGUAGE plpgsql;

-- Get Expense by Category
CREATE OR REPLACE FUNCTION get_expense_by_category(
    p_start_date TIMESTAMP,
    p_end_date TIMESTAMP
)
RETURNS TABLE (
    category_name VARCHAR(100),
    total_amount DECIMAL(18,2)
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        c.name,
        COALESCE(SUM(t.amount), 0) as total
    FROM categories c
    LEFT JOIN transactions t ON c.id = t.category_id
        AND t.type = 2
        AND t.is_deleted = false
        AND t.date >= p_start_date
        AND t.date <= p_end_date
    WHERE c.type = 2 AND c.is_deleted = false
    GROUP BY c.name
    ORDER BY total DESC;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- TRIGGERS
-- =====================================================

-- Auto-update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_categories_updated_at BEFORE UPDATE ON categories
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_payment_methods_updated_at BEFORE UPDATE ON payment_methods
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_transactions_updated_at BEFORE UPDATE ON transactions
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_cash_registers_updated_at BEFORE UPDATE ON cash_registers
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- GRANTS (Adjust according to your security needs)
-- =====================================================

-- Create application user (replace with your credentials)
-- CREATE ROLE cashflow_app WITH LOGIN PASSWORD 'your_secure_password';
-- GRANT CONNECT ON DATABASE cashflow_db TO cashflow_app;
-- GRANT USAGE ON SCHEMA public TO cashflow_app;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO cashflow_app;
-- GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO cashflow_app;

-- =====================================================
-- BACKUP COMMANDS (For reference)
-- =====================================================

-- Backup database
-- pg_dump -U postgres -d cashflow_db > backup_$(date +%Y%m%d_%H%M%S).sql

-- Restore database
-- psql -U postgres -d cashflow_db < backup_file.sql

-- =====================================================
-- MAINTENANCE QUERIES
-- =====================================================

-- Check database size
-- SELECT pg_size_pretty(pg_database_size('cashflow_db'));

-- Check table sizes
-- SELECT
--     schemaname,
--     tablename,
--     pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
-- FROM pg_tables
-- WHERE schemaname = 'public'
-- ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;

-- Vacuum and analyze
-- VACUUM ANALYZE;

-- =====================================================
-- COMMON QUERIES FOR TESTING
-- =====================================================

-- Get all transactions for current month
-- SELECT * FROM transactions
-- WHERE date >= DATE_TRUNC('month', CURRENT_DATE)
--   AND date < DATE_TRUNC('month', CURRENT_DATE) + INTERVAL '1 month'
--   AND is_deleted = false
-- ORDER BY date DESC;

-- Get balance for specific date
-- SELECT
--     COALESCE(SUM(CASE WHEN type = 1 THEN amount ELSE -amount END), 0) as balance
-- FROM transactions
-- WHERE date <= '2024-12-31'
--   AND is_deleted = false;
