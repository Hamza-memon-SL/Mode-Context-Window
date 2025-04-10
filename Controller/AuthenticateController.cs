using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KE.Service.BAL;
using KE.Service.Helper;
using KE.Service.Models;
using KE.Service.ResponseModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace KE.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Authenticate")]
    //[EnableCors("CorsPolicy")]
    public class AuthenticateController : Controller
    {
        BAL.BAL b = null;
        CustomEncryption customEncryption = new CustomEncryption();

        /// <summary>
        /// Initialize Service Parameters
        /// </summary>
        /// <param name="iconfiguration"></param>
        public AuthenticateController(IConfiguration iconfiguration)
        {
            b = new BAL.BAL(iconfiguration);

        }

        /// <summary>
        /// Test Service for checking service response
        /// </summary>
        /// <returns></returns>
        // GET: api/Registration
        [HttpGet]
        public string Get()
        {
            //HttpContext a = HttpContext;
            //object temp = null;
            //a.Items.TryGetValue("userId", out temp);

            return "Service is running! checking deployment";
        }

        /// <summary>
        /// Returns added account in array 
        /// </summary>
        /// <returns></returns>
        // GET: api/Authenticate/GetUserManageAccount
        [HttpGet]
        [Route("gettestmethod")]
        public string gettestmethod()
        {
            string userid = "qwertyui";


            return userid;
        }

        /// <summary>
        /// This service authenticate user with provided email address and password
        /// </summary>
        /// <param name="login"></param>
        /// <returns>On success return User Object, Else return failure</returns>
        // POST: api/Authenticate/PostLoginMobile
        [HttpPost]
        [Route("PostLoginMobile")]
        public IActionResult LoginMobile([FromBody]EncryptedString login)
        {
            ContainerLogin containerLogin = new ContainerLogin();
            containerLogin.data = new ContainerLogin.Data();
            containerLogin.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerLogin.data.Status = 1;
                //    containerLogin.data.Message = Constants.Registration_InvalidServiceCredential;

                //    return Ok(containerLogin);
                //}

                User user = b.LoginMobile(login);
                if (user.isBlock)
                {
                    containerLogin.data.Status = 1;
                    containerLogin.data.Message = Constants.Login_UserBlock;
                }
                else
                {
                    if (user != null)
                    {
                        if (!user.isApproved)
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Login_OTPNotVerified;
                            containerLogin.data.users.Add(new Models.User { isApproved = false, userId = user.userId, mobileNo = user.mobileNo, email = user.email });
                        }
                        else
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Registration_Success;

                            containerLogin.data.users.Add(user);
                            Logging.Log(string.Format("LoginMobile -> IP: {0}", Convert.ToString(HttpContext.Connection.RemoteIpAddress)));
                        }
                    }
                    else
                    {
                        containerLogin.data.Status = 1;
                        containerLogin.data.Message = Constants.Login_Failure;
                    }
                }
                // string stringfyObject = JsonConvert.SerializeObject(containerLogin);
                // return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "Login");

                containerLogin.data.Status = 1;
                containerLogin.data.Message = Constants.Exception_GeneralMessage;
                string sringfyObjectx = JsonConvert.SerializeObject(containerLogin);
                //return Ok(containerLogin);
            }
            // return Ok(containerLogin);
            string stringfyObject = JsonConvert.SerializeObject(containerLogin);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        [HttpPost]
        [Route("PostLoginMobileSM")]
        public IActionResult LoginSM([FromBody]EncryptedString login)
        {
            ContainerLoginSM containerLogin = new ContainerLoginSM();
            containerLogin.data = new ContainerLoginSM.Data();
            containerLogin.data.users = new List<SMUser>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerLogin.data.Status = 1;
                //    containerLogin.data.Message = Constants.Registration_InvalidServiceCredential;

                //    return Ok(containerLogin);
                //}

                SMUser user = b.LoginMobileSM(login);
                if (user.isBlock)
                {
                    containerLogin.data.Status = 1;
                    containerLogin.data.Message = Constants.Login_UserBlock;
                }
                else
                {
                    if (user != null)
                    {
                        if (!user.isApproved)
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Login_OTPNotVerified;
                            containerLogin.data.users.Add(new Models.SMUser { isApproved = false, userId = user.userId, mobileNo = user.mobileNo, email = user.email });
                        }
                        else
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Registration_Success;

                            containerLogin.data.users.Add(user);
                            Logging.Log(string.Format("LoginMobile -> IP: {0}", Convert.ToString(HttpContext.Connection.RemoteIpAddress)));
                        }
                    }
                    else
                    {
                        containerLogin.data.Status = 1;
                        containerLogin.data.Message = Constants.Login_Failure;
                    }
                }
                // string stringfyObject = JsonConvert.SerializeObject(containerLogin);
                // return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "Login");

                containerLogin.data.Status = 1;
                containerLogin.data.Message = Constants.Exception_GeneralMessage;
                string sringfyObjectx = JsonConvert.SerializeObject(containerLogin);
                //return Ok(containerLogin);
            }
            // return Ok(containerLogin);
            string stringfyObject = JsonConvert.SerializeObject(containerLogin);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        
        [HttpPost]
        [Route("PostLoginMobileFP")]
        public IActionResult LoginFP([FromBody]EncryptedString login)
        {
            ContainerLoginSM containerLogin = new ContainerLoginSM();
            containerLogin.data = new ContainerLoginSM.Data();
            containerLogin.data.users = new List<SMUser>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                SMUser user = b.LoginMobileFP(login);
                if (user.isBlock)
                {
                    containerLogin.data.Status = 1;
                    containerLogin.data.Message = Constants.Login_UserBlock;
                }
                else
                {
                    if (user != null)
                    {
                        if (!user.isApproved)
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Login_OTPNotVerified;
                            containerLogin.data.users.Add(new Models.SMUser { isApproved = false, userId = user.userId, mobileNo = user.mobileNo, email = user.email });
                        }
                        else
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Registration_Success;
                            containerLogin.data.users.Add(user);
                        }
                    }
                    else
                    {
                        containerLogin.data.Status = 1;
                        containerLogin.data.Message = Constants.Login_Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "Login");

                containerLogin.data.Status = 1;
                containerLogin.data.Message = Constants.Exception_GeneralMessage;
                string sringfyObjectx = JsonConvert.SerializeObject(containerLogin);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerLogin);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerLogin);
        }

        /// <summary>
        /// This service authenticate user with provided email address and password
        /// </summary>
        /// <param name="login"></param>
        /// <returns>On success return User Object, Else return failure</returns>
        // POST: api/Authenticate/PostLogin
        [HttpPost]
        [Route("PostLogin")]

        public IActionResult Login([FromBody]Login login)
        {
            ContainerLogin containerLogin = new ContainerLogin();
            containerLogin.data = new ContainerLogin.Data();
            containerLogin.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerLogin.data.Status = 1;
                //    containerLogin.data.Message = Constants.Registration_InvalidServiceCredential;

                //    return Ok(containerLogin);
                //}

                User user = b.Login(login);
                if (user.isBlock)
                {
                    Logging.Log("User Block IF");
                    containerLogin.data.Status = 1;
                    containerLogin.data.Message = Constants.Login_UserBlock;
                    Logging.Log("User Block IF + " + containerLogin.data.Message);
                }
                else
                {
                    Logging.Log("ELSE Block");
                    if (user != null)
                    {
                        if (!user.isApproved)
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Login_OTPNotVerified;
                            containerLogin.data.users.Add(new Models.User { isApproved = false, userId = user.userId, mobileNo = user.mobileNo });
                        }
                        else
                        {
                            containerLogin.data.Status = 0;
                            containerLogin.data.Message = Constants.Registration_Success;
                            containerLogin.data.users.Add(user);
                            Logging.Log(string.Format("Login -> IP: {0} -> Email: {1}", Convert.ToString(HttpContext.Connection.RemoteIpAddress), login.email));
                        }
                    }
                    else
                    {

                        containerLogin.data.Status = 1;
                        containerLogin.data.Message = Constants.Login_Failure;
                    }
                }

                return Ok(containerLogin);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "Login");

                containerLogin.data.Status = 1;
                containerLogin.data.Message = Constants.Exception_GeneralMessage;

                return Ok(containerLogin);
            }
        }

        /// <summary>
        /// This service will logout user from portal 
        /// IF Platform == "Web"
        /// 
        /// This service will logout user from mobile
        /// IF Platform == "ios" || Platform == "android"
        /// 
        /// </summary>
        /// <returns>true(Success)/false(Failure)</returns>
        // POST: api/Authenticate/PostLogout
        [HttpPost]
        [Route("PostLogout")]
        public IActionResult PostLogout([FromBody]EncryptedString dataString)
        {
            string decPayload = string.Empty;
            MobileParameter mobileParameter = new MobileParameter();

            try
            {
                decPayload = customEncryption.DecryptStringAES(dataString.d);
                mobileParameter = JsonConvert.DeserializeObject<MobileParameter>(decPayload);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostLogout -> Parsing Issue");
                mobileParameter = new MobileParameter();
                mobileParameter.Platform = "web";
            }

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    if (b.Logout(userid, b.IsMobileRequest(mobileParameter)))
                    {
                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Constants.Success;
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostLogout");
            }

            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service will add account detail to SQL table with provided parameter
        /// </summary>
        /// <param name="accountDetail"></param>
        /// <returns>true(Success)/false(Failure)</returns>
        // POST: api/Authenticate/PostAddAccount
        [HttpPost]
        [Route("PostAddAccount")]
        public IActionResult PostAddAccount([FromBody]EncryptedString accountDetail)
        {
            string decPayload = customEncryption.DecryptStringAES(accountDetail.d);
            AccountDetail data = new AccountDetail();
            data = JsonConvert.DeserializeObject<AccountDetail>(decPayload);
            // MobileParameter m = JsonConvert.DeserializeObject<Login>(decPayload);

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (string.IsNullOrEmpty(data.accountId))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.rfv_AccountId;
                }

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    if (b.PostAddAccount(data, userid, accountDetail.base64Image) == 0)
                    {
                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Constants.PostAddAccount_Success;
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.PostAddAccount_Error;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostAddAccount");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //  return Ok(containerGeneric);
        }


        ////copy
        ///// <summary>
        ///// This service will add account detail to SQL table with provided parameter
        ///// </summary>
        ///// <param name="accountDetail"></param>
        ///// <returns>true(Success)/false(Failure)</returns>
        //// POST: api/Authenticate/PostAddAccount
        //[HttpPost]
        //[Route("PostAddAccount")]
        //public IActionResult PostAddAccount([FromBody]AccountDetail accountDetail)
        //{
        //    string userid = string.Empty;
        //    ContainerGeneric containerGeneric = new ContainerGeneric();
        //    containerGeneric.data = new ContainerGeneric.Data();

        //    try
        //    {
        //        //if (!ModelState.IsValid)
        //        //    return Ok(ModelState);

        //        if (string.IsNullOrEmpty(accountDetail.AccountId))
        //        {
        //            containerGeneric.data.Status = 1;
        //            containerGeneric.data.Message = Constants.rfv_AccountId;
        //        }

        //        if (!b.AuthenticateToken(Request, out userid))
        //        {
        //            containerGeneric.data.Status = 1;
        //            containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
        //        }
        //        else
        //        {
        //            if (b.PostAddAccount(accountDetail, userid) == 0)
        //            {
        //                containerGeneric.data.Status = 0;
        //                containerGeneric.data.Message = Constants.PostAddAccount_Success;
        //            }
        //            else
        //            {
        //                containerGeneric.data.Status = 1;
        //                containerGeneric.data.Message = Constants.PostAddAccount_Error;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandling.LogError(ex, "PostAddAccount");

        //        containerGeneric.data.Status = 1;
        //        containerGeneric.data.Message = Constants.Exception_GeneralMessage;
        //    }

        //    return Ok(containerGeneric);
        //}

        /// <summary>
        /// Returns added account in array 
        /// </summary>
        /// <returns></returns>
        // GET: api/Authenticate/GetUserManageAccount
        [HttpGet]
        [Route("GetUserManageAccount")]
        public GenericResponse GetUserManageAccount()
        {
            List<AccountDetail> accountDetailsList = new List<AccountDetail>();
            string userid = string.Empty;

            ContainerAccountDetail containerAccountDetail = new ContainerAccountDetail();
            containerAccountDetail.data = new ContainerAccountDetail.Data();
            containerAccountDetail.data.AccountDetail = new List<AccountDetail>();

            try
            {
                if (!ModelState.IsValid)
                {
                    
                    var modelState = customEncryption.EncryptStringToStringAES(ModelState.ToString());
                    GenericResponse modelStateResponse = new GenericResponse();
                    modelStateResponse.d = modelState;
                    return modelStateResponse;
                }

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerAccountDetail.data.Status = 1;
                    containerAccountDetail.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    List<AccountDetail> list = b.GetUserManageAccount(userid);

                    if (list != null && list.Count > 0)
                    {
                        containerAccountDetail.data.Status = 0;
                        containerAccountDetail.data.Message = Constants.UserAccountDetail_Success;
                        containerAccountDetail.data.AccountDetail = list;
                    }
                    else
                    {
                        containerAccountDetail.data.Status = 1;
                        containerAccountDetail.data.Message = Constants.UserAccountDetail_Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetUserManageAccount");

                containerAccountDetail.data.Status = 1;
                containerAccountDetail.data.Message = Constants.UserAccountDetail_Failure;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerAccountDetail.data);
            var encryptedResponse= customEncryption.EncryptStringToStringAES(stringfyObject);
            GenericResponse response = new GenericResponse();
            response.d = encryptedResponse;
            return response;
            //  return Ok(containerAccountDetail);
        }

        /// <summary>
        /// Update account detail
        /// </summary>
        /// <param name="accountDetail"></param>
        /// <returns>true/false</returns>
        // HttpPut: api/Authenticate/PutUpdateAccount
        [HttpPost]
        [Route("PutUpdateAccount")]
        public IActionResult PutUpdateAccount([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            AccountDetail accountDetail = new AccountDetail();
            accountDetail = JsonConvert.DeserializeObject<AccountDetail>(decPayload);

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (string.IsNullOrEmpty(accountDetail.accountId))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.rfv_AccountId;
                }

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    if (b.PutUpdateAccount(accountDetail, userid) == 0)
                    {
                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Constants.PutUpdateAccount_Success;
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.PutUpdateAccount_Error;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PutUpdateAccount");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));

        }

        /// <summary>
        /// Delete user account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>true/false</returns>
        // DELETE: api/Authenticate/DeleteAccount
        [HttpPost]
        [Route("DeleteAccount")]
        public IActionResult DeleteAccount([FromQuery]string accountId)
        {

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (string.IsNullOrEmpty(accountId))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.rfv_AccountId;
                }

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    if (b.DeleteAccount(accountId))
                    {
                        b.ExpireOtherDeviceToken(Request);

                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Constants.DeleteAccount_Success;
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.DeleteAccount_Error;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "DeleteAccount");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Exception_GeneralMessage;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            // return Ok(containerGeneric);
        }

        /// <summary>
        /// Return user details
        /// </summary>
        /// <returns>User object</returns>
        // GET: api/Authenticate/GetUserDetail

        [HttpGet]
        [Route("GetUserDetail")]
        public IActionResult GetUserDetail()
        {
            string userid = string.Empty;

            ContainerLogin containerUserDetail = new ContainerLogin();
            containerUserDetail.data = new ContainerLogin.Data();
            containerUserDetail.data.users = new List<User>();

            //MobileParameter mobileParameter = new MobileParameter();
            //mobileParameter.DeviceID = "";
            //mobileParameter.DeviceToken = "";
            //mobileParameter.Platform = "";
            //mobileParameter.isRooted = false;
        
            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerUserDetail.data.Status = 1;
                    containerUserDetail.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    User user = b.GetUserObjectByUserId(userid);

                    if (user != null)
                    {
                        containerUserDetail.data.Status = 0;
                        containerUserDetail.data.Message = Constants.UserAccountDetail_Success;
                        containerUserDetail.data.users.Add(user);
                    }
                    else
                    {
                        containerUserDetail.data.Status = 1;
                        containerUserDetail.data.Message = Constants.UserAccountDetail_Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetUserManageAccount");

                containerUserDetail.data.Status = 1;
                containerUserDetail.data.Message = Constants.UserAccountDetail_Failure;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerUserDetail);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerUserDetail);
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="user"></param>
        /// <returns>true/false</returns>
        // POST: api/Authenticate/EditProfile
        [HttpPost]
        [Route("EditProfile")]
        public IActionResult EditProfile([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            EditUser data = new EditUser();
            MobileParameter mb = new MobileParameter();
            data = JsonConvert.DeserializeObject<EditUser>(decPayload);
            mb = JsonConvert.DeserializeObject<MobileParameter>(decPayload);
            //   EditUser user = new EditUser();

            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    int result = b.EditProfile(userid, data.Password, data.FirstName, data.LastName, data.MobileNo, data.CNIC, data.email, dataString.base64Image, mb.DeviceID, mb.DeviceToken, mb.Platform);
                    if (result == 0)
                    {
                        if (!string.IsNullOrEmpty(data.Password))
                        {
                            b.ExpireOtherDeviceToken(Request);
                        }

                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Constants.Success;
                    }
                    else if (result == 1)
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = "Email already Exist. Please enter a unique Email.";
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "EditProfile");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerGeneric);
        }

        /// <summary>
        /// Return user inbox in array
        /// </summary>
        /// <returns></returns>
        // GET: api/Authenticate/GetUserInbox
        [HttpGet]
        [Route("GetUserInbox")]
        public IActionResult GetUserInbox()
        {
            string userid = string.Empty;

            ContainerUserInbox containerUserInbox = new ContainerUserInbox();
            containerUserInbox.data = new ContainerUserInbox.Data();
            containerUserInbox.data.userInbox = new List<UserInbox>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerUserInbox.data.Status = 1;
                    containerUserInbox.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    string unreadCount = string.Empty;

                    List<UserInbox> listUserInbox = b.GetUserInbox(userid, out unreadCount);

                    if (listUserInbox != null && listUserInbox.Count > 0)
                    {
                        containerUserInbox.data.userInbox = listUserInbox;
                        containerUserInbox.data.UnreadCount = unreadCount;
                        containerUserInbox.data.Status = 0;
                        containerUserInbox.data.Message = Constants.Success;
                    }
                    else
                    {
                        containerUserInbox.data.Status = 1;
                        containerUserInbox.data.Message = Constants.UserInbox_Error;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetUserInbox");

                containerUserInbox.data.Status = 1;
                containerUserInbox.data.Message = Constants.Failure;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerUserInbox);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerUserInbox);
        }

        // GET: api/Authenticate/GetUserInboxPaged?offset=1
        [HttpGet]
        [Route("GetUserInboxPaged")]
        public IActionResult GetUserInboxPaged([FromQuery]int offset)
        {
            string userid = string.Empty;
            ContainerUserInbox containerUserInbox = new ContainerUserInbox();
            containerUserInbox.data = new ContainerUserInbox.Data();
            containerUserInbox.data.userInbox = new List<UserInbox>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerUserInbox.data.Status = 1;
                    containerUserInbox.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    int pageSize = 10;
                    int startPage = offset * pageSize;

                    string unreadCount = string.Empty;

                    List<UserInbox> listUserInbox = b.GetUserInbox(userid, out unreadCount);
                    List<UserInbox> pagedUserInbox = listUserInbox.Skip(startPage).Take(pageSize).ToList();


                    if (pagedUserInbox != null && pagedUserInbox.Count > 0)
                    {
                        containerUserInbox.data.userInbox = pagedUserInbox;
                        containerUserInbox.data.UnreadCount = unreadCount;
                        containerUserInbox.data.Status = 0;
                        containerUserInbox.data.Message = Constants.Success;
                    }
                    else
                    {
                        containerUserInbox.data.Status = 1;
                        containerUserInbox.data.Message = Constants.UserInbox_Error;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetUserInbox Parameterized");

                containerUserInbox.data.Status = 1;
                containerUserInbox.data.Message = Constants.Failure;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerUserInbox);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerUserInbox);
        }

        /// <summary>
        /// Return count of unread inbox item
        /// </summary>
        /// <returns></returns>
        // GET: api/Authenticate/GetUnreadInboxItemsCount
        [HttpGet]
        [Route("GetUnreadInboxItemsCount")]
        public IActionResult GetUnreadInboxItemsCount()
        {
            string userid = string.Empty;
            ContainerUserInbox containerUserInbox = new ContainerUserInbox();
            containerUserInbox.data = new ContainerUserInbox.Data();
            containerUserInbox.data.userInbox = new List<UserInbox>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerUserInbox.data.Status = 1;
                    containerUserInbox.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    string strCount = b.GetUnreadInboxItemsCount(userid);

                    if (!string.IsNullOrEmpty(strCount))
                    {
                        containerUserInbox.data.Status = 0;
                        containerUserInbox.data.Message = Constants.UserAccountDetail_Success;
                        containerUserInbox.data.UnreadCount = strCount;
                    }
                    else
                    {
                        containerUserInbox.data.Status = 1;
                        containerUserInbox.data.Message = Constants.UserAccountDetail_Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetUnreadInboxItemsCount");

                containerUserInbox.data.Status = 1;
                containerUserInbox.data.Message = Constants.Failure;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerUserInbox);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerUserInbox);
        }

        /// <summary>
        /// Update inbox item i.e. mark as read
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        // HttpPut: api/Authenticate/PutInboxItem
        [HttpPost]
        [Route("PutInboxItem")]
        public IActionResult PutInboxItem([FromQuery]string itemId)
        {
            // string itemId = customEncryption.DecryptStringAES(dataString.d);
            string userid = string.Empty;

            ContainerUserInbox containerUserInbox = new ContainerUserInbox();
            containerUserInbox.data = new ContainerUserInbox.Data();
            containerUserInbox.data.userInbox = new List<UserInbox>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerUserInbox.data.Status = 1;
                    containerUserInbox.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    string countUpdate = b.UpdateInboxItem(userid, itemId);

                    if (!string.IsNullOrEmpty(countUpdate))
                    {
                        containerUserInbox.data.Status = 0;
                        containerUserInbox.data.Message = Constants.Success;
                        containerUserInbox.data.UnreadCount = countUpdate;
                    }
                    else
                    {
                        containerUserInbox.data.Status = 1;
                        containerUserInbox.data.Message = Constants.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PutInboxItem");

                containerUserInbox.data.Status = 1;
                containerUserInbox.data.Message = Constants.Failure;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerUserInbox);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerUserInbox);
        }

        /// <summary>
        /// Not using this method for backup perspective
        /// </summary>
        /// <param name="userInbox"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostInboxItem")]
        public IActionResult PostInboxItem([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            UserInbox userInbox = new UserInbox();
            userInbox = JsonConvert.DeserializeObject<UserInbox>(decPayload);


            string userid = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    if (b.AddInboxItemAndSendPushNotification(userInbox))
                    {
                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Constants.Success;
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostInboxItem");
                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //   return Ok(containerGeneric);
        }

        [HttpPost]
        [Route("GetFileByRelativePath")]
        public IActionResult GetFileByRelativePath([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            FileModel fileModel = new FileModel();
            fileModel = JsonConvert.DeserializeObject<FileModel>(decPayload);

            string userid = string.Empty;
            ContainerFileResponse containerFileResponse = new ContainerFileResponse();
            containerFileResponse.data = new ContainerFileResponse.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerFileResponse.data.Status = 1;
                    containerFileResponse.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    FileUpload fileUpload = new FileUpload();
                    string file = fileUpload.GetFileByPath(fileModel.Filename);

                    if (string.IsNullOrEmpty(file))
                    {
                        containerFileResponse.data.Status = 1;
                        containerFileResponse.data.Message = Constants.Failure;
                    }
                    else
                    {
                        containerFileResponse.data.Status = 0;
                        containerFileResponse.data.Message = Constants.Success;
                        containerFileResponse.data.Base64Image = file;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetFileByRelativePath");
                containerFileResponse.data.Status = 1;
                containerFileResponse.data.Message = Constants.Failure;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerFileResponse);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerFileResponse);
        }

        [HttpPost]
        [Route("FileUploadnReturnPath")]
        public IActionResult FileUploadnReturnPath([FromBody]EncryptedString base64ImagePath)
        {
            string userid = string.Empty;
            string strRelativePath = string.Empty;
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    FileUpload fileUpload = new FileUpload();
                    containerGeneric.data.Status = 0;
                    containerGeneric.data.Message = fileUpload.FileAddAndReturnPath(userid, base64ImagePath.d, ImageCateogry.Other, out strRelativePath);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "FileUploadnReturnPathFileUploadnReturnPath");
                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = "FileUploadnReturnPath Exc";
            }

            return Ok(containerGeneric);

            //string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            //return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        // not being used anywhere - As per analysis by marib and sindhya
        // POST api/FileAdd
        [HttpPost]
        [Route("FileAdd")]
        public IActionResult FileAdd([FromBody] BillingComplaint upload)
        {
            try
            {
                FileUpload fileUpload = new FileUpload();

                //Image Work Pending
                //billingComplaint.DMSLink
                //MemoryStream ms = new MemoryStream(billingComplaint.DMSLink, 0, billingComplaint.DMSLink.Length);
                //byte[] byteEncoded = Convert.FromBase64String(billingComplaint.DMSLink);
                //byteEncoded.
                string strRelativePath = string.Empty;

                return Ok(fileUpload.FileAddAndReturnPath("abdd594b-d2a0-40e5-b720-1c15992a94d6", "", ImageCateogry.Profile, out strRelativePath));
            }
            catch (Exception ex)
            {
                return BadRequest(Constants.Exception_GeneralMessage);
            }
        }

        // not being used anywhere - As per analysis by marib and sindhya
        // PUT api/FileUpdate
        [HttpPost()]
        [Route("FileUpdate")]
        public IActionResult FileUpdate([FromBody] BillingComplaint upload)
        {
            try
            {
                FileUpload fileUpload = new FileUpload();

                //Image Work Pending
                //billingComplaint.DMSLink
                //MemoryStream ms = new MemoryStream(billingComplaint.DMSLink, 0, billingComplaint.DMSLink.Length);
                //byte[] byteEncoded = Convert.FromBase64String(billingComplaint.DMSLink);
                //byteEncoded.

                return Ok(fileUpload.FileUpdateAndReturnPath(upload.Code, ""));
            }
            catch (Exception ex)
            {
                return BadRequest(Constants.Exception_GeneralMessage);
            }
        }

        // not being used anywhere - As per analysis by marib and sindhya
        // DELETE api/FileDelete/
        [HttpPost()]
        [Route("FileDelete")]
        public IActionResult FileDelete([FromBody] BillingComplaint upload)
        {
            try
            {
                FileUpload fileUpload = new FileUpload();



                return Ok(fileUpload.FileDelete(upload.Code));
            }
            catch (Exception ex)
            {
                return BadRequest(Constants.Exception_GeneralMessage);
            }
        }

        /// <summary>
        /// Return Technical Complaint By Email
        /// </summary>
        /// <returns>User object</returns>
        // GET: api/Authenticate/GetTechnicalComplaintByEmail
        [HttpPost]
        [Route("GetTechnicalComplaintByEmail")]
        public IActionResult GetTechnicalComplaintByEmail([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ForgotPassword forgotPassword = new ForgotPassword();
            forgotPassword = JsonConvert.DeserializeObject<ForgotPassword>(decPayload);

            string userid = string.Empty;

            ContainerTechnicalComplaint containerTechnical = new ContainerTechnicalComplaint();
            containerTechnical.data = new ContainerTechnicalComplaint.Data
            {
                complaints = new List<CustomTechnicalPushNotification>()
            };

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerTechnical.data.Status = 1;
                    containerTechnical.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    List<CustomTechnicalPushNotification> listTPN = b.GetTechnicalPushNotificationByEmail(forgotPassword.Email);

                    if (listTPN != null && listTPN.Count > 0)
                    {
                        containerTechnical.data.Status = 0;
                        containerTechnical.data.Message = Constants.Success;
                        containerTechnical.data.complaints = listTPN;
                    }
                    else
                    {
                        containerTechnical.data.Status = 1;
                        containerTechnical.data.Message = "No records found";
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetTechnicalComplaintByEmail");

                containerTechnical.data.Status = 1;
                containerTechnical.data.Message = Constants.UserAccountDetail_Failure;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerTechnical);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Delete Technical Complaint
        /// </summary>
        /// <returns>User object</returns>
        // POST: api/Authenticate/DeleteTechnicalComplaint
        [HttpPost()]
        [Route("DeleteTechnicalComplaint")]
        public IActionResult DeleteTechnicalComplaint([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            DeleteTechnicalComplaint deleteComplaint = new DeleteTechnicalComplaint();
            deleteComplaint = JsonConvert.DeserializeObject<DeleteTechnicalComplaint>(decPayload);

            string userid = string.Empty;

            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateToken(Request, out userid))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.InvalidOrExpiredToken;
                }
                else
                {
                    if (b.DeleteTechnicalPushNotificationById(deleteComplaint.Id))
                    {
                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Constants.Success;
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = "No records found";
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "DeleteTechnicalComplaint");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.UserAccountDetail_Failure;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Return user details
        /// </summary>
        /// <returns>User object</returns>
        // GET: api/Authenticate/GetUser
        [HttpPost()]
        [Route("GetUser")]
        public IActionResult GetUserDetailsUsingEmail([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            UserEmail userEmail = new UserEmail();
            userEmail = JsonConvert.DeserializeObject<UserEmail>(decPayload);

            string userid = string.Empty;

            ContainerLogin containerUserDetail = new ContainerLogin();
            containerUserDetail.data = new ContainerLogin.Data();
            containerUserDetail.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                userid = b.VerifyEmail(userEmail.Email);
                if (!string.IsNullOrEmpty(userid))
                {
                    User user = b.GetUserObjectByUserId(userid);

                    if (user != null)
                    {
                        containerUserDetail.data.Status = 0;
                        containerUserDetail.data.Message = Constants.UserAccountDetail_Success;
                        containerUserDetail.data.users.Add(user);
                    }
                    else
                    {
                        containerUserDetail.data.Status = 1;
                        containerUserDetail.data.Message = Constants.UserAccountDetail_Failure;
                    }
                }
                else
                {
                    containerUserDetail.data.Status = 1;
                    containerUserDetail.data.Message = Constants.UserAccountDetail_Failure;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetUserManageAccount");

                containerUserDetail.data.Status = 1;
                containerUserDetail.data.Message = Constants.UserAccountDetail_Failure;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerUserDetail);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(containerUserDetail);
        }
    }
}