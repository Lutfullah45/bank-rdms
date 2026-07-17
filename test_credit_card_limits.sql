-- 5.000 TL limitli birinci kredi kartını ekleme
CALL create_credit_card(
    '11111111111', 
    '1234879654645488', 
    '08/28', 
    '456', 
    5000.00
);

-- 3.000 TL limitli ikinci kredi kartını ekleme
CALL create_credit_card(
    '11111111111', 
    '1234879654645487', 
    '09/28', 
    '789', 
    3000.00
);