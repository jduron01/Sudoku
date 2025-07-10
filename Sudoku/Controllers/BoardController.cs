using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sudoku.Models;

namespace Sudoku.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController(BoardContext context) : ControllerBase
    {
        private readonly BoardContext _context = context;

        // GET: api/Board
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
        {
            return await _context.Boards.ToListAsync();
        }

        // GET: api/Board/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetBoard(long id)
        {
            var board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }

            return board;
        }

        // PUT: api/Board/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoard(long id, Board board)
        {
            if (id != board.Id)
            {
                return BadRequest();
            }

            _context.Entry(board).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Board
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Board>> PostBoard(BoardDTO boardDTO)
        {
            Board board = new()
            {
                Difficulty = boardDTO.Difficulty,
                Grid = new int[9][]
            };

            for (int i = 0; i < 9; i++)
            {
                board.Grid[i] = new int[9];
            }

            if (board.Difficulty is "easy")
            {
                GeneratePuzzle(board.Grid, 35);
            }
            else if (board.Difficulty is "normal")
            {
                GeneratePuzzle(board.Grid, 25);
            }
            else
            {
                GeneratePuzzle(board.Grid, 20);
            }

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
        }

        // DELETE: api/Board/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(long id)
        {
            var board = await _context.Boards.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoardExists(long id)
        {
            return _context.Boards.Any(e => e.Id == id);
        }

        private static void GeneratePuzzle(int[][] grid, int numGivens)
        {
            Random rand = new();

            FillDiagonalBoxes(grid, rand);
            ValidatePuzzle(grid, rand);
            RemoveNumbers(grid, numGivens, rand);
        }

        private static void FillDiagonalBoxes(int[][] grid, Random rand)
        {
            for (int cell = 0; cell < 9; cell += 3)
            {
                int[] nums = [.. Enumerable.Range(1, 9).OrderBy(_ => rand.Next())];
                int index = 0;

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        grid[cell + i][cell + j] = nums[index++];
                    }
                }
            }
        }

        private static bool ValidatePuzzle(int[][] grid, Random rand)
        {
            (int row, int col)? cell = FindEmptyCell(grid);

            if (cell == null)
            {
                return true;
            }

            (int row, int col) = cell.Value;
            int[] nums = [.. Enumerable.Range(1, 9).OrderBy(_ => rand.Next())];

            foreach (int n in nums)
            {
                if (IsSafe(grid, row, col, n))
                {
                    grid[row][col] = n;

                    if (ValidatePuzzle(grid, rand))
                    {
                        return true;
                    }

                    grid[row][col] = 0;
                }
            }

            return false;
        }

        public static void RemoveNumbers(int[][] grid, int numGivens, Random rand)
        {
            int numToRemove = 81 - numGivens;
            List<(int row, int col)> positions = [];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    positions.Add((row, col));
                }
            }

            positions = [.. positions.OrderBy(_ => rand.Next())];

            for (int i = 0; i < numToRemove && i < positions.Count; i++)
            {
                (int row, int col) = positions[i];
                grid[row][col] = 0;
            }
        }

        private static (int row, int col)? FindEmptyCell(int[][] grid)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (grid[row][col] == 0)
                    {
                        return (row, col);
                    }
                }
            }
            return null;
        }

        private static bool IsSafe(int[][] grid, int row, int col, int num)
        {
            return !IsInRow(grid, row, num) &&
                   !IsInCol(grid, col, num) &&
                   !IsInBox(grid, row, col, num);
        }

        private static bool IsInRow(int[][] grid, int row, int val)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row][col] == val)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsInCol(int[][] grid, int col, int val)
        {
            for (int row = 0; row < 9; row++)
            {
                if (grid[row][col] == val)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsInBox(int[][] grid, int row, int col, int val)
        {
            int startRow = row - row % 3;
            int startCol = col - col % 3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (grid[startRow + i][startCol + j] == val)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
