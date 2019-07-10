﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveRoomCountBll
	{
		private sealed class DefaultScheme : SchemeBase
		{
			public override async Task RunAsync()
			{
				int liveRoomCount = await Collector.CollectAsync("0");
				ShowConsoleMsg(liveRoomCount.ToString());
				using (var context = new BilibiliLiveRoomCountMySqlContext())
				{
					context.Database.EnsureCreated();
					var item = BilibiliLiveRoomCount.Create(liveRoomCount);
					context.LiveRoomCount.Add(item);
					context.SaveChanges();
				}
			}
		}
	}
}