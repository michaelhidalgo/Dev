using System;
using System.Web.Mvc;
using O2.DotNetWrappers.ExtensionMethods;
using TeamMentor.CoreLib;

namespace TeamMentor.Website
{
    public class MarkdownController : Controller
    {
        //public static Guid              testArticleGuid;
        public static TM_Xml_Database   tmDatabase;

        static MarkdownController()
        {
             //testArticleGuid = "0736d6f3-976f-421b-b5b9-053b97470b75".guid();
             tmDatabase = TM_Xml_Database.Current;
        }

        public TeamMentor_Article getArticle(string articleId)
        {
            var article = tmDatabase.tmGuidanceItem(articleId.guid());            
            return article;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editor(string articleId, string content)
        {
            var article = getArticle(articleId);
            article.Content.Data.Value = content;
            article.xmlDB_Save_Article(tmDatabase);

            return Redirect("Viewer?articleId={0}".format(articleId));
        }        

        public ActionResult Editor(string articleId)
        {
            var article = getArticle(articleId);
            if (article.notNull())
                ViewData["Content"] = getArticle(articleId).Content.Data_Json;
            else
                ViewData["Content"] = "NO ARTICLE With GUID: {0}".format(articleId);
            ViewData["ArticleId"] = articleId;
            return View(@"~/MVC/Views/MarkDown_Editor.cshtml");
        }

        public ActionResult Viewer(string articleId)
        {
            var markdownToRender = getArticle(articleId).Content.Data_Json;            
            ViewData["Content"] = markdownToRender.renderMarkdown();
            ViewData["ArticleId"] = articleId;
            return View(@"~/MVC/Views/MarkDown_Viewer.cshtml");
        }
    }
}