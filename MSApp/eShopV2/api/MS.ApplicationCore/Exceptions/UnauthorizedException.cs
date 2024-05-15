﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MS.ApplicationCore.Exceptions
{
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException() : base() { }

        public UnauthorizedException(string message) : base(message) { }

        public UnauthorizedException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
