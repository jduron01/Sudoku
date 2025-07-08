namespace Sudoku.Models;

public class Board
{
    public long Id { get; set; }
    public string Difficulty { get; set; } = null!;
    public int[][] Grid { get; set; } = null!;
}
