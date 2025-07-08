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
                InitBoard(board.Grid, 35);
            }
            else if (board.Difficulty is "normal")
            {
                InitBoard(board.Grid, 25);
            }
            else
            {
                InitBoard(board.Grid, 20);
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

        private static void InitBoard(int[][] grid, int numGivens)
        {
            Random rand = new();
            int maxAttempts = numGivens * 20;
            int attempts = 0;
            int placed = 0;

            while (attempts < maxAttempts && placed < numGivens)
            {
                int row = rand.Next(9);
                int col = rand.Next(9);

                if (grid[row][col] != 0)
                {
                    attempts++;
                    continue;
                }

                List<int> candidates = [];

                for (int n = 0; n < 9; n++)
                {
                    if (!IsInRow(grid, row, n) &&
                        !IsInCol(grid, col, n) &&
                        !IsInBox(grid, row, col, n))
                    {
                        candidates.Add(n);
                    }
                }

                if (candidates.Count == 0)
                {
                    attempts++;
                    continue;
                }

                int val = candidates[rand.Next(candidates.Count)];

                grid[row][col] = val;
                attempts = 0;
                placed++;
            }
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
