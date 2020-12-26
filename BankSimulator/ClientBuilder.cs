namespace BankSimulator
{
    public class ClientBuilder
    {
        private Client client;
        public ClientBuilder(string firstName, string lastName)
        {
            client = new Client(firstName, lastName);
        }
        public void BuildFirstName(string firstName)
        {
            client.FirstName = firstName;
        }
        public void BuildLastName(string lastName)
        {
            client.LastName = lastName;
        }
        public void BuildAddress(string address)
        {
            client.Address = address;
            if (!string.IsNullOrEmpty(client.PassportNumber))
            {
                client.IsSuspicious = false;
            }
        }
        public void BuildPassportNumber(string number)
        {
            client.PassportNumber = number;
            if (!string.IsNullOrEmpty(client.Address))
            {
                client.IsSuspicious = false;
            }
        }
        public Client Result()
        {
            return client;
        }
    }
}
