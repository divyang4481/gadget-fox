#region Copyright
/*
* ====================================================================
* Copyright (c) 2007 www.dotsvn.net.  All rights reserved.
*
* This software is licensed as described in the file LICENSE, which
* you should have received as part of this distribution.  
* ====================================================================
*/
#endregion //Copyright

using System;
using System.Globalization;

namespace DotSVN.Common.Util
{
    public class SVNTimeUtil
    {
        public static readonly DateTime EmptyDateTime = new DateTime(0);
        public static String formatDate(ref DateTime date)
        {
            return formatDate(date, false);
        }

        public static String formatDate(DateTime date, bool formatZeroDate)
        {
            if (!formatZeroDate && date.Ticks == 0)
            {
                return null;
            }
            // In Standard DateTime Format Specifiers 'o' represents yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK
            return date.ToString("o");
        }

        public static DateTime parseDate(String dateString)
        {
            DateTime parsedDate;

            // Performance:
            // Parse in the format [2007-09-06T10:20:26.689093Z]
            string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ";
  
            bool parseResult = DateTime.TryParseExact(dateString, dateTimeFormat, new CultureInfo("en-US"), 
                                    DateTimeStyles.AdjustToUniversal, out parsedDate);
            if(!parseResult)
            {
                SVNErrorMessage err = SVNErrorMessage.create(SVNErrorCode.BAD_DATE);
                SVNErrorManager.error(err);    
            }
            return parsedDate;
        }
    }
}