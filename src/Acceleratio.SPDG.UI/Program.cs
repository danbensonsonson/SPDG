using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Acceleratio.SPDG.Generator;

namespace Acceleratio.SPDG.UI
{
    static class Program
    {


        private static void Restart()
        {
            Process.Start(Application.ExecutablePath);
            Environment.Exit(-1);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //CLI Support
            if (args.Length > 0)
            {
                if (args[0].Equals("/Help"))
                    Console.WriteLine("Usage: SysKit.SPDG.exe /Config <fileName>");
                // sending the enter key is not really needed, but otherwise the user thinks the app is still running by looking at the commandline. The enter key takes care of displaying the prompt again.
                SendKeys.SendWait("{ENTER}");
                //Application.Exit();
                ensureCorrectRuntime();
                if (args[0].Equals("/Config") && args.Length > 1)
                {
                    Common.DeserializeDefinition(args[1]);
                    SampleData.PrepareSampleCollections();
                    DataGenerator.SessionID = "Session " + DateTime.Now.ToString("yy-MM-dd") + " " + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString();
                    var generator = DataGenerator.Create(Common.WorkingDefinition);
                    generator.startDataGeneration();

                }
                else
                    Console.WriteLine("Usage: SysKit.SPDG.exe /Config <fileName>");
            }
            else
            {
                ensureCorrectRuntime();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frm01Connect(true));
            }

    }

        private static void ensureCorrectRuntime()
        {
            if (Debugger.IsAttached)
            {
                return;
            }
            var version = DataGenerator.GetSharePointOnPremVersion();

            int prefferedTargetRuntime = 4;
            if (version != null && version.Major < 15)
            {
                //for SharePoint 2010 we MUST run under the CLR 2.0
                prefferedTargetRuntime = 2;
            }

            string configFileName = Process.GetCurrentProcess().ProcessName + ".exe.config";          
            //have to use pure xml and not configuration manager to modify this section because it is readOnly  
            XDocument doc = XDocument.Load(configFileName);
            var startupNode = (from q in doc.Descendants()
                where q.Name == "startup"
                select q).First();

            int currentPrefferedRuntime;
            if (startupNode.Elements().First().Attribute("version").Value == "v4.0")
            {
                currentPrefferedRuntime = 4;
            }
            else
            {
                currentPrefferedRuntime = 2;
            }

            //on SharePoint 2010 if we have .net 4.0 installed we still must preffer the .net 2.0 runtime
            //otherwise the SharePoint farm will not be accessible
            if (currentPrefferedRuntime != prefferedTargetRuntime)
            {
                if (prefferedTargetRuntime == 4)
                {
                    startupNode.Elements().First().Attribute("version").Value = "v4.0";
                    startupNode.Elements().ElementAt(1).Attribute("version").Value = "v2.0.50727";
                }
                else
                {
                    startupNode.Elements().First().Attribute("version").Value = "v2.0.50727";
                    startupNode.Elements().ElementAt(1).Attribute("version").Value = "v4.0";
                }
                doc.Save(configFileName, SaveOptions.None);
                Restart();                
            }            
        }
    }
}
