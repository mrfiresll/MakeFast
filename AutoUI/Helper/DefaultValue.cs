using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoUI
{
    public class DefaultValue
    {
        public static string GetListButtonJson()
        {
            return @"[{'id':'btnAdd','iconcls':'iconfont-jia1','title':'增加','url':'/AutoUI/ConfigUI/Form/PageView?UICode={UICode}','hidden':'false'},
               {'id':'btnStartFlow','iconcls':'iconfont-faqiliucheng','title':'发起流程','url':'/WorkFlow/WFForm/PageView?UICode={UICode}','hidden':'false'},
               {'id':'btnEdit','iconcls':'iconfont-bianji1','title':'编辑','url':'/AutoUI/ConfigUI/Form/PageView?UICode={UICode}&Id={Id}','hidden':'false','Detail':'{ \'mustSelect\': \'OneRow\',\'height\': \'70%\',\'width\': \'80%\'}'},
               {'id':'btnDelete','iconcls':'iconfont-shanchu','title':'删除','url':'','hidden':'false','Detail':'{ \'mustSelect\': \'MultiRow\', \'onclick\': \'thisDelRow()\'}'},
               {'id':'btnExportExcel','iconcls':'iconfont-wenjian','title':'导出Excel','url':'','hidden':'false','Detail':'{ \'onclick\': \'exportExcel()\'}'},
               {'id':'btnSelect','iconcls':'iconfont-dianji','title':'选择','url':'','hidden':'false','Detail':'{ \'mustSelect\': \'MultiRow\', \'onclick\': \'selectRow()\'}'}]"
            ;
        }

        public static string GetListPropertyJson()
        {
            return @"{'idField':'id','toolbar':'#tb','border':'false','fit':'true','remoteSort':'false',
                      'rownumbers':'false','striped':'true','pagination':'true','fitColumns':'true',
                     'checkbox':'true','singleSelect':'true'}";
        }

        public static string MultiTextBoxJson()
        {
            return @"{'height':'200'}";
        }

    }
}