using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEMO_REDIS_01
{
    public class Phone
    {
        public string ID { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public bool IsNull()
        {
            bool isNull = false;
            if (ID == null || Model == null || Manufacturer == null)
            {
                isNull = true;
            }
            return isNull;
        }
    }
}
