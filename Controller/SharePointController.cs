using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;



namespace KE.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/SharePoint")]
    public class SharePointController : Controller
    {
        [HttpGet]
        [Route("GetResponseCode")]
        public IActionResult GetResponseCode()
        {
            //KE.SP.Repository rep = new SP.Repository();


            return Ok("GetResponseCode");
        }


        [HttpGet]
        [Route("GetBankDetails")]
        public IActionResult GetBankDetails()
        {
            ClientContext context = new ClientContext("http://win-3ipu65j836j/");

            //context.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
            //context.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo("adilahmed.khan", "zxcv@1234");

            //context.ExecutingWebRequest += context_ExecutingWebRequest;
            //context.AuthenticationMode = ClientAuthenticationMode.Default;
            //NetworkCredential credential = new NetworkCredential("sharepoint.admin", "zxcv@1234", "systems");
            //context.Credentials = credential;
            //context.RequestTimeout = Timeout.Infinite;


            Web web = context.Web;
            context.Load(web);
            context.ExecuteQuery();

            return Ok("KE Title: " + web.Title);
        }

        private static void context_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {

            try
            {
                e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting authentication header: " + ex.Message);
            }
        }


        [HttpGet]
        [Route("GetADCDetails")]
        public IActionResult GetADCDetails()
        {
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create("http://win-3ipu65j836j/" + " / _api/web/lists");
            endpointRequest.Method = "GET";
            endpointRequest.Accept = "application/json;odata=verbose";
            endpointRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();





            return Ok("Get Bank Details2");
        }
    }
}