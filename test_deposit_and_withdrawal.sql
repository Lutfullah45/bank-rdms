-- 6. Adım: 89 ile biten kart ile 1500 TL yatırma
CALL process_account_transaction(
    '1234987654645489',
    'Deposit',
    1500.00,
    'Elden para yatirma'
);

-- 7. Adım: 89 ile biten kart ile 350 TL çekme
CALL process_account_transaction(
    '1234987654645489',
    'Withdrawal',
    350.00,
    'ATM para cekme'
);