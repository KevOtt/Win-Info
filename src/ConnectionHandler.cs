﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Win_Info
{
    // Setup WMI Connection to cimv2 namespace
    public class ConnectionHandler
    {
        private ManagementScope managementScope;
        public bool validConnection = true;
        public string ConnectionError;

        public void CreateConnection(string connectionTarget, Credential credential)
        {
            // Reset properties
            managementScope = null;
            ConnectionError = null;
            validConnection = true;

            // Setup scope options
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Impersonation = System.Management.ImpersonationLevel.Impersonate;
        
            // Set credentials if not using default
            if(credential.usedefault =!false)
                {
                    connectionOptions.Username = credential.userName;
                    connectionOptions.SecurePassword = credential.securePassword;
                }

            // Create management scope
            string connectionPath = ("\\\\" + connectionTarget + "\\root\\cimv2");
            ManagementScope scope = new ManagementScope(connectionPath, connectionOptions);

            // Try to connect scope

            // Should really catch and rethrow, need cleaner way of getting the status
            try
            {
                scope.Connect();
            }
            catch (System.UnauthorizedAccessException e)
            {
                validConnection = false;
                ConnectionError = ("Unable to Authenticate: " + e.Message);
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                validConnection = false;
                ConnectionError = ("Unable to Connect: " + e.Message);
            }
            catch (Exception e)
            {
                validConnection = false;
                ConnectionError = ("Unknown Error: " + e);
            }
            
            
            // If we didn't get any failures, set valid status and return the scope
            if(validConnection != false && scope.IsConnected == true)
            {
                ConnectionError = null;
                managementScope = scope;
            }
            else
            {
                ConnectionError = "Unknown Error";
                validConnection = false;
            }
        }

        // Return management object from query
        public ManagementObject[] RunQuery(string query)
        {

            // Begin query
            ObjectQuery objectQuery = new ObjectQuery(query);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(managementScope, objectQuery);
            ManagementObjectCollection queryCollection = null;

            try
            {
                queryCollection = searcher.Get();
            }
            catch (Exception e) 
            {
                throw e;
            }

            // Create a new array of management objects based on the number of items in the managementobject collection
            ManagementObject[] managementObjectArray = new ManagementObject[(queryCollection.Count)];

            // Copy each management object from the collection to the array
            queryCollection.CopyTo(managementObjectArray,0);

            // Return our array of management objects regardless of number of members and process on the calling side
            // This is how we're dealing queries that return static values vs numerous members like cim_logicaldisk
            return managementObjectArray;
        }
    }
}
