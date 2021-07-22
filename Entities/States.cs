using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CowinWatcher
{



    public class StateData
    {
        public State[] states { get; set; }
    }

    public class State
    {
        public int state_id { get; set; }
        public string state_name { get; set; }
        public string state_name_l { get; set; }
    }

    public class DistrictData
    {
        public District[] districts { get; set; }
    }
    public class District
    {
        public int state_id { get; set; }
        public int district_id { get; set; }
        public string district_name { get; set; }
        public string district_name_l { get; set; }
    }

}
