using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcUniJudje
{
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                //ловим последнее возникшее исключение
                Exception lastError = Server.GetLastError();

                if (lastError != null)
                {
                    //Записываем непосредственно исключение, вызвавшее данное, в
                    //Session для дальнейшего использования
                    Session["ErrorException"] = lastError.InnerException;
                }

                // Обнуление ошибки на сервере
                Server.ClearError();

                // Перенаправление на свою страницу отображения ошибки

                Response.Redirect("~/Error", true);
            }
            catch (Exception)
            {
                // если мы всёже приходим сюда - значит обработка исключения 
                // сама сгенерировала исключение, мы ничего не делаем, чтобы
                // не создать бесконечный цикл
                Response.Write("К сожалению произошла критическая ошибка. Нажмите кнопку 'Назад' в браузере и попробуйте ещё раз. ");
            }
        }
    }
}