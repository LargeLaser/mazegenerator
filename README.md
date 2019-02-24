# mazegenerator
very simple c# class for generating random 2D grid-based mazes.

## Dependencies
operates only on base types, and will work as is for Unity/XNA etc. without additional libraries or code.

## Examples:
some examples from screenshots of game (the wobbly lines are because the game requires a hand-drawn look) using this class:

![Examples](/examples/examples.png "Examples")

## Usage
create an instance of Maze class, initialise it with a seed to determine the randomness and bounds:

```
using LargeLaser;

Maze maze = new Maze();
// either a pseudo random value, or some stored number.
int seed = System.DateTime.Now.Millisecond;
// width/length of maze.
int size = 6;
maze.create(seed, size, size);
```

to draw or create objects, iterate the maze cells and do whatever necessary with the type, position and orientation. the *createMazeObject* function would add a GameObject, or draw a texture, or add collision, or whatever is neeeded.

```
for (int y = 0; y < maze.Length; ++y)
{
  for (int x = 0; x < maze.Width; ++x)
  {
    Cell cell = maze.cell(x, y);
    
    if (cell.CellType == Cell.Type.Corridor)
    {
      createMazeObject(corridorModel, x, y, cell.Angle);
    }
    else if (cell.CellType == Cell.Type.Corner)
    {
      createMazeObject(cornerModel, x, y, cell.Angle);
    }
    else if (cell.CellType == Cell.Type.Tee)
    {
      createMazeObject(teeModel, x, y, cell.Angle);
    }
    else if (cell.CellType == Cell.Type.End)
    {
      createMazeObject(endModel, x, y, cell.Angle);
    }
    else if (cell.CellType == Cell.Type.Cross)
    {
      createMazeObject(crossModel, x, y, cell.Angle);
    }
  }
}
```

for navigating a maze, use the cell's links to determine valid destinations from the current position:

```
// get cell at our player's current position.
int currentX = (int)System.Math.Round(playerPosition.x);
int currentY = (int)System.Math.Round(playerPosition.z);
Cell currentCell = maze.cell(currentX, currentY);

// where we would like to move to.
int destX = (int)System.Math.Round(destination.x);
int destY = (int)System.Math.Round(destination.z);

if(currentCell.hasLink(destX,destY))
{
  // link exists, we can move to the destination.
  move(dest);
}
else
{
  // can't move!
}

```

