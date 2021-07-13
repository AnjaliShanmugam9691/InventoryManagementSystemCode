using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace InventoryManagementSystem.Utility
{
    public class Utilities
    {
        public static ReportViewer RptViewer(List<ReportParameter> LstPram, String RptName, bool ShowParam = false, bool IsClientDPR = false)
        {
            try
            {
                string ReportFolder = string.Empty;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                ReportViewer Rpt = new ReportViewer();
                if (IsClientDPR)
                {
                    ReportFolder = ConfigurationManager.AppSettings["SSRSReportsClientDPRFolder"];
                }
                else
                {
                    ReportFolder = ConfigurationManager.AppSettings["SSRSReportsFolder"];
                }
                string UserName, PassWord, DomainName = String.Empty;
                UserName = ConfigurationManager.AppSettings["SsrsUserName"];
                PassWord = ConfigurationManager.AppSettings["SsrsPassword"];
                DomainName = ConfigurationManager.AppSettings["SsrsDomain"];
                Rpt.ProcessingMode = ProcessingMode.Remote;
                Rpt.ShowParameterPrompts = ShowParam;
                Rpt.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportServerUrl"]);
                Rpt.ServerReport.ReportPath = "/" + ReportFolder + "/" + RptName;
                Rpt.SizeToReportContent = true;

                IReportServerCredentials irsc = new RptServerCredentials(UserName, PassWord, DomainName);
                
                Rpt.ServerReport.ReportServerCredentials = irsc;
                if (LstPram.Count > 0)
                {
                    Rpt.ServerReport.SetParameters(LstPram);
                }
                Rpt.ServerReport.Refresh();
                return Rpt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class RptServerCredentials : IReportServerCredentials
    {
        private string _UserName;
        private string _PassWord;
        private string _DomainName;

        public RptServerCredentials(string UserName, string PassWord, string DomainName)
        {
            _UserName = UserName;
            _PassWord = PassWord;
            _DomainName = DomainName;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public ICredentials NetworkCredentials
        {
            get { return new NetworkCredential(_UserName, _PassWord, _DomainName); }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user,
         out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}