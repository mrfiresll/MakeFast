using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebBase.MetronicHelper
{
    public class MetronicSideMenuItem
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
    }
    public class MetronicSideMenu
    {
        private List<MetronicSideMenuItem> _items;
        public MetronicSideMenu()
        {
            _items = new List<MetronicSideMenuItem>();
        }

        public MetronicSideMenu(List<MetronicSideMenuItem> items)
        {
            _items = items;
        }

        public string GetHtml()
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<MetronicSideMenuItem> parents = _items.Where(a => !_items.Any(b => b.Id == a.ParentId));

            foreach (var p in parents)
            {
                sb.Append("<li class=\"m-menu__item  m-menu__item--submenu\" aria-haspopup=\"true\"  data-menu-submenu-toggle=\"hover\">");
                SetMenu(sb, p, true);
                sb.Append("</li>");
            }

            return sb.ToString();
        }

        private void SetMenu(StringBuilder sb, MetronicSideMenuItem item,bool first = false)
        {
            IEnumerable<MetronicSideMenuItem> children = _items.Where(a => a.ParentId == item.Id);
            if (children.Count() != 0)
            {
                sb.Append("<a href=\"#\" class=\"m-menu__link m-menu__toggle\">");
                sb.Append("<i class=\"" + item.Icon + "\">");
                sb.Append("</i>");
            }
            else
            {
                sb.Append("<a href=\"#\" onclick=\"showPageInMainFrame('" + item.Url + "')\" class=\"m-menu__link \">");
                sb.Append("<i class=\"m-menu__link-bullet m-menu__link-bullet--dot\">");
                sb.Append("<span></span>");
                sb.Append("</i>");
            }

            sb.Append("<span class=\"m-menu__link-text\">");
            sb.Append(item.Name);
            sb.Append("</span>");
            if (children.Count() != 0)
            {
                sb.Append("<i class=\"m-menu__ver-arrow la la-angle-right\"></i>");
            }
            
            sb.Append("</a>");
           
            if (children.Count() != 0)
            {
                sb.Append("<div class=\"m-menu__submenu \">");
                sb.Append("<span class=\"m-menu__arrow\"></span>");

                sb.Append("<ul class=\"m-menu__subnav\">");

                if(first)
                {
                    sb.Append("<li class=\"m-menu__item  m-menu__item--parent\" aria-haspopup=\"true\" >");
                    sb.Append("<span class=\"m-menu__link\">");
                    sb.Append("<span class=\"m-menu__link-text\">");
                    sb.Append(item.Name);
                    sb.Append("</span>");
                    sb.Append("</span>");
                    sb.Append("</li>");
                }                

                foreach (var tmp in children)
                {
                    if (first)
                    {
                        sb.Append("<li class=\"m-menu__item  m-menu__item--submenu\" aria-haspopup=\"true\"  data-menu-submenu-toggle=\"hover\">");
                    }
                    else
                    {
                        sb.Append("<li class=\"m-menu__item\" aria-haspopup=\"true\">");
                    }
                    SetMenu(sb, tmp);
                    sb.Append("</li>");
                }

                sb.Append("</ul>");

                sb.Append("</div>");
            }
        }
    }
}