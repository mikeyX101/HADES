using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace HADES.Attributes
{
	public class OneTimeFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            try
			{
                if (context.HttpContext.Response.StatusCode != StatusCodes.Status400BadRequest && context.HttpContext.Response.StatusCode != StatusCodes.Status500InternalServerError)
                {
                    string id = context.HttpContext.Request.Query["id"];
                    if (!string.IsNullOrWhiteSpace(id))
					{
                        byte[] idBytes = Convert.FromBase64String(id);
                        if (idBytes.Length == 16)
                        {
                            Guid fileId = new(idBytes);
                            Models.AvailableFile file = Util.FileManager.GetFile(fileId);
                            if (file != null)
                            {
                                Serilog.Log.Information("Removed one time file ({Path}) publicly known as {PublicName}", file.FilePath, file.PublicFileName);
                                Util.FileManager.Remove(fileId);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
			{
                Serilog.Log.Warning(e, "Something happened while deleting a one time file using ID {FileID} in OneTimeFileAttribute", context.HttpContext.Request.Query["id"]);
			}
        }
    }
}
