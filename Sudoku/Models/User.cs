namespace Sudoku.Models;

public class User
{
    public long Id { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}
