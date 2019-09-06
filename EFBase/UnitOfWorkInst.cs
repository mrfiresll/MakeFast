using MFTool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFBase
{
    /// <summary>
    /// 实例用
    /// </summary>
    [Export]
    public class UnitOfWorkInst
    {
        [Import]
        public IUnitOfWork UWork = null;
        public UnitOfWorkInst()
        {
 
        }
    }
}
