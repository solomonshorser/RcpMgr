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
    public delegate void RecipeOperandChangedEventHandler(object sender, EventArgs args);
    public delegate void RecipeStepMovedUpEventHandler(object sender, EventArgs args);
    public delegate void RecipeStepMovedDownEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for RecipeStepControl.xaml
    /// </summary>
    public partial class RecipeStepControl : UserControl
    {
        private RecipeStep _recipeStep = new RecipeStep();

        public event RecipeOperandChangedEventHandler OperandChanged;
        public event RecipeStepMovedUpEventHandler MovedUp;
        public event RecipeStepMovedDownEventHandler MovedDown;

        public RecipeStepControl()
        {
            InitializeComponent();
            //this.NameTextBox.Text = this._recipeStep.Name;
            this.NameTextBox.DataContext = this._recipeStep;
            this.sequenceNumberLabel.DataContext = this._recipeStep;
            this.NotesTextBox.DataContext = this._recipeStep;
            //this.OperandsBox.ItemsSource = this._recipeStep.Operands;
        }


        public RecipeStepControl(String name, String ID, String detail, int seqNum)
        {
            InitializeComponent();
            //this.NameTextBox.Text = name;

            RecipeStep rs = new RecipeStep();

            rs.Name = name;
            rs.ID = ID;
            rs.Details = detail;
            rs.SequenceNumber = seqNum;

            this.sequenceNumberLabel.DataContext = rs;
            this.NameTextBox.DataContext = rs;
            this.NameTextBox.Text = rs.Name;
            this.NotesTextBox.DataContext = rs;
            this._recipeStep = rs;
        }
        public void SetSequenceNumber(int seq)
        {
            this._recipeStep.SequenceNumber = seq;
            this.sequenceNumberLabel.Content = seq; //this shouldn't be necessary but I can't seem to get binding to work.
        }
        public RecipeStepControl(RecipeStep recipeStep)
        {
            InitializeComponent();
            
            //this.NameTextBox.Text = this._recipeStep.Name;
            this.NameTextBox.DataContext = recipeStep;
            this.sequenceNumberLabel.DataContext = recipeStep;
            this.NotesTextBox.DataContext = recipeStep;
            //this.OperandsBox.ItemsSource = _recipeStep.Operands;
            this._recipeStep = recipeStep;
            
            foreach (RecipeComponent rComponent in recipeStep.Operands)
            {
                if (rComponent is Ingredient)
                {
                    IngredientControl ingredientControl = new IngredientControl(rComponent as Ingredient);
                    addIngredientToRecipeStep(ingredientControl, this);
                }
                else if (rComponent is RecipeStep)
                {
                    RecipeStepControl recipeStepControl = new RecipeStepControl(rComponent as RecipeStep);
                    addRecipeStepControl(recipeStepControl);
                }
            }
        }


        public String Summary
        {
            get
            {
                String summary;
                summary = this.NameTextBox.Text + "(";
                int i = 0; 
                foreach (object l in this.OperandsBox.Items)
                {
                    if (l is RecipeComponentLabel)
                        summary += ((RecipeComponentLabel)l).Content;

                    i++;
                    if (i < this.OperandsBox.Items.Count)
                        summary += ", ";

                }
                summary += ")";
                return summary;
            }
        }


        public RecipeStep Step
        {
            get
            {
                return _recipeStep;
            }
        }


        private void OperandsBox_Drop(object sender, DragEventArgs e)
        {
           // String i= (String) e.Data.GetData(DataFormats.StringFormat);
            IngredientControl ictrl = (IngredientControl)e.Data.GetData(Type.GetType("RcpMgr3.IngredientControl"));
            RecipeStepControl rctrl = (RecipeStepControl)e.Data.GetData(Type.GetType("RcpMgr3.RecipeStepControl"));
            
            if (ictrl==null && rctrl ==null)
                e.Effects = DragDropEffects.None;
            else
                addIngredientToRecipeStep(ictrl, rctrl);

        }

        private void addIngredientToRecipeStep(IngredientControl ictrl, RecipeStepControl rctrl)
        {
            if (ictrl != null)
            {
                addIngredientControl(ictrl);
            }
            else if (rctrl != null)
            {
                addRecipeStepControl(rctrl);
            }
            
        }

        internal void addRecipeStepControl(RecipeStepControl rctrl)
        {
            _recipeStep.AddOperand(rctrl.Step);
            String r = rctrl.Summary;
            if (r != null && rctrl.Step.ID != this.Step.ID && okToAdd(rctrl.Step))
            {
                RecipeComponentLabel l = new RecipeComponentLabel();
                l.Text = r;

                l.RecipeComponentDeleted += (object o, EventArgs args) =>
                {
                    _recipeStep.Operands.Remove(rctrl.Step);
                };

                rctrl.OperandChanged += (object o, EventArgs args) =>
                {
                    l.Text = rctrl.Summary;
                };

                this.OperandsBox.Items.Add(l);
            }
        }

        private bool okToAdd(RecipeStep rctrl)
        {
            bool ok = true;
            if (rctrl.ID == this.Step.ID)
            {
                ok = false;
            }
            else
            {
                foreach (RecipeComponent rcomp in rctrl.Operands.FindAll( (RecipeComponent r) => { return r.GetType().Equals (Type.GetType("RcpMgr3.RecipeStep")); } ))
                {
                    //Type t = rcomp.GetType();
                    //if (t.Equals(Type.GetType("RcpMgr3.RecipeStep")))
                        ok = okToAdd((RecipeStep)rcomp);
                        if (!ok)
                            return ok;
                }
            }
            return ok;

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            String stepInfo = this.Summary;

            if (stepInfo.Trim().Length > 0 && this.NameTextBox.Text.Trim().Length > 0)
                DragDrop.DoDragDrop(this, this, System.Windows.DragDropEffects.Copy);
        }

        private void MoveStepUpButton_Click(object sender, RoutedEventArgs e)
        {
            MovedUp.Invoke(sender, e);
        }

        private void MoveStepDownButton_Click(object sender, RoutedEventArgs e)
        {
            MovedDown.Invoke(sender, e);
        }

        private void OperandsBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && this.OperandsBox.SelectedItem != null)
            {
                ((RecipeComponentLabel)this.OperandsBox.SelectedItem).Delete(sender,e);
                this.OperandsBox.Items.Remove(this.OperandsBox.SelectedItem);
                if (OperandChanged != null)
                    this.OperandChanged.Invoke(sender, e);
            }
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Step.Name = this.NameTextBox.Text;
            if (OperandChanged!=null)
                this.OperandChanged.Invoke(sender, e);
        }

        internal void addIngredientControl(IngredientControl ictrl)
        {
            if (!_recipeStep.Operands.Any((RecipeComponent c) => { return c.ID == ictrl.Ingredient.ID; }))
            {
                _recipeStep.AddOperand(ictrl.Ingredient);
                String i = ictrl.SummaryString;
                if (i != null)
                {
                    RecipeComponentLabel ingInfoLabel = new RecipeComponentLabel();
                    ingInfoLabel.Text = i;

                    ictrl.IngredientChanged += (object o, EventArgs args) =>
                    {
                        ingInfoLabel.Text = ictrl.SummaryString;
                    };

                    ingInfoLabel.TextChanged += (object o, EventArgs args) =>
                    {
                        if (OperandChanged != null) OperandChanged(o, args);
                    };

                    ingInfoLabel.RecipeComponentDeleted += (object o, EventArgs args) =>
                    {
                        _recipeStep.Operands.Remove(ictrl.Ingredient);
                    };
                    this.OperandsBox.Items.Add(ingInfoLabel);
                }
            }
        }

        internal RecipeStepControl getStep(string stepID)
        {

            return null;
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            this.backgroundBox.Fill = SystemColors.ActiveCaptionBrush;
            this.NameTextBox.Background = SystemColors.ActiveCaptionBrush;
            this.NameLabel.Background = SystemColors.ControlLightBrush;
            this.NameLabel.ToolTip = "Drag this step to another step.";
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            this.backgroundBox.Fill = SystemColors.WindowBrush;
            this.NameTextBox.Background = SystemColors.WindowBrush;
            this.NameLabel.Background = SystemColors.WindowBrush;
        }

        private void NotesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Step.Details = this.NotesTextBox.Text;
        }
    }
}