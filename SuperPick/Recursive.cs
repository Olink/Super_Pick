using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace SuperPick
{
    class Recursive
    {
        int edittype;
        byte ttype;
        bool killwire = false;
        public List<Vector2> RecursiveEdit(int x, int y, int edittype)
        {
            this.edittype = edittype;
            switch (edittype)
            {
                case 0:
                case 4:
                    this.ttype = Main.tile[x, y].type;
                    break;
                case 2:
                    this.ttype = Main.tile[x, y].wall;
                    break;
                case 6:
                    killwire = true;
                    break;
            }
            List<Vector2> neighbors = new List<Vector2>();
            neighbors.Add(new Vector2(0, -1));
            neighbors.Add(new Vector2(-1, 0));
            neighbors.Add(new Vector2(1, 0));
            neighbors.Add(new Vector2(0, 1));
            List<Vector2> visited = new List<Vector2>();
            List<Vector2> current = new List<Vector2>();
            visited.Add(new Vector2(x, y));
            current.Add(new Vector2(x, y));
            while (true)
            {
                List<Vector2> same = GetSameNeighbors(current[current.Count - 1], neighbors, visited);
                if (same.Count > 0 && visited.Count <= 250)
                {
                    Vector2 next = GetRandomMember(same) + current[current.Count - 1];
                    current.Add(next);
                    visited.Add(next);
                }
                else
                {
                    if (current.Count > 1 && visited.Count <= 250)
                    {
                        current.RemoveAt(current.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return visited;
        }
        private Vector2 GetRandomMember(List<Vector2> list)
        {
            Random rand = new Random();
            int choice = rand.Next(list.Count - 1);
            return list[choice];
        }
        private List<Vector2> GetSameNeighbors(Vector2 tile, List<Vector2> neighbors, List<Vector2> visited)
        {
            bool wall = false;
            if (!this.killwire)
            {
                byte compare = 0;
                switch (this.edittype)
                {
                    case 0:
                    case 4:
                        compare = Main.tile[Convert.ToInt16(tile.X), Convert.ToInt16(tile.Y)].type;
                        break;
                    case 2:
                        compare = Main.tile[Convert.ToInt16(tile.X), Convert.ToInt16(tile.Y)].wall;
                        wall = true;
                        break;
                }
                if (wall)
                {
                    List<Vector2> free = new List<Vector2>();
                    foreach (Vector2 spot in neighbors)
                    {
                        byte first;
                        Vector2 next = spot + tile;
                        first = Main.tile[Convert.ToInt16(next.X), Convert.ToInt16(next.Y)].wall;
                        if (first == compare && !visited.Contains(next))
                        {
                            free.Add(spot);
                        }
                    }
                    return free;
                }
                else
                {
                    List<Vector2> free = new List<Vector2>();
                    foreach (Vector2 spot in neighbors)
                    {
                        byte first;
                        Vector2 next = spot + tile;
                        first = Main.tile[Convert.ToInt16(next.X), Convert.ToInt16(next.Y)].type;
                        if (first == compare && !visited.Contains(next) && Main.tile[Convert.ToInt16(next.X), Convert.ToInt16(next.Y)].active)
                        {
                            free.Add(spot);
                        }
                    }
                    return free;
                }
            }
            else
            {
                List<Vector2> free = new List<Vector2>();
                foreach (Vector2 spot in neighbors)
                {
                    Vector2 next = spot + tile;
                    if (Main.tile[Convert.ToInt16(next.X), Convert.ToInt16(next.Y)].wire == true && !visited.Contains(next))
                    {
                        free.Add(spot);
                    }
                }
                return free;
            }
        }
    }
}
