﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SpiderX.BusinessBase;

namespace SpiderX.Launcher
{
	public sealed class CaseSetting : IEquatable<CaseSetting>
	{
		internal CaseSetting(string nameSpace, string caseName, string[] caseParams, int version)
		{
			Namespace = nameSpace ?? throw new ArgumentNullException(nameof(Namespace));
			CaseName = caseName ?? throw new ArgumentNullException(nameof(CaseName));
			Params = caseParams ?? Array.Empty<string>();
			Version = version;
		}

		internal string[] Params { get; }

		internal string Namespace { get; }

		public string CaseName { get; }

		public int Version { get; }

		public string FullTypeName => Namespace + '.' + CaseName;

		public void InvokeCase()
		{
			Type bllType;
			try
			{
				Assembly a = Assembly.Load(Namespace);
				bllType = a.GetType(FullTypeName, false, true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			if (bllType == null)
			{
				return;
			}
			if (bllType.IsAbstract || bllType.IsNotPublic || !bllType.IsSubclassOf(typeof(BllBase)))
			{
				throw new TypeAccessException("Invalid Type: " + FullTypeName);
			}
			//Create Instance
			BllBase bllInstance;
			try
			{
				bllInstance = (BllBase)Activator.CreateInstance(bllType, false);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			//Invoke Method
			if (Params.Length < 1)
			{
				bllInstance.Run();
			}
			else
			{
				bllInstance.Run(Params);
			}
		}

		public bool Equals(CaseSetting other)
		{
			if (Namespace != other.Namespace)
			{
				return false;
			}
			if (CaseName.EndsWith("bll", StringComparison.CurrentCultureIgnoreCase))
			{
				return CaseName.Contains(other.CaseName, StringComparison.CurrentCultureIgnoreCase);
			}
			return other.CaseName.Contains(CaseName, StringComparison.CurrentCultureIgnoreCase);
		}

		public static CaseSetting CreateDefault(string caseName)
		{
			return CreateDefault(caseName, Array.Empty<string>());
		}

		public static CaseSetting CreateDefault(string caseName, params string[] caseParams)
		{
			return new CaseSetting(GlobalSetting.DefaultNamespace, caseName, caseParams, 0);
		}

		public static CaseSetting FromConfiguration(IConfiguration source)
		{
			string nameSpace = source.GetSection(nameof(Namespace)).Value;
			if (string.IsNullOrEmpty(nameSpace))
			{
				return null;
			}
			string caseName = source.GetSection(nameof(CaseName)).Value;
			if (string.IsNullOrEmpty(caseName))
			{
				return null;
			}
			string finalCaseName = CorrectCaseName(caseName);
			string[] caseParams = source.GetSection(nameof(Params)).GetChildren().Select(p => p.Value).ToArray();
			string vesionStr = source.GetSection(nameof(Version)).Value;
			int version = 0;
			if (!string.IsNullOrEmpty(vesionStr))
			{
				int.TryParse(vesionStr, out version);
			}
			return new CaseSetting(nameSpace, finalCaseName, caseParams, version);
		}

		public static List<CaseSetting> GetListByStringArgs(string[] args)
		{
			List<CaseSetting> caseSettings = new List<CaseSetting>();
			List<string> tempList = new List<string>(8);
			bool toSkip = false;
			int index;
			foreach (string arg in args)
			{
				index = arg.IndexOf(';');
				if (index < 0)
				{
					if (toSkip)
					{
						continue;
					}
					if (CheckSkipStringArg(arg))
					{
						toSkip = true;
						continue;
					}
					tempList.Add(arg);
				}
				else//Exists semicolon
				{
					if (toSkip)
					{
						string rightStr = arg.Substring(index + 1);
						if (CheckSkipStringArg(rightStr))
						{
							continue;
						}
						tempList.Add(rightStr);
						toSkip = false;
					}
					else
					{
						string leftStr = arg.Substring(0, index);
						switch (tempList.Count)
						{
							case 0:
								string name = CorrectCaseName(leftStr);
								caseSettings.Add(CreateDefault(name));
								break;

							case 1:
								caseSettings.Add(CreateDefault(tempList[0], leftStr));
								tempList.Clear();
								break;

							default:
								string[] paramAry = new string[tempList.Count];
								tempList.CopyTo(1, paramAry, 0, paramAry.Length - 1);
								paramAry[paramAry.Length - 1] = leftStr;
								var setting = CreateDefault(tempList[0], paramAry);
								caseSettings.Add(setting);
								tempList.Clear();
								break;
						}
						if (index != arg.Length - 1)
						{
							string rightStr = arg.Substring(index + 1);
							if (CheckSkipStringArg(rightStr))
							{
								toSkip = true;
								continue;
							}
							tempList.Add(CorrectCaseName(rightStr));
						}
					}
				}
			}
			switch (tempList.Count)
			{
				case 0: break;

				case 1:
					caseSettings.Add(CreateDefault(tempList[0]));
					tempList.Clear();
					break;

				default:
					string[] paramAry = new string[tempList.Count - 1];
					tempList.CopyTo(1, paramAry, 0, paramAry.Length);
					caseSettings.Add(CreateDefault(tempList[0], paramAry));
					break;
			}
			return caseSettings;
		}

		private static string CorrectCaseName(string name)
		{
			string trimName = name.Trim();
			if (!trimName.EndsWith("Bll", StringComparison.CurrentCultureIgnoreCase))
			{
				trimName += "Bll";
			}
			return trimName;
		}

		private static bool CheckSkipStringArg(string arg)
		{
			return arg.StartsWith('-') || arg.StartsWith('/');
		}
	}
}