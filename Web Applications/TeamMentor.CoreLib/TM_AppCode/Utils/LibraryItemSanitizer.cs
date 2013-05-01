using FluentSharp;
using O2.DotNetWrappers.ExtensionMethods;

namespace TeamMentor.CoreLib.TM_AppCode.Utils
{
    public class LibraryItemSanitizer
    {
        public void Sanitize(TeamMentor_Article article)
        {
            if (article.Content.DataType.lower() == "html") // tidy the html
            {
                string cdataContent = article.Content.Data.Value.replace("]]>", "]] >");
                    // xmlserialization below will break if there is a ]]>  in the text                

                string tidiedHtml = cdataContent.tidyHtml();

                article.Content.Data.Value = tidiedHtml;

                if (article.serialize(false).inValid())
                    // see if the tidied content can be serialized  and if not use the original data              
                {
                    article.Content.Data.Value = cdataContent;
                }
            }
        }
    }
}