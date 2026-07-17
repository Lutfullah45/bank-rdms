-- 10. Adım: Hesaptan kredi kartına 750.25 TL aktarma
CALL transfer_to_credit_card(
    'TR011234154645788999',
    '1234879654645488',
    750.25,
    'Kredi karti borc odemesi'
);