using System;
using System.Collections.Generic;

namespace BankSimulator
{
    public class Bank
    {
        private double debitPercentage;
        private double depositPercentage;
        private double creditLimit;
        private double transactionLimit;
        private double creditCommission;
        private DateTime depositTerm;
        private string name;
        public string Name { get => name; }
        public double DebitPercentage { get => debitPercentage; }
        public double CreditLimit { get => creditLimit; }
        public double CreditCommission { get => creditCommission; }
        public double TransactionLimit { get => transactionLimit; }
        public DateTime DepositTerm { get => depositTerm; }
        private Dictionary<int, Client> clients = new Dictionary<int, Client>();
        private Dictionary<int, Account> accounts = new Dictionary<int, Account>();
        public void AddClient(Client client, Account account)
        {
            if (!clients.ContainsKey(client.Id))
            {
                clients.Add(client.Id, client);
                accounts.Add(account.Id, account);
            }
            else
            {
                accounts.Add(account.Id, account);
            }
        }
        public void CancelTransaction(Client client, int senderId, int recieverId, double amount)
        {
            if (clients.ContainsKey(client.Id) && accounts.ContainsKey(senderId))
            {
                //Here we check whether we allow canceling
                if (accounts[senderId].CancelTransaction(accounts[senderId].GetTransactionId(recieverId, amount)) == -1)
                {
                    throw new BankException($"Unable to complete operation. Declined.");
                }
            }
            else
            {
                throw new BankException($"This bank {Name} doesn't have this client: {client.FirstName} {client.LastName}");
            }
        }
        public Bank(string name, double debitPercentage, double depositPercentage, double creditLimit, double creditCommission, double transactionLimit, DateTime depositTerm)
        {
            this.name = name;
            this.debitPercentage = debitPercentage;
            this.depositPercentage = depositPercentage;
            this.creditLimit = creditLimit;
            this.creditCommission = creditCommission;
            this.transactionLimit = transactionLimit;
            this.depositTerm = depositTerm;
        }
        public double GetDepositPercentage(double amount)
        {
            if (amount < 50000)
            {
                return debitPercentage + 0.02;
            }
            else
            {
                return debitPercentage + 0.03;
            }
        }
    }
}
