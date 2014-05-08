using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcpMgr3
{
    public class RecipeManager
    {
        private Recipe _recipe;

        private Dictionary<String, Ingredient> _ingredients = new Dictionary<string,Ingredient>();

        private Dictionary<String, RecipeStep> _steps = new Dictionary<string, RecipeStep>();

        public RecipeManager(Recipe r)
        {
            this._recipe = r;
        }

        public Recipe GetRecipe()
        {
            return _recipe;
        }

        public void AddIngredient(Ingredient newIng)
        {
            if (!_ingredients.ContainsKey(newIng.ID))
            {
                _ingredients.Add(newIng.ID, newIng);
                //_recipe.Ingredients = _ingredients.Values.ToList();
                _recipe.AddIngredient(newIng);
            }
            else
            {
                throw new Exception("Ingredient with ID " + newIng.ID + " is already in the recipe.");
            }
        }

        public bool AddIngredientToStep(Ingredient i, RecipeStep r)
        {
            bool ingredientIsNewToStep = true;
            if (!_ingredients.ContainsKey(i.ID))
            {
                AddIngredient(i);
            }
            
            RecipeStep _r = r;

            if (!_r.Operands.Any((RecipeComponent c) => { return c.ID == i.ID; }))
            {
                _r.Operands.Add(i);
            }
            else
            {
                ingredientIsNewToStep = false;
            }


            if (!_steps.ContainsKey(_r.ID))
            {
                AddStep(_r);
            }
            else
            {
                UpdateStep(_r);
            }

            return ingredientIsNewToStep;
        }

        public void UpdateIngredient(Ingredient ing)
        {
            if (_ingredients.ContainsKey(ing.ID))
            {
                _ingredients[ing.ID].Name = ing.Name;
                _ingredients[ing.ID].Notes = ing.Notes;
                _ingredients[ing.ID].Quantity = ing.Quantity;
                _ingredients[ing.ID].UnitOfMeasure = ing.UnitOfMeasure;
            }
            else
            {
                throw new Exception("Ingredient with ID "+ing.ID+" is not in the recipe so it cannot be updated.");
            }
        }

        public void RemoveIngredient(Ingredient rmIng)
        {
            bool ingredientInUse = _steps.Values.Any<RecipeStep>((RecipeStep s) => { return s.ID.Equals(rmIng.ID); });
            if (ingredientInUse)
            {
                throw new Exception("Sorry, but that ingredient is in use.");
            }
            else
            {
                _ingredients.Remove(rmIng.ID);
                //_recipe.Ingredients = _ingredients.Values.ToList();
                _recipe.RemoveIngredient(rmIng);
            }
        }

        public void AddStep(RecipeStep newStp)
        {
            if (!_steps.ContainsKey(newStp.ID))
            {
                _steps.Add(newStp.ID, newStp);
                _recipe.Steps = _steps.Values.ToList();
            }
            else
            {
                throw new Exception("Step with ID " + newStp.ID + " is already in the recipe.");
            }
        }

        public void UpdateStep(RecipeStep stp)
        {
            if (_steps.ContainsKey(stp.ID))
            {
                _steps[stp.ID].Details = stp.Details;
                //_steps[stp.stepID].operand = stp.operand;
                _steps[stp.ID].Name = stp.Name;
                _steps[stp.ID].SequenceNumber = stp.SequenceNumber;
            }
            else
            {
                throw new Exception("Step with ID " + stp.ID + " is not in the recipe so it cannot be updated.");
            }
        }

        public void RemoveStep(RecipeStep rmStep)
        {
            bool stepInUse = _steps.Values.Any<RecipeStep>((RecipeStep s) => { return s.ID.Equals(rmStep.ID); });
            if (stepInUse)
            {
                throw new Exception("Sorry, but that step is in use.");
            }
            else
            {
                _steps.Remove(rmStep.ID);
                _recipe.Steps = _steps.Values.ToList();
            }
        }

        public void AddOrUpdateIngredient(Ingredient ingredient)
        {
            if (!_ingredients.ContainsKey(ingredient.ID))
            {
                this.AddIngredient(ingredient);
            }
            else
            {
                this.UpdateIngredient(ingredient);
            }
        }

    }
}
