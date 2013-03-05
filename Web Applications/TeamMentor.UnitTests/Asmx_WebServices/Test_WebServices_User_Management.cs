﻿using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.Asmx_WebServices
{
    [TestFixture]
    public class Test_WebServices_User_Management : TM_WebServices_InMemory
    {        
        [Test] public void Login()
        {
            tmWebServices.Logout();
            var sessionId_BeforeLogin = tmWebServices.Current_SessionID();
            var login_SessionId       = tmWebServices.Login       (tmConfig.TMSecurity.Default_AdminUserName, tmConfig.TMSecurity.Default_AdminPassword);
            HttpContextFactory.Context.addCookieFromResponseToRequest("Session");            
            var sessionId_AfterLogin  = tmWebServices.Current_SessionID();
                        
            Assert.AreEqual    (sessionId_BeforeLogin, Guid.Empty          , "sessionId should be empty");
            Assert.AreNotEqual (sessionId_AfterLogin , Guid.Empty          , "sessionId should Not empty");
            Assert.AreNotEqual (login_SessionId      , Guid.Empty          , "login_SessionId  should not be empty");
            Assert.AreEqual    (sessionId_AfterLogin, sessionId_AfterLogin , "sessionsIds should be the same");
        }
        [Test] public void CheckThatCurrentUserUserMatchesNewUser()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);
            var tmUser      = userId.tmUser();

            Assert.Greater  (userId, 0);
            Assert.NotNull  (tmUser);
            Assert.AreEqual (userId, tmUser.UserID);

            var sessionId   = tmWebServices.Login(newUser.username, newUser.password);
            HttpContextFactory.Context     .addCookieFromResponseToRequest("Session");
            var currentUser = tmWebServices.Current_User();
            
            Assert.AreNotEqual(Guid.Empty,sessionId);                
            Assert.NotNull    (currentUser , "Current User Not Set");
            Assert.AreEqual   (currentUser.Company      , newUser.company);
            Assert.AreEqual   (DateTime.FromFileTimeUtc(currentUser.CreatedDate).ToLongDateString()  ,
                               DateTime.Now                                     .ToLongDateString());
            Assert.AreEqual   (currentUser.Email        , newUser.email);
            Assert.AreEqual   (currentUser.FirstName    , newUser.firstname);
            Assert.AreEqual   (currentUser.LastName     , newUser.lastname);
            Assert.AreEqual   (currentUser.Title        , newUser.title);
            Assert.AreEqual   (currentUser.UserId       , userId);
            Assert.AreEqual   (currentUser.UserName     , newUser.username);            
        }
        [Test] public void ChangeCurrentUserPassword()        
        {                     
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);

            var originalPassword = newUser.password;
            var newPassword      = "Abcmfl!@#";
            
            var sessionId_OriginalPassword    = tmWebServices.Login(newUser.username, originalPassword);
            HttpContextFactory.Context       .addCookieFromResponseToRequest("Session");
            var currentUser_OriginalPassword  = tmWebServices.Current_User();
            var changePasswordResult          = tmWebServices.SetCurrentUserPassword(originalPassword,newPassword);
            var sessionId_NewPassword         = tmWebServices.Login(newUser.username, newPassword);
            var currentUser_NewPassword       = tmWebServices.Current_User();
            var sessionId_OriginalPassword2   = tmWebServices.Login(newUser.username, originalPassword);
            var currentUser_OriginalPassword2 = tmWebServices.Current_User();

            Assert.Greater    (userId, 0);
            Assert.AreNotEqual(Guid.Empty,sessionId_OriginalPassword  , "Login with original password");   
            Assert.NotNull    (currentUser_OriginalPassword           , "Current User Not Set (original password)");
            Assert.IsTrue     (changePasswordResult                   , "Change password result");
            Assert.AreNotEqual(Guid.Empty,sessionId_NewPassword       , "Login with new password");   
            Assert.NotNull    (currentUser_NewPassword                , "Current User Not Set (new password)");
            Assert.AreEqual   (Guid.Empty,sessionId_OriginalPassword2 , "Login with original password after change");   
            Assert.IsNull     (currentUser_OriginalPassword2          , "Current User Not Set (original password after change)");
        }
        [Test] public void NewPasswordMustBeDifferentFromOlderPassword()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);
            var sessionId   = tmWebServices.Login(newUser.username, newUser.password );
            var changePasswordResult  = tmWebServices.SetCurrentUserPassword(newUser.password , newUser.password );

            Assert.Greater    (userId, 0);     
            Assert.AreNotEqual(Guid.Empty,sessionId  , "Login with original password");   
            Assert.IsFalse    (changePasswordResult  , "Change password should be false if passwords are the same");
        }
        [Test] public void RequireOldPasswordBeforeChangingIntoNewOne()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);

            var originalPassword = newUser.password;
            var newPassword      = "Abcmfl!@#";
            
            var sessionId_OriginalPassword           = tmWebServices.Login(newUser.username, originalPassword);
            var changePasswordResult_NoOriginalPwd   = tmWebServices.SetCurrentUserPassword(newPassword     , newPassword);
            var changePasswordResult_WithOriginalPwd = tmWebServices.SetCurrentUserPassword(originalPassword, newPassword);

            Assert.Greater    (userId, 0);
            Assert.AreNotEqual(Guid.Empty,sessionId_OriginalPassword  , "Login with original password");   
            Assert.IsFalse    (changePasswordResult_NoOriginalPwd     , "Change password result (no original password");
            Assert.IsTrue     (changePasswordResult_WithOriginalPwd   , "Change password result (with original password");
        }
        [Test] public void CheckCurrentUserCSRFToken()
        {
            var newUser     = newTempUser();
            var userId      = tmWebServices.CreateUser(newUser);
            var sessionId   = tmWebServices.Login(newUser.username, newUser.password);
            HttpContextFactory.Context     .addCookieFromResponseToRequest("Session");
            var currentUser = tmWebServices.Current_User();
            
            Assert.Greater  (userId, 0);
            Assert.IsNotNull(currentUser.CSRF_Token                                 , "CSRF_Token was not set");
            Assert.AreEqual (sessionId.str().hash().str(), currentUser.CSRF_Token   , "CSRF_Token didn't match");
            
        }
        [Test] public void SingleUseLoginToken()
        {
            var newUser       = newTempUser();
            var tmUser        = tmWebServices.CreateUser(newUser).tmUser();

            var loginToken_BeforeSet    = tmUser.SecretData.SingleUseLoginToken;
            var loginToken_BeforeUse    = tmUser.current_SingleUseLoginToken();
            var loginToken              = tmUser.SecretData.SingleUseLoginToken;
            var sessionId_UsingToken    = tmWebServices.Login_Using_LoginToken(tmUser.UserName, loginToken);
            var loginToken_AfterUse     = tmUser.SecretData.SingleUseLoginToken;
            var loginToken_NextRequest  = tmUser.current_SingleUseLoginToken();

            Assert.AreEqual   (Guid.Empty, loginToken_BeforeSet , "loginToken_BeforeSet");
            Assert.AreNotEqual(Guid.Empty, loginToken_BeforeUse , "loginToken_BeforeUse");
            Assert.AreEqual   (loginToken, loginToken_BeforeUse , "loginToken_BeforeUse");
            Assert.AreNotEqual(Guid.Empty, sessionId_UsingToken , "sessionId_UsingToken");
            Assert.AreEqual   (Guid.Empty, loginToken_AfterUse  , "loginToken_AfterUse");
            Assert.AreNotEqual(Guid.Empty, loginToken_NextRequest, "loginToken_NextRequest is Empty");
            Assert.AreNotEqual(loginToken, loginToken_NextRequest, "loginToken_NextRequest should be new");
            
        }
        [Test] public void PasswordResetToken()
        {            
            var newUser       = newTempUser();
            var newPassword     = "123SAFsi!";
            var oldPassword     = newUser.password;
            var tmUser        = tmWebServices.CreateUser(newUser).tmUser();

            var token_BeforeSet    = tmUser.SecretData.PasswordResetToken;
            var token_BeforeUse    = tmUser.UserName.current_PasswordResetToken(tmUser.EMail);
            var token              = tmUser.UserName.current_PasswordResetToken(tmUser.EMail);
            var result             = tmWebServices.PasswordReset(tmUser.UserName, token,newPassword);
            var token_AfterUse     = tmUser.SecretData.PasswordResetToken;
            var token_NextRequest  = tmUser.UserName.current_PasswordResetToken(tmUser.EMail);
            var sessionId_NewPwd   = tmWebServices.Login(tmUser.UserName, newPassword);
            var sessionId_OldPwd   = tmWebServices.Login(tmUser.UserName, oldPassword);            

            Assert.AreEqual   (Guid.Empty, token_BeforeSet  , "loginToken_BeforeSet");
            Assert.AreNotEqual(Guid.Empty, token_BeforeUse  , "loginToken_BeforeUse");
            Assert.AreEqual   (token, token_BeforeUse       , "loginToken_BeforeUse");
            Assert.IsTrue     (result                       , "change password result");
            Assert.AreEqual   (Guid.Empty, token_AfterUse   , "loginToken_AfterUse");
            Assert.AreNotEqual(Guid.Empty, token_NextRequest, "loginToken_NextRequest is Empty");
            Assert.AreNotEqual(token, token_NextRequest     , "loginToken_NextRequest should be new");
            Assert.AreNotEqual(Guid.Empty, sessionId_NewPwd , "sessionId with new password");
            Assert.AreEqual   (Guid.Empty, sessionId_OldPwd , "sessionId with old password");
        }
        //Helper methods
        public NewUser newTempUser()
        {
            var password1 = "Sdimfl!@#".add_RandomLetters(10);
            
            var newUser = new NewUser
                {
                    username    = 10.randomLetters(),
                    password    = password1,
                    company     = 10.randomLetters(),
                    email       = 10.randomLetters(),
                    firstname   = 10.randomLetters(),
                    lastname    = 10.randomLetters(),
                    title       = 10.randomLetters()
                };
            return newUser;            
        }
    }
}
