using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace RcpMgr3
{
    public class EnhancedStackPanel : StackPanel
    {
        /// <summary>
        /// Moves an object up in the list.
        /// </summary>
        /// <param name="o"></param>
        public void MoveItemUp(object o)
        {
            //int i = Items.IndexOf(o);
            int i = Children.IndexOf((UIElement)o);
            this.MoveItemUp(i);
        }

        public void MoveItemDown(object o)
        {
            int i = Children.IndexOf((UIElement)o);
            this.MoveItemDown(i);
        }

        /// <summary>
        /// Moves item at index i up in the list.
        /// </summary>
        /// <param name="i"></param>
        public void MoveItemUp(int i)
        {
            if (Children.Count >= 2 && i >=1)
            {
                this.SwapItems(i, i - 1);
            }
        }

        /// <summary>
        /// Moves an item at index i down in the list.
        /// </summary>
        /// <param name="i"></param>
        public void MoveItemDown(int i)
        {
            if (Children.Count >= 2 && i < Children.Count-1)
            {
                this.SwapItems(i, i + 1);
            }
        }

        
        virtual protected void SwapItems(int i, int j)
        {
            UIElement temp = Children[i];
            this.Children.RemoveAt(i);
            this.Children.Insert(j, temp);
        }
    }
}
