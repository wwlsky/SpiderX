﻿using System;
using System.Collections.Generic;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	internal interface IProxyAgent
	{
		DbConfig DbConfig { get; }

		IEnumerable<SpiderProxyEntity> SelectProxyEntities(Func<SpiderProxyEntity, bool> predicate, int recentDays, int count);

		int InsertProxyEntities(IEnumerable<SpiderProxyEntity> entities);

		int UpdateProxyEntity(int id, Action<SpiderProxyEntity> update);

		int UpdateProxyEntities(IEnumerable<int> ids, Action<SpiderProxyEntity> update);

		int DeleteProxyEntity(int id);

		int DeleteProxyEntity(string host, int port);
	}
}