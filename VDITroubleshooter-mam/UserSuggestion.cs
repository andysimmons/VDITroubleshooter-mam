﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDITroubleshooter
{
    class UserSuggestion
    {
        public string CN { get; set; }

        public string SAMAccountName { get; set; }

        public string Department { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return SAMAccountName;
        }
    }
}
