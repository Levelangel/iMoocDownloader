using System;
namespace imooc_download
{
    class Class
    {
        /// <summary>
        /// 课程名字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 课程地址
        /// </summary>
        public ClassUrl Url { get; set; }

        /// <summary>
        /// 课程视频号
        /// </summary>
        public string ClassNumber { get; set; }
    }

    class ClassUrl
    {
        /// <summary>
        /// 标清
        /// </summary>
        public string UrlL { get; set; }
        /// <summary>
        /// 高清
        /// </summary>
        public string UrlM { get; set; }
        /// <summary>
        /// 超清
        /// </summary>
        public string UrlH { get; set; }
    }
}
