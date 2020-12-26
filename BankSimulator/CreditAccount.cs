using System;

namespace BankSimulator
{
    public class CreditAccount : Account
    {
        private double creditLimit;
        private double creditCommission;
        public double CreditLimit { get => creditLimit; }
        public double CreditCommission { get => creditCommission; }
        public override double Money
        {
            get
            {
                UpdateAccount();
                return money;
            }
            protected set
            {
                if (value < -creditLimit)
                {
                    throw new BankException($"Credit account can't have a Money value below CreditLimit: {creditLimit}. Id: {id}");
                }
                money = value;
            }
        }
        public CreditAccount(Bank bank, Client owner) : base(bank, owner)
        {
            this.creditLimit = bank.CreditLimit;
            this.creditCommission = bank.CreditCommission;
        }

        protected override void UpdateAccount()
        {
            if (now - lastTimeChanged >= new TimeSpan(30, 0, 0, 0))
            {
                if (money < 0)
                {
                    money -= creditCommission;
                    lastTimeChanged = now;
                }
            }
        }

        public override int SelfWithdraw(double amount)
        {
            UpdateAccount();
            if (money - amount < -creditLimit) return -1;
            return base.SelfWithdraw(amount);
        }

    }
}
