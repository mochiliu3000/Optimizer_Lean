using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer.model
{
    class Parameters
    {
        public int FastPeriod { get; set; }
        public int SlowPeriod { get; set; }

        public override string ToString()
        {
            return "FastPeriod:" + this.FastPeriod + ",SlowPeriod:" + this.SlowPeriod;
        }

        public void Deserilize(string message)
        {
            string[] param = message.Split(',').Select(p => p.Split(':')[1]).ToArray();
            this.FastPeriod = Convert.ToInt32(param[0]);
            this.SlowPeriod = Convert.ToInt32(param[1]);
        }
    }
}
