using Microsoft.WindowsAPICodePack.Dialogs;
using PACS.Commons.Entities;
using PACS.Commons.Events;
using PACS.Commons.Models;
using PACS.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using PACS.Commons;
using PACS.Shared.Entities;
using Prism.Regions;
using PACS.Extensions;
using PACS.Views;
using PACS.Services;
using System.Threading.Tasks;
using PACS.Shared.DTOs;


/// <summary>
/// 5.4 16：12
/// 更改显示方式
/// </summary>
namespace PACS.ViewModels
{
    class AutoDiagnoseFoldersViewModel : BindableBase
    {

        private bool fileVisibility;
        public bool FileVisibility
        {
            get { return fileVisibility; }
            set
            {
                fileVisibility = value;
                RaisePropertyChanged();
            }
        }
        private object? _selectedItem;
        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }

        /// <summary>
        /// 前端显示文件夹
        /// </summary>
        private ObservableCollection<AutoDiagnoseFolderDTO> autoDiagnoseFolders;
        public ObservableCollection<AutoDiagnoseFolderDTO> AutoDiagnoseFolders
        {
            get { return autoDiagnoseFolders; }
            set { autoDiagnoseFolders = value; RaisePropertyChanged(); }
        }



        private readonly IEventAggregator eventAggregator;
        private UserConfiguration userConfiguration;
        private IAutoDiagnoseService autoDiagnoseService;

        public AutoDiagnoseFoldersViewModel(IEventAggregator eventAggregator, UserConfiguration userConfiguration, IAutoDiagnoseService autoDiagnoseService)
        {
            FileVisibility = true;

            NavigateCommand = new DelegateCommand<AutoDiagnoseFolderDTO>(Navigate);
            BackCommand = new DelegateCommand<String>(Back);

            // 依赖注入
            this.eventAggregator = eventAggregator;
            AutoDiagnoseFolders=userConfiguration.AutoDiagnoseFolders;
            this.userConfiguration = userConfiguration;
            //eventAggregator.GetEvent<MessageEvent>().Subscribe(LoadAutoDiagnoseFolders, arg => arg.Filter.Equals("LoadAutoDiagnoseFolders"));
            this.autoDiagnoseService = autoDiagnoseService;
        }

        /// <summary>
        /// 切换文件夹、文件视图显示
        /// </summary>
        public DelegateCommand<String> BackCommand { get; private set; }

        private void Back(String obj)
        {
            FileVisibility = !FileVisibility;
        }


        //public async void LoadAutoDiagnoseFolders(MessageModel obj)
        //{
        //    var response = await autoDiagnoseService.GetAutoDiagnoseFolders();
        //    if(response.Status)
        //    {
        //        this.autoDiagnoseFolders = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<AutoDiagnoseFolderDTO>>((string)response.Result);
        //    }
        //}

        /// <summary>
        /// 进入选定的文件夹
        /// </summary>
        public DelegateCommand<AutoDiagnoseFolderDTO> NavigateCommand { get; private set; }

        private void Navigate(AutoDiagnoseFolderDTO obj)
        {
            if (obj == null)
                return;
            FileVisibility = !FileVisibility;
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "OpenAutoDiagnoseFile",
                Message = obj.AutoDiagnoseItemId,
            });

        }

    }
}

