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
                        aim = NextCell(size, nonTargetCells, r);
                        break;
                    case "Wound":
    			        GetOffsetCells(aim, size, diagonals).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        aim = NextCell(size, nonTargetCells, r);
			            break;
                    case "Kill":
			            var neighbourhood = diagonals.Union(adjacency);
                        GetOffsetCells(aim, size, neighbourhood).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        aim = NextCell(size, nonTargetCells, r);
			            break;
			    }

			    while (nonTargetCells.Contains(aim))
			    {
                    aim = NextCell(size, nonTargetCells, r);
			    }

			    Console.WriteLine("{0} {1}", aim.X, aim.Y);
			    nonTargetCells.Add(aim);
			}
		}



	    private static Point NextCell(Point size, ICollection<Point> excluded, Random r)
	    {
	        var cell = new Point(r.Next(size.X), r.Next(size.Y));
	        while (excluded.Contains(cell))
	        {
                cell = new Point(r.Next(size.X), r.Next(size.Y));
	        }
	        return cell;
	    }

	    private static IEnumerable<Point> GetOffsetCells(Point cell, Point size, IEnumerable<Size> offsets)
	    {
	        return offsets.Select(offset => Point.Add(cell, offset));
	    }
	}
}
