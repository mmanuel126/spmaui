#region Copyright Syncfusion Inc. 2001-2021.
// Copyright Syncfusion Inc. 2001-2021. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Runtime;
using TreeView.Maui.Core;

namespace spmaui.Helper
{
    #region MessageTemplateSelector
    public class ItemTemplateSelector : DataTemplateSelector
    {
        #region Properties
        public DataTemplate ConversationTemplate { get; set; }
        public DataTemplate ReplyTemplate { get; set; }

        #endregion

        #region Constructor
        public ItemTemplateSelector()
        {
        }
        #endregion

        #region OnSelectTemplate

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
           // var node = item as TreeViewNode;
           // var level = node.;
           // if (level == 0)
            //    return ConversationTemplate;
            return ReplyTemplate;
        }
        #endregion
    }

    #endregion
}
