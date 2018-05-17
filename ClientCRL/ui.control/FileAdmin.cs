using System;
using System.IO;

namespace ClientCRL.ui.control
{
    /// <summary>
    /// File administrator. 
    /// Opens and saves files
    /// </summary>
    class FileAdmin
    {
        /// <summary>
        /// Opens a file from a OpenFileDialog
        /// </summary>
        /// <returns>Return the info of the file</returns>
        public static FileInfo OpenFile()
        {
            try
            {
                var info = SelectFileToOpen();

                if (info.path == "" || info.name == "")
                {
                    return info;
                }

                return OpenFile(info);
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return new FileInfo { name = "", data = "", path = "" };
        }

        /// <summary>
        /// Opens a new file
        /// </summary>
        /// <param name="filename">File path to be opened</param>
        /// <returns>File info struct</returns>
        public static FileInfo OpenFile(FileInfo fileInfo)
        {
            try
            {   
                using (var sr = new StreamReader(Path.Combine(fileInfo.path, fileInfo.name + ".crl")))
                {
                    fileInfo.data = sr.ReadToEnd();

                    return fileInfo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return new FileInfo { name = "", data = "", path = "" };
        }
        
        /// <summary>
        /// Save a file
        /// </summary>
        /// <param name="info">Info of the file to be saved</param>
        public static bool SaveFile(FileInfo info)
        {
            try
            {
                if (info.path == "")
                {
                    info.path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(info.path, info.name + ".crl")))
                {
                    outputFile.WriteLine(info.data);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
                return false;
            }
        }

        /// <summary>
        /// Selects a file to be opened
        /// </summary>
        /// <returns>File info from path and filename</returns>
        private static FileInfo SelectFileToOpen()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".crl",
                Filter = "Compi Report Language (.crl)|*.crl"
            };
            
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                string path = Path.GetDirectoryName(dlg.FileName);
                string name = Path.GetFileNameWithoutExtension(dlg.FileName);

                return new FileInfo { name = name, path = path };
            }

            return new FileInfo { name = "", path = "" };
        }
    }

    /// <summary>
    /// Represents the info of a file, its name, path and data.
    /// </summary>
    struct FileInfo
    {
        public string name;
        public string data;
        public string path;
    }
}
