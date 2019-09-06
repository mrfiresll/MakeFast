using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data;
using System.Web;
using System.IO;
using System.Configuration;
using MFTool;
using UIBase;
using System.ComponentModel.Composition;

public static class HtmlHelperExtend
{
    #region GetEnum

    public static MvcHtmlString GetDBEnum(this HtmlHelper html, string enumCode)
    {
        try
        {
            var dic = CommonHelper.GetDBEnum(enumCode);
            string result = string.Format("var {0} = {1};", enumCode, dic.ToJson());
            return MvcHtmlString.Create(result);
        }
        catch (Exception ex)
        {
            throw new Exception("枚举类型");
        }
    }

    //public static MvcHtmlString ClearEnumCache(this HtmlHelper html, string enumKey, string category = "", string subcategory = "")
    //{
    //    try
    //    {
    //        string key = string.Format("EnumJson_{0}_{1}_{2}", enumKey, category, subcategory);
    //        HttpRuntime.Cache.Remove(key);
    //        return MvcHtmlString.Create("");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BusinessException(ex.Message);
    //    }
    //}

    //public static MvcHtmlString GetEnum(this HtmlHelper html, string enumKey, string enumName = "", string category = "", string subcategory = "")
    //{
    //    try
    //    {
    //        if (string.IsNullOrEmpty(enumName))
    //            enumName = enumKey;
    //        enumName = enumName.Split('.').Last();
    //        var json = FormulaHelper.GetService<IEnumService>().GetEnumJson(enumKey, category, subcategory);
    //        string result = string.Format("var {0} = {1};", enumName, json);
    //        return MvcHtmlString.Create(result);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BusinessException(ex.Message);
    //    }
    //}

    public static MvcHtmlString GetEnum(this HtmlHelper html, Type type, string defaultEnumStr = "", string enumName = "")
    {
        try
        {
            var dic = EnumExtensions.ToDictionary(type);

            if (string.IsNullOrEmpty(enumName))
                enumName = type.Name;

            List<dynamic> textValuePairs = new List<dynamic>();
            foreach(var item in dic)
            {
                if (defaultEnumStr.ToLower() == item.Key.ToLower().ToString())
                {
                    textValuePairs.Add(new { text = item.Value, value = item.Key.ToLower(), selected = true });
                }
                else
                {
                    textValuePairs.Add(new { text = item.Value, value = item.Key.ToLower() });
                }
            }

            string result = string.Format("var {0} = {1};", enumName, textValuePairs.ToJson());
            return MvcHtmlString.Create(result);
        }
        catch (Exception ex)
        {
            throw new Exception("枚举类型" + type.Name + "提取内容时失败", ex);
        }
    }

    public static MvcHtmlString GetEnumWithFormatter(this HtmlHelper html, Type type, string enumName = "")
    {
        try
        {
            var dic = EnumExtensions.ToDictionary(type);

            if (string.IsNullOrEmpty(enumName))
                enumName = type.Name;

            var textValuePairs = dic.Select(a => new { text = a.Value, value = a.Key });
            string result = string.Format("var {0} = {1};", enumName, textValuePairs.ToJson());
            result += "\r\n";
            string foramtter = @"function " + enumName + @"Formatter(value, rowData, rowIndex)
                                {if (value == 0){return;}for (var i = 0; i < " + enumName + @".length; i++)
                                {if (" + enumName + "[i].value == value){return " + enumName + "[i].text;}}}";
            result += foramtter;
            return MvcHtmlString.Create(result);
        }
        catch (Exception ex)
        {
            throw new Exception("枚举类型" + type.Name + "提取内容时失败", ex);
        }
    }

    public static MvcHtmlString DBNames(this HtmlHelper html, string enumName = "DBNames")
    {
        var tmpList = WebConfigHelper.GetDBNames();
        List<dynamic> textValuePairs = new List<dynamic>();
        foreach (var item in tmpList)
        {
            textValuePairs.Add(new { text = item, value = item });
        }

        string result = string.Format("var {0} = {1};", enumName, textValuePairs.ToJson());
        return MvcHtmlString.Create(result);
    }

    #endregion    
}
