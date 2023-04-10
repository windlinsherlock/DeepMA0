using PACS.Commons.Models;
using PACS.Shared.Contact;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PACS.Commons
{
    /// <summary>
    /// 存储全局变量
    /// </summary>
    public class UserConfiguration
    {
        public static List<String> FileList = new List<String>();

        /// <summary>
        /// 待打开的文件夹
        /// </summary>
        private ObservableCollection<FileItem> fileItems;
        public ObservableCollection<FileItem> FileItems
        {
            get { return fileItems; }
            set { fileItems = value; }
        }

        private ObservableCollection<FileFolderModel> localFileFolders;
        public ObservableCollection<FileFolderModel> LocalFileFolders
        {
            get { return localFileFolders; }
            set { localFileFolders = value; }
        }

        private ObservableCollection<FileFolderModel> publicFileFolders;
        public ObservableCollection<FileFolderModel> PublicFileFolders
        {
            get { return publicFileFolders; }
            set { publicFileFolders = value; }
        }

        private ObservableCollection<FileFolderModel> privateFileFolders;
        public ObservableCollection<FileFolderModel> PrivateFileFolders
        {
            get { return privateFileFolders; }
            set { privateFileFolders = value; }
        }

        private ObservableCollection<AutoDiagnoseFolderDTO> autoDiagnoseFolders;
        public ObservableCollection<AutoDiagnoseFolderDTO> AutoDiagnoseFolders
        {
            get { return autoDiagnoseFolders; }
            set { autoDiagnoseFolders = value; }
        }

        public TokenResult Token { get; set; }

        public string UserId { get; set; }

        public bool IsAdmin { get; set; }

    }
}
