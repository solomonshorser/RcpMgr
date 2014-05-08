using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcpMgr3
{
    [Serializable]
    public partial class Ingredient : RecipeComponent
    {
        //public String ID { get; set; }
        //public String Name { get; set; }
        public String Quantity { get; set; }
        public String UnitOfMeasure { get; set; }
        public String Notes { get; set; }

        public Ingredient()
        {
            this.ID = IngredientIdSequencer.getNext();
        }

        public string ToString()
        {
            return "Ingredient:  ID: " + this.ID +
                   "\n\tname: " + this.Name +
                   "\n\tqty: " + this.Quantity +
                   "\n\tuom: " + this.UnitOfMeasure;
        }
    }
}
