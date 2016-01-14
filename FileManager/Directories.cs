using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.AccessControl;

namespace FileManager
{
    class Directories
    {
        DirectoryInfo directory;
        string path;
        string name;
        DirectoryInfo[] subdirectories;
        FileInfo[] files;

        public Directories(string path)
        {
            directory = new DirectoryInfo(path);
            this.path = path;
            name = directory.Name;
            subdirectories = this.AccessDirectories();
            files = this.AccessFiles();
        }
        public Directories(DirectoryInfo dir)
        {
            directory = dir;
            path = dir.FullName;
            name = directory.Name;
            subdirectories = this.AccessDirectories();
            files = this.AccessFiles();
        }
        /// <summary>
        /// Gets subdirectories and avoids the restricted ones
        /// </summary>
        /// <returns>An array of subdirectories</returns>
        public DirectoryInfo[] AccessDirectories()
        {
            try
            {
            //if (Operations.CheckFolderPermissions(directory.FullName, FileSystemRights.Read))     
            //if (!directory.Attributes.ToString().Contains("NotContentIndexed"))
            //    if (Operations.CheckFolderPermissions(directory.FullName, FileSystemRights.FullControl))
            //        if (directory.Name != "System Volume Information")
                        return directory.GetDirectories();
            //return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
        /// <summary>
        /// Gets files in a directory avoiding the restricted ones
        /// </summary>
        /// <returns>An array of files</returns>
        public FileInfo[] AccessFiles()
        {
            try
            {
                return directory.GetFiles();
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }
        public DirectoryInfo Directory
        {
            get
            {
                return directory;
            }
        }
        public DirectoryInfo[] SubDirectories
        {
            get
            {
                return subdirectories;
            }
        }
        public FileInfo[] Files
        {
            get
            {
                return files;
            }
        }
        public string Path
        {
            get
            {
                return path;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
