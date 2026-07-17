CREATE OR REPLACE PROCEDURE create_customer_and_account(
    p_full_name VARCHAR,
    p_identity_number CHAR(11),
    p_birth_place VARCHAR,
    p_birth_date DATE,
    p_account_name VARCHAR,
    p_account_number VARCHAR,
    p_iban VARCHAR
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_customer_id INT;
BEGIN
    -- 1. Müşteriyi Oluştur (Risk limiti varsayılan olarak 10.000 TL atanacak)
    INSERT INTO customers (full_name, identity_number, birth_place, birth_date)
    VALUES (p_full_name, p_identity_number, p_birth_place, p_birth_date)
    RETURNING id INTO v_customer_id;

    -- 2. Müşteriye Ait Vadesiz Hesabı Oluştur
    INSERT INTO accounts (customer_id, account_name, account_number, iban)
    VALUES (v_customer_id, p_account_name, p_account_number, p_iban);
    
    COMMIT;
END;
$$;