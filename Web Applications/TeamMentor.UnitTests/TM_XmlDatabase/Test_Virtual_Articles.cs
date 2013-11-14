using System;
using NUnit.Framework;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;


namespace TeamMentor.UnitTests.TM_XmlDatabase
{
    [TestFixture]
    class Test_Virtual_Articles
    {

        private static readonly Guid    Guid =            new Guid("96E988FA-F681-4F77-B3BA-3E72CD524A3F");
        private static readonly Guid    VirtualId =       new Guid("212EF83B-88C3-4E36-ACDB-332C7C0F0C2C");
        private static readonly Uri     UriToRedirect =   new Uri(@"http://www.owasp.org");
        private TM_Xml_Database database;

        #region Setup
        [SetUp]
        [Assert_Admin]      
        public void SetUp()
        {
            database = new TM_Xml_Database(true);
            database.tmConfig().VirtualArticles = new TMConfig.VirtualArticles_Config
                {AutoRedirectTarget = UriToRedirect.ToString(), 
                 AutoRedirectIfGuidNotFound = true};
        }
        #endregion

        #region Extension Methods test

        [Test]
        public void VirtualArticleXmlFileIsFound ()
        {
            var result = database.getVirtualArticlesFile();
            Assert.IsTrue(result.notNull());
            Assert.IsTrue(result.contains("Virtual_Articles.xml"));
        }


        [Test]
        public void Configuration_Values_Set_Correctly()
        {
            var result = database.tmConfig().VirtualArticles;
            Assert.IsTrue(result.notNull());
            //The value from the config file is a URI
            Assert.IsTrue(result.AutoRedirectTarget.isUri());
            Assert.IsTrue(result.AutoRedirectIfGuidNotFound);
        }

        [Test]
        public void Add_Mapping_Redirect_Test()
        {
            database.add_Mapping_Redirect(Guid, UriToRedirect);
            var count = database.getVirtualArticles().Count;
            Assert.IsTrue(count == 1);

            //Removing the mapping.
            var success = database.remove_Mapping_VirtualId(Guid);
            Assert.IsTrue(success);

        }

        [Test]
        public void Add_Mapping_VirtualId_Test()
        {
            database.add_Mapping_VirtualId(Guid, VirtualId);
            var count = database.getVirtualArticles().Count;
            Assert.IsTrue(count == 1);

            //Removing the mapping.
            var success = database.remove_Mapping_VirtualId(Guid);
            Assert.IsTrue(success);

        }

        [Test]
        public void getVirtualGuid_if_MappingExists_Test ()
        {
            database.add_Mapping_VirtualId(Guid, VirtualId);

            var result = database.getVirtualGuid_if_MappingExists(Guid);
            Assert.AreEqual(result,VirtualId);

            Assert.IsTrue(database.remove_Mapping_VirtualId(Guid));
        }

        [Test]
        public void TeamMentorRedirectsToUriIfGuidDoesNotExist()
        {
            var result = database.get_GuidRedirect(Guid);

            Assert.IsTrue(result.isUri());
            Assert.IsTrue(result.contains(database.tmConfig().VirtualArticles.AutoRedirectTarget));

        }

        [Test]
        public void TeamMentorShouldNotRedirectIfConfigFlagIsOff()
        {
            database.tmConfig().VirtualArticles = new TMConfig.VirtualArticles_Config
                {AutoRedirectIfGuidNotFound = false};
            var redirectUrl = database.get_GuidRedirect(Guid);

           Assert.IsTrue(redirectUrl.isNull());

        }


        [Test]
        public void AddGuidToRedirectUrlIsCorrect()
        {
            database.add_Mapping_Redirect(Guid, UriToRedirect);
            var url = database.get_GuidRedirect(Guid);

            Assert.IsTrue(url.isUri());
            Assert.IsTrue(url == UriToRedirect.ToString());
            database.remove_Mapping_VirtualId(Guid);

        }

        #endregion

    }
}
