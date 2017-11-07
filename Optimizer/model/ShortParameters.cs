using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer.model
{
    class ShortParameters
    {
        public int bollWindow { get; set; }
        public decimal bollDev { get; set; }
        public int cciWindow { get; set; }
        public int atrWindow { get; set; }

        public override string ToString()
        {
            return "bollWindow:" + this.bollWindow + ",bollDev:" + this.bollDev +
                ",cciWindow:" + this.cciWindow + ",atrWindow:" + this.atrWindow;
        }

        public void Deserilize(string message)
        {
            string[] param = message.Split(',').Select(p => p.Split(':')[1]).ToArray();
            this.bollWindow = Convert.ToInt32(param[0]);
            this.bollDev = Convert.ToDecimal(param[1]);
            this.cciWindow = Convert.ToInt32(param[2]);
            this.atrWindow = Convert.ToInt32(param[3]);
        }
    }
}
