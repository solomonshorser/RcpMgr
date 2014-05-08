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
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;

namespace RcpMgr3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IngredientControl> _iCtrls = new List<IngredientControl>();

        private Recipe rcp = new Recipe();

        private RecipeManager rmgr;

        public MainWindow()
        {
            InitializeComponent();
            rmgr = new RecipeManager(rcp);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void NewIngredientMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Ingredient i = new Ingredient();
            addNewIngredient(i);
        }

        private void addNewIngredientControl(IngredientControl ic)
        {
            ic.Margin = new Thickness(3, 3, 3, 3);
            _iCtrls.Add(ic);

            IngredientsStackPanel.Children.Add(ic);
        }

        private void addNewIngredient(Ingredient i)
        {
            IngredientControl ic = new IngredientControl(i);
            //this.IngredientsListBox.Items.Add(ic);
            ic.Margin = new Thickness(3, 3, 3, 3);
            _iCtrls.Add(ic);

            IngredientsStackPanel.Children.Add(ic);
        }


        private void NewRecipeStepMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RecipeStepControl rsc = new RecipeStepControl();
            addNewStep(rsc);
        }

        private void addNewStep(RecipeStepControl rsc)
        {
            //rsc.OperandsBox.Drop += (object dragSender, DragEventArgs dragEvtArgs) =>
            //{
            //    String i = (String)dragEvtArgs.Data.GetData(DataFormats.StringFormat);

            //    if (i != null)
            //    {
            //        Label ingInfoLabel = new Label();
            //        ingInfoLabel.Content = i;

            //        rsc.OperandsBox.Items.Add(ingInfoLabel);
            //    }
            //};

            //this.StepListBox.Items.Add(rsc);
//            rsc.NameTextBox.DataContext = rsc.Step.Name;
            rsc.Margin = new Thickness(3, 3, 3, 3);
            
            this.StepStackPanel.Children.Add(rsc);
            //rsc.sequenceNumberLabel.DataContext = rsc.Index;
            //rsc.sequenceNumberLabel.Content = this.StepStackPanel.Children.IndexOf(rsc)+1;
            int seqNum = this.StepStackPanel.Children.IndexOf(rsc) + 1;
            rsc.SetSequenceNumber(seqNum);
            rsc.MovedUp += (object o, EventArgs args) =>
            {
                this.StepStackPanel.MoveItemUp(rsc);
                int seq = this.StepStackPanel.Children.IndexOf(rsc)+1;
                //rsc.SetSequenceNumber(seq);
            };

            rsc.MovedDown += (object o, EventArgs args) =>
            {
                this.StepStackPanel.MoveItemDown(rsc);
                int seq = this.StepStackPanel.Children.IndexOf(rsc)+1;
                //rsc.SetSequenceNumber(seq);
            };
        }

        private void saveRecipe(object sender, RoutedEventArgs e)
        {
            Recipe r = new Recipe();
            r.Name = this.titleTextBox.Text;

            foreach (IngredientControl ictrl in this._iCtrls)
            {
                r.AddIngredient(ictrl.Ingredient);
            }

            foreach (RecipeStepControl stpctrl in this.StepStackPanel.Children)
            {
                r.AddStep(stpctrl.Step);
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save the current recipe";
            dialog.Filter = "XML-file (.xml)|*.xml";
            dialog.DefaultExt = ".xml";
            dialog.AddExtension = true;
            dialog.FileName = "recipe";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                String filename = dialog.FileName;

                FileInfo file = new FileInfo(filename);
                FileStream fstream = file.Create();
                
                XmlSerializer x = new XmlSerializer(r.GetType());
                x.Serialize(fstream, r);
                //x.Serialize(Console.Out, r);
                //Console.WriteLine();
            }

            
        }

        private void newRecipeMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void openRecipeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Open a recipe XML file";
            dialog.Filter = "XML-file (.xml)|*.xml";
            dialog.DefaultExt = ".xml";
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                Recipe r = new Recipe();

                XmlReader xReader = new XmlTextReader(dialog.FileName);
                XmlSerializer x = new XmlSerializer(r.GetType());
                r = (Recipe)x.Deserialize(xReader);

                if (r != null)
                {
                    this.titleTextBox.Text = r.Name;
                    this.rcp = r;
                    //foreach (Ingredient i in rcp.Ingredients)
                    //{
                    //    this.addNewIngredient(i);
                    //}
                    
                    foreach (RecipeStep s in rcp.Steps)
                    {
                        RecipeStepControl rsp = new RecipeStepControl(s.Name, s.ID, s.Details, s.SequenceNumber);

                        //rsp.Step.Name = s.Name;
                        //rsp.Step.ID = s.ID;
                        //rsp.Step.Details = s.Details;
                        //rsp.Step.SequenceNumber = s.SequenceNumber;
                        foreach (RecipeComponent rc in s.Operands)
                        {
                            if (rc is Ingredient)
                            {
                                //s.RemoveOperand(rc as Ingredient);
                                IngredientControl ic = new IngredientControl(rc as Ingredient);
                                
                                this.addNewIngredientControl(ic);
                                rsp.addIngredientControl(ic);
                            }
                            else if (rc is RecipeStep)
                            {
                                RecipeStepControl rsc = new RecipeStepControl(rc as RecipeStep);
                                rsp.addRecipeStepControl(rsc);
                                this.addNewStep(rsc);
                            }

                        }
                        this.addNewStep(rsp);
                    }
                    this.titleTextBox.Text = r.Name;
                }
            }
        }
    }
}
