CREATE OR REPLACE PROCEDURE create_all_tables()
LANGUAGE plpgsql
AS $$
BEGIN
    CREATE TABLE customers (
        id SERIAL PRIMARY KEY,
        full_name VARCHAR(100) NOT NULL,
        identity_number CHAR(11) UNIQUE NOT NULL,
        birth_place VARCHAR(50) NOT NULL,
        birth_date DATE NOT NULL,
        risk_limit NUMERIC(15, 2) DEFAULT 10000.00,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE accounts (
        id SERIAL PRIMARY KEY,
        customer_id INT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
        account_name VARCHAR(50) NOT NULL,
        account_number VARCHAR(50) UNIQUE NOT NULL,
        iban VARCHAR(26) UNIQUE NOT NULL,
        is_active BOOLEAN DEFAULT TRUE,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE account_cards (
        id SERIAL PRIMARY KEY,
        account_id INT NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
        card_number CHAR(16) UNIQUE NOT NULL,
        expiration_month_year VARCHAR(5) NOT NULL,
        ccv VARCHAR(3) NOT NULL,
        is_active BOOLEAN DEFAULT TRUE,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE credit_cards (
        id SERIAL PRIMARY KEY,
        customer_id INT NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
        card_number CHAR(16) UNIQUE NOT NULL,
        expiration_month_year VARCHAR(5) NOT NULL,
        ccv VARCHAR(3) NOT NULL,
        card_limit NUMERIC(15, 2) NOT NULL,
        is_active BOOLEAN DEFAULT TRUE,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE transaction_types (
        id SERIAL PRIMARY KEY,
        type_name VARCHAR(50) UNIQUE NOT NULL
    );

    CREATE TABLE transactions (
        id SERIAL PRIMARY KEY,
        account_id INT REFERENCES accounts(id) ON DELETE CASCADE,
        credit_card_id INT REFERENCES credit_cards(id) ON DELETE CASCADE,
        transaction_type_id INT NOT NULL REFERENCES transaction_types(id),
        amount NUMERIC(15, 2) NOT NULL,
        description VARCHAR(100),
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );
END;
$$;