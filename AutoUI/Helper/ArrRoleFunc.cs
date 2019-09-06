//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Web;

//namespace AutoUI
//{
//    [Serializable]
//    public class ArrRoleFunc
//    {
//        private List<OAFuncDTO> _source { get; set; }

//        public ArrRoleFunc(List<OAFuncDTO> source)
//        {
//            _source = source;
//        }

//        /// <summary>
//        /// 是否拥有ctrlId控件的权限
//        /// </summary>
//        /// <param name="ctrlId"></param>
//        /// <returns></returns>
//        public bool Have(string ctrlId)
//        {
//            string url = HttpContext.Current.Request.RawUrl;
//            Debug.Assert(!string.IsNullOrEmpty(url), "当前路径为空");
//            OAFuncDTO parent = _source.SingleOrDefault(a => !string.IsNullOrEmpty(a.Attributes) && a.Attributes.Contains(url));
//            Debug.Assert(parent != null, "父项为空");
//            return _source.Any(a => a.Attributes == ctrlId && a.ParentId == parent.Id);
//        }

//        /// <summary>
//        /// 获取对应控件
//        /// </summary>
//        /// <param name="ctrlId"></param>
//        /// <returns></returns>
//        public OAFuncDTO Get(string ctrlId)
//        {
//            string url = HttpContext.Current.Request.RawUrl;
//            Debug.Assert(!string.IsNullOrEmpty(url), "当前路径为空");
//            OAFuncDTO parent = _source.SingleOrDefault(a => !string.IsNullOrEmpty(a.Attributes) && a.Attributes.Contains(url));
//            Debug.Assert(parent != null, "父项为空");
//            return _source.SingleOrDefault(a => a.Attributes == ctrlId && a.ParentId == parent.Id);
//        }

//        public List<OAFuncDTO> Children(int parentId)
//        {
//            return _source.Where(a => a.ParentId == parentId).OrderBy(a => a.Order).ToList();
//        }

//        public List<OAFuncDTO> FirstChildren()
//        {
//            List<OAFuncDTO> tmps = Modules();
//            if(tmps.Count > 0)
//            {
//                OAFuncDTO firstModule = tmps[0];
//                return Children(firstModule.Id);
//            }
//            else
//            {
//                return new List<OAFuncDTO>();
//            }
//        }

//        public List<OAFuncDTO> Modules()
//        {
//            return _source.Where(a => a.EnumFuncType == EnumFuncType.Module).OrderBy(a => a.Order).ToList();
//        }
//    }
//}