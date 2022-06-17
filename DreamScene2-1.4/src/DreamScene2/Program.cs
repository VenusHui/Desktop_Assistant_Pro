using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DreamScene2
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IntPtr hWnd = PInvoke.FindWindow(null, Constant.MainWindowTitle);
            if (hWnd != IntPtr.Zero)
            {
                const int SW_RESTORE = 9;
                PInvoke.ShowWindow(hWnd, SW_RESTORE);
                PInvoke.SetForegroundWindow(hWnd);
                return;
            }

            string extPath = Helper.ExtPath();
            if (!Directory.Exists(extPath))
            {
                Directory.CreateDirectory(extPath);
            }

#if NET5_0_OR_GREATER
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainDialog mainDialog = new MainDialog();
            mainDialog.Show();

            if (args.Length != 0 && args[0] == Constant.Cmd)
            {
                mainDialog.Hide();
            }

            Application.Run();
        }

        static void ExtractResources()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.EndsWith(".dll"))
                {
                    string fileName = resourceName.Substring(nameof(DreamScene2).Length + 1);
                    string filePath = Path.Combine(Application.StartupPath, fileName);
                    if (!File.Exists(filePath))
                    {
                        using (FileStream fileStream = File.Create(filePath))
                        {
                            Stream stream = assembly.GetManifestResourceStream(resourceName);
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }
}
