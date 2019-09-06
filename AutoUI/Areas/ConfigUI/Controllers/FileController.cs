using UIBase;
using System.ComponentModel.Composition;
using MFTool;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections.Generic;

namespace AutoUI.Areas.ConfigUI.Controllers
{
    [Export]
    public class FileController : BaseController
    {
        public JsonResult UpLoadFile()
        {
            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            if (hfc.Count > 0)
            {
                string type = hfc[0].FileName.Substring(hfc[0].FileName.LastIndexOf(".")); //获取上传文件的类型
                string typeDemand = ConfigurationManager.AppSettings["FileTypeAccept"];
                string[] typeDemandArr = typeDemand.Split(',');

                //格式判断
                if (typeDemandArr.Contains(type.ToLower()))
                {
                    int maxLength = int.Parse(ConfigurationManager.AppSettings["maxFileLength_KB"]);

                    //大小判断
                    if (hfc[0].ContentLength > maxLength * 1024)
                    {
                        throw new BusinessException("文件大小不能超过{0}KB".ReplaceArg(maxLength));
                    }

                    string fileId = GuidHelper.CreateTimeOrderID();
                    string fileStorePath = ConfigurationManager.AppSettings["FileStorePath"];
                    if(!System.IO.Directory.Exists(Server.MapPath("/" + fileStorePath)))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("/" + fileStorePath));
                    }
                    fileStorePath += (fileId + "__________" + hfc[0].FileName);
                    string PhysicalPath = Server.MapPath("/" + fileStorePath);//加波浪线前缀，否则得到的物理路径会加上Controller的名称WebManager，导致与实际的物理路径不匹配
                    hfc[0].SaveAs(PhysicalPath);
                    return Json(new { name = hfc[0].FileName, id = fileId });
                }
                else
                {
                    throw new BusinessException("不支持上传该文件类型");
                }
            }
            else
            {
                throw new BusinessException("请选择要上传的文件");
            }            
        }
    }
}
