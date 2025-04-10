using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KE.DAL;
using KE.Service.BAL;
using KE.Service.Helper;
using KE.Service.Models;
using KE.Service.ResponseModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Remotion.Linq.Clauses;

namespace KE.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Registration")]
    //[EnableCors("CorsPolicy")]
    public class RegistrationController : Controller
    {
        BAL.BAL b = null;
        KEBAL kEBAL = null;
        string FolderName = "PushNotificationFiles";
        CustomEncryption customEncryption = new CustomEncryption();
        /// <summary>
        /// For Initializing values
        /// </summary>
        /// <param name="iconfiguration"></param>
        public RegistrationController(IConfiguration iconfiguration)
        {
            b = new BAL.BAL(iconfiguration);
            kEBAL = new KEBAL(iconfiguration);
        }


        /// <summary>
        /// Test response for controller 
        /// </summary>
        /// <returns></returns>
        // GET: api/Registration
        [HttpGet]
        public string Get()
        {
            return "Service is running and deployed!";
        }

        [HttpGet]
        [Route("GetDataCall")]
        public IActionResult GetDataCall([FromQuery]string userId)
        {
            try
            {
                return Ok(b.GetDatabaseCall(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Register new user from mobile and store data to SQL Server Membership table on success user receives OTP on provided mobile number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Registration/PostSignupMobile
        [HttpPost]
        [Route("PostSignupMobile")]
        public IActionResult RegisterMobile([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            User user = new User();
            MobileParameter mp = new MobileParameter();
            user = JsonConvert.DeserializeObject<User>(decPayload);
            mp = JsonConvert.DeserializeObject<User>(decPayload);

            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                string userId = string.Empty;
                string uid = b.CheckUserExist(user.email);
                if (string.IsNullOrEmpty(uid))
                {
                    userId = b.RegisterMobileUser(user.userName, user.password, user.email, user.firstName, user.lastName, user.mobileNo, user.cnic, mp.DeviceID, mp.DeviceToken, mp.Platform);
                }
                else
                {
                    userId = b.UpdateMobileUser(user.userName, user.password, user.email, user.firstName, user.lastName, user.mobileNo, user.cnic, uid, mp.DeviceID, mp.DeviceToken, mp.Platform);
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Registration_Success;
                    containerRegister.data.users = new List<User>();
                    //containerRegister.data.users.Add(new Models.User { userId = userId, mobileNo = user.mobileNo });
                    containerRegister.data.users.Add(new Models.User { userId = userId, mobileNo = user.mobileNo, email = user.email });
                    string stringfyObjectSuccess = JsonConvert.SerializeObject(containerRegister);
                    return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectSuccess));
                }

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Registration_DuplicateEmail;

                string stringfyObjectFailure = JsonConvert.SerializeObject(containerRegister);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectFailure));
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostSignupMobile");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;
                string stringfyObjectcatch = JsonConvert.SerializeObject(containerRegister);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectcatch));
            }

            //  return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Register new user and store data to SQL Server Membership table on success user receives OTP on provided mobile number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Registration/PostSignup
        [HttpPost]
        [Route("PostSignup")]
        public IActionResult Register([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            User user = new User();
            MobileParameter mp = new MobileParameter();
            user = JsonConvert.DeserializeObject<User>(decPayload);
            mp = JsonConvert.DeserializeObject<User>(decPayload);

            Logging.Log(decPayload);

            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                string userId = string.Empty;
                string uid = b.CheckUserExist(user.email);
                if (string.IsNullOrEmpty(uid))
                {
                    userId = b.RegisterUser(user.userName, user.password, user.email, user.firstName, user.lastName, user.mobileNo, user.cnic, mp.DeviceID, mp.DeviceToken, mp.Platform, user.isRooted);
                }
                else
                {
                    userId = b.UpdateUser(user.userName, user.password, user.email, user.firstName, user.lastName, user.mobileNo, user.cnic, uid, mp.DeviceID, mp.DeviceToken, mp.Platform, user.isRooted);
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Registration_Success;
                    containerRegister.data.users = new List<User>();
                    //containerRegister.data.users.Add(new Models.User { userId = userId, mobileNo = user.mobileNo });
                    containerRegister.data.users.Add(new Models.User { userId = userId, mobileNo = user.mobileNo, email = user.email, isRooted = user.isRooted });
                    string stringfyObjectSuccess = JsonConvert.SerializeObject(containerRegister);
                    return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectSuccess));
                }
                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Registration_DuplicateEmail;

                string stringfyObjectFailure = JsonConvert.SerializeObject(containerRegister);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectFailure));
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostSignup");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;
                string stringfyObjectcatch = JsonConvert.SerializeObject(containerRegister);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectcatch));
            }

            //  return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Register new user and store data to SQL Server Membership table on success user receives OTP on provided mobile number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Registration/PostSignup
        [HttpPost]
        [Route("PostSignupSM")]
        public IActionResult RegisterSM([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            SMUser user = new SMUser();
            MobileParameter mp = new MobileParameter();
            user = JsonConvert.DeserializeObject<SMUser>(decPayload);
            mp = JsonConvert.DeserializeObject<SMUser>(decPayload);

            Logging.Log(decPayload);

            ContainerSMRegister containerRegister = new ContainerSMRegister();
            containerRegister.data = new ContainerSMRegister.Data();

            int newUser = -1;

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                string userId = string.Empty;
                string uid = string.Empty;
                if (!string.IsNullOrEmpty(user.email))
                {
                    uid = b.CheckSMUserExist(user.email, user.mobileNo,user.countryCode);
                }
                else
                {
                    uid = b.CheckSMUserExist(user.mobileNo,user.countryCode);
                }
                Logging.Log(uid);
                if (string.IsNullOrEmpty(uid))
                {
                    userId = b.RegisterSMUser(user.Provider,user.ID,user.userName, user.email, user.firstName, user.lastName, user.mobileNo,user.countryCode, user.cnic, mp.DeviceID, mp.DeviceToken, mp.Platform, user.isRooted);
                    newUser = 0;
                   Logging.Log(userId);
                }
                else
                {
                    userId = b.UpdateSMUser(user.Provider,user.ID,user.userName, user.email, user.firstName, user.lastName, user.mobileNo, user.countryCode, user.cnic, uid, mp.DeviceID, mp.DeviceToken, mp.Platform, user.isRooted);
                    newUser = 1;
                }
                if (!string.IsNullOrEmpty(userId) && newUser == 0)
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Registration_Success;
                    containerRegister.data.users = new List<SMUser>();
                    //containerRegister.data.users.Add(new Models.User { userId = userId, mobileNo = user.mobileNo });
                    containerRegister.data.users.Add(new Models.SMUser { userId = userId, mobileNo = user.mobileNo, email = user.email, isRooted = user.isRooted });
                    string stringfyObjectSuccess = JsonConvert.SerializeObject(containerRegister);
                    return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectSuccess));
                }
                else if(!string.IsNullOrEmpty(userId) && newUser == 1)
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = "User has been updated";
                    containerRegister.data.users = new List<SMUser>();
                    //containerRegister.data.users.Add(new Models.User { userId = userId, mobileNo = user.mobileNo });
                    containerRegister.data.users.Add(new Models.SMUser { userId = userId, mobileNo = user.mobileNo, email = user.email, isRooted = user.isRooted });
                    string stringfyObjectSuccess = JsonConvert.SerializeObject(containerRegister);
                    return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectSuccess));
                }
                containerRegister.data.Status = 1;
                //containerRegister.data.Message = Constants.Registration_DuplicateEmail;
                containerRegister.data.Message = Constants.Failure;

                string stringfyObjectFailure = JsonConvert.SerializeObject(containerRegister);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectFailure));
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostSignupSM");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;
                string stringfyObjectcatch = JsonConvert.SerializeObject(containerRegister);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectcatch));
            }

            //  return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        [HttpPost]
        [Route("FPrintRegistration")]
        public IActionResult FPrintRegistration([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            FPRegistration user = new FPRegistration();
            user = JsonConvert.DeserializeObject<FPRegistration>(decPayload);

            ContainerFPRegister containerRegister = new ContainerFPRegister();
            containerRegister.data = new ContainerFPRegister.Data();
            Logging.Log(decPayload);
            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                string result = string.Empty;

                result = b.UpdateFPrint(user.UserId);
                Logging.Log(result);
                if (!string.IsNullOrEmpty(result))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Registration_Success;
                    containerRegister.data.FPAuthToken = result;
                }
                else
                {
                    containerRegister.data.Status = 1;
                    //containerRegister.data.Message = Constants.Registration_DuplicateEmail;
                    containerRegister.data.Message = Constants.Failure;
                    containerRegister.data.FPAuthToken = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "FPrintRegistration");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;
            }


            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Register new user and store data to SQL Server Membership table on success user receives OTP on provided mobile number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Registration/checkUserExist
        [HttpPost]
        [Route("checkUserExist")]
        public IActionResult UserValidation([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ValidateUser user = new ValidateUser();
            user = JsonConvert.DeserializeObject<ValidateUser>(decPayload);

            Logging.Log(decPayload);

            ContainerValidateUser containerValidateUser = new ContainerValidateUser();
            containerValidateUser.data = new ContainerValidateUser.Data();
            string message = string.Empty;

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                
                int result = b.CheckUserExistSSO(user);
                Logging.Log(Convert.ToString(result));
                if (result == 1)
                {
                    containerValidateUser.data.UserExist = false;
                    containerValidateUser.data.Message = "User Not Found";
                }
                else if (result == 0)
                {
                    containerValidateUser.data.UserExist = true;
                    containerValidateUser.data.Message = Constants.Registration_Success;
                }
                else
                {
                    containerValidateUser.data.Status = 1;
                    containerValidateUser.data.UserExist = false;
                    containerValidateUser.data.Message = Constants.Failure;
                }
                
                string stringfyObject = JsonConvert.SerializeObject(containerValidateUser);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "UserValidation");

                containerValidateUser.data.Status = 1;
                containerValidateUser.data.Message = Constants.Failure;
                string stringfyObjectcatch = JsonConvert.SerializeObject(containerValidateUser);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectcatch));
            }
        }

        /// <summary>
        /// Register new user and store data to SQL Server Membership table on success user receives OTP on provided mobile number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Registration/VerifyNumber
        [HttpPost]
        [Route("GetEmailsByCellNumber")]
        public IActionResult GetEmailsByCellNumber([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            VerifiMobileNumber user = new VerifiMobileNumber();
            user = JsonConvert.DeserializeObject<VerifiMobileNumber>(decPayload);

            Logging.Log(decPayload);

            ContainerVerifiMobileNumber containerValidateUser = new ContainerVerifiMobileNumber();
            containerValidateUser.data = new ContainerVerifiMobileNumber.Data();
            string message = string.Empty;

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                //List<VerifyMobile> users = b.VerifyNumber_GetEmails(user.MobileNo);
                containerValidateUser.data.Emails = b.VerifyNumber_GetEmails(user.MobileNo,user.CountryCode);
                containerValidateUser.data.Count = containerValidateUser.data.Emails.Count;
                // Logging.Log(Convert.ToString(result));
                if (containerValidateUser.data.Count >= 0)
                {
                    containerValidateUser.data.IsUserExist = b.VerifyNumber_UserExist(user.MobileNo, user.CountryCode);
                    containerValidateUser.data.Status = 0;
                    containerValidateUser.data.Message = Constants.Registration_Success;
                }
                else
                {
                    containerValidateUser.data.IsUserExist = false;
                    containerValidateUser.data.Status = 1;
                    containerValidateUser.data.Message = Constants.Failure;
                    containerValidateUser.data.Emails = null;
                }

                string stringfyObject = JsonConvert.SerializeObject(containerValidateUser);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
                //return Ok((stringfyObject));
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GetEmailsByCellNumber");

                containerValidateUser.data.Status = 1;
                containerValidateUser.data.Message = Constants.Failure;
                string stringfyObjectcatch = JsonConvert.SerializeObject(containerValidateUser);
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObjectcatch));
            }
        }

        /// <summary>
        /// Register new user and store data to SQL Server Membership table on success user receives OTP on provided mobile number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Registration/PostSignup
        [HttpPost]
        [Route("VerifyEmail")]
        public IActionResult VerifyEmail([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            VerifyEmail user = new VerifyEmail();
            user = JsonConvert.DeserializeObject<VerifyEmail>(decPayload);

            Logging.Log(decPayload);

            ContainerVerifyEmail containerRegister = new ContainerVerifyEmail();
            containerRegister.data = new ContainerVerifyEmail.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                
                string userId = b.VerifyEmail(user.Email);
                Logging.Log(userId);
                if(string.IsNullOrEmpty(userId))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Registration_Success;
                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_DuplicateEmail;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "VerifyEmail");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;
            }

            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //  return Ok(stringfyObject);
        }

        /// <summary>
        /// This service is responsible for verifying the receives OTP with newly registered user on success it will mark user to IsApproved = 1
        /// </summary>
        /// <param name="verifyOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/GenerateOTP
        [HttpPost]
        [Route("GenerateOTP")]
        public IActionResult GenerateOTP([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            OTP OTP = new OTP();
            OTP = JsonConvert.DeserializeObject<OTP>(decPayload);

            ContainerOTPLogin containerRegister = new ContainerOTPLogin();
            containerRegister.data = new ContainerOTPLogin.Data();
            //containerRegister.data.users = new List<User>();


            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                string UserId = string.Empty;
                //int result = -1;
                if (!string.IsNullOrEmpty(OTP.MobileNo) && !string.IsNullOrEmpty(OTP.countryCode))
                {
                    UserId = b.GenerateOTPMobile(OTP);
                }
                if (!string.IsNullOrEmpty(UserId))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;
                    containerRegister.data.UserId = UserId;
                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Failure;
                    containerRegister.data.UserId = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "GenerateOTP");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;

                //return Ok(containerRegister);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service is responsible for verifying the receives OTP with newly registered user on success it will mark user to IsApproved = 1
        /// </summary>
        /// <param name="verifyOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/PostActivateYourAccount
        //[HttpPost]
        //[Route("GenerateOTPSignUp")]
        //public IActionResult GenerateOTPSignUp([FromBody]EncryptedString dataString)
        //{
        //    string decPayload = customEncryption.DecryptStringAES(dataString.d);
        //    OTP OTP = new OTP();
        //    OTP = JsonConvert.DeserializeObject<OTP>(decPayload);

        //    ContainerOTPSignUp containerRegister = new ContainerOTPSignUp();
        //    containerRegister.data = new ContainerOTPSignUp.Data();
        //    //containerRegister.data.users = new List<User>();
        //    UserCheck user = new UserCheck();
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return Ok(ModelState);
        //        if (!string.IsNullOrEmpty(OTP.MobileNo))
        //        {
        //            user = b.GenerateOTPMobileSignUp(OTP);
        //        }
        //        if (user.returnValue == 0)
        //        {
        //            containerRegister.data.Status = 0;
        //            containerRegister.data.Message = Constants.Success;
        //            containerRegister.data.Email = user.email;
        //            containerRegister.data.CNIC = user.CNIC;
        //        }
        //        else if (user.returnValue == 1)
        //        {
        //            containerRegister.data.Status = 0;
        //            containerRegister.data.Message = Constants.Success;

        //            //return Ok(containerRegister);
        //        }
        //        else
        //        {
        //            containerRegister.data.Status = 1;
        //            containerRegister.data.Message = Constants.Failure;

        //            //return Ok(containerRegister);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandling.LogError(ex, "GenerateOTPSignUp");

        //        containerRegister.data.Status = 1;
        //        containerRegister.data.Message = Constants.Failure;

        //        //return Ok(containerRegister);
        //    }
        //    string stringfyObject = JsonConvert.SerializeObject(containerRegister);
        //    return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        //}

        /// <summary>
        /// This service is responsible for verifying the receives OTP with newly registered user on success it will mark user to IsApproved = 1
        /// </summary>
        /// <param name="verifyOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/PostActivateYourAccount
        [HttpPost]
        [Route("PostActivateYourAccount")]
        public IActionResult VerifyOTP([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            VerifyOTP verifyOTP = new VerifyOTP();
            verifyOTP = JsonConvert.DeserializeObject<VerifyOTP>(decPayload);

            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();
            containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (b.IsMobileRequest(verifyOTP) && !string.IsNullOrEmpty(verifyOTP.DeviceID))
                {
                    if (!b.AuthenticateMobileRequest(verifyOTP))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                    }
                }
                Logging.Log(verifyOTP.userId + "-:-" + verifyOTP.PIN);
                if (b.VerifyOTP(verifyOTP.userId, verifyOTP.PIN))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;

                    if (b.IsMobileRequest(verifyOTP))
                    {
                        //containerRegister.data.users.Add(b.GetUserDetailsByUserId(verifyOTP.userId));
                        containerRegister.data.users.Add(b.LoginVIAUserId(verifyOTP));
                        containerRegister.data.users[0].userCode = string.Empty;
                        containerRegister.data.users[0].userCodeEmail = string.Empty;
                    }

                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_OTPIncorrect;

                    //return Ok(containerRegister);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostActivateYourAccount");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;

                //return Ok(containerRegister);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service is responsible for verifying the receives OTP with newly registered user on success it will mark user to IsApproved = 1
        /// </summary>
        /// <param name="verifyOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/VerifyOTP
        [HttpPost]
        [Route("VerifyOTP")]
        public IActionResult VerifyOTPLogin([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            VerifyOTP verifyOTP = new VerifyOTP();
            verifyOTP = JsonConvert.DeserializeObject<VerifyOTP>(decPayload);

            ContainerLoginMobileNo containerRegister = new ContainerLoginMobileNo();
            containerRegister.data = new ContainerLoginMobileNo.Data();
            //containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (b.IsMobileRequest(verifyOTP) && !string.IsNullOrEmpty(verifyOTP.DeviceID))
                {
                    Logging.Log("MobileUser");
                    if (!b.AuthenticateMobileRequest(verifyOTP,verifyOTP.userId))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                    }
                }

                if (b.VerifyOTPLogin(verifyOTP.userId, verifyOTP.PIN))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;
                    containerRegister.data.user = b.LoginVIAUserId(verifyOTP);

                    //if (b.IsMobileRequest(verifyOTP))
                    //{
                    //    containerRegister.data.users.Add(b.GetUserDetailsByUserId(verifyOTP.userId));
                    //    containerRegister.data.users[0].userCode = string.Empty;
                    //    containerRegister.data.users[0].userCodeEmail = string.Empty;
                    //}

                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_OTPIncorrect;

                    //return Ok(containerRegister);
                }


                //if (b.VerifyOTP_ForSignUp(verifyOTP.MobileNo, verifyOTP.PIN))
                //{
                //    containerRegister.data.Status = 0;
                //    containerRegister.data.Message = Constants.Success;                    
                //}
                //else
                //{
                //    containerRegister.data.Status = 1;
                //    containerRegister.data.Message = Constants.Registration_OTPIncorrect;
                //    //return Ok(containerRegister);
                //}
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "VerifyOTPLogin");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;

                //return Ok(containerRegister);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
            //return Ok(stringfyObject);
        }


        /// <summary>
        /// This service is responsible for verifying the receives OTP On Mobile with newly registered user on success it will mark user to IsApproved = 1
        /// </summary>
        /// <param name="verifyOTPOnMobile"></param>
        /// <returns></returns>
        // POST: api/Registration/PostActivateYourAccount
        [HttpPost]
        [Route("PostActivateYourAccountOnMobile")]
        public IActionResult VerifyOTPMobile([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            VerifyOTP verifyOTP = new VerifyOTP();
            verifyOTP = JsonConvert.DeserializeObject<VerifyOTP>(decPayload);

            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();
            containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerRegister.data.Status = 1;
                //    containerRegister.data.Message = Constants.Registration_InvalidServiceCredential;

                //    return Ok(containerRegister);
                //}

                if (b.IsMobileRequest(verifyOTP) && !string.IsNullOrEmpty(verifyOTP.DeviceID))
                {
                    if (!b.AuthenticateMobileRequest(verifyOTP))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                        //return Ok(containerRegister);
                    }
                }

                //if (b.VerifyOTP(verifyOTP.userId, verifyOTP.PIN))
                if (b.VerifyOTPOnMobile(verifyOTP.userId, verifyOTP.PIN))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;
                    containerRegister.data.users.Add(b.LoginVIAUserId(verifyOTP));
                    containerRegister.data.users[0].userCode = string.Empty;
                    containerRegister.data.users[0].userCodeEmail = string.Empty;
                    //if (b.IsMobileRequest(verifyOTP))
                    //{
                        
                    //    //containerRegister.data.users.Add(b.GetUserDetailsByUserId(verifyOTP.userId));
                        
                    //}

                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_OTPIncorrect;

                    //return Ok(containerRegister);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostActivateYourAccountOnMobile");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;

                //return Ok(containerRegister);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service is responsible for verifying the receives OTP with newly registered user on success it will mark user to IsApproved = 1
        /// </summary>
        /// <param name="verifyOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/PostLoginSignUp
        [HttpPost]
        [Route("PostLoginSignUp")]
        public IActionResult LoginFromSignUp([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            VerifyOTP verifyOTP = new VerifyOTP();
            verifyOTP = JsonConvert.DeserializeObject<VerifyOTP>(decPayload);

            ContainerLoginMobileNo containerRegister = new ContainerLoginMobileNo();
            containerRegister.data = new ContainerLoginMobileNo.Data();
            //containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                
                if (!string.IsNullOrEmpty(verifyOTP.userId))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;
                    containerRegister.data.user = b.LoginVIAUserId(verifyOTP);
                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_OTPIncorrect;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PstLoginSU");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;
            }
            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }



        /// <summary>
        /// This service method is responsible for re sending OTP to registered mobile number
        /// </summary>
        /// <param name="resendOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/PostResendOTP
        [HttpPost]
        [Route("PostResendOTP")]
        public IActionResult ResendOTP([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ResendOTP resendOTP = new ResendOTP();
            resendOTP = JsonConvert.DeserializeObject<ResendOTP>(decPayload);

            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();
            containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerRegister.data.Status = 1;
                //    containerRegister.data.Message = Constants.Registration_InvalidServiceCredential;

                //    return Ok(containerRegister);
                //}

                if (b.IsMobileRequest(resendOTP) && !string.IsNullOrEmpty(resendOTP.DeviceID))
                {

                    if (!b.AuthenticateMobileRequest(resendOTP))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                        //  return Ok(containerRegister);
                    }
                }

                string strResult = b.ResendOTPnUpdateUserCode(resendOTP.userId);

                containerRegister.data.Status = 0;
                containerRegister.data.Message = Constants.Success;
                containerRegister.data.users.Add(new Models.User { userId = strResult });

                //      return Ok(containerRegister);
            }
            catch (Exception ex)
            {
                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Registration_Validation_RequiredUserID;

                ExceptionHandling.LogError(ex, "PostResendOTP");
                //   return Ok(containerRegister);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service method is responsible for re sending OTP to registered mobile number also this will send notification to provided mobile number
        /// </summary>
        /// <param name="resendOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/PostResendOTPAndUpdateMobile
        [HttpPost]
        [Route("PostResendOTPAndUpdateMobile")]
        public IActionResult ResendOTPAndUpdateMobile([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ResendOTP resendOTP = new ResendOTP();
            resendOTP = JsonConvert.DeserializeObject<ResendOTP>(decPayload);

            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();
            containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);
                if (b.IsMobileRequest(resendOTP) && !string.IsNullOrEmpty(resendOTP.DeviceID))
                {

                    if (!b.AuthenticateMobileRequest(resendOTP))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                        //return Ok(containerRegister);
                    }
                }
                string strResult = string.Empty;
                //if (b.IsEmailExistOneTimePassword(resendOTP.Email))
                //{
                //    containerRegister.data.Status = 1;
                //    strResult = "Given OTP Email Id is already exists";
                //}
                //else
                //{
                //string strResult = b.ResendOTPnUpdateUserCodenMobile(resendOTP.userId, resendOTP.Mobile);
                strResult = b.ResendOTPnUpdateUserCodenMobile(resendOTP.userId, resendOTP.Mobile, resendOTP.countryCode);
                //}
                Guid guid;

                if (Guid.TryParse(strResult, out guid))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;
                    containerRegister.data.users.Add(new Models.User { userId = strResult });

                    try
                    {
                        Logging.Log(string.Format("- IP: {0} -> UserId: {1} -> Mobile: {2}", Convert.ToString(HttpContext.Connection.RemoteIpAddress), resendOTP.userId, resendOTP.Mobile,resendOTP.countryCode));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "ResendOTPAndUpdateMobile - Logging");
                    }
                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = strResult;
                }

                //return Ok(containerRegister);
            }
            catch (Exception ex)
            {
                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Registration_Validation_RequiredUserID;

                ExceptionHandling.LogError(ex, "ResendOTPAndUpdateMobile");
                //return Ok(containerRegister);

            }

            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));

        }

        /// <summary>
        /// This service method is responsible for re sending OTP to registered mobile number also this will send notification to provided mobile number
        /// </summary>
        /// <param name="resendOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/PostResendOTPnMobileAndUpdateMobile
        [HttpPost]
        [Route("PostResendOTPOnMobileAndUpdateMobile")]
        public IActionResult ResendOTPOnMobileAndUpdateMobile([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ResendOTP resendOTP = new ResendOTP();
            resendOTP = JsonConvert.DeserializeObject<ResendOTP>(decPayload);

            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();
            containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (b.IsMobileRequest(resendOTP) && !string.IsNullOrEmpty(resendOTP.DeviceID))
                {

                    if (!b.AuthenticateMobileRequest(resendOTP))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                        //return Ok(containerRegister);
                    }
                }
                string strResult = string.Empty;

                strResult = b.ResendOTPOnMobilenUpdateUserCodenMobile(resendOTP.userId, resendOTP.Mobile, resendOTP.countryCode);
                //strResult = b.ResendOTPOnMobilenUpdateUserCodenMobile(resendOTP.userId, resendOTP.Mobile, resendOTP.Email);
                Guid guid;

                if (Guid.TryParse(strResult, out guid))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;
                    containerRegister.data.users.Add(new Models.User { userId = strResult });

                    try
                    {
                        Logging.Log(string.Format("- IP: {0} -> UserId: {1} -> Mobile: {2}", Convert.ToString(HttpContext.Connection.RemoteIpAddress), resendOTP.userId, resendOTP.Mobile));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandling.LogError(ex, "ResendOTPAndUpdateMobile - Logging");
                    }
                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = strResult;
                }

                //return Ok(containerRegister);
            }
            catch (Exception ex)
            {
                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Registration_Validation_RequiredUserID;

                ExceptionHandling.LogError(ex, "ResendOTPAndUpdateMobile");
                //return Ok(containerRegister);

            }

            string stringfyObject = JsonConvert.SerializeObject(containerRegister);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));

        }

        /// <summary>
        /// This service is responsible for sending forgot password email to provided email address, if email address is not found in table return failure
        /// </summary>
        /// <param name="forgotPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostForgotPassword")]
        public IActionResult ForgotPassword([FromBody]EncryptedString dataString)
        {

            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ForgotPassword forgotPassword = new ForgotPassword();
            forgotPassword = JsonConvert.DeserializeObject<ForgotPassword>(decPayload);

            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerGeneric.data.Status = 1;
                //    containerGeneric.data.Message = Constants.Registration_InvalidServiceCredential;

                //    return Ok(containerGeneric);
                //}

                if (b.IsEmailExist(forgotPassword.Email))
                {

                    if (!string.IsNullOrEmpty(forgotPassword.Platform) && b.IsMobileRequest(forgotPassword))
                    {
                        string strException = string.Empty;

                        if (b.SendForgotPasswordEmailMobile(forgotPassword.Email, out strException))
                        {
                            containerGeneric.data.Status = 0;
                            containerGeneric.data.Message = Constants.Success; 
                        }
                        else
                        {
                            containerGeneric.data.Status = 1;
                            containerGeneric.data.Message = Constants.Email_SMTP_Error;
                        }
                    }
                    else
                    {
                        containerGeneric.data.Status = 1;
                        containerGeneric.data.Message = Constants.Failure;
                    }

                    //   return Ok(containerGeneric);
                }
                else
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.Registration_EmailNotFound;

                    //  return Ok(containerGeneric);
                }
            }
            catch (Exception ex)
            {
                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;

                ExceptionHandling.LogError(ex, "ForgotPassword");

                //   return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        [HttpPost]
        [Route("GetForgotPasswordString")]
        public IActionResult GetForgotPasswordString([FromBody] EncryptedString dataString)
        {
            var result = b.GetForgotPasswordDetail(dataString.d);
            string stringfyObject = JsonConvert.SerializeObject(result);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service is responsible for reset password with new provided password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ResetPassword resetPassword = new ResetPassword();
            resetPassword = JsonConvert.DeserializeObject<ResetPassword>(decPayload);


            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                string userId= b.VerifyEmail(resetPassword.Email);
                
                if (!string.IsNullOrEmpty(resetPassword.UserPin)  && b.VerifyOTP(userId, resetPassword.UserPin) && b.SetPassword(resetPassword.Email, resetPassword.NewPassword))
                {
                    containerGeneric.data.Status = 0;
                    containerGeneric.data.Message = true.ToString();
                }
                else
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = false.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "ResetPassword");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;

                //  return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service is responsible for reset password with new provided password
        /// </summary>
        /// <param name="PostResetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostResetPassword")]
        public IActionResult PostResetPassword([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            ResetPasswordAdmin resetPassword = new ResetPasswordAdmin();
            resetPassword = JsonConvert.DeserializeObject<ResetPasswordAdmin>(decPayload);

            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (b.ResetPassword(resetPassword))
                {
                    containerGeneric.data.Status = 0;
                    containerGeneric.data.Message = "Success";
                }
                else
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = "Failed";
                }

                // return Ok(containerGeneric);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "ResetPasswordForAdmin");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;

                //  return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// Not using used for backup
        /// </summary>
        /// <param name="userInbox"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddInboxItem")]
        public IActionResult AddInboxItem([FromBody]EncryptedString dataString)
        {
            string decPayload = customEncryption.DecryptStringAES(dataString.d);
            UserInbox userInbox = new UserInbox();
            userInbox = JsonConvert.DeserializeObject<UserInbox>(decPayload);

            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerGeneric.data.Status = 1;
                //    containerGeneric.data.Message = Constants.Registration_InvalidServiceCredential;
                //    return Ok(containerGeneric);
                //}

                containerGeneric.data.Status = 0;
                containerGeneric.data.Message = Convert.ToString(b.AddInboxItemAndSendPushNotification(userInbox));
                //
                //  return Ok(containerGeneric);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "AddInboxItem");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;

                //   return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// This service is responsible for sending push notification to mobile using device token and add same item to user inbox in SQL server
        /// </summary>
        /// <param name="pushNotification"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostPushNotificationAndAddToInbox")]
        public IActionResult PostPushNotificationAndAddToInbox([FromBody]PushNotification pushNotification)
        {
            ContainerPushNotification containerPushNotification = new ContainerPushNotification();
            containerPushNotification.IsPushNotificationAddedToUserInbox = false;
            containerPushNotification.IsPushNotificationSent = false;
            containerPushNotification.Status = "1";

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!b.AuthenticateHeader(Request))
                {
                    containerPushNotification.PushNotificationException = Constants.Registration_InvalidServiceCredential;
                    containerPushNotification.UserInboxException = Constants.Registration_InvalidServiceCredential;
                }
                else
                {
                    if (!string.IsNullOrEmpty(pushNotification.Mobile))
                    {
                        Logging.Log("Sending Push : "+pushNotification.Mobile);
                        containerPushNotification = b.PostPushNotificationAndAddToInbox(pushNotification);
                    }
                    else
                    {
                        containerPushNotification = b.PostBulkPushNotificationAndAddToInbox(pushNotification);
                    }

                }


            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostPushNotificationAndAddToInbox");

                containerPushNotification.PushNotificationException = ex.Message;
                containerPushNotification.UserInboxException = ex.Message;
            }

            return Ok(containerPushNotification);
        }
        //[HttpPost]
        //[Route("PostPushNotificationAndAddToInbox")]
        //public IActionResult PostPushNotificationAndAddToInbox([FromBody]EncryptedString dataString)
        //{
        //    string decPayload = customEncryption.DecryptStringAES(dataString.d);
        //    PushNotification pushNotification = new PushNotification();
        //    pushNotification = JsonConvert.DeserializeObject<PushNotification>(decPayload);


        //    ContainerPushNotification containerPushNotification = new ContainerPushNotification();
        //    containerPushNotification.IsPushNotificationAddedToUserInbox = false;
        //    containerPushNotification.IsPushNotificationSent = false;
        //    containerPushNotification.Status = "1";

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        if (!b.AuthenticateHeader(Request))
        //        {
        //            containerPushNotification.PushNotificationException = Constants.Registration_InvalidServiceCredential;
        //            containerPushNotification.UserInboxException = Constants.Registration_InvalidServiceCredential;
        //        }
        //        else
        //        {
        //            containerPushNotification = b.PostPushNotificationAndAddToInbox(pushNotification);

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandling.LogError(ex, "PostPushNotificationAndAddToInbox");

        //        containerPushNotification.PushNotificationException = ex.Message;
        //        containerPushNotification.UserInboxException = ex.Message;
        //    }

        //    string stringfyObject = JsonConvert.SerializeObject(containerPushNotification);
        //    return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        //    //return Ok(containerPushNotification);
        //}

        [HttpGet()]
        [Route("VerifyTokenValidity")]
        public IActionResult VerifyTokenValidity([FromQuery]EncryptedString dataString)
        {
            string token = customEncryption.DecryptStringAES(dataString.d);

            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //if (!b.AuthenticateHeader(Request))
                //{
                //    containerGeneric.data.Status = 1;
                //    containerGeneric.data.Message = Constants.Registration_InvalidServiceCredential;

                //    return Ok(containerGeneric);
                //}

                containerGeneric.data.Status = 0;
                containerGeneric.data.Message = Convert.ToString(b.GetUserIdByToken(token));

                //return Ok(containerGeneric);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "VerifyTokenValidity");
                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;

                //return Ok(containerGeneric);
            }
            string stringfyObject = JsonConvert.SerializeObject(containerGeneric);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        /// <summary>
        /// RSA - This service authenticate user with provided email address and password 
        /// </summary>
        /// <param name="login"></param>
        /// <returns>On success return User Object, Else return failure</returns>
        // POST: api/Registration/PostLoginMobile
        [HttpPost]
        [Route("PostLoginMobile")]
        public IActionResult LoginMobile([FromBody]EncryptedString login)
        {
            ContainerLogin containerLogin = new ContainerLogin();
            containerLogin.data = new ContainerLogin.Data
            {
                users = new List<User>()
            };

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                User user = b.LoginMobileRSA(login);
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

                            Logging.Log(string.Format("LoginMobile RSA -> IP: {0}", Convert.ToString(HttpContext.Connection.RemoteIpAddress)));
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
                ExceptionHandling.LogError(ex, "Login - RSA");

                containerLogin.data.Status = 1;
                containerLogin.data.Message = Constants.Exception_GeneralMessage;
                string sringfyObjectx = JsonConvert.SerializeObject(containerLogin);
                return Ok(containerLogin);
            }

            string stringfyObject = JsonConvert.SerializeObject(containerLogin);
            return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
        }

        [HttpGet()]
        [Route("ProcessComplaintPushNotification")]
        public IActionResult ProcessComplaintPushNotification()
        {
            string ipAddress = string.Empty;

            ContainerTechnicalPushNotification containerPushNotification = new ContainerTechnicalPushNotification
            {
                countException = 0,
                countTotalComplaints = 0,
                countUniqueComplaints = 0
            };

            try
            {
                if (!b.AuthenticateHeader(Request))
                {
                    return BadRequest();
                }
                else
                {
                    ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    Logging.Log("ProcessComplaintPushNotification Started - " + DateTime.Now + " IP: " + ipAddress);
                    containerPushNotification = b.PostTechnicalPushNotification();
                    Logging.Log("ProcessComplaintPushNotification Ended Summary -> countTotalComplaints: " + containerPushNotification.countTotalComplaints + " - countUniqueComplaints: " + containerPushNotification.countUniqueComplaints + " - countException: " + containerPushNotification.countException);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "ProcessComplaintPushNotification");
            }

            return Ok(containerPushNotification);
        }


        [HttpPost]
        [Route("Notifiy")]
        public string Notify()
        {

            return b.NotificationList();

        }


        [HttpPost]
        [Route("ImportFile")]
        public IActionResult ImportFile([FromForm]IFormFile file)
        {
            int HistoryId =0;
            try
            {
                string stringfyObject = string.Empty;
                List<PushNotification> fetchSetFileDatas = SetNotificationDataFromFile(file);
                if (fetchSetFileDatas.Count == 0)
                {
                    stringfyObject = JsonConvert.SerializeObject("Not valid format or file have empty data");
                    return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
                }

                string filePath= b.UploadToBlob(file);
                PushHistory pushHistory = new PushHistory();
                pushHistory.FileName = file.FileName;
                pushHistory.Status = "In-Process";
                pushHistory.Url = filePath;
                TimeZoneInfo pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);
                pushHistory.SubmitDate = pakistanTime;
                HistoryId = AddPushHistory(pushHistory);


                TaskFactory backgroundTask = new TaskFactory();
                backgroundTask.StartNew(() =>
                {
                    for (int i = 0; i < fetchSetFileDatas.Count; i++)
                    {
                        try
                        {
                            if (fetchSetFileDatas[i].format == "PhoneNumber")
                            {
                                b.PostPushNotificationAndAddToInbox(fetchSetFileDatas[i]);
                            }
                            else
                            {
                                b.PostBulkPushNotificationAndAddToInbox(fetchSetFileDatas[i]);
                            }
                            Thread.Sleep(3000);
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }

                    TimeZoneInfo pakistanTimeZones = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime completedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);
                    UpdatePushHistory(HistoryId,"Completed", completedTime);

                });
                


                stringfyObject = JsonConvert.SerializeObject("true");
                return Ok(customEncryption.EncryptStringToStringAES(stringfyObject));
                
            }
            catch (Exception ex)
            {
                TimeZoneInfo pakistanTimeZones = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime completedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZones);
                UpdatePushHistory(HistoryId, "Completed", completedTime);
                return BadRequest("Message: " + ex.Message  +" Inner Exception: " + ex.InnerException + "Stack Trace : " + ex.StackTrace.ToString());
            }
        }
        private List<PushNotification>  SetNotificationDataFromFile(IFormFile file)
        {
            List<PushNotification> pushNotifications = new List<PushNotification>();
            ISheet sheet;
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    ms.Position = 0;
                    sheet = GetExcelFileType(ms, Path.GetExtension(file.FileName).ToLower());
                }
                string formatName = sheet.GetRow(sheet.FirstRowNum).Cells[1].ToString();
                if (formatName == "PhoneNumber")
                {
                    for (int i = (sheet.FirstRowNum) + 1; i <= sheet.LastRowNum; i++)
                    {
                        try
                        {
                           
                            IRow row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            if (row.Cells.All(d => d.CellType == CellType.Blank))
                            {
                                continue;
                            }
                            PushNotification push = new PushNotification();
                            push.format = formatName;
                            push.Message = row.Cells[0].ToString();
                            push.Mobile = row.Cells[1].ToString();
                            push.DateTime = DateTime.Now;
                            push.Type = "K-Electric";
                            pushNotifications.Add(push);
                            //ContainerPushNotification containerPushNotification = new ContainerPushNotification();
                            //containerPushNotification = b.PostPushNotificationAndAddToInbox(push);

                        }
                        catch (Exception ex)
                        {
                            Logging.Log("File Import Row Missed Due To Not In Format");
                        }
                    }
                }
                else if (formatName == "DtsID")
                {
                    for (int i = (sheet.FirstRowNum) + 1; i <= sheet.LastRowNum; i++)
                    {
                        try
                        {
                            PushNotification push = new PushNotification();
                            IRow row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            if (row.Cells.All(d => d.CellType == CellType.Blank))
                            {
                                continue;
                            }
                            push.format = formatName;
                            push.Message = row.Cells[0].ToString();
                            push.DtsId = row.Cells[1].ToString();
                            push.DateTime = DateTime.Now;
                            push.Type = "K-Electric";
                            pushNotifications.Add(push);
                            //ContainerPushNotification containerPushNotification = new ContainerPushNotification();
                            //containerPushNotification = b.PostBulkPushNotificationAndAddToInbox(push);

                        }
                        catch (Exception ex)
                        {
                            Logging.Log("File Import Row Missed Due To Not In Format");
                        }
                    }
                }
                else if (formatName == "FeederID")
                {
                    for (int i = (sheet.FirstRowNum) + 1; i <= sheet.LastRowNum; i++)
                    {

                        try
                        {
                            PushNotification push = new PushNotification();
                            IRow row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            if (row.Cells.All(d => d.CellType == CellType.Blank))
                            {
                                continue;
                            }
                            push.format = formatName;
                            push.Message = row.Cells[0].ToString();
                            push.FeederId = row.Cells[1].ToString();
                            push.DateTime = DateTime.Now;
                            push.Type = "K-Electric";
                            pushNotifications.Add(push);
                         // ContainerPushNotification containerPushNotification = new ContainerPushNotification();
                         // containerPushNotification = b.PostBulkPushNotificationAndAddToInbox(push);

                        }
                        catch (Exception ex)
                        {
                            Logging.Log("File Import Row Missed Due To Not In Format");
                        }
                    }
                }
                else if (formatName == "AccountNo")
                {
                    for (int i = (sheet.FirstRowNum) + 1; i <= sheet.LastRowNum; i++)
                    {

                        try
                        {
                            PushNotification push = new PushNotification();
                            IRow row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            if (row.Cells.All(d => d.CellType == CellType.Blank))
                            {
                                continue;
                            }
                            push.format = formatName;
                            push.Message = row.Cells[0].ToString();
                            push.AccountNo = row.Cells[1].ToString();
                            push.DateTime = DateTime.Now;
                            push.Type = "K-Electric";
                            pushNotifications.Add(push);
                            //ContainerPushNotification containerPushNotification = new ContainerPushNotification();
                            //containerPushNotification = b.PostBulkPushNotificationAndAddToInbox(push);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log("File Import Row Missed Due To Not In Format");
                        }
                    }
                }
            }

            return pushNotifications;
        }
        private dynamic GetExcelFileType(MemoryStream fs, string fileExtension = "ssss")
        {
            if (fileExtension == ".xls")
            {
                return new HSSFWorkbook(fs).GetSheetAt(0);
            }
            else
            {
                return new XSSFWorkbook(fs).GetSheetAt(0);
                
            }
        }

        private int AddPushHistory(PushHistory pushHistory)
        {
           return  b.AddPushHistoryDetails(pushHistory);
        }

        private void UpdatePushHistory(int Id, string status, DateTime completedDate)
        {
            b.UpdatePushHistoryDetails(Id, status,completedDate);
        }


        [HttpPost]
        [Route("EmailDecrypt")]
        public string EmailDecrypt()
        {
            var res= b.DecryptEmail("sGd7MXFrhskqhPLkP9mc9g==", true);
                                     
            return res;
        }

        [HttpGet]
        [Route("GetAllPushHistory")]
        public string GetAllPushHistory()
        {
            string  stringfyObject = JsonConvert.SerializeObject(b.GetAllPushHistory());
            return customEncryption.EncryptStringToStringAES(stringfyObject);
            
        }

        [HttpGet]
        [Route("GetAllConsumerProfileData")]
        public string GetAllConsumerProfileData()
        {
            string stringfyObject = JsonConvert.SerializeObject(b.GetConsumerDataUrl());
            return customEncryption.EncryptStringToStringAES(stringfyObject);

        }

    }
}