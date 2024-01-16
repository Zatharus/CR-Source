// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.FolderTreeView
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Collections;
using cYo.Common.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class FolderTreeView : TreeViewEx
  {
    private readonly ImageList myImageList;

    public FolderTreeView()
    {
      this.SortNetworkFolders = true;
      this.myImageList = new ImageList()
      {
        ColorDepth = ColorDepth.Depth32Bit,
        ImageSize = new System.Drawing.Size(16, 16),
        TransparentColor = Color.Transparent
      };
      this.myImageList.ImageSize = this.myImageList.ImageSize.ScaleDpi();
      this.myImageList.Images.Add(FolderTreeView.NativeMethods.GetDesktopIcon());
      this.ImageList = this.myImageList;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        FolderTreeView.ClearTree((TreeView) this);
        this.myImageList.Dispose();
      }
      base.Dispose(disposing);
    }

    public void Init()
    {
      FolderTreeView.ClearTree((TreeView) this);
      FolderTreeView.ShellOperations.PopulateTree((TreeView) this, this.ImageList, this.SortNetworkFolders);
      if (this.Nodes.Count <= 0)
        return;
      this.Nodes[0].Expand();
    }

    private static void ClearTree(TreeView tree)
    {
      tree.AllNodes().Select<TreeNode, object>((Func<TreeNode, object>) (tn => tn.Tag)).OfType<IDisposable>().ForEach<IDisposable>((Action<IDisposable>) (d => d.Dispose()));
      tree.Nodes.Clear();
    }

    protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
    {
      this.BeginUpdate();
      try
      {
        FolderTreeView.ShellOperations.ExpandBranch(e.Node, this.ImageList, this.SortNetworkFolders);
      }
      finally
      {
        this.EndUpdate();
      }
    }

    protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
    {
      this.BeginUpdate();
      try
      {
        FolderTreeView.ShellOperations.ExpandBranch(e.Node, this.ImageList, this.SortNetworkFolders);
      }
      finally
      {
        this.EndUpdate();
      }
    }

    [DefaultValue(true)]
    public bool SortNetworkFolders { get; set; }

    public string GetSelectedNodePath()
    {
      return this.SelectedNode != null ? FolderTreeView.ShellOperations.GetFilePath(this.SelectedNode) : string.Empty;
    }

    public bool DrillToFolder(string folderPath)
    {
      bool folderFound = false;
      try
      {
        if (Directory.Exists(folderPath))
        {
          if (folderPath.Length > 3 && folderPath.LastIndexOf("\\") == folderPath.Length - 1)
            folderPath = folderPath.Substring(0, folderPath.Length - 1);
          this.DrillTree(this.Nodes[0].Nodes, folderPath, ref folderFound);
        }
      }
      catch
      {
      }
      if (!folderFound)
        this.SelectedNode = this.Nodes[0];
      return folderFound;
    }

    private void DrillTree(TreeNodeCollection tnc, string path, ref bool folderFound)
    {
      if (path == null)
        return;
      foreach (TreeNode tn in tnc)
      {
        if (folderFound)
          break;
        string filePath = FolderTreeView.ShellOperations.GetFilePath(tn);
        if (string.Equals(path, filePath, StringComparison.OrdinalIgnoreCase))
        {
          this.SelectedNode = tn;
          tn.EnsureVisible();
          folderFound = true;
        }
        else if (string.IsNullOrEmpty(filePath) || path.StartsWith(filePath, StringComparison.OrdinalIgnoreCase))
        {
          tn.Expand();
          this.DrillTree(tn.Nodes, path, ref folderFound);
        }
      }
    }

    public static class NativeMethods
    {
      [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
      private static extern uint ExtractIconEx(
        string lpszFile,
        int nIconIndex,
        IntPtr[] phiconLarge,
        IntPtr[] phiconSmall,
        uint nIcons);

      public static Icon GetDesktopIcon()
      {
        IntPtr[] phiconLarge = new IntPtr[1];
        IntPtr[] phiconSmall = new IntPtr[1];
        int iconEx = (int) FolderTreeView.NativeMethods.ExtractIconEx(Environment.SystemDirectory + "\\shell32.dll", 34, phiconLarge, phiconSmall, 1U);
        return Icon.FromHandle(phiconSmall[0]);
      }
    }

    public static class ShellOperations
    {
      public static Image GetImage(ShellPidl item, bool selected)
      {
        return item.GetImage((ShellIconType) (16 | (selected ? 1 : 0)));
      }

      public static string GetFilePath(TreeNode tn)
      {
        try
        {
          ShellFolder tag = (ShellFolder) tn.Tag;
          if (Directory.Exists(tag.Pidl.PhysicalPath))
            return tag.Pidl.PhysicalPath;
        }
        catch (Exception ex)
        {
        }
        return string.Empty;
      }

      public static void PopulateTree(TreeView tree, ImageList imageList, bool sortNetworkFolders)
      {
        FolderTreeView.ClearTree(tree);
        try
        {
          ShellFolder shellFolder = new ShellFolder(Environment.SpecialFolder.Desktop);
          TreeNode treeNode = new TreeNode(shellFolder.Pidl.DisplayName, 0, 0)
          {
            Tag = (object) shellFolder
          };
          tree.Nodes.Add(treeNode);
          FolderTreeView.ShellOperations.FillNode(treeNode, imageList, sortNetworkFolders);
        }
        catch (Exception ex)
        {
        }
        if (tree.Nodes.Count <= 1)
          return;
        tree.SelectedNode = tree.Nodes[1];
        FolderTreeView.ShellOperations.ExpandBranch(tree.Nodes[1], imageList, sortNetworkFolders);
      }

      public static void ExpandBranch(TreeNode tn, ImageList imageList, bool sortNetworkFolders)
      {
        if (tn.Nodes.Count != 1 || tn.Nodes[0].Tag != null)
          return;
        tn.Nodes.Clear();
        FolderTreeView.ShellOperations.FillNode(tn, imageList, sortNetworkFolders);
      }

      [DllImport("shlwapi.dll")]
      private static extern bool PathIsNetworkPath(string pszPath);

      public static void FillNode(TreeNode tn, ImageList imageList, bool sortNetworkFolders)
      {
        try
        {
          ShellFolder tag = (ShellFolder) tn.Tag;
          List<ShellPidl> children = tag.GetChildren(false, false, true);
          try
          {
            List<TreeNode> treeNodeList = new List<TreeNode>();
            foreach (ShellPidl pidl in children)
            {
              if (!pidl.IsBrowsable && !pidl.IsNetwork && !pidl.IsControlPanel && !pidl.IsRecycleBin && (!string.IsNullOrEmpty(pidl.PhysicalPath) || pidl.HasSubfolders))
              {
                TreeNode treeNode = FolderTreeView.ShellOperations.AddTreeNode(new ShellFolder(pidl), imageList, true);
                treeNodeList.Add(treeNode);
              }
            }
            if (sortNetworkFolders && FolderTreeView.ShellOperations.PathIsNetworkPath(tag.Pidl.PhysicalPath))
              treeNodeList.Sort((Comparison<TreeNode>) ((a, b) => string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase)));
            foreach (TreeNode treeNode in treeNodeList)
            {
              tn.Nodes.Add(treeNode);
              FolderTreeView.ShellOperations.CheckForSubDirs(treeNode);
            }
          }
          finally
          {
            children.ForEach((Action<ShellPidl>) (p => p.Dispose()));
          }
        }
        catch
        {
        }
      }

      private static bool CheckForSubDirs(TreeNode tn)
      {
        if (tn.Nodes.Count != 0)
          return true;
        try
        {
          if (tn.Tag is ShellFolder tag)
          {
            if (tag.Pidl.HasSubfolders)
            {
              TreeNode node = new TreeNode()
              {
                Tag = (object) null
              };
              tn.Nodes.Add(node);
              return true;
            }
          }
        }
        catch
        {
        }
        return false;
      }

      private static TreeNode AddTreeNode(ShellFolder item, ImageList imageList, bool getIcons)
      {
        TreeNode treeNode = new TreeNode()
        {
          Text = item.Pidl.DisplayName,
          Tag = (object) item
        };
        if (getIcons)
        {
          try
          {
            treeNode.ImageKey = item.Pidl.IconKey;
            treeNode.SelectedImageKey = treeNode.ImageKey + "S";
            if (!imageList.Images.ContainsKey(item.Pidl.IconKey))
            {
              Image image1 = FolderTreeView.ShellOperations.GetImage(item.Pidl, false);
              imageList.Images.Add(treeNode.ImageKey, image1);
              image1.Dispose();
              Image image2 = FolderTreeView.ShellOperations.GetImage(item.Pidl, true);
              imageList.Images.Add(treeNode.SelectedImageKey, image2);
              image2.Dispose();
            }
            return treeNode;
          }
          catch
          {
          }
        }
        treeNode.ImageIndex = 1;
        treeNode.SelectedImageIndex = 2;
        return treeNode;
      }
    }
  }
}
