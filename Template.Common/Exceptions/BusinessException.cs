using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Common.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string friendlyMessage) : base(friendlyMessage)
        {
        }
    }
}
