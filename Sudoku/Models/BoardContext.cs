using Microsoft.EntityFrameworkCore;

namespace Sudoku.Models;

public class BoardContext(DbContextOptions<BoardContext> options) : DbContext(options)
{
    public DbSet<Board> Boards { get; set; } = null!;
}
