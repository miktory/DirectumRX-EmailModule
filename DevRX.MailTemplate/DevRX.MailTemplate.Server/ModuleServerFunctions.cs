using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;
using System.Text;

namespace DevRX.MailTemplate.Server
{
  public class ModuleFunctions
  {
    [Public]
    public void SendMailByTemplate(string eMail, string template, System.Collections.Generic.Dictionary<string, object> model)
    {
      var message = Mail.CreateMailMessage();
      var body = MailTemplateSolution.Module.Docflow.PublicFunctions.Module.GetMailBodyAsHtml(template, model);

      message.Body = body;
      message.IsBodyHtml = true;
      message.Subject = "Тестовое Письмо";
      message.To.Add(eMail);
     // message.CC.AddRange(copy);
     
   MailTemplateSolution.Module.Docflow.PublicFunctions.AddLogo(message);
      var fileStream = File.OpenRead(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
      var attachmentName = Path.GetFileName(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
      // Добавить вложение в виде потока с данными из файла в письмо.
      message.AddAttachment(fileStream, attachmentName);
      Mail.Send(message);
    }
  }
}