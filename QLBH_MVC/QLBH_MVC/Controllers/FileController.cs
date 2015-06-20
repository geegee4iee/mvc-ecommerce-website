using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestAjaxUpload
{
    /// <summary>
    /// File Controller
    /// </summary>
    public class FileController : Controller
    {
        #region Actions

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult UploadFile(int? id, int? kind)
        {
            HttpPostedFileBase myFile = null;
            if (kind == 1)
            {
                myFile = Request.Files["ImageFile"];
            }
            if (kind == 2)
            {
                myFile = Request.Files["uImageFile"];
            }

            bool isUploaded = false;
            string message = "File upload failed";

            if (myFile != null && myFile.ContentLength != 0 && myFile.ContentType == "image/jpeg")
            {
                string pathForSaving = Server.MapPath("~/images/product/" + id);
                if (this.CreateFolderIfNeeded(pathForSaving))
                {
                    try
                    {
                        myFile.SaveAs(Path.Combine(pathForSaving, "img.jpg"));
                        isUploaded = true;
                        message = "File uploaded successfully!";
                    }
                    catch (Exception ex)
                    {
                        message = string.Format("File upload failed: {0}", ex.Message);
                    }
                }
            }
            return Json(new { isUploaded = isUploaded, message = message }, "text/html");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the folder if needed.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    /*TODO: You must process this exception.*/
                    result = false;
                }
            }
            else
            {
                if (System.IO.File.Exists(Path.Combine(path, "img.jpg")))
                {
                    System.IO.File.Delete(Path.Combine(path, "img.jpg"));
                }
            }
            return result;
        }

        #endregion
    }
}
