using Microsoft.EntityFrameworkCore;
using TransactionSystemApi.Models;

namespace TransactionSystemApi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
}