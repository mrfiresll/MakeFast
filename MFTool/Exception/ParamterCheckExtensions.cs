﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MFTool
{
    /// <summary>
    /// 用于参数检查的扩展方法
    /// </summary>
    public static class ParamterCheckExtensions
    {
        /// <summary>
        /// 验证指定值的断言<paramref name="assertion"/>是否为真，如果不为真，抛出指定消息<paramref name="message"/>的指定类型<typeparamref name="TException"/>异常
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="assertion">要验证的断言。</param>
        /// <param name="message">异常消息。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        private static void Require<TException>(bool assertion, string message, bool bBusinessException = false) where TException : Exception
        {
            if (assertion)
            {
                return;
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }
            TException exception = (TException)Activator.CreateInstance(typeof(TException), message);
            throw exception;
        }

        /// <summary>
        /// 验证指定值的断言表达式是否为真，不为值抛出<see cref="Exception"/>异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        public static void Required<T>(this T value, Func<T, bool> assertionFunc, string message, bool bBusinessException = false)
        {
            if (assertionFunc == null)
            {
                throw new ArgumentNullException("assertionFunc");
            }

            Require<Exception>(assertionFunc(value), message, bBusinessException);
        }

        /// <summary>
        /// 验证指定值的断言表达式是否为真，不为真抛出<see cref="TException"/>异常
        /// </summary>
        /// <typeparam name="T">要判断的值的类型</typeparam>
        /// <typeparam name="TException">抛出的异常类型</typeparam>
        /// <param name="value">要判断的值</param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        public static void Required<T, TException>(this T value, Func<T, bool> assertionFunc, string message, bool bBusinessException = false) where TException : Exception
        {
            if (assertionFunc == null)
            {
                throw new ArgumentNullException("assertionFunc");
            }
            Require<TException>(assertionFunc(value), message);
        }

        /// <summary>
        /// 检查参数不能为空引用，否则抛出<see cref="ArgumentNullException"/>异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CheckNotNull<T>(this T value, string paramName, bool bBusinessException = false) where T : class
        {
            Require<ArgumentNullException>(value != null, string.Format(Resource.ParameterCheck_NotNull, paramName), bBusinessException);
        }

        /// <summary>
        /// 检查字符串不能为空引用或空字符串，否则抛出<see cref="ArgumentNullException"/>异常或<see cref="ArgumentException"/>异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckNotNullOrEmpty(this string value, string paramName, bool bBusinessException = false)
        {
            value.CheckNotNull(paramName, bBusinessException);
            Require<ArgumentException>(value.Length > 0, string.Format(Resource.ParameterCheck_NotNullOrEmpty_String, paramName), bBusinessException);
        }

        /// <summary>
        /// 检查字符串必须为布尔类型，否则抛出<see cref="ArgumentNullException"/>异常或<see cref="ArgumentException"/>异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        public static void CheckBoolType(this string value, string paramName, bool bBusinessException = false)
        {
            value.CheckNotNull(paramName);
            bool tmp = false;
            bool flag = Boolean.TryParse(value, out tmp);
            Require<ArgumentException>(flag, string.Format(Resource.ParameterCheck_Bool_String, paramName), bBusinessException);
        }

        /// <summary>
        /// 检查Guid值不能为Guid.Empty，否则抛出<see cref="ArgumentException"/>异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckNotEmpty(this Guid value, string paramName, bool bBusinessException = false)
        {
            Require<ArgumentException>(value != Guid.Empty, string.Format(Resource.ParameterCheck_NotEmpty_Guid, paramName), bBusinessException);
        }

        /// <summary>
        /// 检查集合不能为空引用或空集合，否则抛出<see cref="ArgumentNullException"/>异常或<see cref="ArgumentException"/>异常。
        /// </summary>
        /// <typeparam name="T">集合项的类型。</typeparam>
        /// <param name="collection"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckNotNullOrEmpty<T>(this IEnumerable<T> collection, string paramName, bool bBusinessException = false)
        {
            collection.CheckNotNull(paramName, bBusinessException);
            Require<ArgumentException>(collection.Any(), string.Format(Resource.ParameterCheck_NotNullOrEmpty_Collection, paramName), bBusinessException);
        }

        /// <summary>
        /// 检查参数必须小于[或可等于，参数canEqual]指定值，否则抛出<see cref="ArgumentOutOfRangeException"/>异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void CheckLessThan<T>(this T value, string paramName, T target, bool canEqual = false, bool bBusinessException = false) where T : IComparable<T>
        {
            bool flag = canEqual ? value.CompareTo(target) <= 0 : value.CompareTo(target) < 0;
            string format = canEqual ? Resource.ParameterCheck_NotLessThanOrEqual : Resource.ParameterCheck_NotLessThan;
            Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), bBusinessException);
        }

        /// <summary>
        /// 检查参数必须大于[或可等于，参数canEqual]指定值，否则抛出<see cref="ArgumentOutOfRangeException"/>异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void CheckGreaterThan<T>(this T value, string paramName, T target, bool canEqual = false, bool bBusinessException = false) where T : IComparable<T>
        {
            bool flag = canEqual ? value.CompareTo(target) >= 0 : value.CompareTo(target) > 0;
            string format = canEqual ? Resource.ParameterCheck_NotGreaterThanOrEqual : Resource.ParameterCheck_NotGreaterThan;
            Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), bBusinessException);
        }

        /// <summary>
        /// 检查参数必须在指定范围之间，否则抛出<see cref="ArgumentOutOfRangeException"/>异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="start">比较范围的起始值。</param>
        /// <param name="end">比较范围的结束值。</param>
        /// <param name="startEqual">是否可等于起始值</param>
        /// <param name="endEqual">是否可等于结束值</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void CheckBetween<T>(this T value, string paramName, T start, T end, bool startEqual = false, bool endEqual = false, bool bBusinessException = false)
            where T : IComparable<T>
        {
            bool flag = startEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            string message = startEqual
                ? string.Format(Resource.ParameterCheck_BetweenNotEqual, paramName, start, end, start)
                : string.Format(Resource.ParameterCheck_Between, paramName, start, end);

            Require<ArgumentOutOfRangeException>(flag, message, bBusinessException);

            flag = endEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0;
            message = endEqual
                ? string.Format(Resource.ParameterCheck_BetweenNotEqual, paramName, start, end, end)
                : string.Format(Resource.ParameterCheck_Between, paramName, start, end);

            Require<ArgumentOutOfRangeException>(flag, message, bBusinessException);
        }

        /// <summary>
        /// 检查指定路径的文件夹必须存在，否则抛出<see cref="DirectoryNotFoundException"/>异常。
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CheckDirectoryExists(this string directory, string paramName = null, bool bBusinessException = false)
        {
            CheckNotNull(directory, paramName, bBusinessException);
            Require<DirectoryNotFoundException>(Directory.Exists(directory), string.Format(Resource.ParameterCheck_DirectoryNotExists, directory), bBusinessException);
        }

        /// <summary>
        /// 检查指定路径的文件必须存在，否则抛出<see cref="FileNotFoundException"/>异常。
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="bBusinessException">是否业务逻辑错误。</param>
        /// <exception cref="ArgumentNullException">当文件路径为null时</exception>
        /// <exception cref="FileNotFoundException">当文件路径不存在时</exception>
        public static void CheckFileExists(this string filename, string paramName = null, bool bBusinessException = false)
        {
            CheckNotNull(filename, paramName, bBusinessException);
            Require<FileNotFoundException>(File.Exists(filename), string.Format(Resource.ParameterCheck_FileNotExists, filename), bBusinessException);
        }
    }
}