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
            ic.IngredientDeleted += (object o, EventArgs args) =>
            {
                String componentsUsing = "";
                //first check to see if the ingredient is in use anywhere.

                foreach (RecipeStepControl rsc in this.StepStackPanel.Children)
                {
                    bool matchingIngredient = rsc.Step.Operands.Any(
                        (RecipeComponent rc) => { return rc.ID == ic.Ingredient.ID; }
                    );

                    if (matchingIngredient)
                    {
                        componentsUsing += rsc.sequenceNumberLabel.Content +"; "+ rsc.Step.Name + "\n";
                    }
                }
                if (!componentsUsing.Equals(""))
                {
                    MessageBox.Show("Sorry, you cannot remove that ingredient, it is used by: \n" + componentsUsing,"Error!",MessageBoxButton.OK,MessageBoxImage.Error);
                }
                else
                {
                    this.rcp.RemoveIngredient(ic.Ingredient);
                    this.IngredientsStackPanel.Children.Remove(ic);
                }
            };
            _iCtrls.Add(ic);

            IngredientsStackPanel.Children.Add(ic);
        }

        private void addNewIngredient(Ingredient i)
        {
            IngredientControl ic = new IngredientControl(i);

            addNewIngredientControl(ic);
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
                    Recipe recipe = new Recipe();

                    XmlReader xReader = new XmlTextReader(dialog.FileName);
                    XmlSerializer x = new XmlSerializer(recipe.GetType());
                    recipe = (Recipe)x.Deserialize(xReader);

                    if (recipe != null)
                    {
                        this.titleTextBox.Text = recipe.Name;
                        this.rcp = recipe;


                        foreach (RecipeStep step in rcp.Steps)
                        {
                            RecipeStepControl rsp = new RecipeStepControl(step.Name, step.ID, step.Details, step.SequenceNumber);


                            foreach (RecipeComponent rc in step.Operands)
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
                        this.titleTextBox.Text = recipe.Name;
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
