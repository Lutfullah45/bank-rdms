CREATE OR REPLACE PROCEDURE insert_lookup_data()
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO transaction_types (type_name) VALUES 
    ('Deposit'), 
    ('Withdrawal'), 
    ('Shopping'), 
    ('Debt Payment')
    ON CONFLICT DO NOTHING;
END;
$$;