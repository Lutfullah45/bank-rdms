CREATE OR REPLACE PROCEDURE create_account_card(
    p_iban VARCHAR,
    p_card_number CHAR(16),
    p_expiration_month_year VARCHAR(5),
    p_ccv VARCHAR(3)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_account_id INT;
BEGIN
    -- 1. Verilen IBAN numarasına göre hesabı bul
    SELECT id INTO v_account_id FROM accounts WHERE iban = p_iban;
    
    IF v_account_id IS NULL THEN
        RAISE EXCEPTION 'Belirtilen IBAN ile eşleşen bir hesap bulunamadı!';
    END IF;

    -- 2. Kural: Sadece 1 aktif hesap kartı olabilir. Eski kartları pasife çek.
    UPDATE account_cards 
    SET is_active = FALSE 
    WHERE account_id = v_account_id;

    -- 3. Yeni hesap kartını oluştur ve aktif olarak ekle
    INSERT INTO account_cards (account_id, card_number, expiration_month_year, ccv, is_active)
    VALUES (v_account_id, p_card_number, p_expiration_month_year, p_ccv, TRUE);
END;
$$;