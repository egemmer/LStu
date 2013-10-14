using System;
using System.Collections.Generic;
using System.Text;
using DWGdirectX;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace LStu.Core
{
    public class DwgDirectXManager : IDisposable
    {
        private static DwgDirectXManager _instance;
        private OdaHostApp oHost;
        private AcadApplication oApp;

        private DwgDirectXManager()
        {
            oHost = new OdaHostAppClass();
            oApp = oHost.Application;
        }

        public static DwgDirectXManager Instance
        {
            get
            {
                if(_instance == null) _instance = new DwgDirectXManager();
                return _instance;
            }
        }

        ~DwgDirectXManager()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                oHost.PagingController = null;
                oApp.Documents.Close();
                oApp.Quit();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ConvertToPDF(InputOutputParam p)
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(pdfOut);
            Thread t = new Thread(start);
            t.Start(p);
        }

        private void pdfOut(object s)
        {
            if (s is InputOutputParam)
            {
                var inOut = s as InputOutputParam;
                AcadDocument document = oApp.Documents.Open(inOut.dwgFileName, true);

                IOdaPdfExport pdfExport = new OdaPdfExportClass();

                pdfExport.EmbededTTF = false;
                pdfExport.EnableLayers = true;
                pdfExport.IncludeOffLayers = true;
                pdfExport.SHXTextAsGeometry = true;
                pdfExport.SimpleGeomOptimization = false;
                pdfExport.TTFTextAsGeometry = true;
                pdfExport.ZoomToExtentsMode = true;

                foreach (DWGdirectX.IAcadLayout al in document.Layouts)
                {
                    pdfExport.AddPage(al.Name);
                }
                pdfExport.ExportPdf(document.Database, inOut.pdfFileName, OdPdfVersion.odPDFv1_5);

                WaitFileCreate(inOut.pdfFileName);

                document.Close(false);
            }
            else
            {
                throw new ArgumentException("LStu.Core.InputOutputParam");
            }
        }

        private void WaitFileCreate(string fileName)
        {
            while (!System.IO.File.Exists(fileName)) { Thread.Sleep(100); }

            int count = 0;
            while (IsOpening(fileName))
            {
                Thread.Sleep(100);
                if (count > 9) break; // 超出10次
                count++;
            }

            IntPtr vHandle = _lopen(fileName, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {
            }
            CloseHandle(vHandle);
        }


        public bool IsOpening(string fileName)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                return false;
            }
            catch
            {
                return true;
            }
            finally
            {
                if(stream != null)
                stream.Close();
            }
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);
    }
}
