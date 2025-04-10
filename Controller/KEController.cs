using System;
using System.Linq;
using KE.Service.BAL;
using KE.Service.Helper;
using KE.Service.Models;
using KE.Service.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace KE.Service.Controllers
{
    //[BasicAuthorize]
    [Produces("application/json")]
    [Route("api/KE")]
    //[EnableCors("CorsPolicy")]
    public class KEController : Controller
    {
        KEBAL keBAL = null;
        BAL.BAL bAL = null;
        CustomEncryption customEncryption = new CustomEncryption();
        IConfiguration _iconfiguration;
        public bool isSapRequest;
        
        
        public double absoluteTime;
        public double slidingTime;
        IDistributedCache _distributedCache;
        DistributedCacheEntryOptions cacheExpiry;
        /// <summary>
        /// Initialize field values
        /// </summary>
        /// <param name="iconfiguration"></param>
        public KEController(IConfiguration iconfiguration ,IDistributedCache distributedCache)
        {
            
            keBAL = new KEBAL(iconfiguration);
            bAL = new BAL.BAL(iconfiguration);
            _iconfiguration= iconfiguration;
            isSapRequest =  _iconfiguration.GetSection("SapService").GetSection("IsSapService").Value == "False" ? false : true ;
            absoluteTime = Convert.ToDouble(_iconfiguration.GetSection("CacheExpiry").GetSection("AbsoluteTime").Value);
            slidingTime = Convert.ToDouble(_iconfiguration.GetSection("CacheExpiry").GetSection("SlidingTime").Value);
            _distributedCache= distributedCache;

            cacheExpiry = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(absoluteTime),
                SlidingExpiration = TimeSpan.FromHours(slidingTime)
            };
        }


       

        /// <summary>
        /// Test service for response checking
        /// </summary>
        /// <returns></returns>
        // GET: api/KE
        [HttpGet]
        public string Get()
        {
            return "Service is running!";
        }

        /// <summary>
        /// Return account detail from SAP by account number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns>Json response</returns>
        /// // GET: api/GetAccountDetail
        [HttpGet]
        [Route("GetAccountDetail")]
        [ResponseCache(VaryByQueryKeys = new string[] { "AccountNumber" }, Duration = 60)]
        public GenericResponse GetAccountDetail([FromQuery]string AccountNumber)
        {
            #region For Getting UserId From Session
            //HttpContext a = HttpContext;
            //object temp = null;
            //a.Items.TryGetValue("userId", out temp);
            //   string accountNumber = customEncryption.DecryptStringAES(dataString.d); 
            #endregion

            string userid = string.Empty;
  
            GenericResponse response = new GenericResponse();
            string stringfyObject = "";
            JsonResponseConverter accountDetails = new JsonResponseConverter();
            try
            {
                if (!isSapRequest)
                {


                    var values = "{\"d\":{\"__metadata\":{\"id\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400030093001')\",\"uri\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400030093001')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.ZCP_GETACCOUNTDETAILS\"},\"contractNo\":\"\",\"Message\":\"Success\",\"Status\":\"0\",\"businessPartner\":\"0103010109\",\"contractCount\":\"00001\",\"contractAcc\":\"400030093001\",\"consumerCategory\":\"Defaulter\",\"smsSubscription\":\"TRUE\",\"mobileNumber\":\"3002807160\",\"Details\":{\"results\":[{\"__metadata\":{\"id\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/DetailsSet('400030093001')\",\"uri\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/DetailsSet('400030093001')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.Details\"},\"feederId\":\"FDR-003561\",\"dtsId\":\"DTS-510856\",\"contractNo\":\"0032911812\",\"feederName\":\"FEED MILL RMU\",\"dtsName\":\"MADINA MASJID 02\",\"tarrifType\":\"A1-R\",\"contractAcc\":\"400030093001\",\"ibcCode\":\"148\",\"MeterNo\":\"SCH22244\",\"SanctionLoad\":\"          1.0000000\",\"ConsumerNo\":\"N/A\"}]},\"DetailsNavSet\":{\"__deferred\":{\"uri\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400030093001')/DetailsNavSet\"}}}}";
                    //var value = "{\"d\":{\"__metadata\":{\"id\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400000202094')\",\"uri\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400000202094')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.ZCP_GETACCOUNTDETAILS\"},\"contractNo\":\"\",\"Message\":\"Success\",\"Status\":\"0\",\"businessPartner\":\"0100093645\",\"contractCount\":\"00001\",\"contractAcc\":\"400000202094\",\"consumerCategory\":\"Star\",\"smsSubscription\":\"TRUE\",\"mobileNumber\":\"3332149693\",\"Details\":{\"results\":[{\"__metadata\":{\"id\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/DetailsSet('400000202094')\",\"uri\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/DetailsSet('400000202094')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.Details\"},\"feederId\":\"FDR-000674\",\"dtsId\":\"DTS-534074\",\"contractNo\":\"0030003135\",\"feederName\":\"AL-BURHAN\",\"dtsName\":\"P-7/2 PMT, SAMAR PARK BLOCK-D\",\"tarrifType\":\"A1-R\",\"contractAcc\":\"400000202094\",\"ibcCode\":\"124\",\"MeterNo\":\"SAI29729\",\"SanctionLoad\":\"          4.0000000\",\"ConsumerNo\":\"N/A\"}]},\"DetailsNavSet\":{\"__deferred\":{\"uri\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400000202094')/DetailsNavSet\"}}}}";
                    JObject parsed = JObject.Parse(values);
                    JsonResponseConverter accountDetailss = JsonConvert.DeserializeObject<JsonResponseConverter>(parsed.ToString());
                    stringfyObject = JsonConvert.SerializeObject(accountDetailss.D);
                    response.d = customEncryption.EncryptStringToStringAES(stringfyObject);
                    return response;

                }
                
                string tempResponse = string.Empty;

               
                var cache  = _distributedCache.GetString("GetAccountDetail-"+AccountNumber);

                if (cache == null)
                {

                    tempResponse = keBAL.GetAccountDetail(AccountNumber, "JSON");
                    _distributedCache.SetString("GetAccountDetail-" + AccountNumber, tempResponse, cacheExpiry);
                }
                else
                {
                    tempResponse = cache;
                }

                    
                if (tempResponse.Contains("Exception"))
                {
                    response.d = tempResponse;
                    return response;
                }


                if (string.IsNullOrEmpty(tempResponse))
                {
                    // old code changed
                    // containerGeneric.data.Status = 1;
                    // containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    // return Ok(containerGeneric);
                    string mockdata = "{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400030093001')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.ZCP_GETACCOUNTDETAILS\"},\"contractNo\":\"\",\"Message\":\"Success\",\"Status\":0,\"businessPartner\":\"0103010109\",\"contractCount\":\"00001\",\"contractAcc\":\"400030093001\",\"consumerCategory\":\"Regular\",\"smsSubscription\":\"TRUE\",\"mobileNumber\":\"3002807160\",\"Details\":{\"results\":[{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/DetailsSet('400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/DetailsSet('400030093001')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.Details\"},\"feederId\":\"FDR-003561\",\"dtsId\":\"DTS-510856\",\"contractNo\":\"0032911812\",\"feederName\":\"FEED MILL RMU\",\"dtsName\":\"MADINA MASJID 02\",\"tarrifType\":\"A1-R\",\"contractAcc\":\"400030093001\",\"ibcCode\":148,\"MeterNo\":\"SFF07187\",\"SanctionLoad\":\"          1.0000000\",\"ConsumerNo\":\"N/A\"}]},\"DetailsNavSet\":{\"__deferred\":{\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet('400030093001')/DetailsNavSet\"}}}";

                    
                    JObject parsed = JObject.Parse(tempResponse);
                    accountDetails = JsonConvert.DeserializeObject<JsonResponseConverter>(parsed.ToString());
                    stringfyObject = JsonConvert.SerializeObject(accountDetails.D);
                    response.d = customEncryption.EncryptStringToStringAES(stringfyObject);
                    return response;
                    
                }
                else
                {
                    try
                    {
                        
                        JObject parsed = JObject.Parse(tempResponse);
                        accountDetails = JsonConvert.DeserializeObject<JsonResponseConverter>(parsed.ToString());
                        
                    }
                    catch (Exception ex)
                    {
                        stringfyObject = JsonConvert.SerializeObject("Parsing Issue");

                        ExceptionHandling.LogError(ex, "getaccountdetail > Parsing Issue");
                        //containerGeneric.data.Status = 1;
                        //containerGeneric.data.Message = Constants.ServiceResponseParsingError;
                        // return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                // old code change business logic
                //containerGeneric.data.Status = 1;
                //containerGeneric.data.Message = Constants.Exception_GeneralMessage;
                // return Ok(containerGeneric);
                stringfyObject = JsonConvert.SerializeObject("Failed");
                ExceptionHandling.LogError(ex, "getaccountdetail");
            }

            stringfyObject = JsonConvert.SerializeObject(accountDetails.D);
            response.d= customEncryption.EncryptStringToStringAES(stringfyObject);
            return response;
        }


        //[HttpGet]
        //[Route("GetAccountDetailAsync")]
        //[ResponseCache(VaryByQueryKeys = new string[] { "AccountNumber" }, Duration = 60)]
        //public async Task<string> GetAccountDetailAsync([FromQuery]string AccountNumber)
        //{
        //    //using (WebClient webClient = new WebClient())
        //    //{
        //    //    return await (webClient.DownloadStringTaskAsync(string.Format("https://fioridev.ke.com.pk:44200/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet(contractAcc=%27{0}%27)/?$expand=Details&$format=json", AccountNumber)));
        //    //}

        //    //using (HttpClient client = new HttpClient())
        //    //{
        //    //    return Ok(await client.GetStringAsync(string.Format("https://fioridev.ke.com.pk:44200/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/ZCP_GETACCOUNTDETAILSSet(contractAcc=%27{0}%27)/?$expand=Details&$format=json", AccountNumber)));
        //    //}
        //}

        /// <summary>
        /// Return feeder information by dtsId
        /// </summary>
        /// <param name="dtsId"></param>
        /// <returns>Json response</returns>
        [HttpGet]
        [Route("GetLoadsheddingStatus")]
        [ResponseCache(VaryByQueryKeys = new string[] { "AccountNumber" }, Duration = 1)]
        public GenericResponse GetLoadsheddingStatus([FromQuery]string dtsId)
        {
            GenericResponse genericResponse = new GenericResponse();
            LoadshedStatusContainer response = new LoadshedStatusContainer();
            FeederDetails fd = new FeederDetails();

            try
            {

                fd = getFeederDetails(dtsId);

                if (fd.message.Contains("Exception"))
                {
                    genericResponse.d = fd.message;
                    return genericResponse;
                }

                if (fd.status != null && fd.status != "1" && fd.dtsID != null)
                {
                    response.feederID = fd.feederID;
                    response.feederStatus = fd.feederStatus.ToUpper();
                    response.remarks = fd.remarks;
                    response.dtsID = fd.dtsID;
                    response.dtsName = fd.dtsName;
                    response.brkID = fd.brkID;
                    response.region = fd.region;
                    response.grid = fd.grid;
                    response.ibc = fd.ibc;
                    response.lossCategory = fd.lossCategory;
                    response.faultSource = fd.faultSource;
                    response.outageType = fd.outageType;
                    response.feederName = fd.feederName;
                    response.theftRatio = fd.theftRatio;
                    response.bfDate = fd.bfDate;
                    response.bfTime = fd.bfTime;
                    response.areaStatus = fd.areaStatus.ToUpper();
                    response.faultReportDate = fd.faultReportDate;
                    response.consCount = fd.consCount;
                    response.faultReportTime = fd.faultReportTime;
                    response.Shift1_from = fd.Shift1_from;
                    response.Shift1_to = fd.Shift1_to;
                    response.Shift2_from = fd.Shift2_from;
                    response.Shift2_to = fd.Shift2_to;
                    response.Shift3_from = fd.Shift3_from;
                    response.Shift3_to = fd.Shift3_to;
                    response.Shift4_from = fd.Shift4_from;
                    response.Shift4_to = fd.Shift4_to;
                    response.Shift5_from = fd.Shift5_from;
                    response.Shift5_to = fd.Shift5_to;
                    response.Shift6_from = fd.Shift6_from;
                    response.Shift6_to = fd.Shift6_to;
                    response.RemarksLL = fd.RemarksLL;
                    response.Status = 0;
                    response.Message = "Success";
                }
                else
                {
                    response.Status = 1;
                    response.Message = "Failed";
                }

                string stringfyObject = JsonConvert.SerializeObject(response);
                genericResponse.d = customEncryption.EncryptStringToStringAES(stringfyObject);
                return genericResponse;
            }
            catch(Exception ex) 
            {
                genericResponse.d = "6WJOTxTNlPEmMWvSe6SLTPiQvAsjN73bB44DxjPrQjsbpNgbmJhCALa9biOR5ONhOh2Z6EcSDMisz1sSi6cL9S8omvbA2m7CfjUuBqkZkOm7qd0PMce+x3f+OSnIhVg/Rgctu48r5qoD1D6K8PuAMaCSUq/GtJMuEkFjnaHGgTuDiXm/XKA3i+5BEJZbErEDtegJ6IrNrLW8SFkJbCqBs6+j/5944xfJoQpO5SXJ879pOXN5TGoMopQ7NrBuQgw9iX5NPyretnweA5AcqLbyH8nCLBXlkllLcHDniy7TaWrDPQOuSJAPNgKMZr5/a3n1prAozfV6hyR8KVaQUjqLtlto0UsjIzWaAa/G23SLJYF6Rdh46fgyE/6lH2wSk8l9jrCiEaV1c1PsTGzOFB7R6fB0etNHEKNHvl2Om/ub5Rtz4Gc/CybkBoSb67wF5ah8w6gO0sZuOXpzBamjFatWg1DJOcqWuHK6Ty/B3qccDUCbHtiFjxXtkdRw55szptMOxIKKGMz1TUnI52VGqGAs/xrkcMC5aPtwXHgKDryiBC5Ge/G1ds0qGXs5s5NuiqZPESKU/AeBIsgXzYa4GKJcbKThPMLFQibzEyiBUz8JnzseNbHz1zQOyBvSJj3qQJV4sy7NvMYOuaRVmqHVS65RmltLjAD64wyN3d8VkskJ5SV3u95yHqfHToTeaxuIhbZKGKvO9+51Idemhmfj3UqGhaNiVSXX6YJLjKVbhfTw+76S7O+fWrAnJNEacSVq5W4fcNWaA6NhWEqClfx75tVPAzb/Vg/UIDL624gUgqQFQw/PDIEls1Pd7+AGgt0gAW2p5PQALP66K8zwRiqm8a/Fg6fFDUpg/GO7xLscliNPcSrbJlxhmE+WlZijggTcrgSQ";
                return genericResponse;
            }

        }



        public FeederDetails getFeederDetails(string dtsID)
        {
            try
            {
                FeederDetails details = new FeederDetails();
                JsonFeederConverter feederDetail = new JsonFeederConverter();

                var result = keBAL.GetFeederData(dtsID);

                if (result.Contains("Exception"))
                {
                    string value = "{\"d\":{\"Message\":\"Success\",\"Status\":\"0\",\"dtsID\":\"534074\",\"dtsName\":\"P-7/2 PMT, SAMAR PARK BLOCK-D\",\"brkID\":\"\",\"region\":\"\",\"grid\":\"\",\"ibc\":\"\",\"lossCategory\":\"LL\",\"faultSource\":\"\",\"outageType\":null,\"remarks\":\"Your area is exempted from load-shedding\",\"feederID\":\"674\",\"feederName\":\"AL-BURHAN\",\"theftRatio\":\"\",\"bfDate\":\"\",\"bfTime\":\"\",\"feederStatus\":\"ON\",\"areaStatus\":\"ON\",\"faultReportDate\":\"\",\"faultReportTime\":\"\",\"consCount\":\"\",\"groupID\":\"LL4\",\"Shift1_from\":\"(null)\",\"Shift1_to\":\"(null)\",\"Shift2_from\":\"(null)\",\"Shift2_to\":\"(null)\",\"Shift3_from\":\"(null)\",\"Shift3_to\":\"(null)\",\"Shift4_from\":\"(null)\",\"Shift4_to\":\"(null)\",\"Shift5_from\":\"(null)\",\"Shift5_to\":\"(null)\",\"RemarksLL\":\"Your area is exempted from load-shedding\",\"TICKET_NO\":\"\"}}";
                    JObject parsed = JObject.Parse(value);
                    feederDetail = JsonConvert.DeserializeObject<JsonFeederConverter>(parsed.ToString());
                }


                if (result != null)
                {



                    try
                    {
                        JObject parsed = JObject.Parse(result);
                        feederDetail = JsonConvert.DeserializeObject<JsonFeederConverter>(result.ToString());
                    }
                    catch (Exception ex)
                    {
                        string value = "{\"d\":{\"Message\":\"Success\",\"Status\":\"0\",\"dtsID\":\"534074\",\"dtsName\":\"P-7/2 PMT, SAMAR PARK BLOCK-D\",\"brkID\":\"\",\"region\":\"\",\"grid\":\"\",\"ibc\":\"\",\"lossCategory\":\"LL\",\"faultSource\":\"\",\"outageType\":null,\"remarks\":\"Your area is exempted from load-shedding\",\"feederID\":\"674\",\"feederName\":\"AL-BURHAN\",\"theftRatio\":\"\",\"bfDate\":\"\",\"bfTime\":\"\",\"feederStatus\":\"ON\",\"areaStatus\":\"ON\",\"faultReportDate\":\"\",\"faultReportTime\":\"\",\"consCount\":\"\",\"groupID\":\"LL4\",\"Shift1_from\":\"(null)\",\"Shift1_to\":\"(null)\",\"Shift2_from\":\"(null)\",\"Shift2_to\":\"(null)\",\"Shift3_from\":\"(null)\",\"Shift3_to\":\"(null)\",\"Shift4_from\":\"(null)\",\"Shift4_to\":\"(null)\",\"Shift5_from\":\"(null)\",\"Shift5_to\":\"(null)\",\"RemarksLL\":\"Your area is exempted from load-shedding\",\"TICKET_NO\":\"\"}}";
                        JObject parsed = JObject.Parse(value);
                        feederDetail = JsonConvert.DeserializeObject<JsonFeederConverter>(parsed.ToString());

                    }


                    if (!string.IsNullOrEmpty(feederDetail.D.feederStatus))
                    {
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift1_from))
                        {
                            feederDetail.D.Shift1_from = feederDetail.D.Shift1_from.Length == 4 ? getTime(feederDetail.D.Shift1_from) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift1_to))
                        {
                            feederDetail.D.Shift1_to = feederDetail.D.Shift1_to.Length == 4 ? getTime(feederDetail.D.Shift1_to) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift2_from))
                        {
                            feederDetail.D.Shift2_from = feederDetail.D.Shift2_from.Length == 4 ? getTime(feederDetail.D.Shift2_from) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift2_to))
                        {
                            feederDetail.D.Shift2_to = feederDetail.D.Shift2_to.Length == 4 ? getTime(feederDetail.D.Shift2_to) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift3_from))
                        {
                            feederDetail.D.Shift3_from = feederDetail.D.Shift3_from.Length == 4 ? getTime(feederDetail.D.Shift3_from) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift3_to))
                        {
                            feederDetail.D.Shift3_to = feederDetail.D.Shift3_to.Length == 4 ? getTime(feederDetail.D.Shift3_to) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift4_from))
                        {
                            feederDetail.D.Shift4_from = feederDetail.D.Shift4_from.Length == 4 ? getTime(feederDetail.D.Shift4_from) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift4_to))
                        {
                            feederDetail.D.Shift4_to = feederDetail.D.Shift4_to.Length == 4 ? getTime(feederDetail.D.Shift4_to) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift5_from))
                        {
                            feederDetail.D.Shift5_from = feederDetail.D.Shift5_from.Length == 4 ? getTime(feederDetail.D.Shift5_from) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift5_to))
                        {
                            feederDetail.D.Shift5_to = feederDetail.D.Shift5_to.Length == 4 ? getTime(feederDetail.D.Shift5_to) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift6_from))
                        {
                            feederDetail.D.Shift6_from = feederDetail.D.Shift6_from.Length == 4 ? getTime(feederDetail.D.Shift6_from) : "";
                        }
                        if (!string.IsNullOrEmpty(feederDetail.D.Shift6_to))
                        {
                            feederDetail.D.Shift6_to = feederDetail.D.Shift6_to.Length == 4 ? getTime(feederDetail.D.Shift6_to) : "";
                        }
                        details = feederDetail.D;
                    }
                    else
                    {
                        details.message = feederDetail.D.message;
                        details.status = feederDetail.D.status;
                    }
                }
                
                return details;

            }
            catch (Exception ex)
            {
                FeederDetails details = new FeederDetails();
                details.RemarksLL = "Your area is exempted from load-shedding";
                details.Shift1_from = "";
                details.Shift1_to = "";
                details.Shift2_from = "";
                details.Shift2_to = "";
                details.Shift3_from = "";
                details.Shift3_to = "";
                details.Shift4_from = "";
                details.Shift4_to = "";
                details.Shift5_from = "";
                details.Shift5_to = "";
                details.Shift6_from = "";
                details.Shift6_to = "";
                details.areaStatus = "ON";
                details.bfDate = "";
                details.bfTime = "";
                details.consCount = "";
                details.dtsID = "534074";
                details.dtsName = "P-7/2 PMT, SAMAR PARK BLOCK-D";
                details.faultReportDate = "";
                details.faultReportTime = "";
                details.faultSource = "";
                details.feederID = "674";
                details.feederName = "AL-BURHAN";
                details.feederStatus = "ON";
                details.grid = "";
                details.ibc = "";
                details.lossCategory = "LL";
                details.message = "Success";
                details.outageType = null;
                details.region = "";
                details.remarks = "Your area is exempted from load-shedding";
                details.status = "0";
                details.theftRatio= "";

                return details;
            }
        }

        protected string getTime(string time)
        {
            string longTime = string.Empty;
            try
            {
                longTime = time.Insert(2, ":");
                longTime = Convert.ToDateTime(longTime).ToLongTimeString();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return longTime;
        }

        /// <summary>
        /// Returns array of bills by account number FROM KE
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("GetDuplicateBills")]
        [ResponseCache(VaryByQueryKeys = new string[] { "accountNo", "month" }, Duration = 1)]
        public IActionResult GetDuplicateBills([FromQuery]string accountNo, string month)
        {
            
            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                string tempResponse = string.Empty;
                tempResponse = keBAL.GetDuplicateBills(accountNo, month);
                //var cache = _distributedCache.GetString("GetDuplicateBills-"+accountNo+"-"+month);
                
                //if (cache == null)
                //{
                //    tempResponse = keBAL.GetDuplicateBills(accountNo, month);
                //    _distributedCache.SetString("GetDuplicateBills-" + accountNo + "-" + month, tempResponse,cacheExpiry);
                //}
                //else
                //{
                //    tempResponse = cache;
                //}

                 

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    //  return Ok(containerGeneric);
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "GetDuplicateBills > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetDuplicateBills");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }


        /// <summary>
        /// Return bill in base64 format from KE
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="monthDashYear"></param>
        /// <returns>base64</returns>
        [HttpGet]
        [Route("GetBillPdf")]
        [ResponseCache(VaryByQueryKeys = new string[] { "accountNo", "monthDashYear" }, Duration = 1)]
        public IActionResult GetBillPdf([FromQuery]string accountNo, string monthDashYear)
        {
            //string dta = customEncryption.DecryptStringAES(dataString.d);

            //string accountNo = JsonConvert.DeserializeObject<string>(dta);
            //string monthDashYear = JsonConvert.DeserializeObject<string>(dta);


            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {

                string tempResponse = string.Empty;

                var cache = _distributedCache.GetString("GetBillPdf-"+accountNo+"-"+monthDashYear);

                if (cache == null)
                {
                    tempResponse = keBAL.GetBillPdf(accountNo, monthDashYear);
                    _distributedCache.SetString("GetBillPdf-" + accountNo + "-" + monthDashYear, tempResponse, cacheExpiry);
                }
                else
                {
                    tempResponse = cache;
                }

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    //return Ok(containerGeneric);
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "GetBillPdf > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetBillPdf");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Return array of billing dataset 
        /// </summary>
        /// <param name="contract"></param>
        /// <returns>Json response</returns>
        [HttpGet]
        [Route("GetBillingDataSet")]
        //[ResponseCache(VaryByQueryKeys = new string[] { "contract" }, Duration = 60)]
        public IActionResult GetBillingDataSet([FromQuery]string contract)
        {
            // string dta = customEncryption.DecryptStringAES(dataString.d);
            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                string tempResponse = string.Empty;

                var cache = _distributedCache.GetString("GetBillingDataSet-" + contract);
                if (cache == null)
                {
                    tempResponse = keBAL.BillingDataSet(contract);
                    _distributedCache.SetString("GetBillingDataSet-" + contract, tempResponse, cacheExpiry);
                }
                else
                {
                    tempResponse = cache;
                }
                


                //string CacheKE = "BillingDetails-" + contract;
                //if (CacheHelper.GetCache(CacheKE) == null)
                //{
                //    Logging.Log("--IF Case-- cache created " + CacheKE);
                //    tempResponse = keBAL.BillingDataSet(contract);
                //    CacheHelper.SetCache(CacheKE, tempResponse);
                //}
                //else
                //{
                //    Logging.Log("--Else Case-- cache exist " + CacheKE);
                //    tempResponse = (string)CacheHelper.GetCache(CacheKE);
                //}

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    //  return Ok(containerGeneric);
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "BillingDataSet > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "BillingDataSet");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }


        /// <summary>
        /// Return complaint array by contract no and contract account
        /// </summary>
        /// <param name="contractNo"></param>
        /// <param name="contractAcc"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMyComplaints")]
        [ResponseCache(VaryByQueryKeys = new string[] { "contractNo", "contractAcc" }, Duration = 30)]
        public IActionResult GetMyComplaints([FromQuery]string contractNo, string contractAcc)
        {
            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();
            //  string dta = customEncryption.DecryptStringAES(dataString.d);
            //  string contractNo = JsonConvert.DeserializeObject<string>(dta);
            //  string contractAcc = JsonConvert.DeserializeObject<string>(dta);
            try
            {
                if (!isSapRequest)
                {
                   
                   string mock = "{\"d\":{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/GetMyComplaints(contractNo='0032911812',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/GetMyComplaints(contractNo='0032911812',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.GetMyComplaint\"},\"Message\":\"Success\",\"Status\":\"0\",\"contractNo\":\"0032911812\",\"contractAcc\":\"400030093001\",\"complaints\":{\"results\":[{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='6017510001',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='6017510001',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Voltage\",\"date\":\"20230505\",\"contractNo\":\"0032911812\",\"type\":\"Technical\",\"number\":\"6017510001\",\"status\":\"Closed\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016488716',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016488716',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Meter Faults\",\"date\":\"20230412\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8016488716\",\"status\":\"Completed\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016488776',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016488776',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Dues Clearance Certificate\",\"date\":\"20230412\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8016488776\",\"status\":\"Completed\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016482067',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016482067',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Wrong Reading\",\"date\":\"20230411\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8016482067\",\"status\":\"Completed\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015935336',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015935336',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230113\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8015935336\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015935353',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015935353',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230113\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8015935353\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015912770',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015912770',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230110\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8015912770\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015903860',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015903860',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230109\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8015903860\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400030093001\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015903861',contractAcc='400030093001')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015903861',contractAcc='400030093001')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230109\",\"contractNo\":\"0032911812\",\"type\":\"Billing\",\"number\":\"8015903861\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400030093001\"}]}}}";
                   //string mockResponse = "{\"d\":{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/GetMyComplaints(contractNo='0033604814',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/GetMyComplaints(contractNo='0033604814',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.GetMyComplaint\"},\"Message\":\"Success\",\"Status\":\"0\",\"contractNo\":\"0033604814\",\"contractAcc\":\"400038105750\",\"complaints\":{\"results\":[{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='6017464712',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='6017464712',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Supply Off\",\"date\":\"20230420\",\"contractNo\":\"0033604814\",\"type\":\"Technical\",\"number\":\"6017464712\",\"status\":\"Closed\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016547738',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016547738',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230420\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8016547738\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016507644',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016507644',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230414\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8016507644\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016456761',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016456761',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230406\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8016456761\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016458000',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016458000',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230406\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8016458000\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016449706',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8016449706',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Duplicate Bill\",\"date\":\"20230405\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8016449706\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015884223',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015884223',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Token Bill\",\"date\":\"20230104\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8015884223\",\"status\":\"Closed On Interaction\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015885980',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015885980',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Bill has been lost / not avail\",\"date\":\"20230104\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8015885980\",\"status\":\"Open\",\"contractAcc\":\"400038105750\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015494886',contractAcc='400038105750')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCRM_CP_WS_SRV/complaintsSet(number='8015494886',contractAcc='400038105750')\",\"type\":\"ZCRM_CP_WS_SRV.complaints\"},\"title\":\"Refund/Allowance other than IR\",\"date\":\"20221110\",\"contractNo\":\"0033604814\",\"type\":\"Billing\",\"number\":\"8015494886\",\"status\":\"Completed\",\"contractAcc\":\"400038105750\"}]}}}";
                   JObject parsed= JObject.Parse(mock); 
                   JsonComplainConverter accountDetails = JsonConvert.DeserializeObject<JsonComplainConverter>(parsed.ToString());
                   string jsons = JsonConvert.SerializeObject(accountDetails);
                   return Ok( customEncryption.EncryptStringToStringAES(jsons));

                  
                   

                }


                string tempResponse = keBAL.GetMyComplaints(contractNo, contractAcc);

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    //return Ok(containerGeneric);
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "GetMyComplaints > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetMyComplaints");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Post complaint data on SAP
        /// </summary>
        /// <param name="technicalComplaint"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostTechnicalComplaint")]
        public IActionResult PostTechnicalComplaint([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            TechnicalComplaint technicalComplaint = new TechnicalComplaint();
            technicalComplaint = JsonConvert.DeserializeObject<TechnicalComplaint>(decPayload);
            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);


                string tempResponse = keBAL.PostTechnicalComplaint(technicalComplaint,bAL);

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    //    return Ok(containerGeneric);
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "PostTechnicalComplaint > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostTechnicalComplaint");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Post billing complaint data on SAP
        /// </summary>
        /// <param name="billingComplaint"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostBillingComplaint")]
        public IActionResult PostBillingComplaint([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            BillingComplaint billingComplaint = new BillingComplaint();
            billingComplaint = JsonConvert.DeserializeObject<BillingComplaint>(decPayload);

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                HttpContext a = HttpContext;
                object objUserId = null;
                a.Items.TryGetValue("userId", out objUserId);

                string tempResponse = keBAL.PostBillingComplaint(billingComplaint, Convert.ToString(objUserId), dataString.base64Image, bAL);

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    //   return Ok(containerGeneric);
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "PostBillingComplaint > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostBillingComplaint");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This will provide stats of the consumer’s feeder to them, the following info will be displayed:
        /// </summary>
        /// <param name="fdrId"></param>
        /// <param name="dtsId"></param>
        /// <returns></returns>
        /// Nw not using this service 
        [HttpGet]
        [Route("GetActualPlannedGraphData")]
        [ResponseCache(VaryByQueryKeys = new string[] { "fdrId", "dtsId" }, Duration = 60)]
        public IActionResult GetActualPlannedGraphData(string fdrId, string dtsId)
        {
            //string dta = customEncryption.DecryptStringAES(dataString.d);

            //string fdrId = JsonConvert.DeserializeObject<string>(dta);
            //string dtsId = JsonConvert.DeserializeObject<string>(dta);

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {

                string tempResponse = keBAL.GetActualPlannedGraphData(fdrId, dtsId);

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "GetActualPlannedGraphData > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetActualPlannedGraphData");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //    return Ok(containerGeneric);
        }

        /// <summary>
        /// Provide graphical analysis for last 24 months bill (for comparative purposes). Along with an option to view your current bill. 
        /// </summary>
        /// <param name="contractNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBillingHistory")]
        //[ResponseCache(VaryByQueryKeys = new string[] { "contractNo" }, Duration = 60)]
        public IActionResult GetBillingHistory(string contractNo)
        {

            // string contractNo = customEncryption.DecryptStringAES(dataString.d);

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                string tempResponse = string.Empty;

                var cache = _distributedCache.GetString("GetBillingHistory-"+contractNo);

                if (cache == null)
                {
                    tempResponse = keBAL.GetBillingHistory(contractNo);
                    _distributedCache.SetString("GetBillingHistory-" + contractNo, tempResponse,cacheExpiry);
                }
                else
                {
                    tempResponse = cache;
                }

                //string CacheKE = "BillingHistory-" + contractNo;
                //if (CacheHelper.GetCache(CacheKE) == null)
                //{
                //    Logging.Log("--IF Case--- cache created" + CacheKE);
                //    tempResponse = keBAL.GetBillingHistory(contractNo);
                //    CacheHelper.SetCache(CacheKE, tempResponse);
                //}
                //else
                //{
                //    Logging.Log("--Else Case--- cache exist" + CacheKE);
                //    tempResponse = (string)CacheHelper.GetCache(CacheKE);
                //}

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    return Ok(containerGeneric);
                }
                else
                {
                    try
                    {
                        JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
                        jsonSetting.DateParseHandling = DateParseHandling.None;

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse, jsonSetting));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "GetBillingHistory > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetBillingHistory");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Post cash billing complaint to SAP
        /// </summary>
        /// <param name="postCashBillingComplaint"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostCashBillingComplaint")]
        public IActionResult PostCashBillingComplaint([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            PostCashBillingComplaint postCashBillingComplaint = new PostCashBillingComplaint();
            postCashBillingComplaint = JsonConvert.DeserializeObject<PostCashBillingComplaint>(decPayload);


            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {


                HttpContext a = HttpContext;
                object objUserId = null;
                a.Items.TryGetValue("userId", out objUserId);

                string tempResponse = keBAL.PostCashBillingComplaint(postCashBillingComplaint, Convert.ToString(objUserId), dataString.base64Image);

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    //      return Ok(containerGeneric);
                }
                else
                {
                    try
                    {

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "PostCashBillingComplaint > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //        return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostCashBillingComplaint");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //   return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Returns array of Usage History set
        /// </summary>
        /// <param name="contractNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUsageHistorySet")]
        //[ResponseCache(VaryByQueryKeys = new string[] { "contractNo" }, Duration = 60)]
        public IActionResult GetUsageHistorySet([FromQuery]string contractNo)
        {
            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            if (!isSapRequest)
            {
                      
                var tempResponse = "{\"d\":{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistorySet('0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistorySet('0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistory\"},\"Message\":\"Success\",\"Status\":\"0\",\"contract\":\"0033604814\",\"pastYear\":{\"results\":[{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='10',contract='0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='10',contract='0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistoryOut\"},\"month\":\"10\",\"contract\":\"0033604814\",\"usage\":\"4041\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='11',contract='0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='11',contract='0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistoryOut\"},\"month\":\"11\",\"contract\":\"0033604814\",\"usage\":\"323\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='12',contract='0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='12',contract='0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistoryOut\"},\"month\":\"12\",\"contract\":\"0033604814\",\"usage\":\"121\"}]},\"currentYear\":{\"results\":[{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='01',contract='0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='01',contract='0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistoryOut\"},\"month\":\"01\",\"contract\":\"0033604814\",\"usage\":\"115\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='02',contract='0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='02',contract='0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistoryOut\"},\"month\":\"02\",\"contract\":\"0033604814\",\"usage\":\"138\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='03',contract='0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='03',contract='0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistoryOut\"},\"month\":\"03\",\"contract\":\"0033604814\",\"usage\":\"227\"},{\"__metadata\":{\"id\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='04',contract='0033604814')\",\"uri\":\"https://fioriprd.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistoryOutSet(month='04',contract='0033604814')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistoryOut\"},\"month\":\"04\",\"contract\":\"0033604814\",\"usage\":\"318\"}]}}}";
                JObject parsed = JObject.Parse(tempResponse);
                UsageHistory usageHistory = JsonConvert.DeserializeObject<UsageHistory>(parsed.ToString());
                tempResponse = JsonConvert.SerializeObject(usageHistory);
                return Ok(customEncryption.EncryptStringToStringAES(tempResponse));

            }

            try
            {
                UsageHistory usageHistory = new UsageHistory();
                string tempResponse = string.Empty;

                var cache = _distributedCache.GetString("GetUsageHistorySet-" + contractNo);
                if (cache == null)
                {
                    tempResponse = keBAL.GetUsageHistory(contractNo);
                    _distributedCache.SetString("GetUsageHistorySet-" + contractNo, tempResponse, cacheExpiry);
                }
                else
                {
                    tempResponse = cache;
                }

                if (tempResponse != null)
                {
                    JObject parsed = JObject.Parse(tempResponse);
                    usageHistory = JsonConvert.DeserializeObject<UsageHistory>(parsed.ToString());
                    if(usageHistory.D.PastYear != null && usageHistory.D.PastYear.Results.Count() > 0)
                    {
                        if (usageHistory.D.PastYear.Results.Count() > 0 && usageHistory.D.PastYear.Results.Count() < 12)
                        {
                            //usageHistory.D.PastYear.Results = keBAL.CompleteUsageCycle(usageHistory.D.PastYear.Results);
                        }
                    }
                    if (usageHistory.D.CurrentYear.Results.Count() > 0 && usageHistory.D.CurrentYear.Results.Count() < 12)
                    {
                        //usageHistory.D.CurrentYear.Results = keBAL.CompleteUsageCycle(usageHistory.D.CurrentYear.Results[]);
                    }
                   
                }
                //    CacheHelper.SetCache(CacheKE, tempResponse);
                //}
                //else
                //{
                //    Logging.Log("--Else Case-- - cache exist" + CacheKE);
                //    tempResponse = (string)CacheHelper.GetCache(CacheKE);
                //}

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    tempResponse = "{\"d\":{\"__metadata\":{\"id\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistorySet('')\",\"uri\":\"https://fioriqa.ke.com.pk:44300/sap/opu/odata/sap/ZCONS_WEB_PORTAL_SRV/GetUsageHistorySet('')\",\"type\":\"ZCONS_WEB_PORTAL_SRV.GetUsageHistory\"},\"Message\":\"Success\",\"Status\":\"0\",\"contract\":\"\",\"pastYear\":{\"results\":[]},\"currentYear\":{\"results\":[]}}}";
                    JObject parsed = JObject.Parse(tempResponse);
                    usageHistory = JsonConvert.DeserializeObject<UsageHistory>(parsed.ToString());
                    tempResponse = JsonConvert.SerializeObject(usageHistory);
                    return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                    //  return Ok(containerGeneric);
                }
                else
                {
                    try
                    {
                        JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
                        jsonSetting.DateParseHandling = DateParseHandling.None;
                        tempResponse = JsonConvert.SerializeObject(usageHistory);
                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        //return Ok(JsonConvert.DeserializeObject(tempResponse, jsonSetting));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "GetUsageHistorySet > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //  return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetUsageHistorySet");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                // return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Get maintenance shutdown details
        /// </summary>
        /// <param name="fdrId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMainenaceShutDown")]
        [ResponseCache(VaryByQueryKeys = new string[] { "fdrId", "dtsId" }, Duration = 1)]
        public IActionResult GetMainenaceShutDown([FromQuery]string fdrId, string dtsId)
        {


            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {

                string tempResponse = keBAL.GetMainenaceShutDown(dtsId, fdrId);

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                    return Ok("bO/5qimskgqeZfFU0OWe8uPdbLpHnfJGuNchHIpDJ+djp2zkypeXF1r+mKbpH7cwVvrIMPtX267yhcnXhwiYKnhIPE8DcVmt38OICVrzHLTxXhOPXaEzp22I+SUwBOjgs3q/jMiz4LwDtn23CCwl2Ye/c+6fYzgHvUjflDkWp0OSSUKmpOezXmW6mXHwfhXfhjCc6IEcWZ8D/zkKTViAesocGoyzwWTmhoB9GcOD/2CqfR2stgDg+wJMDDtKkTFiZQ6KuqNR4mDFJ4Q/fk2oeh6xZi21H0/4TPBSVc+E0FXS31U6CU/hhHIzfmaFTcP5mojTVxfvzs7aMA50ybNspmnYvsyzyS01lfJ/HIdeZLSNDbcjjl1zevu/iQdR/xGEdE9sj+mvZhjdKJGeqpX0Rj34Pus1w3S3sSMAk0JVF6xyvtsxdFKjva8ebQtx7CtPiJhMQf6Ae09SJWa0kf/xjhXJ8vaEb7Cs10suAfdG+qUX7AwUAAF3fm45g8ELWykcYP5B300zz/sUAVm/brNLIQCjRY7ksJvxhBRy6b9O4+xtsihDzu892MOnFdFurJkBjDLImqJ4fHfJRDSx60Yts6qYalJLcw1qHSMl6D8DL/aYAfrl3Ar8oC2evc9x24qLZHcyYaanj4/xZ57oDnTwIdqvPuBWYPoUu04AAGdB+MM=");
                    //return Ok(containerGeneric);
                }
                else
                {
                    try
                    {
                        JsonSerializerSettings jsonSetting = new JsonSerializerSettings();
                        jsonSetting.DateParseHandling = DateParseHandling.None;

                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                        // return Ok(JsonConvert.DeserializeObject(tempResponse, jsonSetting));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "GetMainenaceShutDown > Parsing Issue");
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;

                        //return Ok(containerGeneric);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetMainenaceShutDown");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Post: Change Status Service
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="TicketNo"></param>
        /// <returns>Json Response</returns>
        [HttpPost]
        [Route("PostChangeStatus")]
        public IActionResult PostChangeStatus([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            TicketStatus ticketStatus = new TicketStatus();
            ticketStatus = JsonConvert.DeserializeObject<TicketStatus>(decPayload);

            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                HttpContext a = HttpContext;
                object objUserId = null;
                a.Items.TryGetValue("userId", out objUserId);

                Logging.Log("Change Ticket Status Log: UserId: " + Convert.ToString(objUserId) + " - TicketNo: " + ticketStatus.TicketNo + " - Ticket Status: " + ticketStatus.Status);

                string tempResponse = keBAL.PostChangeStatus(ticketStatus);

                if (string.IsNullOrEmpty(tempResponse))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.ServiceResponseEmpty;
                }
                else
                {
                    try
                    {
                        return Ok(customEncryption.EncryptStringToStringAES(tempResponse));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "PostChangeStatus > Parsing Issue");

                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.ServiceResponseParsingError;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostChangeStatus");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// /Test Message for checking sms service is working
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("SendSMS")]
        //public bool SendSMS()
        //{

        //    return keBAL.SendSMS();
        //}

        [HttpPost]
        [Route("SendPromoSMS")]
        public string SendPromoSMS([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            SMSValue SMSvalue = new SMSValue();
            SMSvalue = JsonConvert.DeserializeObject<SMSValue>(decPayload);

            bool tempResponse = false;

            try
            {
                tempResponse = keBAL.SendPromoSMS(SMSvalue);

                if (!tempResponse)
                {
                    return "false";
                }
                else
                {
                    return "true";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "SendSMS");

            }

            return Convert.ToString(tempResponse);
        }

        // add comments
    }
}