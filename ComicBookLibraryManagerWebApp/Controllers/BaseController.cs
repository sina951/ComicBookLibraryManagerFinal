using ComicBookShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    // adding the abstract keyword so we cannot instantiate BaseController directly
    // Controller is an MVC base controller class! Other classes like ComicBookController.cs will in iherit
    // as well from but from the BaseController class! (Class Hierchy)
    public abstract class BaseController : Controller
    {
        private bool _disposed = false;

        protected Context Context { get; private set; }
        // procted means that the descendants controller classes can accesthis field
        // which represents our our instance of database context
        protected Repository Repository { get; private set; }

        public BaseController()
        {
            Context = new Context();
            Repository = new Repository(Context);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Context.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }
    }
}