﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.Samples
{
	public sealed class TestBll : BllBase
	{
		public override void Run(params string[] args)
		{
			Run();
		}

		public override void Run()
		{
			var conf = DbClient.FindConfig("SqlServerTest", true);
			if (conf == null)
			{
				throw new DbConfigNotFoundException();
			}
		}
	}
}