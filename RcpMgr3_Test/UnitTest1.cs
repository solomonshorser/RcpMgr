using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RcpMgr3;
using System.Collections.Generic;

namespace RecipeTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void simpleTest()
        {
            Recipe testRecipe = new Recipe();

            Ingredient i1 = new Ingredient();
            Console.WriteLine(i1.ID);
            Ingredient i2 = new Ingredient();
            Console.WriteLine(i2.ID);
            i1.Name = "pepper";
            i2.Name = "salt";

            i1.UnitOfMeasure = "TEASPOON";
            i2.UnitOfMeasure = "OUNCE";

            i1.Quantity = "1";
            i2.Quantity = "2";

            List<Ingredient> iList = new List<Ingredient>();

            iList.Add( i1);
            iList.Add(  i2);

            //testRecipe.Ingredients = iList;
            testRecipe.AddIngredients(iList);

            Console.WriteLine(testRecipe.Ingredients[0].Name);

            
            Assert.AreEqual(testRecipe.Ingredients[0].Name.Equals("pepper"),true);
            Assert.AreEqual(testRecipe.Ingredients[1].Name.Equals("salt"), true);
        }

        [TestMethod]
        public void testRecipeManager()
        {
            Recipe testRecipe = new Recipe();

            Ingredient i1 = new Ingredient();
            Console.WriteLine(i1.ID);
            Ingredient i2 = new Ingredient();
            Console.WriteLine(i2.ID);
            RecipeManager rmgr = new RecipeManager(testRecipe);

            rmgr.AddIngredient(i1);
            rmgr.AddIngredient(i2);
            try
            {
                rmgr.AddIngredient(i1);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Ingredient with ID "));
            } 
        }

        [TestMethod]
        public void testRecipeManagerWithSteps()
        {
            Recipe testRecipe = new Recipe();

            Ingredient i1 = new Ingredient();
            Console.WriteLine(i1.ID);
            Ingredient i2 = new Ingredient();
            Console.WriteLine(i2.ID);
            RecipeManager rmgr = new RecipeManager(testRecipe);

            RecipeStep s1 = new RecipeStep();
            RecipeStep s2 = new RecipeStep();

            //s1.operand = i1.ingredientID;
            s1.Name = "chop";
            s1.Operands.Add(i1);

            //s2.operand = i2.ingredientID;
            s2.Name = "blend";
            s2.Operands.Add(i2);

            rmgr.AddIngredient(i1);
            rmgr.AddIngredient(i2);

            rmgr.AddStep(s1);
            rmgr.AddStep(s2);

            try
            {
                rmgr.AddStep(s1);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Step with ID "));
            }
        }


        [TestMethod]
        public void testRemoveIngredient()
        {
            Recipe testRecipe = new Recipe();

            Ingredient i1 = new Ingredient();
            i1.Name = "salt";
            Console.WriteLine(i1.ID);
            Ingredient i2 = new Ingredient();
            i2.Name = "pepper";
            Console.WriteLine(i2.ID);
            Ingredient i3 = new Ingredient();
            i3.Name = "flour";
            Console.WriteLine(i3.ID);
            RecipeManager rmgr = new RecipeManager(testRecipe);

            RecipeStep s1 = new RecipeStep();
            RecipeStep s2 = new RecipeStep();

            //s1.operand = i1.ingredientID;
            s1.Name = "chop";
            s1.Operands.Add(i1);

            //s2.operand = i2.ID
            s2.Operands.Add(i2);
            s2.Name = "blend";

            rmgr.AddIngredient(i1);
            rmgr.AddIngredient(i2);

            rmgr.AddStep(s1);
            rmgr.AddStep(s2);

            Console.WriteLine(rmgr.GetRecipe().Ingredients.Count);
            Assert.IsTrue(rmgr.GetRecipe().Ingredients.Count == 2);

            rmgr.RemoveIngredient(i3);

            Assert.IsTrue(rmgr.GetRecipe().Ingredients.Count == 2);

            try
            {
                rmgr.RemoveIngredient(i2);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Sorry, but that Ingredient is in use."));
            }
        }

        [TestMethod]
        public void testRemoveStep()
        {
            Recipe testRecipe = new Recipe();

            Ingredient i1 = new Ingredient();
            Console.WriteLine(i1.ID);
            Ingredient i2 = new Ingredient();
            Console.WriteLine(i2.ID);
            Ingredient i3 = new Ingredient();
            Console.WriteLine(i3.ID);
            RecipeManager rmgr = new RecipeManager(testRecipe);

            RecipeStep s1 = new RecipeStep();
            RecipeStep s2 = new RecipeStep();

            RecipeStep s3 = new RecipeStep();

            //s1.operand = i1.ingredientID;
            s1.Operands.Add(i1);
            s1.Name = "chop";

            s2.Operands.Add(i2);
            //s2.operand = i2.ingredientID;
            s2.Name = "blend";

            rmgr.AddIngredient(i1);
            rmgr.AddIngredient(i2);

            rmgr.AddStep(s1);
            rmgr.AddStep(s2);

            Assert.IsTrue(rmgr.GetRecipe().Steps.Count == 2);

            rmgr.RemoveStep(s3);

            Assert.IsTrue(rmgr.GetRecipe().Steps.Count == 2);

            try
            {
                rmgr.RemoveStep(s2);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Sorry, but that step is in use."));
            }
        }

        [TestMethod]
        public void testUpdateIngredient()
        {
            Recipe r1 = new Recipe();

            Ingredient i1 = new Ingredient();

            i1.Name = "i1";
            i1.Quantity = "2";
            i1.UnitOfMeasure = "CUP";
            i1.Notes = "test notes";

            Console.WriteLine(i1.ToString());

            RecipeManager rmgr = new RecipeManager(r1);
            rmgr.AddIngredient(i1);

            Console.WriteLine(rmgr.GetRecipe().Ingredients[0].Name);

            Assert.IsTrue(rmgr.GetRecipe().Ingredients[0].Name.Equals("i1"));

            i1.Name = "changed name";

            Assert.IsTrue(rmgr.GetRecipe().Ingredients[0].Name.Equals("changed name"));
        }

        [TestMethod]
        public void testAddIngredientsToRecipeStep()
        {

            Recipe r1 = new Recipe();

            Ingredient i1 = new Ingredient();
            Ingredient i2 = new Ingredient();

            RecipeManager rmgr = new RecipeManager(r1);

            i1.Name = "pepper";

            i2.Name = "salt";

            RecipeStep rs = new RecipeStep();

            rs.Name = "mix";

            rmgr.AddIngredientToStep(i1, rs);
            rmgr.AddIngredientToStep(i2, rs);
            rmgr.AddIngredientToStep(i1, rs);

            Console.WriteLine(rs.ToString());
        }
    }
}
