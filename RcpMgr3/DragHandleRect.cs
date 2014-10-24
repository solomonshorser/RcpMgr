using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
namespace RcpMgr3
{
    class DragHandleRect : UserControl
    {
        private Brush _onHoverBrush;
        private Brush _onClickBrush;
        private Brush _idleBrush;

        public DragHandleRect()
        {
            this.MouseDown += DragHandleRect_MouseDown;
            this.MouseEnter += DragHandleRect_MouseEnter;
            this.MouseUp += DragHandleRect_MouseUp;
        }

        void DragHandleRect_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Background = this._idleBrush;
        }

        void DragHandleRect_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Background = this._onClickBrush;
        }

        void DragHandleRect_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Background = this._onHoverBrush;
        }

        public Brush OnHoverBrush
        {
            get
            {
                return this._onHoverBrush;
            }
            set
            {
                this._onHoverBrush = value; 
            }
        }

        public Brush OnClickBrush
        {
            get
            {
                return this._onClickBrush;
            }
            set
            {
                this._onClickBrush = value;
            }
        }

        public Brush IdleBrush
        {
            get
            {
                return this._idleBrush;
            }

            set
            {
                this._idleBrush = value;
                this.Background = this._idleBrush;
            }
        }
    }
}
