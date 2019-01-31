using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DukcapilWebServices.Utility
{
    public class SessionKey
    {
        public SessionKey()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static string CURRENT_USER_SESSION_KEY
        {
            get { return "SessionUserProfile"; }
        }

        public static string CURRENT_USER_ROLE_SESSION_KEY
        {
            get { return "SessionUserRole"; }
        }

        public static string CURRENT_USER_IP_ADDRESS_SESSION_KEY
        {
            get { return "SessionUserIPAddress"; }
        }

        public static string CURRENT_SEARCH_KEYWORD_SESSION_KEY
        {
            get { return "SessionSearchKeyword"; }
        }

        public static string CURRENT_PAGE_INDEX_SESSION_KEY
        {
            get { return "SessionPageIndex"; }
        }

        public static string CURRENT_PAGE_LIST_SESSION_KEY
        {
            get { return "SessionPageList"; }
        }
    }
}