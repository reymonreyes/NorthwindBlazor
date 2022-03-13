﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Dtos
{
    public class ServiceResult
    {
        public bool IsSuccessful { get; set; }
        public List<KeyValuePair<string, string>>? Errors { get; set; }
    }
}
