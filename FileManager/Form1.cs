using System;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FileManager
{
    public partial class FileManager : Form
    {
        Directories currentDir;
        
        public FileManager()
        {
            InitializeComponent();
            //used for testing in order to avoid long time loading
            PopulateTreeView(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            //Gets all folders and files from the computer
            //DriveInfo[] allDrives = DriveInfo.GetDrives();
            //foreach (DriveInfo d in allDrives)
            //{
            //    if (d.IsReady)
            //        PopulateTreeView(d.Name);
            //}
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }
        /// <summary>
        /// Populates the tree view with a directory and its subdirectories
        /// </summary>
        /// <param name="path">The path of the source directory</param>
        private void PopulateTreeView(string path)
        {
            TreeNode rootNode;
            currentDir = new Directories(path);
            if (currentDir.Directory.Exists)
            {
                rootNode = new TreeNode(currentDir.Name);
                rootNode.Tag = currentDir.Directory;
                AddSubNodes(currentDir.SubDirectories, rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }
        /// <summary>
        /// Adds secondary nodes representing subdirectories
        /// </summary>
        /// <param name="subDirs">The array of subdirectories</param>
        /// <param name="nodeToAddTo">The superior node</param>
        private void AddSubNodes(DirectoryInfo[] subDirs,TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            if(subDirs!=null)
            foreach (DirectoryInfo subDir in subDirs)
            {
                currentDir = new Directories(subDir);
                aNode = new TreeNode(currentDir.Directory.Name, 0, 0);
                aNode.Tag = currentDir.Directory;
                aNode.ImageKey = "folder";
                if (currentDir.SubDirectories != null)
                    {
                        AddSubNodes(currentDir.SubDirectories, aNode);
                        nodeToAddTo.Nodes.Add(aNode);
                    }
            }
        }
        /// <summary>
        /// Shows the clicked directory content in the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            currentDir=new Directories((DirectoryInfo)newSelected.Tag);
            PopulateListView(currentDir);
            
        }

        /// <summary>
        /// Picks an icon for every file format
        /// </summary>
        /// <param name="item"></param>
        private void SetImageExtension(ListViewItem item)
        {
            if (item.Text.Contains("jpg") || item.Text.Contains("JPG") || item.Text.Contains("png") || item.Text.Contains("bmp") || item.Text.Contains("jpeg") || item.Text.Contains("gif"))
                item.ImageIndex = 3;
            else if (item.Text.Contains("txt") || item.Text.Contains("pdf") || item.Text.Contains("doc") || item.Text.Contains("docx"))
                item.ImageIndex = 2;
            else if (item.Text.Contains("mp3") || item.Text.Contains("wav"))
                item.ImageIndex = 4;
            else if (item.Text.Contains("mov") || item.Text.Contains("mp4") || item.Text.Contains("avi") || item.Text.Contains("mkv") || item.Text.Contains("wmv"))
                item.ImageIndex = 5;
            else
                item.ImageIndex= 1;
        }
        /// <summary>
        /// Shows the content of the given directory in the list view
        /// </summary>
        /// <param name="dirinfo">Source directory</param>
        private void PopulateListView(Directories dirinfo)
        {
            listView1.Items.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            if (dirinfo.SubDirectories != null)
            {
                foreach (DirectoryInfo dir in dirinfo.SubDirectories)
                {
                    item = new ListViewItem(dir.Name, 0);
                    subItems = new ListViewItem.ListViewSubItem[]
                  {new ListViewItem.ListViewSubItem(item, "Directory"), 
                   new ListViewItem.ListViewSubItem(item, 
				dir.LastAccessTime.ToShortDateString())};
                    item.SubItems.AddRange(subItems);
                    item.Tag = dir;
                    item.ImageIndex = 0;
                    listView1.Items.Add(item);
                }
            }
            if (dirinfo.Files != null)
            {
                foreach (FileInfo file in dirinfo.Files)
                {
                    item = new ListViewItem(file.Name, 1);
                    subItems = new ListViewItem.ListViewSubItem[]
                  { new ListViewItem.ListViewSubItem(item, "File"), 
                   new ListViewItem.ListViewSubItem(item, 
				file.LastAccessTime.ToShortDateString())};
                    item.SubItems.AddRange(subItems);
                    item.Tag = file;
                    SetImageExtension(item);
                    listView1.Items.Add(item);
                }
            }
            comboBox1.Text = dirinfo.Path;
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        /// <summary>
        /// Acces the selected directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
                if (listView1.SelectedItems[0].Tag is DirectoryInfo)
                {
                    currentDir = new Directories((DirectoryInfo)listView1.SelectedItems[0].Tag);
                    PopulateListView(currentDir);
                }
                else
                {
                    FileInfo file = new FileInfo(((FileInfo)listView1.SelectedItems[0].Tag).FullName);
                    Process.Start(file.FullName);
                }

        }
        /// <summary>
        /// Populates the list view with the files/directories found at the given copyPaths
        /// </summary>
        /// <param name="copyPaths">List of copyPaths</param>
        private void AddToListView(string pathResult)
        {
            
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            DirectoryInfo dir;
            FileInfo file;
            if (pathResult != null)
            {
                if (Operations.CheckType(pathResult) == Operations.isDirectory)
                    {
                        dir = new DirectoryInfo(pathResult);
                        item = new ListViewItem(dir.Name, 0);
                        subItems = new ListViewItem.ListViewSubItem[]
                  {new ListViewItem.ListViewSubItem(item, "Directory"), 
                   new ListViewItem.ListViewSubItem(item, 
				dir.LastAccessTime.ToShortDateString())};
                        item.SubItems.AddRange(subItems);
                        item.Tag = dir;
                        item.ImageIndex = 0;
                        listView1.Items.Add(item);
                    }
                    else
                    {
                        file = new FileInfo(pathResult);
                        item = new ListViewItem(file.Name, 1);
                        subItems = new ListViewItem.ListViewSubItem[]
                  { new ListViewItem.ListViewSubItem(item, "File"), 
                   new ListViewItem.ListViewSubItem(item, 
				file.LastAccessTime.ToShortDateString())};
                        item.SubItems.AddRange(subItems);
                        item.Tag = file;
                        SetImageExtension(item);
                        listView1.Items.Add(item);
                    }
                }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Deletes the selected items after confirmation
        /// </summary>
        private void DeleteCommand()
        {
            if (listView1.SelectedItems.Count != 0 || listView1.CheckedItems.Count != 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete the selected files?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (listView1.CheckedItems.Count != 0)
                        foreach (ListViewItem item in listView1.CheckedItems)
                            Operations.DeleteItem(item.Tag);
                    else if (listView1.SelectedItems.Count != 0)
                        foreach (ListViewItem item in listView1.SelectedItems)
                            Operations.DeleteItem(item.Tag);
                    currentDir = new Directories(comboBox1.Text);
                    PopulateListView(currentDir);
                }
            }
        }
        /// <summary>
        /// Searches for files/directories in the computer by the keyword given in comboBox
        /// </summary>
        private void SearchCommand()
        {
            if (Directory.Exists(comboBox1.Text))
            {
                currentDir = new Directories(comboBox1.Text);
                PopulateListView(currentDir);
            }
            else
            {
                Operations show = new Operations();
                show.FoundResult += new Operations.ChangedListHandler(AddToListView);
                listView1.Items.Clear();
                this.Cursor = Cursors.WaitCursor;
                
                foreach (DriveInfo d in DriveInfo.GetDrives())
                {
                    if (d.IsReady)
                    {
                        currentDir = new Directories(d.Name);
                        show.DirSearch(comboBox1.Text, currentDir.Directory);
                    }
                }
                if (listView1.Items.Count != 0)
                {
                    comboBox1.Text = "Search results: " + comboBox1.Text;
                    MessageBox.Show(listView1.Items.Count + " results found", "Search done");
                }
                else
                    comboBox1.Text = "No results";
                this.Cursor = Cursors.Default;
            }
        }
        /// <summary>
        /// Inserts previously selected files into the current directory
        /// </summary>
        private void PasteCommand()
        {
            
            if (Operations.copyPaths.Count == 0)
                MessageBox.Show("No files to paste!");
            else
                if (Directory.Exists(comboBox1.Text))
                {
                    this.Cursor = Cursors.WaitCursor;
                    foreach (string path in Operations.copyPaths)
                    {
                        if (Operations.CheckType(path) == Operations.isFile)
                            Operations.FileCopy(path, comboBox1.Text);
                        else
                            Operations.DirectoryCopy(path, comboBox1.Text);
                    }
                    Operations.delAfterCopy = false;
                    Operations.copyPaths = null;
                    currentDir = new Directories(comboBox1.Text);
                    PopulateListView(currentDir);
                    this.Cursor = Cursors.Default;
                }
                else
                    MessageBox.Show("Invalid target");
        }
        /// <summary>
        /// Saves the selected files' to copy copyPaths into a list
        /// </summary>
        private void CopyCommand()
        {
            Operations.copyPaths = new List<string>();
            Operations.delAfterCopy = false;
            if(listView1.SelectedItems.Count!=0)
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    if (item.Tag is DirectoryInfo)
                    {
                        Operations.copyPaths.Add(((DirectoryInfo)item.Tag).FullName);
                    }
                    else
                    {
                        Operations.copyPaths.Add(((FileInfo)item.Tag).FullName);
                    }
                }
            else if(listView1.CheckedItems.Count!=0)
                foreach (ListViewItem item in listView1.CheckedItems)
                {
                    if (item.Tag is DirectoryInfo)
                    {
                        Operations.copyPaths.Add(((DirectoryInfo)item.Tag).FullName);
                    }
                    else
                    {
                        Operations.copyPaths.Add(((FileInfo)item.Tag).FullName);
                    }
                }
        }
        /// <summary>
        /// Saves the selected files' to copy copyPaths into a list and sets the "delete after copy" flag to true
        /// </summary>
        private void CutCommand()
        {
            CopyCommand();
            Operations.delAfterCopy = true;
        }


        //Buttons
        private void button3_Click(object sender, EventArgs e)
        {
            CopyCommand();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            CutCommand();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            PasteCommand();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            SearchCommand();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            DeleteCommand();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            currentDir = new Directories(comboBox1.Text);
            if (currentDir.Directory.Parent != null)
                currentDir = new Directories(currentDir.Directory.Parent);
            PopulateListView(currentDir);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            currentDir = new Directories(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            PopulateListView(currentDir);
        }
        //mainMenu
        private void copyToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            CopyCommand();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCommand();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PasteCommand();
        }
        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.SmallIcon;
        }

        private void listViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        private void tileViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.CheckBoxes = false;
            listView1.View = View.Tile;
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCommand();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        //secondaryMenu
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyCommand();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutCommand();
        }

        private void pasteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PasteCommand();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteCommand();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nichifor Ana-Maria", "Author");
        }

        private void itemCheckBoxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.CheckBoxes == true)
                listView1.CheckBoxes = false;
            else if(listView1.View!=View.Tile)
                listView1.CheckBoxes = true;
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
                if (listView1.SelectedItems[0].Tag is DirectoryInfo)
                {
                    currentDir = new Directories((DirectoryInfo)listView1.SelectedItems[0].Tag);
                    PopulateListView(currentDir);
                }
                else
                {
                    FileInfo file = new FileInfo(((FileInfo)listView1.SelectedItems[0].Tag).FullName);
                    Process.Start(file.FullName);
                }
        }    
    }
}
