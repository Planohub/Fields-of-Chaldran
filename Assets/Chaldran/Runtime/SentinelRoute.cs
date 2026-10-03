using System;
using System.Collections.Generic;

namespace Chaldran
{
    public readonly struct GridPoint
    {
        public readonly int X, Y;
        public GridPoint(int x, int y) { X = x; Y = y; }
    }

    public static class SentinelRoute
    {
        // Cardinal movement avoids diagonal corner cutting. Search is bounded by the room.
        public static List<GridPoint> Find(int startX, int startY, int minX, int maxX, int minY, int maxY,
            Func<int, int, bool> walkable, Func<int, int, bool> goal)
        {
            if (startX < minX || startX > maxX || startY < minY || startY > maxY || walkable == null || goal == null)
                return null;
            int width = maxX - minX + 1;
            int start = (startY - minY) * width + startX - minX;
            Queue<int> open = new Queue<int>();
            Dictionary<int, int> parents = new Dictionary<int, int> { [start] = -1 };
            open.Enqueue(start);
            int[] dx = { 1, 0, -1, 0 }, dy = { 0, 1, 0, -1 };
            while (open.Count > 0)
            {
                int node = open.Dequeue();
                int x = node % width + minX, y = node / width + minY;
                if (goal(x, y))
                {
                    List<GridPoint> path = new List<GridPoint>();
                    for (int cursor = node; cursor != start; cursor = parents[cursor])
                        path.Add(new GridPoint(cursor % width + minX, cursor / width + minY));
                    path.Reverse();
                    return path;
                }
                for (int direction = 0; direction < 4; direction++)
                {
                    int nx = x + dx[direction], ny = y + dy[direction];
                    if (nx < minX || nx > maxX || ny < minY || ny > maxY) continue;
                    int next = (ny - minY) * width + nx - minX;
                    if (parents.ContainsKey(next) || !walkable(nx, ny)) continue;
                    parents[next] = node;
                    open.Enqueue(next);
                }
            }
            return null;
        }
    }
}
