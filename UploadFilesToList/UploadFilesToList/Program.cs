using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace UploadFilesToList
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string siteUrl = args[0];
                string thread = args[1];
                using (ClientContext client = new ClientContext(siteUrl))
                {
                    string passWd = "FroFro123#";
                    SecureString securePassWd = new SecureString();
                    foreach (var c in passWd.ToCharArray())
                    {
                        securePassWd.AppendChar(c);
                    }
                    client.Credentials = new NetworkCredential("administrator", securePassWd, "fusionis");
                    var formLib = client.Web.Lists.GetByTitle("Documents");
                    client.Load(formLib.RootFolder);
                    client.ExecuteQuery();
                    for (int i=0; i<250000; ++i)
                    {
                        Console.WriteLine("doc " + i);
                        var fileCreationInformation = new FileCreationInformation();
                        //Assign to content byte[] i.e. documentStream

                        fileCreationInformation.Content = Encoding.ASCII.GetBytes("<html><body><p>this is a webpage " + i + "</p></body></html>");
                        //Allow owerwrite of document

                        fileCreationInformation.Overwrite = true;
                        //Upload URL

                        fileCreationInformation.Url = siteUrl + "/Shared Documents/million-" + thread + "-" + i + ".html";
                        Microsoft.SharePoint.Client.File uploadFile = formLib.RootFolder.Files.Add(
                            fileCreationInformation);

                        uploadFile.ListItemAllFields.Update();
                        client.ExecuteQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
