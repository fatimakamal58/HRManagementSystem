using System;
using System.Collections.Generic;
using System.Text;

namespace HRManagementSystem.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName)
        : base($"{entityName} was not found.")
        {
        }
    }
}
