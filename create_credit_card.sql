CREATE OR REPLACE PROCEDURE create_credit_card(
    p_identity_number CHAR(11),
    p_card_number CHAR(16),
    p_expiration_month_year VARCHAR(5),
    p_ccv VARCHAR(3),
    p_card_limit NUMERIC(15, 2)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_customer_id INT;
    v_risk_limit NUMERIC(15, 2);
    v_current_total_limit NUMERIC(15, 2);
BEGIN
    -- 1. Müşteriyi TC Kimlik numarasından bul ve risk limitini al
    SELECT id, risk_limit INTO v_customer_id, v_risk_limit 
    FROM customers 
    WHERE identity_number = p_identity_number;
    
    IF v_customer_id IS NULL THEN
        RAISE EXCEPTION 'Belirtilen TC Kimlik numarasına sahip müşteri bulunamadı!';
    END IF;

    -- 2. Müşterinin mevcut aktif kredi kartlarının toplam limitini hesapla
    SELECT COALESCE(SUM(card_limit), 0) INTO v_current_total_limit
    FROM credit_cards
    WHERE customer_id = v_customer_id AND is_active = TRUE;

    -- 3. Risk limiti aşımı kontrolü
    IF (v_current_total_limit + p_card_limit) > v_risk_limit THEN
        RAISE EXCEPTION 'İşlem reddedildi: Kredi kartı limitleri toplamı, müşterinin risk limitini aşamaz!';
    END IF;

    -- 4. Güvenlik kontrolü geçildiyse kredi kartını oluştur
    INSERT INTO credit_cards (customer_id, card_number, expiration_month_year, ccv, card_limit, is_active)
    VALUES (v_customer_id, p_card_number, p_expiration_month_year, p_ccv, p_card_limit, TRUE);
END;
$$;