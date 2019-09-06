using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    public class CacheManager
    {
        private CacheManager()
        { }

        private static ICache cache = null;

        static CacheManager()
        {
            ////

            //可以创建不同的cache对象
            cache = (ICache)Activator.CreateInstance(typeof(MemoryCache));// 这里可以根据配置文件来选择
            //cache = (ICache)Activator.CreateInstance(typeof(CustomerCache));
        }

        #region ICache

        /// <summary>
        /// 获取缓存，假如缓存中没有，可以执行委托，委托结果，添加到缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="acquire">委托 为null会异常</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<T> acquire, int cacheTime = 600)
        {
            if (cache.Contains(key))
            {
                return GetData<T>(key);
            }
            else
            {
                T result = acquire.Invoke();//执行委托  获取委托结果   作为缓存值
                cache.Add(key, result, cacheTime);
                return result;
            }
        }

        /// <summary>
        /// 当前缓存数据项的个数
        /// </summary>
        public static int Count
        {
            get { return cache.Count; }
        }

        /// <summary>
        /// 如果缓存中已存在数据项键值，则返回true
        /// </summary>
        /// <param name="key">数据项键值</param>
        /// <returns>数据项是否存在</returns>
        public static bool Contains(string key)
        {
            return cache.Contains(key);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetData<T>(string key)
        {
            return cache.Get<T>(key);
        }

        /// <summary>
        /// 添加缓存数据。
        /// 如果另一个相同键值的数据已经存在，原数据项将被删除，新数据项被添加。
        /// </summary>
        /// <param name="key">缓存数据的键值</param>
        /// <param name="value">缓存的数据，可以为null值</param>
        public static void Add(string key, object value)
        {
            if (Contains(key))
                cache.Remove(key);
            cache.Add(key, value);
        }

        /// <summary>
        /// 添加缓存数据。
        /// 如果另一个相同键值的数据已经存在，原数据项将被删除，新数据项被添加。
        /// </summary>
        /// <param name="key">缓存数据的键值</param>
        /// <param name="value">缓存的数据，可以为null值</param>
        /// <param name="expiratTime">缓存过期时间间隔(单位：秒) 默认时间600秒</param>
        public static void Add(string key, object value, int expiratTime = 600)
        {
            cache.Add(key, value, expiratTime);
        }

        /// <summary>
        /// 删除缓存数据项
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        /// 删除所有缓存数据项
        /// </summary>
        public static void RemoveAll()
        {
            cache.RemoveAll();
        }
        #endregion

    }
}
