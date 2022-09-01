﻿using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.User;

public class UserRepository : EFRepository<Entities.User>, IUserRepository {
    public UserRepository(ScheduleContext context) : base(context) { }

    public override async Task<bool> Add(Entities.User item) {
        var user = MapAdd(item);

        if (Context.Users.Any(u => u.Login == user.Login)) return false;

        await Context.Users.AddAsync(user);

        return true;
    }

    public override async Task<bool> AddRange(IEnumerable<Entities.User> entities) {
        var enumerable = entities.ToList();

        for (var i = 0; i < enumerable.Count; i++) {
            if (Context.Users.Any(u => u.Login == enumerable[i].Login)) return false;
            enumerable[i] = MapAdd(enumerable[i]);

        }

        await Context.Users.AddRangeAsync(enumerable);

        return true;
    }

    public override IQueryable<Entities.User> ReadById(int id) {
        return Read().Where(el => el.Id == id).AsQueryable();
    }

    public override async Task<bool> UpdateAsync(Entities.User item) {
        if (await Context.Users.AnyAsync(u => (u.Id == item.Id) ? u.Login != item.Login : u.Login == item.Login)) return false;

        Context.Entry(item).State = EntityState.Modified;

        Context.Users.Update(item);

        return true;
    }

    public override bool Update(Entities.User item) {
        if (Context.Users.Any(u => (u.Id == item.Id) ? u.Login != item.Login : u.Login == item.Login)) return false;

        Context.Entry(item).State = EntityState.Modified;

        Context.Users.Update(item);

        return true;
    }

    public override async Task<bool> Delete(int id) {
        var user = await Context.Users.Where(userQuery => userQuery.Id == id)
            .Include(user => user.Homework)
            .Include(user => user.UsersRoles)
            .FirstOrDefaultAsync();


        if (user == null) return false;

        var userOwnedGroups = user.UsersRoles.Where(role => role.IsOwner).Select(role => role.Group).ToList();

        if (userOwnedGroups.All(g => g != null)) Context.Groups.RemoveRange(userOwnedGroups!);
        Context.Homework.RemoveRange(user.Homework);
        Context.Users.Remove(user);

        return true;
    }

    public override bool Add(ref Entities.User item) {
        var user = MapAdd(item);

        if (Context.Users.Any(u => u.Login == user.Login)) return false;

        item = Context.Users.Add(user).Entity;

        return true;
    }

    protected override Entities.User MapAdd(Entities.User item) {
        return new Entities.User {
            Id = 0,
            IsAdmin = false,
            UsersRoles = new List<UserRole>(),
            Homework = new List<HomeworkTask>(),
            ProfileImage = item.ProfileImage,
            Login = item.Login,
            Name = item.Name,
            Password = item.Password,
            Salt = item.Salt
        };
    }
}