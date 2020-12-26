using System;

namespace BankSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //Clint builder
            ClientBuilder builder = new ClientBuilder("Lidiya", "Gorbokonenko");
            builder.BuildAddress("Spb");
            builder.BuildPassportNumber("N12345678");

            Client client1 = builder.Result();
            Console.WriteLine($"Is client sus: {client1.IsSuspicious}");

            builder = new ClientBuilder("Natalya", "Melnikova");
            builder.BuildAddress("Kz");
            builder.BuildPassportNumber("12345677898");

            Client client2 = builder.Result();
            Console.WriteLine($"Is client2 sus: {client2.IsSuspicious}");

            //Transactions check
            Bank bank = new Bank("sber", 0.03, 0.05, 20000.0, 1000.0, 20000.0, DateTime.Now + new TimeSpan(365, 0, 0, 0));
            var debitCard2 = client2.MakeAccount(AccountType.Debit, bank);
            var debitCard = client1.MakeAccount(AccountType.Debit, bank);
            Console.WriteLine($"Now on debit: {debitCard.Money}");
            client1.PutAccount(debitCard.Id, 30000.0);
            Console.WriteLine($"Now on debit (put 30000): {debitCard.Money}");
            debitCard.TimeTravel(new TimeSpan(365, 0, 0, 0));
            Console.WriteLine($"Now on debit (time travel 365 days): {debitCard.Money}");
            var creditCard = client1.MakeAccount(AccountType.Credit, bank);
            Console.WriteLine($"Now on credit: {creditCard.Money}");
            client1.PutAccount(creditCard.Id, 5000);
            Console.WriteLine($"Now on credit (put 5000): {creditCard.Money}");
            client1.Withdraw(27000);
            Console.WriteLine($"Client cards (withdraw 27000): {debitCard.Money} {creditCard.Money}");
            client1.TransferAccount(debitCard.Id, creditCard.Id, 1000.0);
            Console.WriteLine($"Client cards (transfer debit->credit 1000): {debitCard.Money} {creditCard.Money}");
            client1.Transfer(debitCard2.Id, 7000);
            Console.WriteLine($"Client1 (transfer 7000 to client2): {debitCard.Money}, {creditCard.Money}");
            Console.WriteLine($"Client2: {debitCard2.Money}");
            client1.CancelLastTransaction(creditCard.Id);
            Console.WriteLine("Last transaction canceled!");
            Console.WriteLine($"Client1: {debitCard.Money}, {creditCard.Money}");
            Console.WriteLine($"Client2: {debitCard2.Money}");
            client1.WithdrawAccount(creditCard.Id, 7000);
            Console.WriteLine($"Client1 (withdraw 7000 credit): {debitCard.Money}, {creditCard.Money}");
            creditCard.TimeTravel(new TimeSpan(60, 0, 0, 0));
            Console.WriteLine($"Client1 (credit time travel 2 month): {debitCard.Money}, {creditCard.Money}");
            var deposit = client2.MakeDeposit(bank, 50000);
            Console.WriteLine($"Client2 cards: {debitCard2.Money}, {deposit.Money}");
            //client2.WithdrawAccount(deposit.Id, 1000);
            deposit.TimeTravel(new TimeSpan(365, 0, 0, 0));
            Console.WriteLine($"Client2 cards (deposit time travel 1 year): {debitCard2.Money}, {deposit.Money}");

        }
    }
}
