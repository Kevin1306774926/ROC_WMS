using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROC.Models;

namespace ROC.Data
{
    public partial interface IMenuClassService 
    {
        List<JsonTree> GetMenuClassTree();
        List<JsonTree> GetMenuTree();
    }

    /// <summary>
    /// 菜单服务类
    /// </summary>
    public partial class MenuClassService : IMenuClassService
    {

        public List<JsonTree> GetMenuClassTree()
        {
            using (MyDbContext db = new MyDbContext())
            {
                var list = db.MenuClasses.ToList<MenuClass>();
                return CreateMenuClass(list, null);
            }
                
        }

        /// <summary>
        /// 创建菜单分类数
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private List<JsonTree> CreateMenuClass(List<MenuClass> list, int? pid)
        {
            List<MenuClass> rootList = list.FindAll(t => t.ParentId == pid);
            List<JsonTree> rootNode = new List<JsonTree>();
            foreach (var item in rootList)
            {
                JsonTree node = new JsonTree();
                node.id = item.Id.ToString();
                node.text = item.Name;
                node.attributes = new attributes() { parentId = pid };
                node.children = CreateMenuClass(list, item.Id);
                rootNode.Add(node);
            }
            return rootNode;
        }

        public List<JsonTree> GetMenuTree()
        {
            using (MyDbContext db = new MyDbContext())
            {
                var list = db.MenuClasses.Include("Menus").ToList<MenuClass>();                
                return CreateMenuTree(list, null);
            }               
        }
        private List<JsonTree> CreateMenuTree(List<MenuClass> list, int? pid)
        {
            List<MenuClass> rootList = list.FindAll(t => t.ParentId == pid);
            List<JsonTree> rootNode = new List<JsonTree>();
            foreach (var item in rootList)
            {
                JsonTree node = new JsonTree();
                node.id = item.Id.ToString();
                node.text = item.Name;
                node.attributes = new attributes() { parentId = pid };
                node.children = CreateMenuTree(list, item.Id);
                node.children.AddRange(ConvertJsonTree(item.Menus));
                if (node.children.Count > 0)
                {
                    rootNode.Add(node);
                }
            }
            return rootNode;
        }

        private List<JsonTree> ConvertJsonTree(ICollection<Menu> collections)
        {
            List<JsonTree> list = new List<JsonTree>();
            foreach (var item in collections)
            {
                JsonTree node = new JsonTree();
                node.id = item.Controller;
                node.text = item.Name;
                node.attributes = new attributes() { parentId = item.Class, url = item.Url, controller = item.Controller };
                list.Add(node);
            }
            return list;
        }

    }

    public class JsonTree
    {
        public string id { get; set; }
        public string text { get; set; }
        public attributes attributes { get; set; }
        public List<JsonTree> children { get; set; }
    }

    public class attributes
    {
        public string url { get; set; }
        public int? parentId { get; set; }
        public string controller { get; set; }
    }
}
