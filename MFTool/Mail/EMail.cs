using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EFUltilities.Mail
{
    public delegate void DelSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    public class EMail
    {
        public void SendEmail(string host, string title ,string body, string fromMail, string pwb, string toMail, DelSendCompleted callBack)
        {
            string from = fromMail;

            MailMessage newEmail = new MailMessage();

            #region 发送方邮件
            newEmail.From = new MailAddress(from, from);
            #endregion

            #region 发送对象，可群发
            newEmail.To.Add(new MailAddress(toMail));  //接收方邮箱一
            //newEmail.To.Add(new MailAddress("132@hotmail.com"));  //接收方邮箱二
            #endregion


            #region Subject
            newEmail.Subject = title;  //标题
            #endregion

            #region Body
            newEmail.Body = body;  //内容
            #endregion

            #region 附件
            //Attachment MsgAttach = new Attachment("");//可通过一个FileUpload地址获取附件地址
            //newEmail.Attachments.Add(MsgAttach);
            #endregion

            #region Deployment
            newEmail.IsBodyHtml = true;                //是否支持html
            newEmail.Priority = MailPriority.Normal;  //优先级
            #endregion

            //发送方服务器信息
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(fromMail, pwb);
            smtpClient.Host = host; //主机

            //smtpClient.Send(newEmail);   //同步发送,程序将被阻塞

            #region 异步发送, 会进入回调函数SendCompletedCallback，来判断发送是否成功
            smtpClient.SendCompleted += new SendCompletedEventHandler(callBack);//回调函数
            string userState = "测试";
            smtpClient.SendAsync(newEmail, userState);

            #endregion

        }
    }
}
