using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebBase.MetronicHelper
{
    public class MetronicHorizontalMenuItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Action { get; set; }
    }
    public class MetronicHorizontalMenu
    {
        private StringBuilder _sb;
        public MetronicHorizontalMenu(List<MetronicHorizontalMenuItem> items)
        {
            _sb = new StringBuilder();
            if (items != null)
            {
                foreach (MetronicHorizontalMenuItem item in items)
                {
                    Add(item.Name, item.Icon, item.Action);
                }
            }            
        }
        public static MetronicHorizontalMenu Create(List<MetronicHorizontalMenuItem> items = null)
        {
            return new MetronicHorizontalMenu(items);
        }

        public MetronicHorizontalMenu Add(string name, string icon, string action)
        {
            _sb.Append("<li class=\"m-menu__item\" data-menu-submenu-toggle=\"click\" data-redirect=\"true\">");

            _sb.Append("<a href=\"#\" onclick=\"" + action + "\" class=\"m-menu__link m-menu__toggle\">");

            _sb.Append("<i class=\"" + icon + "\"></i>");

            _sb.Append("<span class=\"m-menu__link-text\">");
            _sb.Append(name);
            _sb.Append("</span>");

            _sb.Append("</a>");

            _sb.Append("</li>");
            return this;
        }

        public MvcHtmlString Show()
        {             
            return MvcHtmlString.Create(_sb.ToString());
        }
    }
}