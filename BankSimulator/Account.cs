using System;
using System.Collections.Generic;

namespace BankSimulator
{
    public abstract class Account : IAccount
    {
        //Private fields
        protected int id;
        private static int idCount = 0;
        protected double money;
        protected DateTime lastTimeChanged;
        private double transactionLimit;
        private static int transactionCount = 0;
        protected Client owner;

        //Time travel addon
        protected DateTime now;

        //Properties
        public int Id { get => id; }
        public virtual double Money { get; protected set; }
        public IAccount Successor { get; set; }

        //Info
        protected static Dictionary<int, Account> accounts = new Dictionary<int, Account>();
        protected Dictionary<int, (int, double)> transactions = new Dictionary<int, (int, double)>();
        protected Stack<int> transactionIds = new Stack<int>();

        //Methods
        public Account(Bank bank, Client owner)
        {
            this.id = idCount;
            idCount++;
            accounts.Add(this.id, this);
            this.lastTimeChanged = DateTime.Now;
            this.transactionLimit = bank.TransactionLimit;
            this.owner = owner;
            bank.AddClient(owner, this);
        }
        public int GetTransactionId(int recieverId, double amount)
        {
            foreach (var item in transactions)
            {
                if (item.Value.Item1 == recieverId && item.Value.Item2 == amount)
                {
                    return item.Key;
                }
            }
            return -1;
        }
        public int CancelTransaction(int transactionId)
        {
            UpdateAccount();
            if (transactions.ContainsKey(transactionId))
            {
                //try
                //{
                //    accounts[transactions[transactionId].Item1].Withdraw(transactions[transactionId].Item2);
                //}
                //catch (BankException)
                //{
                //    return -1;
                //}
                if (accounts[transactions[transactionId].Item1].SelfWithdraw(transactions[transactionId].Item2) == -1)
                {
                    return -1;
                }
                Put(transactions[transactionId].Item2);
                transactions.Remove(transactionId);
                return 0;
            }
            return -1;
        }
        public int Put(double amount)
        {
            UpdateAccount();
            if (amount < 0) return -1;
            money += amount;
            return 0;
        }
        public void Withdraw(double amount)
        {
            if (SelfWithdraw(amount) == -1)
            {
                if (Successor != null)
                {
                    Successor.Withdraw(amount);
                }
                else
                {
                    throw new BankException($"Unable to complete operation. Declined.");
                }
            }
        }
        public void Transfer(int id, double amount)
        {
            if (SelfTransfer(id, amount) == -1)
            {
                if (Successor != null)
                {
                    Successor.Transfer(id, amount);
                }
                else
                {
                    throw new BankException($"Unable to complete operation. Declined.");
                }
            }
        }
        public int CancelLastTransaction()
        {
            return CancelTransaction(transactionIds.Pop());
        }
        public void TimeTravel(TimeSpan amount)
        {
            //Console.WriteLine($"Old date: {lastTimeChanged.ToString("dd.MMM.yyyy HH:mm:ss")}");
            DateTime newDate = lastTimeChanged;
            while (amount >= new TimeSpan(1, 0, 0, 0))
            {
                amount -= new TimeSpan(1, 0, 0, 0);
                newDate += new TimeSpan(1, 0, 0, 0);
                //Console.WriteLine($"+= date: {newDate.ToString("dd.MMM.yyyy HH:mm:ss")}");
                now = newDate;
                UpdateAccount();
            }
        }
        
        //Helper methods
        protected abstract void UpdateAccount();
        public virtual int SelfWithdraw(double amount)
        {
            if (owner.IsSuspicious && amount > transactionLimit) return -1;
            UpdateAccount();
            if (amount < 0) return -1;
            Money -= amount;
            return 0;
        }
        public int SelfTransfer(int id, double amount)
        {
            if (owner.IsSuspicious && amount > transactionLimit) return -1;
            UpdateAccount();
            if (amount < 0 || !accounts.ContainsKey(id)) return -1;
            if (SelfWithdraw(amount) == -1) return -1;
            accounts[id].Put(amount);
            transactions.Add(transactionCount, (id, amount));
            transactionIds.Push(transactionCount);
            transactionCount++;
            return 0;
        }
    }
}
