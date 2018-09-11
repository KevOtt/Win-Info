using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace Win_Info
{
    public class Credential
    {
        public bool usedefault;
        public SecureString securePassword;
        public string userName;
        public string domain;

        // Parse the domain identifier from the domainUserName
        // return false if invalid, otherwise set username and domain
        // members and return true
        public bool ParseDomain(string domainUserName)
        {
            // if user name contains "\" used with sAMAccountName
            if (domainUserName.ToLower().Contains('\\'))
            {
                // if first charachter is "\" return invalid
                if (domainUserName[(0)] == '\\')
                {
                    return false;
                }
                // if domainuser name contains "\" and "@" return false
                if (domainUserName.ToLower().Contains('@'))
                {
                    return false;
                }
                // Split on "\" if more than two substrings return invalid
                // Else set domain and username
                string[] subStrings = domainUserName.Split('\\');
                if ((subStrings.Count()) > 2)
                {
                    return false;
                }
                else
                {
                    domain = subStrings[0];
                    userName = subStrings[1];
                    return true;
                }
            }
            // if user name contains "@" for UPN format
            if (domainUserName.ToLower().Contains('@'))
            {
                //if first character is "@" return invalid
                if (domainUserName[(0)] == '@')
                {
                    return false;
                }
                // if domainuser name contains "\" and "@" return false
                if (domainUserName.ToLower().Contains('\\'))
                {
                    return false;
                }
                // Split on "@" if more than two substrings return invalid
                // Else set domain and username
                string[] subStrings = domainUserName.Split('@');
                if ((subStrings.Count()) > 2 | subStrings[1] == "")
                {
                    return false;
                }
                else
                {
                    userName = subStrings[0];
                    domain = subStrings[1];
                    return true;
                }

            }
            if(domainUserName != "")
            {
                domain = "Localhost";
                userName = domainUserName;
                return true;
            }
            else
            {
                return false;
            }
        }

        // Parse the supplied string for characters that are invalid
        // within AD context, return false if invalid
        public bool ParseInvalidCharacters(string domainUserName)
        {
            if (domainUserName.IndexOfAny(",~:!#$%^&'(){}_ ".ToCharArray()) != -1)
            {
                return false;
            }
            return true;
        }
    }
}
