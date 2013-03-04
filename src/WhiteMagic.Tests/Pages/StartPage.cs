using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace WhiteMagic.Tests.Pages
{
    [ContentType(DisplayName = "StartPage", GUID = "6b323d8d-3246-4e08-a298-96cfff178614", Description = "")]
    public class StartPage : PageData
    {
        [CultureSpecific]
        [Editable(true)]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string MainBody { get; set; }
         
    }
}