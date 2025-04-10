using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KE.Service.BAL;
using KE.Service.Helper;
using KE.Service.Models;
using KE.Service.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KE.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Reg")]
    public class RegController : Controller
    {
        BAL.BAL b = null;
        KEBAL kEBAL = null;

        /// <summary>
        /// Initialize variables
        /// </summary>
        /// <param name="iConfiguration"></param>
        public RegController(IConfiguration iConfiguration)
        {
            b = new BAL.BAL(iConfiguration);
            kEBAL = new KEBAL(iConfiguration);
        }

        /// <summary>
        /// Return test service response
        /// </summary>
        /// <returns></returns>
        // GET: api/Registration
        [HttpGet]
        public string Get()
        {
            return "Service is running!";
        }

        /// <summary>
        /// Register a new user
        /// On Successful receives OTP on provide mobile number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Registration/PostSignup
        [HttpPost]
        [Route("PostSignup")]
        public IActionResult Register([FromBody]User user)
        {
            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateHeader(Request))
                {

                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_InvalidServiceCredential;

                    return Ok(containerRegister);
                }

                string userId = b.RegisterUser(user.userName, user.Password, user.Email, user.FullName, user.MobileNo, user.CNIC, user.DeviceID, user.DeviceToken, user.Platform);

                if (!string.IsNullOrEmpty(userId))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Registration_Success;
                    containerRegister.data.users = new List<User>();
                    containerRegister.data.users.Add(new Models.User { userId = userId });

                    return Ok(containerRegister);
                }

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Registration_DuplicateEmail;

                return Ok(containerRegister);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostSignup");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;

                return Ok(containerRegister);
            }
        }

        /// <summary>
        /// This service verifies the recieves OTP 
        /// </summary>
        /// <param name="verifyOTP"></param>
        /// <returns>On Mobile Case response is User object else response is true/false</returns>
        // POST: api/Registration/PostActivateYourAccount
        [HttpPost]
        [Route("PostActivateYourAccount")]
        public IActionResult VerifyOTP([FromBody]VerifyOTP verifyOTP)
        {
            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();
            containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateHeader(Request))
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_InvalidServiceCredential;

                    return Ok(containerRegister);
                }

                if (b.IsMobileRequest(verifyOTP) && !string.IsNullOrEmpty(verifyOTP.DeviceID))
                {
                    if (!b.AuthenticateMobileRequest(verifyOTP))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                        return Ok(containerRegister);
                    }
                }

                if (b.VerifyOTP(verifyOTP.userId, verifyOTP.PIN))
                {
                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;

                    if (b.IsMobileRequest(verifyOTP))
                    {
                        containerRegister.data.users.Add(b.GetUserDetailsByUserId(verifyOTP.userId));
                        containerRegister.data.users[0].userCode = string.Empty;
                    }

                    return Ok(containerRegister);
                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_OTPIncorrect;

                    return Ok(containerRegister);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostActivateYourAccount");

                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Failure;

                return Ok(containerRegister);
            }
        }

        /// <summary>
        /// If user didn't receives the OTP, This service will send OTP to registered email address
        /// </summary>
        /// <param name="resendOTP"></param>
        /// <returns></returns>
        // POST: api/Registration/PostResendOTP
        [HttpPost]
        [Route("PostResendOTP")]
        public IActionResult ResendOTP([FromBody]ResendOTP resendOTP)
        {
            ContainerRegister containerRegister = new ContainerRegister();
            containerRegister.data = new ContainerRegister.Data();
            containerRegister.data.users = new List<User>();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateHeader(Request))
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Registration_InvalidServiceCredential;

                    return Ok(containerRegister);
                }

                if (b.IsMobileRequest(resendOTP) && !string.IsNullOrEmpty(resendOTP.DeviceID))
                {
                    //if (string.IsNullOrEmpty(resendOTP.DeviceID))
                    //{
                    //    containerRegister.data.Status = 1;
                    //    containerRegister.data.Message = Constants.Registration_RFV_DeviceId;

                    //    return Ok(containerRegister);
                    //}

                    if (!b.AuthenticateMobileRequest(resendOTP))
                    {
                        containerRegister.data.Status = 1;
                        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                        return Ok(containerRegister);
                    }
                }

                //if (string.IsNullOrEmpty(resendOTP.userId))
                //{
                //    containerRegister.data.Status = 1;
                //    containerRegister.data.Message = Constants.Registration_Validation_RequiredUserID;

                //    return Ok(containerRegister);
                //}

                User user = b.GetUserDetailsByUserId(resendOTP.userId);

                if (user != null)
                {
                    //user.userMobile   //for sms sending
                    //user.userCode     //for sms sending

                    containerRegister.data.Status = 0;
                    containerRegister.data.Message = Constants.Success;
                    containerRegister.data.users.Add(new Models.User { userId = user.userId });

                    kEBAL.SendOTPSMS(user.MobileNo, user.userCode, user.userId);



                    //if (b.IsMobileRequest(resendOTP))
                    //{
                    //    //containerRegister.data.users.Add(user);
                    //    //containerRegister.data.users[0].userCode = string.Empty;
                    //}

                    return Ok(containerRegister);
                }
                else
                {
                    containerRegister.data.Status = 1;
                    containerRegister.data.Message = Constants.Failure;

                    return Ok(containerRegister);
                }
            }
            catch (Exception ex)
            {
                containerRegister.data.Status = 1;
                containerRegister.data.Message = Constants.Registration_Validation_RequiredUserID;

                ExceptionHandling.LogError(ex, "PostResendOTP");
                return Ok(containerRegister);
            }
        }

        /// <summary>
        /// THis service will send forgot password email to provided email address; if email not found returns not found email message
        /// </summary>
        /// <param name="forgotPassword"></param>
        /// <returns></returns>
        /// // POST: api/Registration/PostForgotPassword
        [HttpPost]
        [Route("PostForgotPassword")]
        public IActionResult ForgotPassword([FromBody]ForgotPassword forgotPassword)
        {
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateHeader(Request))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.Registration_InvalidServiceCredential;

                    return Ok(containerGeneric);
                }

                if (b.IsEmailExist(forgotPassword.Email))
                {

                    //if (b.IsMobileRequest(verifyOTP) && !string.IsNullOrEmpty(verifyOTP.DeviceID))
                    //{
                    //    if (!b.AuthenticateMobileRequest(verifyOTP))
                    //    {
                    //        containerRegister.data.Status = 1;
                    //        containerRegister.data.Message = Constants.DeviceAuthenticateFailed;

                    //        return Ok(containerRegister);
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(forgotPassword.Platform) && b.IsMobileRequest(forgotPassword))   //  forgotPassword.Platform.ToLowerInvariant() == "mobile")
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
                            containerGeneric.data.Message = strException;
                        }

                    }
                    else
                    {
                        containerGeneric.data.Status = 0;
                        containerGeneric.data.Message = Convert.ToString(b.SendEmail(forgotPassword.Subject, forgotPassword.Body, forgotPassword.Email));
                    }

                    return Ok(containerGeneric);
                }
                else
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.Registration_EmailNotFound;

                    return Ok(containerGeneric);
                }
            }
            catch (Exception ex)
            {
                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;

                ExceptionHandling.LogError(ex, "ForgotPassword");

                return Ok(containerGeneric);
            }
        }

        /// <summary>
        /// This service will reset password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        // POST: api/Registration/ResetPassword
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword([FromBody]ResetPassword resetPassword)
        {
            ContainerGeneric containerGeneric = new ContainerGeneric();
            containerGeneric.data = new ContainerGeneric.Data();

            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                if (!b.AuthenticateHeader(Request))
                {
                    containerGeneric.data.Status = 1;
                    containerGeneric.data.Message = Constants.Registration_InvalidServiceCredential;
                    return Ok(containerGeneric);
                }

                //1 - User submit the page by entering new password
                //[aspnet_Membership_SetPasswordCustom]

                containerGeneric.data.Status = 0;
                containerGeneric.data.Message = Convert.ToString(b.SetPassword(resetPassword.Email, resetPassword.NewPassword));

                return Ok(containerGeneric);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "ResetPassword");

                containerGeneric.data.Status = 1;
                containerGeneric.data.Message = Constants.Failure;

                return Ok(containerGeneric);
            }
        }

        /// <summary>
        /// This service will add push notification on mobile and on user inbox with provided mobile number
        /// </summary>
        /// <param name="pushNotification"></param>
        /// <returns></returns>
        /// POST: api/Registration/PostPushNotificationAndAddToInbox
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

                containerPushNotification = b.PostPushNotificationAndAddToInbox(pushNotification);
            }
            catch (Exception ex)
            {
                ExceptionHandling.LogError(ex, "PostPushNotificationAndAddToInbox");

                containerPushNotification.PushNotificationException = ex.Message;
                containerPushNotification.UserInboxException = ex.Message;
            }

            return Ok(containerPushNotification);
        }
    }
}