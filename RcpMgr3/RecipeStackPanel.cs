using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RcpMgr3
{
    class RecipeStackPanel : EnhancedStackPanel
    {
        override protected void SwapItems(int i, int j)
        {
            RecipeStepControl temp = (RecipeStepControl)Children[i];

            ((RecipeStepControl)Children[i]).SetSequenceNumber(j+1);
            ((RecipeStepControl)Children[j]).SetSequenceNumber(i+1);

            this.Children.RemoveAt(i);
            this.Children.Insert(j, temp);
        }
    }
}
