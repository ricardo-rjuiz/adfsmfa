﻿//******************************************************************************************************************************************************************************************//
// Copyright (c) 2017 Neos-Sdi (http://www.neos-sdi.com)                                                                                                                                    //                        
//                                                                                                                                                                                          //
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),                                       //
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,   //
// and to permit persons to whom the Software is furnished to do so, subject to the following conditions:                                                                                   //
//                                                                                                                                                                                          //
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.                                                           //
//                                                                                                                                                                                          //
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,                                      //
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,                            //
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                               //
//                                                                                                                                                                                          //
// https://adfsmfa.codeplex.com                                                                                                                                                             //
// https://github.com/neos-sdi/adfsmfa                                                                                                                                                      //
//                                                                                                                                                                                          //
//******************************************************************************************************************************************************************************************//
using System;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Neos.IdentityServer.MultiFactor;
using Neos.IdentityServer.MultiFactor.Administration.Resources;
using System.Runtime.CompilerServices;


namespace Neos.IdentityServer.MultiFactor.Administration
{
    #region ManagementService
    /// <summary>
    /// ManagementService class 
    /// </summary>
    internal static class ManagementService
    {
        private static ADFSServiceManager _manager = null;

        private static DataFilterObject _filter = new DataFilterObject();
        private static DataPagingObject _paging = new DataPagingObject();
        private static DataOrderObject _order = new DataOrderObject();

        private static string EventLogSource = "ADFS MFA Administration";
        private static string EventLogGroup = "Application";

        /// <summary>
        /// ManagementService static constructor
        /// </summary>
        static ManagementService()
        {
            if (!EventLog.SourceExists(EventLogSource))
                EventLog.CreateEventSource(ManagementService.EventLogSource, ManagementService.EventLogGroup);
        }

        /// <summary>
        /// EnsureService() method implmentation
        /// </summary>
        internal static void EnsureService()
        {
          //  if (_manager == null)
                Initialize(null, true);
        }

        /// <summary>
        /// Filter Property
        /// </summary>
        internal static DataFilterObject Filter
        {
            get { return _filter; }
            set { _filter = value;  }
        }

        /// <summary>
        /// Paging Property
        /// </summary>
        internal static DataPagingObject Paging
        {
            get { return _paging; }
        }

        /// <summary>
        /// Order property
        /// </summary>
        internal static DataOrderObject Order
        {
            get { return _order; }
        }
        /// <summary>
        /// ADFSManager property
        /// </summary>
        internal static ADFSServiceManager ADFSManager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Config property
        /// </summary>
        internal static MFAConfig Config
        {
            get { return _manager.Config; }
        }

        /// <summary>
        /// Initialize method 
        /// </summary>
        internal static void Initialize(bool loadconfig = false)
        {
            Initialize(null, loadconfig);
        }

        /// <summary>
        /// Initialize method implementation
        /// </summary>
        internal static void Initialize(PSHost host = null, bool loadconfig = false)
        {
            if (_manager == null)
            {
                _manager = new ADFSServiceManager();
                _manager.Initialize();
            }
            if (loadconfig)
            {
                try
                {
                    _manager.EnsureLocalConfiguration(host);
                }
                catch (CmdletInvocationException cm)
                {
                    EventLog.WriteEntry(EventLogSource, errors_strings.ErrorMFAUnAuthorized + "\r\r" + cm.Message, EventLogEntryType.Error, 30901);
                    throw cm;
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry(EventLogSource, string.Format(errors_strings.ErrorLoadingMFAConfiguration, ex.Message), EventLogEntryType.Error, 30900);
                    throw ex;
                }
            }
        }
        /// <summary>
        /// CheckRepositoryAttribute method implementation
        /// </summary>
        internal static bool CheckRepositoryAttribute(string attributename, int choice = 0)
        {
            EnsureService();
            return RuntimeRepository.CheckRepositoryAttribute(Config, attributename, choice);
        }

        /// <summary>
        /// GetUserRegistration method implementation
        /// </summary>
        internal static Registration GetUserRegistration(string upn)
        {
            EnsureService();
            return RuntimeRepository.GetUserRegistration(Config, upn);
        }

        /// <summary>
        /// SetUserRegistration method implementation
        /// </summary>
        internal static Registration SetUserRegistration(Registration reg, bool resetkey = false, bool noemail = false)
        {
            EnsureService();
            return RuntimeRepository.SetUserRegistration(Config, reg, resetkey, noemail);
        }

        /// <summary>
        /// AddUserRegistration method implementation
        /// </summary>
        internal static Registration AddUserRegistration(Registration reg, bool addkey = true, bool noemail = false)
        {
            EnsureService();
            return RuntimeRepository.AddUserRegistration(Config, reg, addkey, noemail);
        }

        /// <summary>
        /// DeleteUserRegistration method implementation
        /// </summary>
        internal static bool DeleteUserRegistration(Registration reg, bool dropkey = true)
        {
            EnsureService();
            return RuntimeRepository.DeleteUserRegistration(Config, reg, dropkey);
        }

        /// <summary>
        /// EnableUserRegistration method implementation
        /// </summary>
        internal static Registration EnableUserRegistration(Registration reg)
        {
            EnsureService();
            return RuntimeRepository.EnableUserRegistration(Config, reg);
        }

        /// <summary>
        /// DisableUserRegistration method implementation
        /// </summary>
        internal static Registration DisableUserRegistration(Registration reg)
        {
            EnsureService();
            return RuntimeRepository.DisableUserRegistration(Config, reg);
        }

        /// <summary>
        /// GetUserRegistrations method implementation
        /// </summary>
        internal static RegistrationList GetUserRegistrations(DataFilterObject filter, DataOrderObject order, DataPagingObject paging)
        {
            EnsureService();
            return RuntimeRepository.GetUserRegistrations(Config, filter, order, paging);
        }

        /// <summary>
        /// GetAllUserRegistrations method implementation
        /// </summary>
        internal static RegistrationList GetAllUserRegistrations(DataOrderObject order, bool enabledonly = false)
        {
            EnsureService();
            return RuntimeRepository.GetAllUserRegistrations(Config, order, enabledonly);
        }

        /// <summary>
        /// GetUserRegistrationsCount method implementation
        /// </summary>
        internal static int GetUserRegistrationsCount(DataFilterObject filter)
        {
            EnsureService();
            return RuntimeRepository.GetUserRegistrationsCount(Config, filter);
        }

        /// <summary>
        /// GetEncodedUserKey method implmentation
        /// </summary>
        internal static string GetEncodedUserKey(string upn)
        {
            EnsureService();
            return RuntimeRepository.GetEncodedUserKey(Config, upn);
        }

        /// <summary>
        /// GetEncodedUserKey method implmentation
        /// </summary>
        internal static string NewUserKey(string upn)
        {
            EnsureService();
            return RuntimeRepository.NewUserKey(Config, upn);
        }

        /// <summary>
        /// ImportUsers method implementation
        /// </summary>
        internal static void ImportADDSUsers(string ldappath, bool enable)
        {
            EnsureService();
            RuntimeRepository.ImportADDSUsers(Config, ldappath, enable);
        }
    }

    public static class ADFSManagementRights
    {
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsSystem()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            return identity.IsSystem;
        }

        public static bool AllowedGroup(string group)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(group);
        }
    }
    #endregion
}

