CREATE OR REPLACE PROCEDURE transfer_to_credit_card(
    p_iban VARCHAR,
    p_credit_card_number CHAR(16),
    p_amount NUMERIC(15, 2),
    p_description VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_account_id INT;
    v_credit_card_id INT;
    v_current_balance NUMERIC(15, 2) := 0;
    v_deposit_id INT;
    v_withdrawal_id INT;
    v_payment_type_id INT;
BEGIN
    -- 1. Verilen IBAN numarasına göre hesabı bul
    SELECT id INTO v_account_id FROM accounts WHERE iban = p_iban AND is_active = TRUE;
    
    IF v_account_id IS NULL THEN
        RAISE EXCEPTION 'Aktif hesap bulunamadı!';
    END IF;

    -- 2. Verilen kart numarasına göre kredi kartını bul
    SELECT id INTO v_credit_card_id FROM credit_cards WHERE card_number = p_credit_card_number AND is_active = TRUE;
    
    IF v_credit_card_id IS NULL THEN
        RAISE EXCEPTION 'Aktif kredi kartı bulunamadı!';
    END IF;

    -- 3. İşlem tiplerini al
    SELECT id INTO v_deposit_id FROM transaction_types WHERE type_name = 'Deposit';
    SELECT id INTO v_withdrawal_id FROM transaction_types WHERE type_name = 'Withdrawal';
    SELECT id INTO v_payment_type_id FROM transaction_types WHERE type_name = 'Debt Payment';

    -- 4. Hesaptaki mevcut bakiyeyi (Yatırılan - Çekilen) hesapla
    SELECT COALESCE(SUM(
        CASE 
            WHEN transaction_type_id = v_deposit_id THEN amount 
            WHEN transaction_type_id = v_withdrawal_id THEN -amount 
            ELSE 0 
        END
    ), 0) INTO v_current_balance
    FROM transactions
    WHERE account_id = v_account_id;

    -- 5. Bakiye kontrolü
    IF v_current_balance < p_amount THEN
        RAISE EXCEPTION 'Yetersiz bakiye! İşlem gerçekleştirilemedi.';
    END IF;

    -- 6. Hesaptan para çıkışını (Withdrawal) kaydet
    INSERT INTO transactions (account_id, transaction_type_id, amount, description)
    VALUES (v_account_id, v_withdrawal_id, p_amount, p_description);

    -- 7. Kredi kartına borç ödemesini (Debt Payment) kaydet
    INSERT INTO transactions (credit_card_id, transaction_type_id, amount, description)
    VALUES (v_credit_card_id, v_payment_type_id, p_amount, p_description);
END;
$$;