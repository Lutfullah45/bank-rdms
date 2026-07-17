CREATE OR REPLACE PROCEDURE process_account_transaction(
    p_card_number CHAR(16),
    p_transaction_type VARCHAR(50), 
    p_amount NUMERIC(15, 2),
    p_description VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_account_id INT;
    v_type_id INT;
    v_current_balance NUMERIC(15, 2) := 0;
    v_deposit_id INT;
    v_withdrawal_id INT;
BEGIN
    -- 1. Kartı bul ve bağlı olduğu hesabı al
    SELECT account_id INTO v_account_id 
    FROM account_cards 
    WHERE card_number = p_card_number AND is_active = TRUE;

    IF v_account_id IS NULL THEN
        RAISE EXCEPTION 'Aktif hesap kartı bulunamadı!';
    END IF;

    -- 2. İşlem tipini id olarak al (Deposit veya Withdrawal)
    SELECT id INTO v_type_id FROM transaction_types WHERE type_name = p_transaction_type;

    IF v_type_id IS NULL THEN
        RAISE EXCEPTION 'Geçersiz işlem tipi!';
    END IF;

    -- 3. Eğer para çekme işlemiyse hesap bakiyesini kontrol et
    IF p_transaction_type = 'Withdrawal' THEN
        SELECT id INTO v_deposit_id FROM transaction_types WHERE type_name = 'Deposit';
        SELECT id INTO v_withdrawal_id FROM transaction_types WHERE type_name = 'Withdrawal';
        
        -- Hesaptaki mevcut bakiyeyi (Yatırılan - Çekilen) hesapla
        SELECT COALESCE(SUM(
            CASE 
                WHEN transaction_type_id = v_deposit_id THEN amount 
                WHEN transaction_type_id = v_withdrawal_id THEN -amount 
                ELSE 0 
            END
        ), 0) INTO v_current_balance
        FROM transactions
        WHERE account_id = v_account_id;

        IF v_current_balance < p_amount THEN
            RAISE EXCEPTION 'Yetersiz bakiye! İşlem gerçekleştirilemedi.';
        END IF;
    END IF;

    -- 4. İşlemi kaydet
    INSERT INTO transactions (account_id, transaction_type_id, amount, description)
    VALUES (v_account_id, v_type_id, p_amount, p_description);
END;
$$;