using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using NCode.Composition.DisposableParts;

namespace UIBase
{
    /// <summary>
    /// 存储ComposablePartCatalog的方案
    /// 要使用依赖注入必须使用ComposablePartCatalog
    /// 将其存入HttpContext.Current.Items
    /// </summary>
    public class MefDependencyResolver : IDependencyResolver
    {
        private DisposableWrapperCatalog _catalog;
        private const string _iockey = "iocKey";

        public MefDependencyResolver(DisposableWrapperCatalog log)
        {
            _catalog = log;
        }

        public CompositionContainer Container
        {
            get
            {
                CompositionContainer container;
                if (!HttpContext.Current.Items.Contains(_iockey))
                {
                    container = new CompositionContainer(_catalog, CompositionOptions.DisableSilentRejection);
                    HttpContext.Current.Items.Add(_iockey, container);
                    //HttpContext.Current.DisposeOnPipelineCompleted(container);
                }
                else
                {
                    container = (CompositionContainer)HttpContext.Current.Items[_iockey];
                }
                return container;
            }
        }

        public object GetService(Type serviceType)
        {
            string contractName = AttributedModelServices.GetContractName(serviceType);
            return Container.GetExportedValueOrDefault<object>(contractName);

        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.GetExportedValues<object>(serviceType.FullName);
        }
    }
}