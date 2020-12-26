using System;

namespace DAL
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message)
        {

        }
    }
}
