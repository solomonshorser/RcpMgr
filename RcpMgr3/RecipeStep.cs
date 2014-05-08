using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcpMgr3
{
    [Serializable]
    public partial class RecipeStep : RecipeComponent
    {

        public String ID { get; set; }
        public String Name { get; set; }

        private List<RecipeComponent> _operands = new List<RecipeComponent>();
        public List<RecipeComponent> Operands
        {
            get
            {
                return _operands;
            }
        }

        public String Details { get; set; }
        public int SequenceNumber { get; set; }

        public RecipeStep()
        {
            this.ID = StepIdSequencer.getNext();
            _operands = new List<RecipeComponent>();
        }

        public void AddOperand(RecipeComponent op)
        {
            _operands.Add(op);
        }

        public void AddOperands(List<RecipeComponent> ops)
        {
            _operands.AddRange(ops);
        }

        public void RemoveOperand(RecipeComponent op)
        {
            _operands.Remove(op);
        }

        public String ToString()
        {
            String output = ID + " " + Name;

            foreach (RecipeComponent c in Operands)
            {
                output += "\n\t" + c.ToString();
            }

            return output;
        }
    }
}
