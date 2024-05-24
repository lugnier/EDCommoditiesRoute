using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.TreeView.Engine;


namespace Toto
{
    public class CommoditiesByStationsTreeViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Level0 { get; set; }
        public DataTemplate Level1 { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var treeNodeView = item as TreeViewNode;
            if (treeNodeView == null)
            {
                return null;
            }

            if (treeNodeView.Level == 0)
            {
                return Level0;
            }
            else
            {
                return Level1;
            }
        }
    }
}
