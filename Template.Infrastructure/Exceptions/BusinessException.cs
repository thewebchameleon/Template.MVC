using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string friendlyMessage) : base(friendlyMessage)
        {
        }
    }
}
