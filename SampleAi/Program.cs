using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SampleAi
{
	class Program
	{
		// Управляющая программа battleships.exe будет запускать этот файл и перенаправлять стандартные потоки ввода и вывода.
		//
		// Вам нужно читать информацию с консоли и писать команды на консоль.
		// Конец ввода — это сигнал к завершению программы.

        private static readonly IEnumerable<Size> diagonals = 
            new List<Size> { new Size(1, 1), new Size(1, -1), new Size(-1, -1), new Size(-1, 1) };
        private static readonly IEnumerable<Size> adjacency = 
            new List<Size> { new Size(0, 1), new Size(1, 0), new Size(0, -1), new Size(-1, 0) };

		static void Main()
		{
			var r = new Random();
		    var aim = new Point(0, 0);
            var boardSize = new Size(20, 20);
		    var nonTargetCells = new HashSet<Point>();
			while (true)
			{
				var line = Console.ReadLine();
				if (line == null) return;
				// line имеет один из следующих форматов:
				// Init <map_width> <map_height> <ship1_size> <ship2_size> ...
				// Wound <last_shot_X> <last_shot_Y>
				// Kill <last_shot_X> <last_shot_Y>
				// Miss <last_shot_X> <last_shot_Y>
				// Один экземпляр вашей программы может быть использван для проведения нескольких игр подряд.

				// Сообщение Init сигнализирует о том, что началась новая игра.

                var message = line.Split(' ');
			    switch (message[0])
			    {
			        case "Init": 
                        nonTargetCells.Clear();
                        aim = new Point(0, 0);
                        break;
                    case "Miss":
                        aim = NextCell(boardSize, nonTargetCells, r);
                        break;
                    case "Wound":
    			        GetOffsetCells(aim, boardSize, diagonals).ToList().ForEach(cell => nonTargetCells.Add(cell));

                        // Try and destroy the current ship
			            var destroyed = false;
			            var firstCell = aim;

			            foreach (var direction in adjacency)
			            {                            
			                aim = firstCell;
                            aim = Point.Add(aim, direction);
			                if (!WithinBoard(aim, boardSize) || nonTargetCells.Contains(aim)) continue;
                            Console.WriteLine("{0} {1}", aim.X, aim.Y);
			                nonTargetCells.Add(aim);

                            line = Console.ReadLine();
				            if (line == null) return;
                            message = line.Split(' ');

			                while (message[0] != "Miss")
			                {
			                    if (message[0] == "Kill")
			                    {
                                    GetOffsetCells(aim, boardSize, adjacency).ToList().ForEach(cell => nonTargetCells.Add(cell));
			                        break;
			                    }
                                aim = Point.Add(aim, direction);
                                if (!WithinBoard(aim, boardSize) || nonTargetCells.Contains(aim)) break;
                                Console.WriteLine("{0} {1}", aim.X, aim.Y);
                                nonTargetCells.Add(aim);

                                line = Console.ReadLine();
                                if (line == null) return;
                                message = line.Split(' ');
			                }
			            }

                        aim = NextCell(boardSize, nonTargetCells, r);
			            break;
                    case "Kill":
			            var neighbourhood = diagonals.Union(adjacency);
                        GetOffsetCells(aim, boardSize, neighbourhood).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        aim = NextCell(boardSize, nonTargetCells, r);
			            break;
			    }

			    Console.WriteLine("{0} {1}", aim.X, aim.Y);
			    nonTargetCells.Add(aim);
			}
		}

	    private static bool WithinBoard(Point cell, Size size)
	    {
	        var withinVertically = 0 <= cell.X && cell.X < size.Height;
	        var withinHorizontally = 0 <= cell.Y && cell.Y < size.Width;
	        return withinVertically && withinHorizontally;
	    }

	    private static Point NextCell(Size size, ICollection<Point> excluded, Random r)
	    {
	        var cell = new Point(r.Next(size.Height), r.Next(size.Width));
	        while (excluded.Contains(cell))
	        {
                cell = new Point(r.Next(size.Height), r.Next(size.Width));
	        }
	        return cell;
	    }

	    private static IEnumerable<Point> GetOffsetCells(Point cell, Size size, IEnumerable<Size> offsets)
	    {
	        return offsets.Select(offset => Point.Add(cell, offset));
	    }
	}
}
