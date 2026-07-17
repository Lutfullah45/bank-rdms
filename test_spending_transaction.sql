-- 4. Adım: 88 ile biten karttan Yaz Tatili harcaması
CALL process_credit_card_spending(
    '1234879654645488', 
    750.25, 
    'Yaz Tatili'
);

-- 5. Adım: 87 ile biten karttan Pandemi harcaması
CALL process_credit_card_spending(
    '1234879654645487', 
    15.50, 
    'Pandemi'
);