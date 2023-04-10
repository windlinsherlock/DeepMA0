using PACS.Extensions;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PACS.Models
{
    public class NavigatorItem : BindableBase
    {
        private readonly IRegionManager _regionManager;

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }


        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Title { get; set; }
 

        /// <summary>
        /// 菜单命名空间
        /// </summary>
        public string NameSpace { get; set; }



        /// <summary>
        /// 菜单命名空间
        /// </summary>
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (IsSelected)
                {
                    
                }
            } 
        }
    }
}
