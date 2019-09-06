using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Remoting;
using MFTool;
using Newtonsoft.Json;
using MF_Base;
using AutoUI.Controllers;
using System.Text.RegularExpressions;
using MF_Base.Model;
using AutoUI.Areas.ConfigUI.EasyUICtrl;
using UIBase;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace AutoUI.Areas.ConfigUI.Controllers
{
    [Export]
    public class FormController : BaseController
    {
        private Type _entityType;
        protected virtual Type EntityType
        {
            get
            {
                if (_entityType == null)
                {
                    string uiCode = QueryString("UICode");
                    string formId = QueryString("formId");
                    FormConfig formDef = null;
                    if (!string.IsNullOrEmpty(formId))
                    {
                        formDef = UnitOfWork.GetByKey<FormConfig>(formId);
                        formDef.CheckNotNull("未找到表单Id:" + formId);
                    }
                    else if (!string.IsNullOrEmpty(uiCode))
                    {
                        formDef = UnitOfWork.GetSingle<FormConfig>(a => a.UICode == uiCode);
                        formDef.CheckNotNull("未找到表单UICode:" + uiCode);
                    }

                    string entityFullName = formDef.EntityFullName;
                    string projName = formDef.DBName;
                    _entityType = ReflectionHelper.GetTypeBy(projName, entityFullName);
                }

                return _entityType;
            }
        }


        public virtual ActionResult PageView(string UICode)
        {
            var form = UnitOfWork.GetSingle<FormConfig>(a => a.UICode == UICode);
            form.CheckNotNull("未找到表单UICode:" + UICode);
            string formHtml = "", script = "";
            FetchFormHtmlAndScript(form, ref formHtml, ref script);

            ViewBag.FormHtml = formHtml;
            ViewBag.Script = script;
            ViewBag.FormId = form.Id;
            return View();
        }

        [ValidateInput(false)]
        public JsonResult Save()
        {
            string formData = QueryString("formData");
            formData.CheckNotNullOrEmpty("formData");

            var dic = formData.JsonToDictionary();
            //add
            if (string.IsNullOrEmpty(dic.GetValue("Id")))
            {
                return Add(dic);
            }
            //update
            else
            {
                return Update(dic);
            }
        }

        public JsonResult Add(Dictionary<string, object> dic)
        {
            var entity = Activator.CreateInstance(EntityType, new object[] { });

            #region 主表数据
            var mainTabKey = GuidHelper.CreateTimeOrderID();
            dic.SetValue("Id", mainTabKey);
            dic.SetValue("CreateUserID", GetCurrentUserID());
            dic.SetValue("CreateUserName", GetCurrentUserName());
            dic.SetValue("ModifyUserID", GetCurrentUserID());
            dic.SetValue("ModifyUserName", GetCurrentUserName());        

            BeforeAdd(dic);//
            ConvertHelper.UpdateEntity(entity, dic, false);

            var addMethod = typeof(EFBase.UnitOfWork).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            addMethod.CheckNotNull("UnitOfWork中的Add方法未找到或者不可访问");
            addMethod = addMethod.MakeGenericMethod(EntityType);
            var res = addMethod.Invoke(UnitOfWork, new object[] { entity });
            #endregion

            #region 子表数据
            var propertyInfos = EntityType.GetProperties().Where(a => a.PropertyType.Name == "List`1" && a.GetAccessors().Any(b => b.IsVirtual));
            foreach (var prop in propertyInfos)
            {
                var innerType = prop.PropertyType.GenericTypeArguments[0];
                //基类必须是Entity                       
                if (innerType.BaseType.Name != "Entity")
                {
                    throw new Exception("List内对应类型的基类必须是Entity");
                }

                var subListJson = dic.GetValue(prop.Name);
                var subDicList = subListJson.JsonToDictionaryList();

                var subAddMethod = typeof(EFBase.UnitOfWork).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
                subAddMethod.CheckNotNull("UnitOfWork中的Add方法未找到或者不可访问");
                subAddMethod = subAddMethod.MakeGenericMethod(innerType);

                //获取子表的主外键字段名
                var v = (ForeignKeyAttribute[])prop.GetCustomAttributes(typeof(ForeignKeyAttribute), false);
                v.CheckNotNullOrEmpty("外键字段{0}未定义ForeignKeyAttribute".ReplaceArg(prop.Name));
                var foreignKeyName = v[0].Name;
                foreignKeyName.CheckNotNull("外键字段{0}的ForeignKeyAttribute的未传外键名参数".ReplaceArg(prop.Name));

                foreach (var subDic in subDicList)
                {
                    var subEntity = Activator.CreateInstance(innerType, new object[] { });

                    //子表主键赋值
                    subDic.SetValue("Id", GuidHelper.CreateTimeOrderID());
                    //子表的主表外键字段赋值
                    subDic.SetValue(foreignKeyName, mainTabKey);

                    subDic.SetValue("CreateUserID", GetCurrentUserID());
                    subDic.SetValue("CreateUserName", GetCurrentUserName());
                    subDic.SetValue("ModifyUserID", GetCurrentUserID());
                    subDic.SetValue("ModifyUserName", GetCurrentUserName());

                    ConvertHelper.UpdateEntity(subEntity, subDic, false);

                    subAddMethod.Invoke(UnitOfWork, new object[] { subEntity });
                }
            }
            #endregion
            
            UnitOfWork.Commit();

            return Json(res);
        }

        protected virtual void BeforeAdd(Dictionary<string, object> dic)
        {

        }

        public JsonResult Delete(string ids)
        {
            ids.CheckNotNullOrEmpty("guid");
            var idArr = ids.JsonToObject<IEnumerable<string>>();
            BeforeDelete(idArr);//

            var delMethod = typeof(EFBase.UnitOfWork).GetMethod("DeleteByKey", BindingFlags.Instance | BindingFlags.Public);
            delMethod.CheckNotNull("UnitOfWork中的DeleteByKey方法未找到或者不可访问");
            delMethod = delMethod.MakeGenericMethod(EntityType);
            
            foreach (var id in idArr)
            {
                delMethod.Invoke(UnitOfWork, new object[] { id });
            }
            return Json(UnitOfWork.Commit());
        }

        protected virtual void BeforeDelete(IEnumerable<string> ids)
        {

        }

        public JsonResult Update(Dictionary<string, object> dic)
        {
            BeforeUpdate(dic);//
            var mainTabKey = dic.GetValue("Id");
            #region 主表数据
            dic.SetValue("ModifyUserID", GetCurrentUserID());
            dic.SetValue("ModifyUserName", GetCurrentUserName());           

            //取
            var getMethod = typeof(EFBase.UnitOfWork).GetMethod("GetByKey", BindingFlags.Instance | BindingFlags.Public);
            getMethod.CheckNotNull("UnitOfWork中的GetByKey方法未找到或者不可访问");
            getMethod = getMethod.MakeGenericMethod(EntityType);
            var entity = getMethod.Invoke(UnitOfWork, new object[] { mainTabKey });

            //更新
            ConvertHelper.UpdateEntity(entity, dic, false);
            #endregion

            #region 子表数据
            var propertyInfos = EntityType.GetProperties().Where(a => a.PropertyType.Name == "List`1" && a.GetAccessors().Any(b => b.IsVirtual));
            foreach (var prop in propertyInfos)
            {
                var innerType = prop.PropertyType.GenericTypeArguments[0];
                //基类必须是Entity                       
                if (innerType.BaseType.Name != "Entity")
                {
                    throw new Exception("List内对应类型的基类必须是Entity");
                }

                var subListJson = dic.GetValue(prop.Name);
                var subDicList = subListJson.JsonToDictionaryList();
                var existIdList = subDicList.Select(a => a.GetValue("Id"));

                var subQuery = prop.GetValue(entity, null);
                //拷贝至subDBList中(否则foreach中执行delete会报错)
                Type genericListType = typeof(List<>);
                Type concreteListType = genericListType.MakeGenericType(innerType);
                var subDBList = (System.Collections.IEnumerable)Activator.CreateInstance(concreteListType, new object[] { subQuery });

                //增
                var subAddMethod = typeof(EFBase.UnitOfWork).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
                subAddMethod.CheckNotNull("UnitOfWork中的Add方法未找到或者不可访问");
                subAddMethod = subAddMethod.MakeGenericMethod(innerType);
                //取
                var subGetMethod = typeof(EFBase.UnitOfWork).GetMethod("GetByKey", BindingFlags.Instance | BindingFlags.Public);
                subGetMethod.CheckNotNull("UnitOfWork中的GetByKey方法未找到或者不可访问");
                subGetMethod = subGetMethod.MakeGenericMethod(innerType);              

                //获取子表的主外键字段名
                var v = (ForeignKeyAttribute[])prop.GetCustomAttributes(typeof(ForeignKeyAttribute), false);
                v.CheckNotNullOrEmpty("外键字段{0}未定义ForeignKeyAttribute".ReplaceArg(prop.Name));
                var foreignKeyName = v[0].Name;
                foreignKeyName.CheckNotNull("外键字段{0}的ForeignKeyAttribute的未传外键名参数".ReplaceArg(prop.Name));

                foreach (var subDic in subDicList)
                {
                    subDic.SetValue("ModifyUserID", GetCurrentUserID());
                    subDic.SetValue("ModifyUserName", GetCurrentUserName());

                    var subEntity = subGetMethod.Invoke(UnitOfWork, new object[] { subDic.GetValue("Id") });

                    //更新
                    if (subEntity != null)
                    {
                        ConvertHelper.UpdateEntity(subEntity, subDic, true);
                    }
                    //新增
                    else
                    {
                        subEntity = Activator.CreateInstance(innerType, new object[] { });

                        //子表主键赋值
                        subDic.SetValue("Id", GuidHelper.CreateTimeOrderID());
                        //子表的主表外键字段赋值
                        subDic.SetValue(foreignKeyName, mainTabKey);

                        subDic.SetValue("CreateUserID", GetCurrentUserID());
                        subDic.SetValue("CreateUserName", GetCurrentUserName());

                        ConvertHelper.UpdateEntity(subEntity, subDic, false);
                        subAddMethod.Invoke(UnitOfWork, new object[] { subEntity });
                    }
                }

                //删
                var subDeleteMethod = typeof(EFBase.UnitOfWork).GetMethod("DeleteByKey", BindingFlags.Instance | BindingFlags.Public);
                subDeleteMethod.CheckNotNull("UnitOfWork中的DeleteByKey方法未找到或者不可访问");
                subDeleteMethod = subDeleteMethod.MakeGenericMethod(innerType);

                foreach (var subItem in subDBList)
                {
                    string id = innerType.GetProperty("Id").GetValue(subItem).ToString();
                    if (!existIdList.Contains(id))
                    {
                        subDeleteMethod.Invoke(UnitOfWork, new object[] { id });
                    }
                }
            }
            #endregion
            
            UnitOfWork.Commit();
            return Json(entity);
        }

        protected virtual void BeforeUpdate(Dictionary<string, object> dic)
        {

        }

        public JsonResult Get(string formId, string Id)
        {
            var getMethod = typeof(EFBase.UnitOfWork).GetMethod("GetByKey", BindingFlags.Instance | BindingFlags.Public);
            getMethod.CheckNotNull("UnitOfWork中的GetByKey方法未找到或者不可访问");
            getMethod = getMethod.MakeGenericMethod(EntityType);
            var res = getMethod.Invoke(UnitOfWork, new object[] { Id });
            Dictionary<string, object> dicRes = new Dictionary<string, object>();
            res.CheckNotNull("未找到Id为{0}的数据".ReplaceArg(Id));
            if (res != null)
            {
                dicRes = res.ToJsonIgnoreLoop().JsonToDictionary();
                var propertyInfos = EntityType.GetProperties();
                foreach (var prop in propertyInfos)
                {
                    //对应外键表
                    //一对一
                    if (prop.PropertyType.BaseType.Name == "Entity" && prop.GetAccessors().Any(a => a.IsVirtual))//virtual
                    {
                        object foreignObj = prop.GetValue(res, null);
                        if (foreignObj != null)
                        {
                            var dic = foreignObj.ToJsonIgnoreLoop().JsonToDictionary();
                            var foreignTableName = prop.Name;
                            foreach (var tmp in dic)
                            {
                                dicRes.SetValue(foreignTableName + "__________" + tmp.Key, tmp.Value);//xxx__________xxxx 相当于xxx对象的xxxx属性
                            }
                        }
                    }                    
                }
            }

            return Json(dicRes);
        }

        private void FetchFormHtmlAndScript(FormConfig form, ref string formLayout, ref string script)
        {
            formLayout = Server.HtmlDecode(form.LayOutSetting);
            var ctrlAttrList = form.GetCtrlAttrList();
            foreach (var ctrlAttr in ctrlAttrList)
            {
                //控件html
                var ctrl = GeneralEasyuiCtrl(ctrlAttr);
                if (ctrl == null) continue;

                script += ctrl.GetScript() + "\n\r";
                string easyuiCtrlHtml = ctrl.GetCtrlHtm();
                if (string.IsNullOrEmpty(easyuiCtrlHtml))
                    continue;

                //替换标签
                string fieldToReplace = ctrlAttr.GetValue("ColumnName");
                if (string.IsNullOrEmpty(fieldToReplace))
                {
                    fieldToReplace = ctrlAttr.GetValue("FieldName");
                }
                if (!string.IsNullOrEmpty(formLayout))
                    formLayout = formLayout.Replace("{" + fieldToReplace + "}", easyuiCtrlHtml);
            }
        }

        private CtrlBase GeneralEasyuiCtrl(Dictionary<string, object> ctrl)
        {
            string strCtrlType = ctrl.GetValue("CtrlType");
            if (string.IsNullOrEmpty(strCtrlType))
                return null;

            string dataOptions = ctrl.GetValue("Detail");
            var dOptionDic = dataOptions.JsonToDictionary();
            var styleDic = new Dictionary<string, object>();
            var classNames = new List<string>();
            var fieldName = ctrl.GetValue("FieldName");

            if (!string.IsNullOrEmpty(dataOptions))
            {
                #region style
                List<string> dOptionKeyToRemove = new List<string>();
                foreach (var item in dOptionDic)
                {
                    if (item.Key.Contains("style"))
                    {
                        var tmp = item.Key.Split('_');
                        if (tmp.Length == 2)
                        {
                            styleDic.SetValue(tmp[1], item.Value);
                            dOptionKeyToRemove.Add(item.Key);
                        }
                    }
                }
                foreach (var item in dOptionKeyToRemove)
                {
                    dOptionDic.Remove(item);//如果是style 部分属性设置，则从dataoptions中移除        
                }
                #endregion
                #region  class

                if (string.IsNullOrEmpty(ctrl.GetValue("IsVisible")) || ctrl.GetValue("IsVisible") != "是")
                {
                    dOptionDic.Add("visible", "false");
                }
                #endregion
                if (string.IsNullOrEmpty(ctrl.GetValue("Enable")) || ctrl.GetValue("Enable") != "是")
                {
                    dOptionDic.SetValue("disabled", true);
                }
            }
            EasyUICtrlPrepareData prepareData = new EasyUICtrlPrepareData()
            {
                ClassNames = classNames,
                Style = styleDic,
                DataOptions = dOptionDic
            };

            var easyUICtrl = EasyUICtrlFactory.GetCtrl(strCtrlType, fieldName, prepareData);
            return easyUICtrl;
        }
    }
}
