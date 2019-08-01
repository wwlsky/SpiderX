﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Extensions.Http;

namespace SpiderX.Http
{
	public sealed class SpiderHttpClient : HttpClient
	{
		private readonly SocketsHttpHandler _innerHandler;

		public CookieContainer CookieContainer => _innerHandler.CookieContainer;

		public SpiderHttpClient() : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseCookies = false })
		{
		}

		public SpiderHttpClient(IWebProxy proxy) : this(new SocketsHttpHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseProxy = true, Proxy = proxy, UseCookies = false })
		{
		}

		public SpiderHttpClient(SocketsHttpHandler handler) : base(handler)
		{
			_innerHandler = handler;
			Timeout = TimeSpan.FromMilliseconds(5000);
		}

		public TimeSpan RequestInterval { get; set; } = TimeSpan.FromSeconds(5);

		public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string requestUrl)
		{
			HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestUrl);
			return await SendAsync(request, true);
		}

		public async Task<string> GetOrRetryAsync(Uri uri, Predicate<string> validator = null, int retryTimes = 9)
		{
			string result = null;
			if (RequestInterval > TimeSpan.Zero)
			{
				for (int i = 0; i < retryTimes + 1; i++)
				{
					try
					{
						var text = await GetStringAsync(uri);
						result = text?.Trim();
					}
					catch (Exception)
					{
						result = null;
						Thread.Sleep(RequestInterval);
						continue;
					}
					if (validator?.Invoke(result) != false)
					{
						break;
					}
					Thread.Sleep(RequestInterval);
				}
			}
			else
			{
				for (int i = 0; i < retryTimes + 1; i++)
				{
					try
					{
						var text = await GetStringAsync(uri);
						result = text?.Trim();
					}
					catch (Exception)
					{
						result = null;
						continue;
					}
					if (validator?.Invoke(result) != false)
					{
						break;
					}
				}
			}
			return result;
		}

		public async Task<string> SendOrRetryAsync(HttpRequestMessage requestMessage, Predicate<string> validator = null, int retryTimes = 9, bool disposeRequestIfFail = true)
		{
			string result = null;
			HttpResponseMessage responseMessage = null;
			if (RequestInterval > TimeSpan.Zero)
			{
				for (int i = 0; i < retryTimes + 1; i++)
				{
					responseMessage = await SendAsync(requestMessage, false);
					if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
					{
						Thread.Sleep(RequestInterval);
						continue;
					}
					string tempText = (await responseMessage.ToTextAsync())?.Trim();
					if (validator?.Invoke(result) != false)
					{
						result = tempText;
						break;
					}
					Thread.Sleep(RequestInterval);
				}
			}
			else
			{
				for (int i = 0; i < retryTimes + 1; i++)
				{
					responseMessage = await SendAsync(requestMessage, false);
					if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
					{
						continue;
					}
					string tempText = (await responseMessage.ToTextAsync())?.Trim();
					if (validator?.Invoke(result) != false)
					{
						result = tempText;
						break;
					}
				}
			}
			if (result == null)
			{
				if (disposeRequestIfFail)
				{
					requestMessage.Dispose();
				}
				responseMessage?.Dispose();
			}
			return result;
		}

		private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool disposeRequestIfFail)
		{
			try
			{
				return await SendAsync(request);
			}
			catch (Exception)
			{
				if (disposeRequestIfFail)
				{
					request.Dispose();
				}
				return null;
			}
		}
	}
}