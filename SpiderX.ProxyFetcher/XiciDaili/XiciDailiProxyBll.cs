﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Extensions;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class XiciDailiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new XiciDailiProxyApiProvider();

		public override void Run()
		{
			base.Run();
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			using (var webClient = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(webClient, XiciDailiProxyApiProvider.NnUrlTemplate, 10);
				if (entities.Count < 1)
				{
					return;
				}
				entities.ForEach(e => e.Source = caseName);
				ShowDebugInfo("CollectCount: " + entities.Count.ToString());
				int insertCount = pa.InsertProxyEntities(entities);
				ShowDebugInfo("InsertCount: " + insertCount.ToString());
			}
		}

		public override void Run(params string[] args)
		{
			Run();
		}

		private List<SpiderProxyEntity> GetProxyEntities(SpiderWebClient webClient, string urlTemplate, int maxPage)
		{
			List<SpiderProxyEntity> entities = new List<SpiderProxyEntity>(maxPage * 32);
			Task[] tasks = new Task[maxPage];
			for (int p = 1; p <= maxPage; p++)
			{
				string url = GetRequestUrl(urlTemplate, p);
				string referer = GetRefererUrl(urlTemplate, p);
				tasks[p - 1] = GetResponseMessageAsync(webClient, url, referer)
					.ContinueWith(responseMsg =>
					{
						HttpResponseMessage responseMessage = responseMsg.Result;
						if (responseMessage == null)
						{
							return;
						}
						using (responseMessage)
						{
							if (!responseMessage.IsSuccessStatusCode)
							{
								return;
							}
							responseMessage.Content.ToStreamReaderAsync()
							.ContinueWith(t =>
							{
								StreamReader reader = t.Result;
								var tempList = ApiProvider.GetProxyEntities(reader);
								if (!tempList.IsNullOrEmpty())
								{
									lock (entities)
									{
										entities.AddRange(tempList);
									}
								}
							});
						}
					});
				Thread.Sleep(RandomEvent.Next(4000, 6000));
			}
			try
			{
				Task.WaitAll(tasks);
			}
			catch
			{
			}
			return entities;
		}

		private static async Task<HttpResponseMessage> GetResponseMessageAsync(SpiderWebClient webClient, string requestUrl, string referer)
		{
			var request = CreateRequestMessage(requestUrl, referer);
			try
			{
				return await webClient.SendAsync(request);
			}
			catch (Exception)
			{
				request.Dispose();
				return null;
			}
		}

		private static HttpRequestMessage CreateRequestMessage(string requestUrl, string referer)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
			request.Headers.Referrer = new Uri(referer);
			return request;
		}

		private static string GetRequestUrl(string urlTemplate, int page) => urlTemplate + page.ToString();

		private static string GetRefererUrl(string urlTemplate, int page) => urlTemplate + Math.Max(1, Math.Abs(page - RandomEvent.Next(-4, 5))).ToString();
	}
}