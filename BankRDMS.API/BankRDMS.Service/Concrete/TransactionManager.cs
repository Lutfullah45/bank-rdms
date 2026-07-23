using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Service.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankRDMS.Service.Concrete
{
    public class TransactionManager : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionManager(AppDbContext context)
        {
            _context = context;
        }

        // --- YARDIMCI METOTLAR: Bakiyeyi ve Borcu Anlık Hesaplar ---

        private async Task<decimal> GetAccountBalanceAsync(int accountId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .ToListAsync();

            decimal balance = 0;
            foreach (var t in transactions)
            {
                if (t.TransactionTypeId == 1) // 1: Para Yatırma
                    balance += t.Amount;
                else if (t.TransactionTypeId == 2 || t.TransactionTypeId == 4) // 2: Çekme, 4: Kredi Kartına Transfer
                    balance -= t.Amount;
            }
            return balance;
        }

        private async Task<decimal> GetCreditCardDebtAsync(int creditCardId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.CreditCardId == creditCardId)
                .ToListAsync();

            decimal debt = 0;
            foreach (var t in transactions)
            {
                if (t.TransactionTypeId == 3) // 3: Kredi Kartı Harcaması
                    debt += t.Amount;
                else if (t.TransactionTypeId == 4) // 4: Kredi Kartına Ödeme (Transfer)
                    debt -= t.Amount;
            }
            return debt;
        }

        // --- SERVİS METOTLARI ---

        public async Task DepositAsync(string cardNumber, decimal amount, string description)
        {
            var accountCard = await _context.AccountCards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
            if (accountCard == null) throw new Exception("Account card not found.");
            if (amount <= 0) throw new Exception("Amount must be greater than zero.");

            var transaction = new Transaction
            {
                AccountId = accountCard.AccountId,
                Amount = amount,
                Description = description,
                TransactionTypeId = 1 // Para Yatırma ID'si
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task WithdrawAsync(string cardNumber, decimal amount, string description)
        {
            var accountCard = await _context.AccountCards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
            if (accountCard == null) throw new Exception("Account card not found.");
            if (amount <= 0) throw new Exception("Amount must be greater than zero.");

            // Anlık Bakiye Kontrolü
            decimal currentBalance = await GetAccountBalanceAsync(accountCard.AccountId);
            if (currentBalance < amount) throw new Exception("Insufficient balance.");

            var transaction = new Transaction
            {
                AccountId = accountCard.AccountId,
                Amount = amount,
                Description = description,
                TransactionTypeId = 2 // Para Çekme ID'si
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task PurchaseAsync(string creditCardNumber, decimal amount, string description)
        {
            var creditCard = await _context.CreditCards.FirstOrDefaultAsync(c => c.CardNumber == creditCardNumber);
            if (creditCard == null) throw new Exception("Credit card not found.");
            if (amount <= 0) throw new Exception("Amount must be greater than zero.");

            // Anlık Borç ve Limit Kontrolü
            decimal currentDebt = await GetCreditCardDebtAsync(creditCard.Id);
            if (creditCard.CardLimit - currentDebt < amount) throw new Exception("Insufficient credit limit.");

            var transaction = new Transaction
            {
                CreditCardId = creditCard.Id,
                Amount = amount,
                Description = description,
                TransactionTypeId = 3 // Kredi Kartı Harcama ID'si
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        // --- GÜNCELLENEN METOT BURASI ---
        public async Task TransferToCreditCardAsync(string accountCardNumber, string creditCardNumber, decimal amount, string description)
        {
            // 1. Önce Swagger'dan gelen kart numarası ile AccountCard'ı buluyoruz
            var accountCard = await _context.AccountCards.FirstOrDefaultAsync(c => c.CardNumber == accountCardNumber);
            if (accountCard == null) throw new Exception("Account card not found.");

            // 2. Bulduğumuz kartın bağlı olduğu asıl hesabı (Account) buluyoruz
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountCard.AccountId);
            if (account == null) throw new Exception("Account not found.");

            // Kredi kartını buluyoruz
            var creditCard = await _context.CreditCards.FirstOrDefaultAsync(c => c.CardNumber == creditCardNumber);
            if (creditCard == null) throw new Exception("Credit card not found.");

            if (amount <= 0) throw new Exception("Amount must be greater than zero.");

            // Anlık Bakiye Kontrolü (Bulduğumuz asıl hesabın bakiyesini kontrol ediyoruz)
            decimal currentBalance = await GetAccountBalanceAsync(account.Id);
            if (currentBalance < amount) throw new Exception("Insufficient account balance.");

            var transaction = new Transaction
            {
                AccountId = account.Id,
                CreditCardId = creditCard.Id,
                Amount = amount,
                Description = description,
                TransactionTypeId = 4 // Kredi Kartına Transfer/Ödeme ID'si
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.CreditCard)
                .Include(t => t.TransactionType)
                .ToListAsync();
        }

        public Transaction GetTransactionById(int id)
        {
            return _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.CreditCard)
                .Include(t => t.TransactionType)
                .FirstOrDefault(t => t.Id == id)!;
        }
    }
}