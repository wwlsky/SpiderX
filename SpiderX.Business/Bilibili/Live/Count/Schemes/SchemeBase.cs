﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.Bilibili
{
    public partial class BilibiliLiveCountBll
    {
        private abstract class SchemeBase
        {
            public CollectorBase Collector { get; set; }

            public abstract void Run();
        }
    }
}