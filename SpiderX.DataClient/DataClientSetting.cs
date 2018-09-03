﻿using System.Collections.Generic;
using System.Threading;

namespace SpiderX.DataClient
{
	public sealed class DataClientSetting
	{
		private static DataClientSetting _instance;

		public static DataClientSetting Instance
		{
			get
			{
				if (_instance == null)
				{
					Interlocked.CompareExchange(ref _instance, new DataClientSetting(), null);
				}
				return _instance;
			}
		}

		private List<DbConfig> _dbConfigs;

		public IReadOnlyList<DbConfig> DbConfigs => _dbConfigs;

		public void InitializeConfigs(IEnumerable<DbConfig> configs)
		{
			if (_dbConfigs == null)
			{
				Interlocked.CompareExchange(ref _dbConfigs, new List<DbConfig>(configs), null);
			}
		}

		public void ForceInitializeConfigs(IEnumerable<DbConfig> configs)
		{
			Interlocked.Exchange(ref _dbConfigs, new List<DbConfig>(configs));
		}

		public DbConfig FindConfig(string name, bool isTest)
		{
			return _dbConfigs.Find(p => p.IsTest == isTest && p.Name == name);
		}
	}
}