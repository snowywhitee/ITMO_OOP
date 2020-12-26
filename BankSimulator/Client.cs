using System.Collections.Generic;
using System.Linq;

namespace BankSimulator
{
    public class Client
    {
        //Private fields
        private int id;
        private static int idCount = 0;
        private string firstName;
        private string lastName;
        private string address;
        private string passportNumber;
        private bool isSuspicious = true;

        //Properties
        public bool IsSuspicious { get => isSuspicious; set => isSuspicious = value; }
        public int Id { get => id; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Address { get => address; set => address = value; }
        public string PassportNumber { get => passportNumber; set => passportNumber = value; }

        //Info
        private Dictionary<int, Account> accounts = new Dictionary<int, Account>();
        private List<Account> priorities = new List<Account>();

        //Methods
        public Client(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                throw new BankException($"Invalid client information, firstName: {firstName}, lastName: {lastName}");
            }
            this.id = idCount;
            idCount++;
            this.firstName = firstName;
            this.lastName = lastName;
        }
        public Account MakeDeposit(Bank bank, double amount)
        {
            Account account = new Deposit(bank, amount, this);
            accounts.Add(account.Id, account);
            return account;
        }
        public Account MakeAccount(AccountType accountType, Bank bank)
        {
            if (accountType == AccountType.Credit)
            {
                Account account = new CreditAccount(bank, this);
                accounts.Add(account.Id, account);
                if (priorities.Count != 0)
                {
                    account.Successor = priorities.Last();
                }
                priorities.Add(account);
            }
            if (accountType == AccountType.Debit)
            {
                Account account = new DebitAccount(bank, this);
                accounts.Add(account.Id, account);
                if (priorities.Count != 0)
                {
                    account.Successor = priorities.Last();
                }
                priorities.Add(account);
            }
            return priorities.Last();
        }
        public void Withdraw(double amount)
        {
            priorities.Last().Withdraw(amount);
        }
        public void Transfer(int id, double amount)
        {
            priorities.Last().Transfer(id, amount);
        }
        public void TransferAccount(int sendId, int recieveId, double amount)
        {
            if (accounts.ContainsKey(sendId))
            {
                if (accounts[sendId].SelfTransfer(recieveId, amount) == -1)
                {
                    throw new BankException($"Unable to complete operation. Declined.");
                }
            }
            else
            {
                throw new BankException($"This Client {firstName} {lastName} doesn't have this card: {sendId}");
            }
        }
        public void PutAccount(int id, double amount)
        {
            if (accounts.ContainsKey(id))
            {
                if (accounts[id].Put(amount) == -1)
                {
                    throw new BankException($"Unable to complete operation. Declined.");
                }
            }
            else
            {
                throw new BankException($"This Client {firstName} {lastName} doesn't have this card: {id}");
            }
        }
        public void WithdrawAccount(int id, double amount)
        {
            if (accounts.ContainsKey(id))
            {
                if (accounts[id].SelfWithdraw(amount) == -1)
                {
                    throw new BankException($"Unable to complete operation. Declined.");
                }
            }
            else
            {
                throw new BankException($"This Client {firstName} {lastName} doesn't have this card: {id}");
            }
        }
        public void CancelLastTransaction(int id)
        {
            if (accounts.ContainsKey(id))
            {
                if (accounts[id].CancelLastTransaction() == -1)
                {
                    throw new BankException($"Unable to complete operation. Declined.");
                }
            }
            else
            {
                throw new BankException($"This Client {firstName} {lastName} doesn't have this card: {id}");
            }
        }
    }
}
