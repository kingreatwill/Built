using System;
using System.Collections.Generic;
using System.Text;

namespace Built.AOP.Castle.Demo1
{
    /// <summary>
    ///IPerson 的摘要说明
    /// </summary>
    public interface IPerson
    {
        /// <summary>
        /// 姓名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 地址
        /// </summary>
        string Address { get; }

        /// <summary>
        /// 正在做什么
        /// </summary>
        /// <returns></returns>
        string Doing();
    }

    /// <summary>
    ///Person 的摘要说明
    /// </summary>
    public class Person : IPerson
    {
        public Person()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        #region IPerson 成员

        public string Name
        {
            get { return "我是花生米"; }
        }

        public string Address
        {
            get { return "我住在 http://pignut-wang.iteye.com/ "; }
        }

        public string Doing()
        {
            return "我正在写blog";
        }

        #endregion IPerson 成员
    }
}