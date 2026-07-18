using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Models;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Expenses;
using KakeiboShare.Domain.Groups;
using KakeiboShare.Domain.Settlements;
using KakeiboShare.Domain.Users;

namespace KakeiboShare.Application.Tests.Fakes;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}

internal sealed class FakeGroupRepository : IGroupRepository
{
    private readonly Dictionary<Guid, Group> _groups = new();

    public void Seed(Group group) => _groups[group.Id] = group;

    public Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_groups.GetValueOrDefault(id));

    public Task<Group?> GetByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default) =>
        Task.FromResult(_groups.Values.FirstOrDefault(g => g.InviteCode.Value == inviteCode.Trim().ToUpperInvariant()));

    public Task<IReadOnlyList<Group>> ListByMemberAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Group>>(_groups.Values.Where(g => g.MemberUserIds.Contains(userId)).ToList());

    public Task<IReadOnlyList<GroupMemberDetail>> ListMemberDetailsAsync(Guid groupId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<GroupMemberDetail>>([]);

    public Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        _groups[group.Id] = group;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Group group, CancellationToken cancellationToken = default)
    {
        _groups[group.Id] = group;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        _groups.Remove(groupId);
        return Task.CompletedTask;
    }
}

internal sealed class FakeExpenseRepository : IExpenseRepository
{
    private readonly Dictionary<Guid, ExpenseDetail> _details = new();

    public IReadOnlyList<ExpenseDetail> AllDetails => _details.Values.ToList();

    public void Seed(ExpenseDetail detail) => _details[detail.Expense.Id] = detail;

    public Task<Expense?> GetWithSharesAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_details.GetValueOrDefault(id)?.Expense);

    public Task<ExpenseDetail?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_details.GetValueOrDefault(id));

    public Task<IReadOnlyList<Expense>> ListByGroupAsync(Guid groupId, bool? settled, CancellationToken cancellationToken = default)
    {
        var list = _details.Values
            .Where(d => d.Expense.GroupId == groupId)
            .Select(d => d.Expense)
            .Where(e => settled switch
            {
                true => e.IsSettled,
                false => !e.IsSettled,
                _ => true,
            })
            .ToList();
        return Task.FromResult<IReadOnlyList<Expense>>(list);
    }

    public Task<IReadOnlyList<ExpenseSummary>> ListSummariesByGroupAsync(Guid groupId, bool? settled,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ExpenseSummary>>([]);

    public Task AddAsync(Expense expense, string description, DateOnly expenseDate,
        IReadOnlyDictionary<Guid, int?> shareRatios, CancellationToken cancellationToken = default)
    {
        _details[expense.Id] = new ExpenseDetail(expense, description, expenseDate, shareRatios);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Expense expense, string description, DateOnly expenseDate,
        IReadOnlyDictionary<Guid, int?> shareRatios, CancellationToken cancellationToken = default)
    {
        _details[expense.Id] = new ExpenseDetail(expense, description, expenseDate, shareRatios);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        _details.Remove(expense.Id);
        return Task.CompletedTask;
    }
}

internal sealed class FakeSettlementRepository : ISettlementRepository
{
    public Settlement? LastAdded { get; private set; }

    public Task<Settlement?> GetWithTransfersAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult<Settlement?>(null);

    public Task<IReadOnlyList<Settlement>> ListByGroupAsync(Guid groupId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Settlement>>([]);

    public Task AddAsync(Settlement settlement, CancellationToken cancellationToken = default)
    {
        LastAdded = settlement;
        return Task.CompletedTask;
    }
}

internal sealed class FakeMembershipChecker : IGroupMembershipChecker
{
    public HashSet<(Guid GroupId, Guid UserId)> Allowed { get; } = [];

    public Task EnsureMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (!Allowed.Contains((groupId, userId)))
            throw new ForbiddenException("グループのメンバーではありません");
        return Task.CompletedTask;
    }

    public Task<Guid> EnsureMemberOfExpenseAsync(Guid expenseId, Guid userId, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = new();
    private readonly Dictionary<string, User> _byEmail = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<User> AllUsers => _users.Values.ToList();

    public void Seed(User user)
    {
        _users[user.Id] = user;
        _byEmail[user.Email] = user;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.GetValueOrDefault(id));

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _byEmail.TryGetValue(email.Trim(), out var user);
        return Task.FromResult(user);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(_byEmail.ContainsKey(email.Trim()));

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = user;
        _byEmail[user.Email] = user;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyDictionary<Guid, User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyDictionary<Guid, User>>(new Dictionary<Guid, User>());
}
