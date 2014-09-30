using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RcpMgr3
{
    public class LabelWithTextUpdateEvents : Label
    {

        public event EventHandler TextChanged;

        public String Text
        {
            get
            {
                return (String)this.Content;
            }

            set
            {
                String oldContent = null;
                if (this.Content != null)
                {
                    oldContent = this.Content.ToString();
                }

                this.Content = value;
                    
                if ((oldContent != null && !oldContent.Equals(value)) ||
                    (oldContent == null && value != null))
                {
                    if (TextChanged != null)
                    {
                        EventArgs args = new EventArgs();
                        TextChanged(this, args);
                    }
                }
                
                //else
                //{
                //    this.Content = value;
                //    if (TextChanged != null)
                //    {
                //        EventArgs args = new EventArgs();
                //        TextChanged(this, args);
                //    }
                //}
            }
        }
    }

    public class RecipeComponentLabel : LabelWithTextUpdateEvents
    {
        public event EventHandler RecipeComponentDeleted;

        public String RecipeComponentID { get; set; }

        public void Delete(object o, EventArgs args)
        {
            RecipeComponentDeleted.Invoke(o, args);
        }
    }
}
