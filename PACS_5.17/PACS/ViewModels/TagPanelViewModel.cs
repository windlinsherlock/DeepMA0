using PACS.Common;
using PACS.Commons.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PACS.ViewModels
{
    public class TagPanelViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogHostService dialog;

        /// <summary>
        /// 选中的Tag
        /// </summary>
        private TagItem selectedItem;
        public TagItem SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);

                if(selectedItem != null)
                {
                    Color = SelectedItem.Color;
                }
            }
        }

        /// <summary>
        /// 选中Tag的颜色
        /// </summary>
        private Color color;

        public Color Color 
        { 
            get { return color; } 
            set 
            { 
                color = value; 
                RaisePropertyChanged();

                eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
                {
                    Filter = "Color",
                    Object = Color,
                    Message = SelectedItem.Desc
                });
            } 
        }

        private ObservableCollection<TagItem> tagItems;
        public ObservableCollection<TagItem> TagItems
        {
            get { return tagItems; }
            set { tagItems = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 添加Tag
        /// </summary>
        public DelegateCommand SaveCommand { get; private set; }

        private async void Save()
        {
            DialogParameters param = new DialogParameters();

            var dialogResult = await dialog.ShowDialog("AddTagView", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                string TagName = dialogResult.Parameters.GetValue<string>("TagName");
                var item = new TagItem(dialog, eventAggregator) { Desc = TagName };
                TagItems.Add(item);
                item.ExecuteCommand.Execute();
            }
        }

        /// <summary>
        /// 删除Tag
        /// </summary>
        public DelegateCommand DeleteCommand { get; private set; }

        private void Delete()
        {
            TagItems.Remove(SelectedItem);
        }

        public TagPanelViewModel(IDialogHostService dialog,IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.dialog = dialog;

            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);

            TagItems = new ObservableCollection<TagItem>();
            TagItems.Add(new TagItem(dialog,eventAggregator) {  Color = Color.FromRgb(255,0,0), Desc = "病灶区域"});

            SelectedItem = TagItems[0];

            eventAggregator.GetEvent<MessageEvent>().Subscribe(ColorChanged, arg => arg.Filter.Equals("ColorChanged"));
            eventAggregator.GetEvent<MessageEvent>().Subscribe(CreateTag, arg => arg.Filter.Equals("Tag"));
        }

        /// <summary>
        /// 通过消息机制绑定选择Tag的颜色
        /// </summary>
        /// <param name="message"></param>
        public void ColorChanged(MessageModel message)
        {
            var item = (TagItem)message.Object;

            if(SelectedItem == item)
            {
                Color = item.Color;
            }
        }

        /// <summary>
        /// 根据消息创建Tag
        /// </summary>
        /// <param name="model"></param>
        public void CreateTag(MessageModel model)
        {
            var color = (Color)model.Object;

            foreach(var item in TagItems)
            {
                if (item.Desc.Equals(model.Message) && item.Color.Equals(color))
                    return;
            }

            TagItems.Add(new TagItem(dialog,eventAggregator) { Color = color, Desc = model.Message });
        }

    }

    public class TagItem : BindableBase
    {
        private Color color;

        public Color Color { get { return color; } set { color = value; RaisePropertyChanged(); } }

        public string Desc { get; set; }

        private readonly IDialogHostService dialog;
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// 修改颜色
        /// </summary>
        public DelegateCommand ExecuteCommand { get; private set; }

        private async void ExecuteAsync()
        {
            DialogParameters param = new DialogParameters();
            param.Add("Color", Color);


            var dialogResult = await dialog.ShowDialog("ColorPickerView", param);

            if (dialogResult.Result == ButtonResult.OK)
            {
                var r = dialogResult.Parameters.GetValue<byte>("R");
                var g = dialogResult.Parameters.GetValue<byte>("G");
                var b = dialogResult.Parameters.GetValue<byte>("B");
                Color = Color.FromRgb(r, g, b);

                eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
                {
                    Filter = "ColorChanged",
                    Object = this
                });

            }
            ;
        }

        

        public TagItem(IDialogHostService dialog, IEventAggregator eventAggregator)
        {
            this.dialog = dialog;
            this.eventAggregator = eventAggregator;
            ExecuteCommand = new DelegateCommand(ExecuteAsync);
        }
    }
}
