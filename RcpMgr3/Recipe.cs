using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcpMgr3
{
    [Serializable]
    public class Recipe
    {
        private List<Ingredient> _ingredients = new List<Ingredient>();
        private List<RecipeStep> _steps = new List<RecipeStep>();

        public String Name { get; set; }

        public List<RecipeStep> Steps
        {
            get
            {
                return _steps;
            }

            set
            {
                _steps = value;
            }
        }

        public List<Ingredient> Ingredients
        { 
            get 
            {
                return _ingredients;
            }
        }

        public void AddIngredient(Ingredient i)
        {
            _ingredients.Add(i);
        }

        public void AddIngredients(List<Ingredient> ings)
        {
            _ingredients.AddRange(ings);
        }

        public void RemoveIngredient(Ingredient i)
        {
            _ingredients.Remove(i);
        }

        public void AddStep(RecipeStep rs)
        {
            _steps.Add(rs);
        }


        internal void RemoveStep(RecipeStep rs)
        {
            _steps.RemoveAll(x => x.ID.Equals(rs.ID));
        }
    }
}
