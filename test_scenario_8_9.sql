-- 8. Adım: Aynı hesaba yeni (90 ile biten) hesap kartını tanımlama
CALL create_account_card(
    'TR011234154645788999',
    '1234987654645490', 
    '11/30',
    '456'
);

-- 9. Adım: Yeni eklenen aktif kart ile 125 TL para çekme
CALL process_account_transaction(
    '1234987654645490',
    'Withdrawal',
    125.00,
    'Yeni kart ile ATM cekimi'
);