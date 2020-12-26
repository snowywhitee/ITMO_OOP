using System;

namespace BankSimulator
{
    public interface IAccount
    {
        IAccount Successor { get; set; }
        void Transfer(int id, double amount);
        void Withdraw(double amount);
        double Money { get; }
        int Put(double amount);
        public int CancelLastTransaction();
    }
    public class BankException : Exception
    {
        public BankException(string msg) : base(msg)
        {

        }
    }
}
