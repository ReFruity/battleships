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

		static void Main()
		{
			var r = new Random();
		    Point aim = new Point(0, 0), size = new Point(20, 20);
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
                        aim = NextCell(aim, size);
                        break;
                    case "Wound":
    			        GetDiagonalCells(aim, size).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        aim = NextCell(aim, size);
			            break;
                    case "Kill":
                        GetDiagonalCells(aim, size).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        GetAdjacentCells(aim, size).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        aim = NextCell(aim, size);
			            break;
			    }

			    while (nonTargetCells.Contains(aim))
			    {
                    aim = NextCell(aim, size);
			    }

			    Console.WriteLine("{0} {1}", aim.X, aim.Y);
			    nonTargetCells.Add(aim);
			}
		}



	    private static Point NextCell(Point cell, Point size)
	    {
            cell.Y++;
            if (cell.Y >= size.Y)
            {
                cell.Y = 0;
                cell.X++;
            }
            if (cell.X >= size.X)
            {
                cell.X = 0;
                cell.Y = 0;
            }	    
	        return cell;
	    }

	    private static IEnumerable<Point> GetDiagonalCells(Point cell, Point size)
	    {
            var diagonals = new List<Size> { new Size(1, 1), new Size(1, -1), new Size(-1, -1), new Size(-1, 1) };
	        return diagonals.Select(diagonal => Point.Add(cell, diagonal));
	    }

        private static IEnumerable<Point> GetAdjacentCells(Point cell, Point size)
        {
            var adjacency = new List<Size> { new Size(0, 1), new Size(1, 0), new Size(0, -1), new Size(-1, 0) };
            return adjacency.Select(adj => Point.Add(cell, adj));
        }
	}
}
