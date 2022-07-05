using ContentFactory.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ContentFactory.Code
{
    public static class BuildHelper
    {
        public static string result = "";
        public static int CatId;
        public static int Level = 1;

        public static HtmlString BuildModels(this IHtmlHelper html, List<Model> model)
        {
            string str = "";
            if (model.Any())
            {
                foreach (var item in model)
                {
                    str = $"{str}<li class='nav-item'><a class='nav-link main-link cat-link' " +
                            $"data-ajax = 'true' data-ajax-method = 'GET' data-ajax-mode = 'replace' data-ajax-update = '#results' href='/Admin/EditModel?Id={item.Id}' " +
                            $"alt='{item.Id}'><i class='fa fa-fw fa-edit'></i><span>{item.Name}</span></a></li>";

                }
            }
            return new HtmlString(str);

        }
        public static HtmlString BuildCatalog(this IHtmlHelper html, List<Category> model)
        {
            result = "";
            Level = 1;
            if (model.Any())
            {
                foreach (var item in model)
                {
                    result = $"{result}<li class='nav-item'>" +
                        $"<a class='nav-link main-link' data-bs-toggle='collapse' data-parent = '#accordion' href='#GroupCollapse{item.Id}'>" +
                        $"<i class='fa fa-fw fa-database'></i><span>{item.Name}</span><i class='fa fa-fw fa-angle-down'></i>" +
                        $"</a><ul id='GroupCollapse{item.Id}' class='nav nav-pills nav-stacked collapse'>" +
                        $"<li class='nav-item' style='text-align:center;border-bottom:1px solid gray; padding-bottom:2px; background-color:azure;'>" +
                        $"<a class='nav-link' style='font-weight:600;color:black;' href='/Admin/AddProduct?Category={item.Id}'>" +
                        $"<i class='fa fa-cog fa-plus fa-fw'></i><span> Добавить категорию</span></a></li>";
                    CatId = item.Id;

                    result = CreateMenuItems(item.Catalogs);

                    // CatId = 0;

                }
                result = $"{result}</ul>";
                result = result.Insert(result.IndexOf("li class='") + 10, "active ");
            }
            return new HtmlString(result);
        }
        public static string CreateMenuItems(List<Catalog> cat, int? parentId = null)
        {

            var items = cat.Where(d => d.ParentId == parentId).OrderBy(i => i.Name);
            if (parentId == null) Level = 1;
            if (items.Any())
            {
                foreach (var item in items)
                {
                    if (item.Children.Count > 0)
                    {
                        result = $"{result}<li class='nav-item'>" +
                            $"<a class='nav-link main-link' data-bs-toggle = 'collapse' data-parent = '#accordion' href='#collapse{item.Id}'>" +
                            $"<i class='fa fa-fw fa-database'></i><span>{item.Name}</span><i class='fa fa-fw fa-angle-down'></i>" +
                            $"</a><ul id='collapse{item.Id}' class='nav nav-pills nav-stacked collapse'>" +
                            $"<li class='nav-item' style='text-align:center;border-bottom:1px solid gray; padding-bottom:2px; background-color:azure;'>" +
                            $"<a class='nav-link' style='font-weight:600;color:black;' href='/Admin/AddProduct?Category={CatId}&Parent={item.Id}'>" +
                            $"<i class='fa fa-cog fa-plus fa-fw'></i><span> Добавить подкатегорию</span></a></li>";
                        Level++;
                    }
                    else
                    {
                        if (Level == 2)
                        {

                            result = $"{result}<li class='nav-item'><a class='nav-link main-link cat-link' " +
                                $"data-ajax = 'true' data-ajax-method = 'GET' data-ajax-mode = 'replace' data-ajax-update = '#results' href='/Admin/AddImages?CatalogId={item.Id}' " +
                                $"alt='{item.Id}'><i class='fa fa-fw fa-database'></i><span>{item.Name}</span></a></li>";

                        }
                        else
                        {
                            result = $"{result}<li class='nav-item'><a class='nav-link main-link cat-link' href='/Admin/AddProduct?Category={CatId}&Parent={item.Id}' alt='{item.Id}'><i class='fa fa-fw fa-database'></i><span>{item.Name}</span></a></li>";
                        }

                    }
                    CreateMenuItems(cat, item.Id);


                }
                Level--;
                result = $"{result}</ul>";

            }

            return result;
        }
    }
}
