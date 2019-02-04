﻿using System.Net.Http;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public class KxDailiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new KxDailiProxyApiProvider();

		public override void Run()
		{
			base.Run();
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			var urls = ApiProvider.GetRequestUrls();
			using (SpiderWebClient webClient = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(webClient, HttpMethod.Get, urls, urls.Count * 10);
				if (entities.Count < 1)
				{
					return;
				}
				entities.ForEach(e => e.Source = caseName);
				ShowConsoleMsg("CollectCount: " + entities.Count.ToString());
				int insertCount = pa.InsertProxyEntities(entities);
				ShowConsoleMsg("InsertCount: " + insertCount.ToString());
			}
		}

		public override void Run(params string[] args)
		{
			Run();
		}
	}
}