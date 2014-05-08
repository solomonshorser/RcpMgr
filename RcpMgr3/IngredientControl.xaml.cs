using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace RcpMgr3
{
    //public class IngredientChangedEventArgs : EventArgs
    //{
    //    public String SummaryString { get; set; }
    //}

    public delegate void IngredientChangedEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for IngredientControl.xaml
    /// </summary>
    [Serializable]
    public partial class IngredientControl : UserControl
    {
        private bool _uiEnabled = true;

        public event IngredientChangedEventHandler IngredientChanged;

        private Ingredient _ingredient;

        public Ingredient Ingredient
        {
            get
            {
                return _ingredient;
            }
        }


        public bool UIEnabled
        {
            get
            {
                return _uiEnabled;
            }

            set 
            {
                this.NameTextBox.IsReadOnly = !value;
                this.QuantityTextBox.IsReadOnly = !value;
                this.UnitOfMeasureTextBox.IsReadOnly = !value;

                if (value)
                    this.Background = SystemColors.InactiveBorderBrush;
                else
                    this.Background = SystemColors.ActiveBorderBrush;
            }
        }


        public IngredientControl()
        {
            InitializeComponent();
        }

        public IngredientControl(Ingredient i)
        {
            InitializeComponent();
            Dependents = new List<Label>();
            SetIngredient(i);
        }

        public void SetIngredient(Ingredient i)
        {
            this.IngredientIDLabel.DataContext = i;
            this.NameTextBox.DataContext = i;
            this.QuantityTextBox.DataContext = i;
            this.UnitOfMeasureTextBox.DataContext = i;
            _ingredient = i;
            this.updateDependents();
            
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            //String ingInfo = SummaryString;
            
            //if (ingInfo.Trim().Length>0)
            //    //DragDrop.DoDragDrop(this, ingInfo.Trim() + " ("+ this.IngredientIDLabel.Content.ToString()+")", System.Windows.DragDropEffects.Copy);
            //    DragDrop.DoDragDrop(this, this.SummaryString, System.Windows.DragDropEffects.Copy);
        }

        public String SummaryString
        {
            get
            {
                //return QuantityTextBox.Text + " " + UnitOfMeasureTextBox.Text + " " + NameTextBox.Text; 
                return this._ingredient.Quantity + " " + this._ingredient.UnitOfMeasure + " " + this._ingredient.Name;
            }
        }

        public List<Label> Dependents { get; set; }

        private void UnitOfMeasureTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            updateDependents();
        }

        private void updateDependents()
        {
            if (IngredientChanged != null)
            {
                EventArgs args = new EventArgs();
                IngredientChanged(this, args);
            }
            this.IngredientIDLabel.Content = this.SummaryString + " (" + this._ingredient.ID + ")";

            //_ingredient.Name = this.NameTextBox.Text;
            //_ingredient.Quantity = this.QuantityTextBox.Text;
            //_ingredient.UnitOfMeasure = this.UnitOfMeasureTextBox.Text;

            //if (Dependents!=null && Dependents.Count > 0)
            //{
            //    foreach (Label d in Dependents)
            //    {
            //        d.Content = SummaryString;
            //    }
            //}
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            updateDependents();
        }

        private void QuanityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            updateDependents();
        }

        private void IngredientIDLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleIDLabelMouseDown(sender, e);
        }

        private void IngredientIDLabel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void HandleIDLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            String ingInfo = SummaryString;

            if (ingInfo.Trim().Length > 0)
                //DragDrop.DoDragDrop(this, ingInfo.Trim() + " ("+ this.IngredientIDLabel.Content.ToString()+")", System.Windows.DragDropEffects.Copy);
                DragDrop.DoDragDrop(this, this, System.Windows.DragDropEffects.Copy);
        }
    }
}
