using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;

namespace DevRX.MailTemplateSolution.Module.Docflow.Client
{
  partial class ModuleFunctions
  {

    /// <summary>
    /// 
    /// </summary>
    [Public]
    public void Test()
    {
      
      Functions.Module.Remote.Test();
      
    }
    
    [Public]
    public void GetAttachment()
    {
    //  var document = Sungero.Content.ElectronicDocuments.CreateDocumentWithCreationDialog();
     // message.AddAttachment(document.LastVersion);
     SelectDocumentDialog();
      var fileStream = File.OpenRead(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
      var attachmentName = Path.GetFileName(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
   //   return fileStream;
    }
    
        [LocalizeFunction("Выбрать документ", "Выбрать документ")]
    public virtual void SelectDocumentDialog()
    {
      var dialog = Dialogs.CreateInputDialog("Прикрепить документ");
      var id = dialog.AddSelect("Документ", true, Sungero.Content.ElectronicDocuments.Null);
      var x = id.Value;
    }
    
        [LocalizeFunction("Найти все обращения компании", "Поиск обращений компании")]
    public virtual void FindRequestByCompany123()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск обращений компании");
      var id = dialog.AddSelect("Организация", true, Sungero.Parties.CompanyBases.Null);
    }
      

  }
}