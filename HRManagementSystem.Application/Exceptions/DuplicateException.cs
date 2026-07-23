using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException(string entityName)
            : base($"{entityName} already exists.")
        {
        }
    }
}
