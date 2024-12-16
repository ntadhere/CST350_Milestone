namespace CST350_Milestone.Filter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class RazorViewRenderer
    {
        public static string RenderViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"Unable to find view: {viewName}");
                }

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}