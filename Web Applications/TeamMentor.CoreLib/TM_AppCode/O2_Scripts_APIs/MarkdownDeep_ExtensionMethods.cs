namespace TeamMentor.CoreLib
{
	public static class MarkdownDeepExtensions
	{        
		public static string markdown_transform(this string markdown)
		{						
		    var md = new MarkdownDeep.Markdown
		                 {
                             SafeMode = true,       // was false in the MarkdownDeep demo app
                             ExtraMode = true,      
                             AutoHeadingIDs = true, 
                             MarkdownInHtml = true, 
                             NewWindowForExternalLinks = true
		                 };
            return md.Transform(markdown);            
		}
	}
}
