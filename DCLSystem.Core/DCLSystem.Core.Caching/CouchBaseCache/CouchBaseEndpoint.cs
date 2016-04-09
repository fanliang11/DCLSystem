using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCLSystem.Core.Caching.CouchBaseCache
{
   public class CouchBaseEndpoint
    {
        /// <summary>
        /// 主机
        ///</summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public string Host
        {
            get;
            set;
        }

        /// <summary>
        /// 端口
        /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
        public int Port
        {
            get;
            set;
        }

       /// <summary>
       /// 用户名
       /// </summary>
        /// <remarks>
        /// 	<para>创建：范亮</para>
        /// 	<para>日期：2016/4/2</para>
        /// </remarks>
       public string BucketName
       {
           get; 
           set;
       }

       /// <summary>
       /// 密码
       /// </summary>
       /// <remarks>
       /// 	<para>创建：范亮</para>
       /// 	<para>日期：2016/4/2</para>
       /// </remarks>
       public string BucketPassword
       {
           get; set;
       }

       /// <summary>
       /// DB实例
       /// </summary>
       public string Db
       {
           get; set;
       }

       public int MaxSize
       {
           get;
           set;
       }

       public int MinSize
       {
           get;
           set;
       }
    }
}
