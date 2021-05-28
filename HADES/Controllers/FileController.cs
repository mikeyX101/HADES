using System;
using System.IO;
using HADES.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HADES.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        [HttpGet]
        [OneTimeFile]
        public IActionResult File([FromQuery] string id)
        {
            IActionResult result;

            byte[] idBytes = Convert.FromBase64String(id);
            if (idBytes.Length == 16)
            {
                Guid fileId = new(idBytes);
                Models.AvailableFile file = Util.FileManager.GetFile(fileId);
                Models.IUser user = Util.ConnexionUtil.CurrentUser(User);
                if (user != null && user.GetId() == file.OwnerID)
				{
                    if (file != null)
                    {
                        FileStream fileStream = System.IO.File.OpenRead(file.FilePath);
                        BufferedStream fileBuffer = new(fileStream);

                        // File is deleted in OneTimeFileAttribute after the download is finished
                        result = File(fileBuffer, file.MimeType, file.PublicFileName);
                        Serilog.Log.Information("File {PhysicalFile} with a type of {MimeType} has been served as {PublicFileName} by FileController", file.FilePath, file.MimeType, file.PublicFileName);
                    }
                    else
                    {
                        Serilog.Log.Warning("A request to FileController ended because {Reason}", "no file was found.");
                        result = NotFound();
                    }
                }
                else
				{
                    Serilog.Log.Warning("A request to FileController ended because {Reason}", "a user tried to access a file only another user has access to");
                    result = Unauthorized();
                }
            }
            else
			{
                Serilog.Log.Warning("A request to FileController ended because {Reason}", "of invalid data in the id query string");
                result = BadRequest();
            }
            
            return result;
        }
    }
}
