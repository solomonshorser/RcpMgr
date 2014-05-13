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

            rsc.Margin = new Thickness(3, 3, 3, 3);
            
            this.StepStackPanel.Children.Add(rsc);
            int seqNum = this.StepStackPanel.Children.IndexOf(rsc) + 1;
            rsc.SetSequenceNumber(seqNum);
            rsc.MovedUp += (object o, EventArgs args) =>
            {
                this.StepStackPanel.MoveItemUp(rsc);
                int seq = this.StepStackPanel.Children.IndexOf(rsc)+1;
            };

            rsc.MovedDown += (object o, EventArgs args) =>
            {
                this.StepStackPanel.MoveItemDown(rsc);
                int seq = this.StepStackPanel.Children.IndexOf(rsc)+1;
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
            }

            
        }

        private bool clearScreen()
        {
            //Only need to show the dialoge if there is content.
            if (!this.titleTextBox.Text.Equals("") || this.IngredientsStackPanel.Children.Count > 0 || this.StepStackPanel.Children.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("If you do this, you will lose any changes you have made to the current file.", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK);

                if (result == MessageBoxResult.OK)
                {
                    this.rcp = new Recipe();
                    this.titleTextBox.Text = "";
                    this.IngredientsStackPanel.Children.Clear();
                    this.StepStackPanel.Children.Clear();
                    return true;
                }
                return false;
            }
            return true;
        }

        private void newRecipeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            clearScreen();
        }

        private void openRecipeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool screenCleared = clearScreen();

            if (screenCleared)
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


                        foreach (RecipeStep s in rcp.Steps)
                        {
                            RecipeStepControl rsp = new RecipeStepControl(s.Name, s.ID, s.Details, s.SequenceNumber);


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
                                    bool addToMain = false;
                                    RecipeStepControl recipeControl = getExistingRecipeStepControl((rc as RecipeStep).ID);

                                    if (recipeControl == null)
                                    {
                                        recipeControl = new RecipeStepControl(rc as RecipeStep);
                                        addToMain = true;
                                    }

                                    rsp.addRecipeStepControl(recipeControl);

                                    if (addToMain)
                                        this.addNewStep(recipeControl);
                                }
                            }
                            this.addNewStep(rsp);
                        }
                        this.titleTextBox.Text = r.Name;
                    }
                }
            }
        }

        private RecipeStepControl getExistingRecipeStepControl(string id)
        {
            foreach (RecipeStepControl c in this.StepStackPanel.Children.OfType<RecipeStepControl>())
            {
                if (c.Step.ID.Equals(id))
                    return c;
            }
            return null;
        }
    }
}
