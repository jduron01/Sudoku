import { useState, useEffect, useRef, type RefObject } from 'react';
import './App.css';

interface Board {
  id: number;
  grid: number[][];
}

interface Cell {
  row: number;
  col: number;
}

function App() {
  const [board, setBoard] = useState<Board | null>();
  const [selectedCell, setSelectedCell] = useState<Cell | null>();
  const [error, setError] = useState<string | null>();
  const dialogRef: RefObject<HTMLDialogElement | null> = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const fetchBoard = async (): Promise<void> => {
      try {
        const response: Response = await fetch('https://localhost:7169/api/Board/');

        if (!response.ok) {
          throw new Error(`HTTP error status: ${response.status}`);
        }

        const data: Board[] = await response.json();

        if (data.length > 0) {
          setBoard(data[data.length - 1]);
        }
      } catch (e: unknown) {
        if (e instanceof Error) {
          setError(e.message);
        } else {
          setError('An unknown error occurred.');
        }
      }
    };

    fetchBoard();
  }, []);

  if (error) {
    return (<div>Error: {error}</div>);
  }

  const createBoard = async (difficulty: string): Promise<void> => {
    try {
      const response: Response = await fetch('https://localhost:7169/api/Board/', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ 'difficulty': difficulty })
      });

      if (!response.ok) {
        throw new Error(`HTTP error status: ${response.status}`);
      }

      const data: Board = await response.json();

      setBoard(data);
    } catch (e: unknown) {
      if (e instanceof Error) {
        setError(e.message);
      } else {
        setError('An unknown error occurred.');
      }
    }
  }

  const openDialog = (): void => {
    if (dialogRef.current) {
      dialogRef.current.showModal();
    }
  };

  const closeDialog = (): void => {
    if (dialogRef.current) {
      dialogRef.current.close();
    }
  }

  return (
    <>
      <div>
        <h1>React Sudoku Board</h1>

        {board && (
          <table>
            <tbody>
              {[...Array(9)].map((_, row) => (
                <tr key={row}>
                  {[...Array(9)].map((_, col) => (
                    <td
                      key={col}
                      className={
                        selectedCell && selectedCell.row === row && selectedCell.col === col
                          ? 'clicked-bg'
                          : 'default-bg'
                      }
                      onClick={() => { setSelectedCell({ row, col }); }}
                    >
                      {board.grid[row][col] === 0 ? '' : board.grid[row][col]}
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        )}

        <div className='options'>
          <button className='new-puzzle' onClick={openDialog}>New Puzzle</button>
          <button className='load-puzzle' onClick={undefined}>Load Puzzle</button>
          <button className='delete-puzzle' onClick={undefined}>Delete Puzzle</button>
        </div>

        <dialog ref={dialogRef}>
          <h2>Select Difficulty</h2>
          <div className='difficulty-options'>
            <ul>
              <li><button onClick={() => { createBoard('easy'); closeDialog(); }}>Easy</button></li>
              <li><button onClick={() => { createBoard('normal'); closeDialog(); }}>Normal</button></li>
              <li><button onClick={() => { createBoard('hard'); closeDialog(); }}>Hard</button></li>
            </ul>
          </div>
          <button className='close-button' onClick={closeDialog} aria-label='close'>&#88;</button>
        </dialog>

      </div>
    </>
  );
}

export default App;
