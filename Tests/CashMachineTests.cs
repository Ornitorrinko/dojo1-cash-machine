using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Dojo.Tests
{
    // TODO: testar BankAccount

    [TestFixture]
    public class CashMachineTests
    {
        private BankAccount _bankAccount;
        private CashMachine _cashMachine;

        [SetUp]
        public void Initialize()
        {
            _bankAccount = new BankAccount();
            _cashMachine = new CashMachine(_bankAccount);
        }

        [TestCase(100)]
        [TestCase(200)]
        [TestCase(300)]
        public void ShouldGetAccountBalance(decimal expected)
        {
            _bankAccount.Credit(expected);

            var result = _cashMachine.GetAccountBalance();

            Assert.AreEqual(expected, result);
        }

        [TestCase(50)]
        [TestCase(80)]
        public void ShouldMakeADeposit(decimal value)
        {
            _cashMachine.MakeADeposit(value);

            Assert.AreEqual(value, _cashMachine.GetAccountBalance());
        }

        [TestCase(30)]
        public void ShouldMakeWithdraw(decimal value)
        {
            _bankAccount.Credit(value);
            var prevBalance = _cashMachine.GetAccountBalance();

            _cashMachine.MakeWithDraw(value);

            var actualBalance = _cashMachine.GetAccountBalance();
            Assert.AreEqual(prevBalance, actualBalance);
        }

        [TestCase(50)]
        public void ShouldThrowsWhenWithdrawWithoutEnoughBalance(decimal value)
        {
            _bankAccount.Credit(value - 10);

            Assert.Throws<InsufficientFundsException>(() => _cashMachine.MakeWithDraw(value));
        }

        [TestCase(-50)]
        public void ShouldThrowsWhenWithdrawWithNegativeValue(decimal value)
        {
            Assert.Throws<NegativeWithDrawException>(() => _cashMachine.MakeWithDraw(value));
        }
    }

    public class InsufficientFundsException : Exception { }

    public class NegativeWithDrawException : Exception { }

    public class CashMachine
    {
        private const int MinValue = 0;
        public BankAccount BankAccount { get; }

        public CashMachine(BankAccount bankAccount)
        {
            BankAccount = bankAccount;
        }

        public decimal GetAccountBalance()
        {
            return BankAccount.GetBalance();
        }

        public void MakeADeposit(decimal value)
        {
            BankAccount.Credit(value);
        }

        public void MakeWithDraw(decimal value)
        {
            if (value < MinValue)
            {
                throw new NegativeWithDrawException();
            }

            if (value > GetAccountBalance())
            {
                throw new InsufficientFundsException();
            }

            BankAccount.Credit(GetAccountBalance() - value);

        }
    }

    public class BankAccount
    {
        private IList<Transaction> Transactions { get; }
        private IEnumerable<Transaction> Credits => Transactions.Where(e => e.Type == TransactionType.Credit);
        private IEnumerable<Transaction> Debits => Transactions.Where(e => e.Type == TransactionType.Debit);

        public BankAccount()
        {
            Transactions = new List<Transaction>();
        }

        public void Debit(decimal value)
        {
            Transactions.Add(new Transaction(TransactionType.Debit, value));
        }

        public void Credit(decimal value)
        {
            Transactions.Add(new Transaction(TransactionType.Credit, value));
        }

        public decimal GetBalance()
        {
            return Credits.Sum(e => e.Value) - Debits.Sum(e => e.Value);
        }
    }

    public class Transaction
    {
        public TransactionType Type { get; }
        public decimal Value { get; }

        public Transaction(TransactionType type, decimal value)
        {
            Type = type;
            Value = value;
        }
    }

    public enum TransactionType
    {
        Debit,
        Credit
    }
}
