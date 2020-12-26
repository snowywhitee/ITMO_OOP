using System;

namespace BankSimulator
{
    public class Deposit : Account
    {
        private double depositPercentage;
        private DateTime depositTerm;
        private double bonusAmount = 0.0;
        public override double Money
        {
            get
            {
                UpdateAccount();
                return money;
            }
            protected set
            {
                if (value < 0)
                {
                    throw new BankException($"Deposit Money value can't be negative {money} -> {value}");
                }
                money = value;
            }
        }
        public DateTime Term { get => depositTerm; }
        public double DepositPercentage { get => depositPercentage; }
        public Deposit(Bank bank, double amount, Client owner) : base(bank, owner)
        {
            this.depositPercentage = bank.GetDepositPercentage(amount);
            Money = amount;
            this.depositTerm = bank.DepositTerm;
        }

        public override int SelfWithdraw(double amount)
        {
            if (now < depositTerm) return -1;
            return base.SelfWithdraw(amount);
        }

        protected override void UpdateAccount()
        {
            bonusAmount += money * (depositPercentage / 365);
            if (now - lastTimeChanged >= new TimeSpan(30, 0, 0, 0))
            {
                money += bonusAmount;
                bonusAmount = 0;
                lastTimeChanged = now;
            }
        }
    }
}
