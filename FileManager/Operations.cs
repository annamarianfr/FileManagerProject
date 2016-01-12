using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Threading;

namespace FileManager
{
    public class Operations
    {
        public static bool isDirectory = true;
        public static bool isFile = false;
        public static List<string> copyPaths;
        public static bool delAfterCopy = false;
        public delegate void ChangedListHandler(string pathResult);
        public event ChangedListHandler FoundResult;

        /// <summary>
        /// Check whether the object is a directory or a file
        /// </summary>
        /// <param name="item">The object to check</param>
        /// <returns>True if it is a directory or false if it is a file</returns>
        public static bool CheckType(Object item)
        {
            if (item is DirectoryInfo)
                return isDirectory;
            else
                return isFile;   
        }
        /// <summary>
        /// Checks whether the given string is a path to a directory or a file
        /// </summary>
        /// <param name="item">Item's path</param>
        /// <returns>True if it is a directory or false if it is a file</returns>
        public static bool CheckType(string item)
        {
            if (Directory.Exists(item))
                return isDirectory;
            else
                return isFile;
        }
        /// <summary>
        /// Deletes directories or files
        /// </summary>
        /// <param name="item">The object to delete</param>
        public static void DeleteItem(Object item)
        {
            if(CheckType(item)==isDirectory)
                Directory.Delete(((DirectoryInfo)item).FullName, true);
            else
                File.Delete(((FileInfo)item).FullName);
        }
        /// <summary>
        /// Deletes directories or files after they have been copied
        /// </summary>
        /// <param name="item">Item's path</param>
        public static void CutItem(string item)
        {
            if (CheckType(item) == isDirectory)
            {
                DirectoryInfo cutDir = new DirectoryInfo(item);
                Directory.Delete(cutDir.FullName, true);
            }
            else
            {
                FileInfo cutFile = new FileInfo(item);
                File.Delete(cutFile.FullName);
            }
        }
        /// <summary>
        /// Searches for files/directories by keyword
        /// </summary>
        /// <param name="keyw">The keyword to search for</param>
        /// <param name="dir">The directory to search in</param>
        public void DirSearch(string keyw, DirectoryInfo dir)
        {
            try
            {
                foreach (FileInfo f in dir.GetFiles())
                    if (f.Name.Contains(keyw))
                        FoundResult(f.FullName);
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    if (d.Name.Contains(keyw))
                    {
                        FoundResult(d.FullName);
                    }
                    DirSearch(keyw,d);
                }
            }
            catch (System.Exception ex)
            {
                
            }
        }
        /// <summary>
        /// Copies files
        /// </summary>
        /// <param name="sourcePath">File to copy path</param>
        /// <param name="targetPath">Destination path</param>
        public static void FileCopy(string sourcePath, string targetPath)
        {
            FileInfo sourceFile = new FileInfo(sourcePath);
            string destination = Path.Combine(targetPath, sourceFile.Name);
            File.Copy(sourceFile.FullName, destination, true);
            if (delAfterCopy)
                CutItem(sourcePath);
        }

        /// <summary>
        /// Copies directories and their content
        /// </summary>
        /// <param name="sourcePath">Directory to copy path</param>
        /// <param name="targetPath">Destination path</param>
        public static void DirectoryCopy(string sourcePath, string targetPath)
        {
            DirectoryInfo dir = new DirectoryInfo(sourcePath);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourcePath);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            targetPath = Path.Combine(targetPath, dir.Name);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(targetPath, file.Name);
                
                file.CopyTo(temppath, true);
            }

            if(dirs.Length!=0)
                foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(targetPath, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath);
                    }
            if (delAfterCopy)
                CutItem(sourcePath);
        }
        /// <summary>
        /// Check folder permissions
        /// </summary>
        /// <param name="directoryPath">Directory path</param>
        /// <param name="accessType">The type of access which is being checked</param>
        /// <returns></returns>
        public static bool CheckFolderPermissions(string directoryPath, FileSystemRights accessType)
        {
            bool hasAccess = true;
            try
            {
                AuthorizationRuleCollection collection = Directory.GetAccessControl(directoryPath).GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                foreach (FileSystemAccessRule rule in collection)
                {
                    if ((rule.FileSystemRights & accessType) > 0)
                    {
                        return hasAccess;
                    }
                }
            }
            catch (Exception ex)
            {
                hasAccess = false;
            }
            return hasAccess;
        }
    }
}

