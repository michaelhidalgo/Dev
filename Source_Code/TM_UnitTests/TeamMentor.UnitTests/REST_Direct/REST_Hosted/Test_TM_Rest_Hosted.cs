﻿using System;
using System.ServiceModel;
using FluentSharp.Web35;
using NUnit.Framework;
using FluentSharp.CoreLib;
using TeamMentor.CoreLib;

namespace TeamMentor.UnitTests.REST

{	    
    [TestFixture]
    //[Ignore]                // can't run these on TeamCity due to security protection for binding WCF address into Port                            
    public class Test_TM_Rest_Hosted : TM_Rest_Hosted
    {
        [SetUp] public void setup()
        {
            WCFHost_Start();
            
            if (HostStarted.isFalse())
                Assert.Ignore("Neet to set the security protection for binding WCF address into Port");
                // need to find a better solution or a way to automate the setup
                // references:http://blogs.msdn.com/b/paulwh/archive/2007/05/04/addressaccessdeniedexception-http-could-not-register-url-http-8080.aspx
                //            http://stackoverflow.com/questions/885744/wcf-servicehost-access-rights
                //            when running inside VisualStudio we can use the Design_Time_Addresses trick
        }

        [TearDown] public void tearDown()
        {
            WCFHost_Stop();
        }

            
        [Test] public  void Test_WCFHost_Start()
        {
            // stop the current one
            Assert.IsTrue(HostStarted);
            WCFHost_Stop();
            Assert.IsFalse(HostStarted);

            // create a rest service with a random port and without using the Design_Time_Addresses 
            // NOTE: this test will fail if VS is run with Admin privs
            var currentTemplate = Tests_Consts.TM_REST_Url_Template;
            var currentPort     = Tests_Consts.TM_REST_Service_Port;
            Tests_Consts.TM_REST_Url_Template = currentTemplate.replace("Design_Time_Addresses","ABCD");
            Tests_Consts.TM_REST_Service_Port = 20000.randomNumber();
            var tmRestHosted = new TM_Rest_Hosted();
            Assert.IsNotNull(tmRestHosted);
            tmRestHosted.WCFHost_Start();
            Assert.IsFalse  (tmRestHosted.HostStarted);
            Tests_Consts.TM_REST_Service_Port = currentPort;
            Tests_Consts.TM_REST_Url_Template = currentTemplate;
        }
        [Test]  public void CheckWebServiceHost()
        {           
            var url = TmRestHost.BaseAddress.str() + "/Version";
            var html = url.GET();
            Assert.IsTrue(html.valid(), "Html fetch failed");
            //test version
            var version = TmRest.Version();
            Assert.NotNull(version,"Version was null");
            "version (hosted access): {0}".info(version);
            //test sessionID
            var sessionId = TmRest.SessionId();
            Assert.NotNull(sessionId, "sessionID was null");
            "sessionID (hosted access): {0}".info(sessionId);
        }
        
        [Test]  public void CheckLogin()
        {
            var identity_IsAuthenticated = TmRest.RBAC_CurrentIdentity_IsAuthenticated();
            var identity_Name            = TmRest.RBAC_CurrentIdentity_Name();
            var identity_Roles           = TmRest.RBAC_CurrentPrincipal_Roles();
            var identity_IsAdmin         = TmRest.RBAC_IsAdmin();

            Assert.IsTrue (identity_IsAuthenticated);
            Assert.IsNull  (identity_Name);
            Assert.AreEqual(identity_Roles.size(), 2);
            Assert.IsFalse (identity_IsAdmin);
            var username   = tmConfig.TMSecurity.Default_AdminUserName;
            var pwd        = tmConfig.TMSecurity.Default_AdminPassword;
            var sessionId  = TmRest.Login(username, pwd);
            
            Assert.AreNotEqual(Guid.Empty,sessionId);
                        
            identity_IsAuthenticated = TmRest.RBAC_CurrentIdentity_IsAuthenticated();
            identity_Name            = TmRest.RBAC_CurrentIdentity_Name();

            Assert.IsTrue  (identity_IsAuthenticated);
            Assert.AreEqual(username, identity_Name);
            
            //without the CSRF-token these should fail

            Assert.IsFalse  (TmRest.RBAC_IsAdmin());
            Assert.AreEqual (TmRest.RBAC_CurrentPrincipal_Roles().size(), 2);

            sessionId.set_Guid_as_CsrfToken_on_Request();
            
            //Now the mappings should work
            
            Assert.IsTrue  (TmRest.RBAC_IsAdmin());
            Assert.AreEqual(TmRest.RBAC_CurrentPrincipal_Roles().size(), 6);            
        }

        [Test]
        public void CheckLogin_Again() // make sure there are no session values left behind
        {
            CheckLogin();
        }

        //[Assert_Admin]
        [Test]  public void CheckErrorHandling()
        {
            Assert.Throws<ProtocolException >(()=>TmRest.users());
            CheckLogin();       //logs in as admin

            TmRest.users();
        } 
    }
}