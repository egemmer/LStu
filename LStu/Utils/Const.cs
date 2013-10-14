using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LStu.Models;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace LStu.Utils
{
    public class Const
    {
        public const char DOT = '.';

        /**
         * The Unix separator character.
         */
        private const char UNIX_SEPARATOR = '/';

        /**
         * The Windows separator character.
         */
        private const char WINDOWS_SEPARATOR = '\\';

        public static bool NotEmpty(string s)
        {
            return s != null && s.Length > 0;
        }

        public static bool IsNullOrEmpty(string s)
        {
            return String.IsNullOrEmpty(s) || s == "undefined";
        }

        public static string GetExtension(string filename)
        {
            string retval = String.Empty;
            if (IsNullOrEmpty(filename)) return retval;

            int indexOf = filename.LastIndexOf(DOT);

            if (indexOf >= 0)
            {
                retval = filename.Substring(indexOf);
            }

            return retval;
        }

        public static int IndexOfLastSeparator(String filename)
        {
            if (filename == null) return -1;
            int lastUnixPos = filename.LastIndexOf(UNIX_SEPARATOR);
            int lastWindowsPos = filename.LastIndexOf(WINDOWS_SEPARATOR);
            return Math.Max(lastUnixPos, lastWindowsPos);
        }

        public static int IndexOfExtension(String filename)
        {
            if (filename == null)
            {
                return -1;
            }
            int extensionPos = filename.LastIndexOf(DOT);
            int lastSeparator = IndexOfLastSeparator(filename);
            return lastSeparator > extensionPos ? -1 : extensionPos;
        }

        public static string GetSimpleFileName(string filename)
        {
            if (filename == null) return null;
            int index = IndexOfLastSeparator(filename);
            return filename.Substring(index + 1);
        }

        public static String RemoveExtension(String filename)
        {
            if (filename == null)
            {
                return null;
            }
            int index = IndexOfExtension(filename);
            if (index == -1)
            {
                return filename;
            }
            else
            {
                return filename.Substring(0, index);
            }
        }

        public static void Direcotry(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static string GetStatusDesc(int status)
        {
            switch (status)
            {
                case 0: return "等待审核";
                case 1: return "借阅同意";
                case 2: return "借阅拒绝";
                case 3: return "已借阅";
                default: return "未知状态";
            }
        }

        public static bool CompressDirectory(string filename, Stream stream)
        {
            var retval = false;
            if (Directory.Exists(filename))
            {
                string[] filenames = Directory.GetFiles(filename, "*.*", SearchOption.AllDirectories);
                try
                {
                    using (ZipOutputStream s = new ZipOutputStream(stream))
                    {
                        s.SetLevel(9);
                        byte[] buffer = new byte[4096];

                        foreach (string file in filenames)
                        {
                            ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                            entry.DateTime = DateTime.Now;
                            s.PutNextEntry(entry);

                            using (FileStream fs = File.OpenRead(file))
                            {
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    s.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                            }
                        }

                        s.Finish();
                        s.Close();
                        retval = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception during processing {0}", ex);
                }
            }

            return retval;
        }

        public static string StripSubfix(string s)
        {
            var index = s.IndexOf('-');
            if (index != -1)
            {
                s = s.Substring(0, index);
            }
            else
            {
                if (s.Length > 4)
                {
                    s = s.Substring(0, 4);
                }
            }

            if (s.Length == 4)
            {
                try
                {
                    int n = Convert.ToInt32(s);
                    if (n >= 1997 && n <= 2999) return s;
                }
                catch {                }
            }
            else if (s.Length == 2)
            {
                s = "20" + s;
                try
                {
                    int n = Convert.ToInt32(s);
                    if (n >= 1997 && n <= 2999) return s;
                }
                catch { }
            }
            return "其它";
        }
    }
}