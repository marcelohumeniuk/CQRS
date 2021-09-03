using Microsoft.AspNetCore.Mvc;

namespace CQRS.UI.Web.ViewComponents
{
    public class SummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default");
        }
    }
}