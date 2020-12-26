
using System;

namespace BankSimulator
{
    public class DebitAccount : Account
    {
        private double debitPercentage;
        private double bonusAmount = 0.0;
        public double DebitPercentage { get => debitPercentage; }
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
                    throw new BankException($"Debit account can't have a negative Money value. Id: {id}");
                }
                money = value;
            }
        }
        public DebitAccount(Bank bank, Client owner) : base(bank, owner)
        {
            this.debitPercentage = bank.DebitPercentage;
        }
        
        public override int SelfWithdraw(double amount)
        {
            UpdateAccount();
            if (Money < amount) return -1;
            return base.SelfWithdraw(amount);
        }

        protected override void UpdateAccount()
        {
            bonusAmount += money * (debitPercentage / 365);
            if (now - lastTimeChanged >= new TimeSpan(30, 0, 0, 0))
            {
                money += bonusAmount;
                bonusAmount = 0;
                lastTimeChanged = now;
            }
        }
    }
    
}
