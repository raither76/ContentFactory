using ContentFactory.Data;
using ContentFactory.Models;
using Microsoft.AspNetCore.Identity;

namespace ContentFactory.Services
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string password = "_Aa123456";
            if (await roleManager.FindByNameAsync("Администратор") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Администратор"));
            }
            if (await roleManager.FindByNameAsync("Менеджер") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Менеджер"));
            }
            if (await roleManager.FindByNameAsync("Пользователь") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Пользователь"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Администратор");
                }
            }
        }
    }
    public class VideoInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            int count = context.Videos.Count();
            switch (count)
            {
                case 0:
                    Video v;
                    for (int i = 1; i < 3; i++)
                    {
                        v = new Video() { Name = $"Видео - {i}", Description = $"Описание видео - {i}" };
                        context.Videos.Add(v);
                        await context.SaveChangesAsync();
                    }
                    break;
                case 1:
                    v = new Video() { Name = $"Новое видео", Description = $"Описание видео" };
                    context.Videos.Add(v);
                    await context.SaveChangesAsync();
                    break;
                default:
                    break;
            }



        }
    }
    public class TxtInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            var items = context.Txts.OrderBy(x => x.Id).ToList();
            if (items != null && items.Count < 5)
            {
                context.Txts.RemoveRange(items);
                await context.SaveChangesAsync();
                items.AddRange(new List<Txt>() {
                new Txt() { Name ="Базовые фото"},
                new Txt() { Name ="Добавим планов и деталей"},
                new Txt() { Name ="Покажем функции и варианты носки"},
                new Txt() { Name ="Добавим аксессуаров и создадим образ"},
                new Txt() { Name ="Добавим динамику"}});
                context.Txts.AddRange(items);
                await context.SaveChangesAsync();


            }

        }
    }
}
