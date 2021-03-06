﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.LaGou.DbEntities
{
	public sealed class LaGouHrDailyRecordEntity
	{
		public int Id { get; set; }

		public long UserId { get; set; }

		public int ResumeProcessRate { get; set; }

		public int ResumeProcessDay { get; set; }

		public DateTime LastLoginTime { get; set; }

		public int DateNumber { get; set; }

		public DateTime DbCreateTime { get; set; }
	}
}