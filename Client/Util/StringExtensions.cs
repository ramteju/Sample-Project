using Client.Common;
using Client.Logging;
using Entities;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Client.Util
{
    public static class StringExtensions
    {
        public static string SafeSubstring(this string text, int start, int length)
        {
            
            try
            {
                 ;
                if (!String.IsNullOrEmpty(text))
                    return text.Length <= start ? ""
                        : text.Length - start <= length ? text.Substring(start)
                        : text.Substring(start, length);
                 ;
                return string.Empty;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
        public static string ReplaceAt(this string str, int index, int length, string replace)
        {
            
            try
            {
                return str.Remove(index, Math.Min(length, str.Length - index))
                           .Insert(index, replace);
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
    }
}
