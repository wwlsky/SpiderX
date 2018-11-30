﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiderX.Extensions
{
	public static class StringExtension
	{
		public static bool ContainsAny(this string text, params string[] strs)
		{
			return Array.Exists(strs, s => text.Contains(s));
		}

		public static bool ContainsAny(this string text, StringComparison comparisonType, params string[] strs)
		{
			return Array.Exists(strs, s => text.Contains(s, comparisonType));
		}

		public static bool ContainsAny(this string text, IEnumerable<string> strs)
		{
			return strs.Any(s => text.Contains(s));
		}

		public static bool ContainsAny(this string text, StringComparison comparisonType, IEnumerable<string> strs)
		{
			return strs.Any(s => text.Contains(s, comparisonType));
		}

		public static bool ContainsAll(this string text, params string[] strs)
		{
			return Array.TrueForAll(strs, s => text.Contains(s));
		}

		public static bool ContainsAll(this string text, StringComparison comparisonType, params string[] strs)
		{
			return Array.TrueForAll(strs, s => text.Contains(s, comparisonType));
		}

		public static bool ContainsAll(this string text, IEnumerable<string> strs)
		{
			return strs.All(s => text.Contains(s));
		}

		public static bool ContainsAll(this string text, StringComparison comparisonType, IEnumerable<string> strs)
		{
			return strs.All(s => text.Contains(s, comparisonType));
		}

		//It'll be removed after upgrading to '.Net Standard 2.1' which provides 'String.Contains(String, StringComparison)'.
		private static bool Contains(this string text, string value, StringComparison comparisonType)
		{
			return text.IndexOf(value, comparisonType) >= 0;
		}
	}
}