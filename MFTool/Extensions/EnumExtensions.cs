using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MFTool
{
    /// <summary>
    ///     枚举扩展方法类
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// key = enumString
        /// value = Description;     
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        ///   foreach (var e in Enum.GetValues(typeof(State)))
        ///   {
        ///      rule[(State)e] = new Dictionary<Operation, Action>();
        ///   }待替换
        public static Dictionary<string, string> ToDictionary(Type enumType)
        {
            Dictionary<string, string> listitem = new Dictionary<string, string>();
            Array vals = Enum.GetValues(enumType);
            foreach (Enum enu in vals)
            {
                listitem.Add(enu.ToString(), enu.ToDescription());
            }

            return listitem;

        }

        /// <summary>
        /// 获取枚举项的Description特性的描述文字
        /// </summary>
        /// <param name="enumeration"> </param>
        /// <returns> </returns>
        public static string ToDescription(this Enum enumeration)
        {
            Type type = enumeration.GetType();
            MemberInfo[] members = type.GetMember(enumeration.CastTo<string>());
            if (members.Length > 0)
            {
                return members[0].ToDescription();
            }
            //return enumeration.CastTo<string>();
            return "DescriptionAttr未定义";
        }
              

        /// <summary>
        /// 完全包括  
        /// 枚举[FlagsAttribute]适用 值形式 1 2 4 8 16....
        /// </summary>
        /// <param name="enumeration"> </param>
        /// <returns> </returns>
        public static bool Contain(this Enum enumeration, Enum enum1)
        {
            return (enumeration.CastTo<int>() & enum1.CastTo<int>()) == enum1.CastTo<int>();
        }

        /// <summary>
        /// 可能有多重描述
        /// 适用 值形式 1 2 4 8 16....
        /// </summary>
        /// <param name="enumeration"></param>
        /// <param name="enum1"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string ToMultiDescription(this Enum enumeration, char split = ' ')
        {
            Type type = enumeration.GetType();
            string objName = enumeration.CastTo<string>();
            string result = "";
            MemberInfo[] members = type.GetMember(objName);
            if (members.Length > 0)
            {
                result = members[0].ToDescription();
            }
            else
            {
                string[] strs = objName.Split(',');//CastTo结果是由逗号分隔的
                if (strs.Length == 0)
                {
                    result = objName;
                }
                else
                {
                    for (int i = 0; i < strs.Length; i++)
                    {
                        MemberInfo[] tmpMembers = type.GetMember(strs[i].Trim());
                        if (tmpMembers.Length > 0)
                        {
                            result += tmpMembers[0].ToDescription() + split;
                        }
                    }
                }                
            }            

            return result.Trim(split);
        }
        
        /// <summary>
        /// 拥有，enum1可能有其他一些其没有的枚举值 
        /// 枚举[FlagsAttribute]适用 值形式 1 2 4 8 16....
        /// </summary>
        /// <param name="enumeration"> </param>
        /// <returns> </returns>
        public static bool Have(this Enum enumeration, Enum enum1)
        {
            int thisN = enumeration.CastTo<int>();
            int aimN = enum1.CastTo<int>();
            while (aimN != 0)
            {
                if ((thisN & aimN) != aimN)
                {
                    aimN = thisN & aimN;
                }
                else
                {
                    return true;
                }
            }
            
            return false;
        }
    }


}