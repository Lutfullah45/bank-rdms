CREATE OR REPLACE PROCEDURE process_credit_card_spending(
    p_card_number CHAR(16),
    p_amount NUMERIC(15, 2),
    p_description VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_card_id INT;
    v_card_limit NUMERIC(15, 2);
    v_total_debt NUMERIC(15, 2) := 0;
    v_shopping_type_id INT;
    v_payment_type_id INT;
BEGIN
    -- 1. Kartı bul ve limitini al
    SELECT id, card_limit INTO v_card_id, v_card_limit 
    FROM credit_cards 
    WHERE card_number = p_card_number AND is_active = TRUE;

    IF v_card_id IS NULL THEN
        RAISE EXCEPTION 'Aktif kredi kartı bulunamadı!';
    END IF;

    -- 2. Lookup tablosundan işlem tiplerini (Harcama ve Borç Ödeme) al
    SELECT id INTO v_shopping_type_id FROM transaction_types WHERE type_name = 'Shopping';
    SELECT id INTO v_payment_type_id FROM transaction_types WHERE type_name = 'Debt Payment';

    -- 3. Kartın mevcut borcunu (Harcamalar - Ödemeler) hesapla
    SELECT COALESCE(SUM(
        CASE 
            WHEN transaction_type_id = v_shopping_type_id THEN amount 
            WHEN transaction_type_id = v_payment_type_id THEN -amount 
            ELSE 0 
        END
    ), 0) INTO v_total_debt
    FROM transactions
    WHERE credit_card_id = v_card_id;

    -- 4. Limit yetersizliği kontrolü
    IF (v_total_debt + p_amount) > v_card_limit THEN
        RAISE EXCEPTION 'Yetersiz bakiye! İşlem kredi kartı limitini aşıyor.';
    END IF;

    -- 5. İşlemi kaydet
    INSERT INTO transactions (credit_card_id, transaction_type_id, amount, description)
    VALUES (v_card_id, v_shopping_type_id, p_amount, p_description);
END;
$$;